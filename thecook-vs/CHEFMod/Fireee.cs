using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ChefMod
{
    public class Fireee : NetworkBehaviour
    {
        public GameObject owner;
        public TeamIndex teamIndex = TeamIndex.Player;
        public bool onFire = false;
        public float critStat = 0;
        public CharacterMaster master = null;

        private CharacterBody body;
        private float radius = 10;
        public float damagePerFrame = 3;
        private float oilTime = 30f;
        private float burnTime = 10f;

        private GameObject oilPrefab;
        private GameObject firePrefab;
        private float startTime;
        private float igniteTime;
        private bool ground = false;
        private int framecounter = 0;

        private static List<HurtBox> hurtBoxBuffer = new List<HurtBox>();
        private static SphereSearch sphereSearch = new SphereSearch();
        private static GameObject ExplosionEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/IgniteExplosionVFX");
        private static GameObject FireMaybe = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick");

        void Start()
        {
            startTime = Time.fixedTime;
            body = GetComponent<CharacterBody>();
            if (!body)
            {
                Chat.AddMessage("ping gnome. no body");
                Destroy(this.gameObject);
            }
            //body.baseMaxHealth = -1f;
            //body.AddBuff(BuffIndex.Immune);
            //body.AddBuff(BuffIndex.HiddenInvincibility);
            //body.AddBuff(BuffIndex.Cloak);
            body.healthComponent.godMode = true;
            body.baseMaxHealth = 99999999f;

            var direction = this.transform.root.GetComponentInChildren<CharacterDirection>();
            foreach (var thisItem in direction.modelAnimator.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                thisItem.gameObject.SetActive(false);
            }
            foreach (var thisItem in direction.modelAnimator.GetComponentsInChildren<MeshRenderer>())
            {
                thisItem.gameObject.SetActive(false);
            }
            //ground = body.characterMotor.isGrounded;

            //oilPrefab = goku("Oyl");

            //checkforhomies();
        }

        void FixedUpdate()
        {
            if (!onFire && body.HasBuff(BuffIndex.OnFire))
            {
                ignate();
            }

            if (!onFire && !ground && body.characterMotor.isGrounded)
            {
                ground = true;
                oilPrefab = goku("Oyl");
                checkforhomies();
            }

            if (shouldDie())
            {
                Destroy(body.gameObject);
            }

            if (framecounter % 30 == 0)
            {
                doSomething();
            }
            framecounter++;
        }

        private GameObject goku(String asset)
        {
            GameObject prefab;
            prefab = Instantiate(Assets.chefAssetBundle.LoadAsset<GameObject>(asset));
            var direction = this.transform.root.GetComponentInChildren<CharacterDirection>();
            prefab.transform.position = body.footPosition - 7 * Vector3.up;
            prefab.transform.localScale *= 2f;
            Quaternion floorRotation = Quaternion.FromToRotation(Vector3.up, body.characterMotor.estimatedGroundNormal);
            Quaternion ninety = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
            float randomtheta = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            Quaternion random = Quaternion.FromToRotation(Vector3.forward, new Vector3(Mathf.Cos(randomtheta), 0, Mathf.Sin(randomtheta)));
            prefab.transform.rotation = this.transform.rotation * ninety * floorRotation;
            prefab.transform.SetParent(this.transform);
            return prefab;
        }

        private bool shouldDie()
        {
            bool oiltimeout = (!onFire) && Time.fixedTime - startTime > oilTime;
            bool firetimeout = onFire && Time.fixedTime - igniteTime > burnTime;
            return oiltimeout || firetimeout;
        }

        public void ignate()
        {
            if (onFire)
            {
                return;
            }

            onFire = true;
            igniteTime = Time.fixedTime;
            esplode();

            //Destroy(oilPrefab);
            //firePrefab = goku("Fyre");
            oilPrefab.GetComponent<MeshRenderer>().material = chefPlugin.segfab.GetComponent<ParticleSystemRenderer>().material;
            firePrefab = Instantiate(chefPlugin.segfab, this.transform.position, Quaternion.identity);

            hitmyhomiesup();

            //Util.PlaySound("OilFire", base.gameObject);
        }

        private void esplode()
        {
            Vector3 corePosition = body.corePosition;
            sphereSearch.origin = corePosition;
            sphereSearch.mask = LayerIndex.entityPrecise.mask;
            sphereSearch.radius = radius;
            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(teamIndex));
            sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            sphereSearch.OrderCandidatesByDistance();
            sphereSearch.GetHurtBoxes(hurtBoxBuffer);
            sphereSearch.ClearCandidates();
            for (int i = 0; i < hurtBoxBuffer.Count; i++)
            {
                HurtBox hurtBox = hurtBoxBuffer[i];
                if (hurtBox.healthComponent)
                {
                    DotController.InflictDot(hurtBox.healthComponent.gameObject, owner, DotController.DotIndex.Burn, 1.5f + 1.5f, 0.5f);
                }
            }
            hurtBoxBuffer.Clear();
            EffectManager.SpawnEffect(ExplosionEffectPrefab, new EffectData
            {
                origin = corePosition,
                scale = radius,
                rotation = Util.QuaternionSafeLookRotation(Vector3.up)
            }, true);
        }

        private void doSomething()
        {
            float damage = this.damagePerFrame;
            HashSet<GameObject> hashSet = new HashSet<GameObject>();
            RoR2.TeamIndex attackerTeamIndex = teamIndex;

            hashSet.Add(body.gameObject);
            hashSet.Add(this.owner);

            Vector3 dims = new Vector3(1, 0.2f, 1);
            RaycastHit[] array = Physics.BoxCastAll(body.footPosition, dims * radius, Vector3.forward, Quaternion.identity, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
            for (int j = 0; j < array.Length; j++)
            {
                Collider collider = array[j].collider;
                if (collider.gameObject)
                {
                    RoR2.HurtBox component = collider.GetComponent<RoR2.HurtBox>();
                    if (component)
                    {
                        RoR2.HealthComponent healthComponent = component.healthComponent;
                        if (healthComponent)
                        {
                            GameObject gameObject = healthComponent.gameObject;
                            if (!hashSet.Contains(gameObject))
                            {
                                hashSet.Add(gameObject);
                                if (RoR2.FriendlyFireManager.ShouldSplashHitProceed(healthComponent, attackerTeamIndex))
                                {
                                    if (onFire)
                                    {
                                        bool crit = false;
                                        if (master) crit = Util.CheckRoll(critStat, master);
                                        healthComponent.TakeDamage(new RoR2.DamageInfo
                                        {
                                            position = healthComponent.body.corePosition,
                                            attacker = this.owner,
                                            inflictor = base.gameObject,
                                            crit = crit,
                                            damage = damage,
                                            damageColorIndex = RoR2.DamageColorIndex.Default,
                                            damageType = RoR2.DamageType.Generic,
                                            force = Vector3.zero,
                                            procCoefficient = chefPlugin.oilProc.Value
                                        }) ;
                                    }
                                    else
                                    {
                                        if (!healthComponent.body.HasBuff(BuffIndex.ClayGoo))
                                        {
                                            healthComponent.body.AddTimedBuff(BuffIndex.ClayGoo, 5f);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void hitmyhomiesup()
        {
            RaycastHit[] array = Physics.SphereCastAll(body.corePosition, radius, Vector3.up, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
            for (int j = 0; j < array.Length; j++)
            {
                Collider collider = array[j].collider;
                if (collider.gameObject)
                {
                    RoR2.HurtBox component = collider.GetComponent<RoR2.HurtBox>();
                    if (component)
                    {
                        RoR2.HealthComponent healthComponent = component.healthComponent;
                        if (healthComponent)
                        {
                            Fireee fire = healthComponent.body.GetComponent<Fireee>();
                            if (fire)
                            {
                                fire.ignate();
                            }
                        }
                    }
                }
            }
        }
        private void checkforhomies()
        {
            RaycastHit[] array = Physics.SphereCastAll(body.corePosition, radius, Vector3.up, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
            for (int j = 0; j < array.Length; j++)
            {
                Collider collider = array[j].collider;
                if (collider.gameObject)
                {
                    RoR2.HurtBox component = collider.GetComponent<RoR2.HurtBox>();
                    if (component)
                    {
                        RoR2.HealthComponent healthComponent = component.healthComponent;
                        if (healthComponent)
                        {
                            Fireee fire = healthComponent.body.GetComponent<Fireee>();
                            if (fire && fire.onFire)
                            {
                                this.ignate();
                            }
                        }
                    }
                }
            }
        }

        private void DestroyFX()
        {

        }
    }
}