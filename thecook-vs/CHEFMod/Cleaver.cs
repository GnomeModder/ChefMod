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
                Vector3 right = new Vector3(aimRay.direction.z, 0, -1 * aimRay.direction.x).normalized;
                base.StartAimMode(0.5f, false);

                var coom = chefPlugin.cleaverPrefab.GetComponent<CoomerangProjectile>();
                coom.fieldComponent = characterBody.GetComponent<FieldComponent>();
                coom.followRet = true;

                FireProjectileInfo info = new FireProjectileInfo()
                {
                    projectilePrefab = ChefMod.chefPlugin.cleaverPrefab,
                    position = aimRay.origin + 1.5f * aimRay.direction + 1.5f * Vector3.up + 2 * right,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),// * Quaternion.FromToRotation(Vector3.left, Vector3.up),
                    owner = base.gameObject,
                    damage = base.characterBody.damage * 0.25f,
                    force = 50f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    target = null,
                    speedOverride = 16f,
                    fuseOverride = -1f
                };

                ProjectileManager.instance.FireProjectile(info);
            }

            Util.PlaySound("CleaverThrow", base.gameObject);
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
