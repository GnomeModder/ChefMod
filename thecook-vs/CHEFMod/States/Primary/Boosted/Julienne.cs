using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using ChefMod.Components;

namespace EntityStates.Chef
{
    public class Julienne : BaseState
    {
        public static GameObject projectilePrefab;
        public static float damageCoefficient = 1.3f;

        public static float fireInterval = 0.08f;
        private float stopwatch = fireInterval;
        private int stabcount;

        public override void OnEnter()
        {
            base.OnEnter();

            stabcount = (int)(16f * attackSpeedStat);

            childLocator = base.GetModelChildLocator();
            rightShoulder = childLocator.FindChild("RightShoulder");

            base.StartAimMode(2f, false);

            base.PlayAnimation("Gesture, Override", "AltPrimary");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (stabcount > 0)
            {
                stopwatch += Time.fixedDeltaTime;
                float scaledFireInterval = fireInterval / this.attackSpeedStat;
                if (stopwatch > scaledFireInterval)
                {
                    stopwatch -= fireInterval / scaledFireInterval;
                    stabcount--;
                    Throw();
                }
            }
            else
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void Throw()
        {
            Util.PlaySound("Play_ChefMod_Cleaver_Throw", base.gameObject);
            if (!knifeThrown)
            {
                knifeThrown = true;
                rightShoulder.gameObject.SetActive(false);
            }
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                FireProjectileInfo info = new FireProjectileInfo()
                {
                    projectilePrefab = Julienne.projectilePrefab,
                    position = aimRay.origin,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction), //Util.QuaternionSafeLookRotation(difference),
                    owner = base.gameObject,
                    damage = this.damageStat * Slice.damageCoefficient,
                    force = (1.5f + this.attackSpeedStat) * 3f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    //target = victim.gameObject,
                    speedOverride = 160f,
                    fuseOverride = -1f
                };
                ProjectileManager.instance.FireProjectile(info);
            }
        }


        public override void OnExit()
        {
            rightShoulder.gameObject.SetActive(true);
            base.PlayAnimation("Gesture, Override", "AltPrimaryEnd");
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        private bool knifeThrown = false;
        private Transform rightShoulder;
        private ChildLocator childLocator;
    }
}
