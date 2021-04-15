using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Networking;

namespace ChefMod
{
    public class Fireee : NetworkBehaviour
    {
        public bool onFire = false;

        private GameObject owner;
        private TeamIndex teamIndex;
        private CharacterBody body;
        private float damagePerFrame;
        private bool crit;
        private float radius = 10;
        private float oilTime = 30f;
        private float burnTime = 10f;

        private GameObject oilPrefab;
        private GameObject firePrefab;
        private float startTime;
        private float igniteTime;
        private bool ground = false;
        private int framecounter = 0;
        private Rigidbody rig;

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

            oilPrefab = goku("Oyl");
            rig = GetComponent<Rigidbody>();
            rig.velocity += Vector3.down;

            var projCont = this.gameObject.GetComponent<ProjectileController>();
            var projDamg = this.gameObject.GetComponent<ProjectileDamage>();
            var teamFilt = this.gameObject.GetComponent<TeamFilter>();

            this.owner = projCont.owner;
            this.teamIndex = teamFilt.teamIndex;
            this.damagePerFrame = projDamg.damage;
            this.crit = projDamg.crit;

            //ground = body.characterMotor.isGrounded;

            //checkforhomies();
        }

        void FixedUpdate()
        {
            if (!onFire && body.HasBuff(RoR2Content.Buffs.OnFire))
            {
                ignate();
            }

            if (!onFire && !ground && rig.velocity.magnitude < 1f)// && body.characterMotor.isGrounded)
            {
                ground = true;
                rig.isKinematic = true;
                Ray ray = new Ray(transform.position, Vector3.down);
                RaycastHit[] casts = Physics.RaycastAll(ray);
                Quaternion floorRotation = Quaternion.FromToRotation(Vector3.up, casts[0].normal);
                Quaternion ninety = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
                oilPrefab.transform.rotation = this.transform.rotation * ninety * floorRotation;
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
            prefab.transform.position = body.footPosition - 2.5f * Vector3.up;
            prefab.transform.SetParent(this.transform);
            prefab.transform.localScale *= 2.5f;
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
            firePrefab = Instantiate(chefPlugin.segfab, this.transform.position - Vector3.up, Quaternion.identity);

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
                                        healthComponent.TakeDamage(new RoR2.DamageInfo
                                        {
                                            position = healthComponent.body.corePosition,
                                            attacker = this.owner,
                                            inflictor = base.gameObject,
                                            crit = this.crit,
                                            damage = damage,
                                            damageColorIndex = RoR2.DamageColorIndex.Default,
                                            damageType = RoR2.DamageType.Generic,
                                            force = Vector3.zero,
                                            procCoefficient = chefPlugin.oilProc.Value
                                        }) ;
                                    }
                                    else
                                    {
                                        if (!healthComponent.body.HasBuff(RoR2Content.Buffs.ClayGoo))
                                        {
                                            healthComponent.body.AddTimedBuff(RoR2Content.Buffs.ClayGoo, 5f);
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
            RaycastHit[] array = Physics.SphereCastAll(body.corePosition, 1.2f * radius, Vector3.up, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
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