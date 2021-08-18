using System;
using System.Collections.Generic;
using System.Text;
using ChefMod.Components;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Chef
{
    public class Slice : BaseState
    {
        public static GameObject projectilePrefab;
        public static float damageCoefficient = 1.2f;
        public bool returned = false;
        public bool knifeThrown = false;
        public KnifeHandler knifeHandler;
        private Transform rightShoulder;

        private ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            knifeHandler = base.GetComponent<KnifeHandler>();
            //tracker = base.characterBody.GetComponent<HuntressTracker>();
            //victim = tracker.GetTrackingTarget();
            childLocator = base.GetModelChildLocator();
            rightShoulder = childLocator.FindChild("RightShoulder");
            base.StartAimMode(2f, false);
            base.PlayAnimation("Gesture, Override", "AltPrimary");
            ThrowKnife();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                if (knifeThrown && !knifeHandler.knifeThrown)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
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

        private void ThrowKnife()
        {
            Util.PlaySound("Play_ChefMod_Cleaver_Throw", base.gameObject);
            rightShoulder.gameObject.SetActive(false);
            if (base.isAuthority)
            {
                knifeHandler.ThrowKnife();
                knifeThrown = true;
                Ray aimRay = base.GetAimRay();

                FireProjectileInfo info = new FireProjectileInfo()
                {
                    projectilePrefab = Slice.projectilePrefab,
                    position = aimRay.origin,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction), //Util.QuaternionSafeLookRotation(difference),
                    owner = base.gameObject,
                    damage = this.damageStat * Slice.damageCoefficient,
                    force = (1.5f + this.attackSpeedStat) * 1.5f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    //target = victim.gameObject,
                    speedOverride = 160f,
                    fuseOverride = -1f
                };
                ProjectileManager.instance.FireProjectile(info);
            }
        }
    }
}
