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
        public float baseDuration = 0.1f;
        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                base.StartAimMode(0.2f, false);

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

                Util.PlaySound("DIng", base.gameObject);
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
            skillLocator.primary.SetBaseSkill(chefPlugin.primaryDef);
            skillLocator.secondary.SetBaseSkill(chefPlugin.secondaryDef);
            skillLocator.utility.SetBaseSkill(chefPlugin.utilityDef);

            Util.PlaySound("Fireball", base.gameObject);
            Util.PlaySound("DIng", base.gameObject);

            //skillLocator.secondary.RunRecharge(chefPlugin.secondaryDef.baseRechargeInterval);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}