using System;
using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using ChefMod;

namespace EntityStates.Chef
{
    class Cleaver : BaseSkillState
    {
        public float baseDuration = 0.5f;
        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

                chefPlugin.cleaverPrefab.GetComponent<CoomerangProjectile>().fieldComponent = characterBody.GetComponent<FieldComponent>();

                FireProjectileInfo info = new FireProjectileInfo()
                {
                    projectilePrefab = ChefMod.chefPlugin.cleaverPrefab,
                    position = aimRay.origin + 1.5f * aimRay.direction,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction) * Quaternion.FromToRotation(Vector3.left, Vector3.up),
                    owner = base.gameObject,
                    damage = base.characterBody.damage / 5f,
                    force = 50f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    target = null,
                    speedOverride = 16f,
                    fuseOverride = -1f
                };

                ProjectileManager.instance.FireProjectile(info);
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
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
