using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using R2API.Networking;

namespace ChefMod.Components
{
    public class OilController : NetworkBehaviour
    {
        public static GameObject firePrefab;
        public static GameObject oilDecalPrefab;
        public static float oilLifetime = 30f;
        public static float burnLifetime = 10f;
        public static float damageInterval = 0.5f;
        public static float procCoefficient = 0.2f;
        public static float damageCoefficient = 0.25f;
        public static GameObject ExplosionEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/IgniteExplosionVFX");

        private float stopwatch = 0f;
        private float damageStopwatch = 0f;
        public bool boosted = false;
        public bool pendingIgnite = false;

        public bool onGround = false;

        [SyncVar]
        public bool onFire = false;

        [SyncVar]
        private bool shouldDie = false;        

        private CharacterBody ownerBody;
        private CharacterBody myBody;
        private Rigidbody rig;
        private GameObject owner;
        private TeamIndex teamIndex;
        private bool crit;
        private GameObject oilBallInstance = null;
        private GameObject oilDecalInstance = null;
        private GameObject fireInstance = null;

        public void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (NetworkServer.active)
            {
                damageStopwatch += Time.fixedDeltaTime;
                if (damageStopwatch > damageInterval)
                {
                    damageStopwatch -= damageInterval;
                    TickDamage();
                }
                shouldDie = (stopwatch >= oilLifetime && !onFire) || (onFire && stopwatch >= burnLifetime);
            }

            if (!onGround && rig.velocity.magnitude < 1f)
            {
                onGround = true;
                rig.isKinematic = true;
                Quaternion randy = new Quaternion(0, 1, 0, UnityEngine.Random.Range(0, 360f));
                Destroy(oilBallInstance);
                oilDecalInstance = Instantiate(oilDecalPrefab, this.transform.position - Vector3.up, randy);

                if (NetworkServer.active)
                {
                    if (pendingIgnite)
                    {
                        Ignite();
                        IgniteNearby();
                    }
                    else
                    {
                        FindNearby();
                    }
                }
            }
            if (onFire && onGround)
            {
                if (!fireInstance)
                {
                    fireInstance = Instantiate(firePrefab, this.transform.position - Vector3.up, Quaternion.identity);
                }
            }

            if (shouldDie)
            {
                Destroy(myBody.gameObject);
            }
        }

        public void TickDamage()
        {
            if (!NetworkServer.active) return;
            float damage = ownerBody.damage * damageCoefficient;
            HashSet<GameObject> hashSet = new HashSet<GameObject>();
            RoR2.TeamIndex attackerTeamIndex = teamIndex;

            hashSet.Add(myBody.gameObject);
            hashSet.Add(this.owner);

            Vector3 dims = new Vector3(1, 0.2f, 1);
            RaycastHit[] array = Physics.BoxCastAll(myBody.footPosition, dims * 10f, Vector3.forward, Quaternion.identity, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
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
                                    if (healthComponent.body && healthComponent.body.baseNameToken != "OilBeetle")
                                    {
                                        if (onFire)
                                        {
                                            DamageInfo di = new DamageInfo
                                            {
                                                position = healthComponent.body.corePosition,
                                                attacker = this.owner,
                                                inflictor = base.gameObject,
                                                crit = this.crit,
                                                damage = damage,
                                                damageColorIndex = DamageColorIndex.Default,
                                                damageType = DamageType.Generic,
                                                force = Vector3.zero,
                                                procCoefficient = procCoefficient,
                                                procChainMask = default(ProcChainMask)
                                            };
                                            /*di.AddModdedDamageType(chefPlugin.chefSear);
                                            if (boosted)
                                            {
                                                di.AddModdedDamageType(chefPlugin.chefFireballOnHit);
                                            }*/
                                            NetworkingHelpers.DealDamage(di, component, true, true, false);
                                            if (!healthComponent.body.HasBuff(RoR2Content.Buffs.OnFire))
                                            {
                                                DotController.InflictDot(healthComponent.gameObject, owner, DotController.DotIndex.Burn);
                                            }
                                            if (healthComponent.body.HasBuff(RoR2Content.Buffs.ClayGoo))
                                            {
                                                foreach (CharacterBody.TimedBuff t in healthComponent.body.timedBuffs)
                                                {
                                                    if (t.buffIndex == RoR2Content.Buffs.ClayGoo.buffIndex && t.timer > 2f)
                                                    {
                                                        OilExplosion.Explode(ownerBody, healthComponent.body, this.crit, boosted);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (onGround && !pendingIgnite)// && !healthComponent.body.HasBuff(RoR2Content.Buffs.ClayGoo)
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
        }

        public void OnDestroy()
        {
            Destroy(oilDecalInstance);
            Destroy(fireInstance);
            Destroy(oilBallInstance);
        }

        public void Start()
        {
            myBody = GetComponent<CharacterBody>();
            if (!myBody)
            {
                Chat.AddMessage("ping gnome. no body");
                Destroy(this.gameObject);
            }
            myBody.healthComponent.godMode = true;
            myBody.baseMaxHealth = 99999999f;

            var direction = this.transform.root.GetComponentInChildren<CharacterDirection>();
            foreach (var thisItem in direction.modelAnimator.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                thisItem.gameObject.SetActive(false);
            }
            foreach (var thisItem in direction.modelAnimator.GetComponentsInChildren<MeshRenderer>())
            {
                thisItem.gameObject.SetActive(false);
            }

            oilBallInstance = InstantiateAsset("OilBall");
            rig = GetComponent<Rigidbody>();
            rig.velocity += Vector3.down;

            var projCont = this.gameObject.GetComponent<ProjectileController>();
            var projDamg = this.gameObject.GetComponent<ProjectileDamage>();
            var teamFilt = this.gameObject.GetComponent<TeamFilter>();

            this.owner = projCont.owner;
            this.teamIndex = teamFilt.teamIndex;
            this.crit = projDamg.crit;

            if (owner)
            {
                ownerBody = owner.GetComponent<CharacterBody>();
            }

            FindNearby();
        }

        public void Ignite()
        {
            if (NetworkServer.active && !onFire)
            {
                if (!onGround)
                {
                    pendingIgnite = true;
                    return;
                }
                onFire = true;
                stopwatch = 0f;
                Explode();
                IgniteNearby();
            }
        }

        private void IgniteNearby()
        {
            RaycastHit[] array = Physics.SphereCastAll(myBody.corePosition, 15f, Vector3.up, 15f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
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
                            OilController fire = healthComponent.body.GetComponent<OilController>();
                            if (fire)
                            {
                                if (boosted)
                                {
                                    fire.boosted = true;
                                }
                                fire.Ignite();
                            }
                        }
                    }
                }
            }
        }

        private void FindNearby()
        {
            if (onFire) return;

            RaycastHit[] array = Physics.SphereCastAll(myBody.corePosition, 15f, Vector3.up, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
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
                            OilController fire = healthComponent.body.GetComponent<OilController>();
                            if (fire)
                            {
                                // Some lag can be attributed to having multiple stacking in one place, so I'm going to extend the lifetime of nearby existing ones
                                // to cut down on the existing gameobjects in the world.

                                // If there are any oil splats within 4 meters, then we'll extend their duration,
                                // and delete ourselves by setting our stopwatch to the max duration we can.
                                var distanceToNearbyOil = Vector3.Distance(myBody.corePosition, healthComponent.body.corePosition);
                                if (distanceToNearbyOil <= 4f)
                                {
                                    if (fire.onGround)
                                    {
                                        fire.stopwatch = 0;
                                        stopwatch = oilLifetime;
                                    }

                                } else // If they're further than 4 meters away, then we can ignite ourselves.
                                {
                                    if (fire.onFire)
                                    {
                                        this.Ignite();
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        private void Explode()
        {
            if (!NetworkServer.active) return;
            Vector3 corePosition = myBody.corePosition;
            EffectManager.SpawnEffect(ExplosionEffectPrefab, new EffectData
            {
                origin = corePosition,
                scale = 10f,
                rotation = Util.QuaternionSafeLookRotation(Vector3.up)
            }, true);
        }

        private GameObject InstantiateAsset(String asset)
        {
            GameObject prefab;
            prefab = Instantiate(Assets.chefAssetBundle.LoadAsset<GameObject>(asset));
            //var direction = this.transform.root.GetComponentInChildren<CharacterDirection>();
            prefab.transform.position = myBody.footPosition - 2.5f * Vector3.up;
            prefab.transform.SetParent(this.transform);
            prefab.transform.localScale *= 2.5f;
            return prefab;
        }
    }
}
