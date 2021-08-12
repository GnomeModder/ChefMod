using R2API;
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
        [SyncVar]
        public bool onFire = false;
        public bool ground = false;
        private bool edging = false;

        private GameObject owner;
        private TeamIndex teamIndex;
        private CharacterBody body;
        private float damagePerFrame;
        private bool crit;
        private float radius = 10;
        public static float oilTime = 30f;
        public static float burnTime = 10f;
        public static float procCoefficient = 0.2f;

        private float flameStopwatch = 0f;
        public static float flameTickRate = 0.5f;

        private GameObject oilPrefab;
        private GameObject firePrefab;

        [SyncVar]
        private float startTime;
        [SyncVar]
        private float igniteTime;

        private Rigidbody rig;

        private static List<HurtBox> hurtBoxBuffer = new List<HurtBox>();
        private static SphereSearch sphereSearch = new SphereSearch();
        public static GameObject ExplosionEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/IgniteExplosionVFX");
        private static GameObject FireMaybe = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick");
        private CharacterBody characterBody;

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

            oilPrefab = goku("OilBall");
            rig = GetComponent<Rigidbody>();
            rig.velocity += Vector3.down;

            var projCont = this.gameObject.GetComponent<ProjectileController>();
            var projDamg = this.gameObject.GetComponent<ProjectileDamage>();
            var teamFilt = this.gameObject.GetComponent<TeamFilter>();

            this.owner = projCont.owner;
            this.teamIndex = teamFilt.teamIndex;
            this.damagePerFrame = projDamg.damage;
            this.crit = projDamg.crit;

            if (owner)
            {
                characterBody = owner.GetComponent<CharacterBody>();
            }

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
                //Ray ray = new Ray(transform.position, Vector3.down);
                //RaycastHit[] casts = Physics.RaycastAll(ray);
                //Quaternion floorRotation = Quaternion.FromToRotation(Vector3.up, casts[0].normal);
                //Quaternion ninety = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
                //oilPrefab.transform.rotation = this.transform.rotation * ninety * floorRotation;
                Quaternion randy = new Quaternion(0, 1, 0, UnityEngine.Random.Range(0, 360f));
                Destroy(oilPrefab);
                oilPrefab = Instantiate(chefPlugin.oilfab, this.transform.position - Vector3.up, randy);
                if (edging) ignate();
                checkforhomies();
            }

            if (shouldDie())
            {
                Destroy(oilPrefab);
                Destroy(firePrefab);
                Destroy(body.gameObject);
            }

            flameStopwatch += Time.fixedDeltaTime;
            if (flameStopwatch > flameTickRate)
            {
                flameStopwatch -= flameTickRate;
                doSomething();
            }
        }

        private GameObject goku(String asset)
        {
            GameObject prefab;
            prefab = Instantiate(Assets.chefAssetBundle.LoadAsset<GameObject>(asset));
            //var direction = this.transform.root.GetComponentInChildren<CharacterDirection>();
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

            if (!ground)
            {
                edging = true;
                return;
            }

            onFire = true;

            igniteTime = Time.fixedTime;
            esplode();

            //Destroy(oilPrefab);
            //firePrefab = goku("Fyre");
            firePrefab = Instantiate(chefPlugin.firefab, this.transform.position - Vector3.up, Quaternion.identity);

            hitmyhomiesup();

            //Util.PlaySound("OilFire", base.gameObject);
        }

        private void esplode()
        {
            if (!NetworkServer.active) return;
            Vector3 corePosition = body.corePosition;
            /*sphereSearch.origin = corePosition;
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
                    DotController.InflictDot(hurtBox.healthComponent.gameObject, owner, DotController.DotIndex.Burn);
                }
            }
            hurtBoxBuffer.Clear();*/
            EffectManager.SpawnEffect(ExplosionEffectPrefab, new EffectData
            {
                origin = corePosition,
                scale = radius,
                rotation = Util.QuaternionSafeLookRotation(Vector3.up)
            }, true);
            //OilExplosion.Explode(characterBody, null, this.crit, body.corePosition);
        }

        private void doSomething()
        {
            if (!NetworkServer.active) return;
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
                                        DamageInfo di = new RoR2.DamageInfo
                                        {
                                            position = healthComponent.body.corePosition,
                                            attacker = this.owner,
                                            inflictor = base.gameObject,
                                            crit = this.crit,
                                            damage = damage,
                                            damageColorIndex = RoR2.DamageColorIndex.Default,
                                            damageType = RoR2.DamageType.Generic,
                                            force = Vector3.zero,
                                            procCoefficient = Fireee.procCoefficient
                                        };
                                        di.AddModdedDamageType(chefPlugin.chefSear);
                                        healthComponent.TakeDamage(di);
                                        if (!healthComponent.body.HasBuff(RoR2Content.Buffs.OnFire))
                                        {
                                            DotController.InflictDot(healthComponent.gameObject, owner, DotController.DotIndex.Burn);
                                        }
                                        if (healthComponent.body.HasBuff(RoR2Content.Buffs.ClayGoo))
                                        {
                                            OilExplosion.Explode(characterBody, healthComponent.body, this.crit, false);
                                        }
                                    }
                                    else
                                    {
                                        if (healthComponent.body.HasBuff(RoR2Content.Buffs.OnFire) || healthComponent.body.HasBuff(RoR2Content.Buffs.AffixRed))
                                        {
                                            ignate();
                                        }
                                        else if (!healthComponent.body.HasBuff(RoR2Content.Buffs.ClayGoo))
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
            RaycastHit[] array = Physics.SphereCastAll(body.corePosition, 1.5f * radius, Vector3.up, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
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
            if (onFire) return;
            RaycastHit[] array = Physics.SphereCastAll(body.corePosition, 1.5f * radius, Vector3.up, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
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
    }
}