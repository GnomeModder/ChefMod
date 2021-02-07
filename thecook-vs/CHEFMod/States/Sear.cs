using System;
using ChefMod;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Chef
{
    class Sear : BaseSkillState
    {
        public float damageCoefficient = 5;
        public float maxDistance = 25f;
        public float baseDuration = 1f;
        public float throwTime = 0.25f;

        private float duration;
        private bool hasThrown;
        public override void OnEnter() {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;

            base.PlayAnimation("Fullbody, Override", "Secondary", "Secondary.playbackRate", duration);
            base.PlayAnimation("Gesture, Override", "Secondary", "Secondary.playbackRate", duration);

            base.StartAimMode(2f, false);
        }

        private void Throw() {
            if (base.isAuthority) {
                Ray aimRay = base.GetAimRay();

                FireProjectileInfo info = new FireProjectileInfo() {
                    projectilePrefab = chefPlugin.foirballPrefab,
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

            if (fixedAge > duration * throwTime && !hasThrown) {
                hasThrown = true;
                Throw();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            Util.PlaySound("Fireball", base.gameObject);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (hasThrown)
                return InterruptPriority.Any;

            return InterruptPriority.PrioritySkill;
        }
    }
}