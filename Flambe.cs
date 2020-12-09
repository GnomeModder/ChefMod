using System;
using ChefMod;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Chef
{
    class Flambe : BaseSkillState
    {
        public float damageCoefficient = 5;
        public float maxDistance = 25f;
        public float baseDuration = 0.01f;
        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

                FireProjectileInfo info = new FireProjectileInfo()
                {
                    projectilePrefab = chefPlugin.flamballPrefab,
                    position = aimRay.origin + 1.5f * aimRay.direction,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction) * Quaternion.FromToRotation(Vector3.left, Vector3.up),
                    owner = base.gameObject,
                    damage = base.characterBody.damage * 5f,
                    force = 50f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    target = null,
                    speedOverride = 100f,
                    fuseOverride = -1f
                };

                ProjectileManager.instance.FireProjectile(info);

                Vector3 scale = new Vector3(0.25f, 1, 0.25f);
                RaycastHit[] array = Physics.BoxCastAll(aimRay.origin + maxDistance * 0.5f * aimRay.direction, 0.5f * maxDistance * scale, aimRay.direction, Quaternion.identity, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal); ;
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
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            skillLocator.secondary.SetBaseSkill(chefPlugin.secondaryDef);
            skillLocator.utility.SetBaseSkill(chefPlugin.utilityDef);

            //skillLocator.secondary.RunRecharge(chefPlugin.secondaryDef.baseRechargeInterval);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}