using System;
using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using ChefMod;

namespace EntityStates.Chef
{
    class Slice : BaseSkillState
    {
        private bool hasThrown;
        private bool hasReturned = false;
        //private HurtBox victim;

        private ChildLocator childLocator;
        //private HuntressTracker tracker;

        public override void OnEnter()
        {
            base.OnEnter();

            //tracker = base.characterBody.GetComponent<HuntressTracker>();
            //victim = tracker.GetTrackingTarget();

            childLocator = base.GetModelChildLocator();

            CoomerangProjectile.Returned += setReturned;

            base.StartAimMode(2f, false);

            base.PlayAnimation("Gesture, Override", "AltPrimary");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (hasReturned) // || !victim)
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (!hasThrown)
            {
                hasThrown = true;
                Throw();
            }
        }


        private void Throw()
        {
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                Vector3 right = new Vector3(aimRay.direction.z, 0, -1 * aimRay.direction.x).normalized;

                Vector3 shoulderPos = childLocator.FindChild("RightShoulder").position;
                //Vector3 difference = victim.transform.position - shoulderPos;
                chefPlugin.knifePrefab.GetComponent<CoomerangProjectile>().shoulder = childLocator.FindChild("RightShoulder");

                FireProjectileInfo info = new FireProjectileInfo()
                {
                    projectilePrefab = chefPlugin.knifePrefab,
                    position = shoulderPos + aimRay.direction,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction), //Util.QuaternionSafeLookRotation(difference),
                    owner = base.gameObject,
                    damage = base.characterBody.damage * 0.90f,
                    force = (1.5f + base.attackSpeedStat) * 1.5f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    //target = victim.gameObject,
                    speedOverride = 160f,
                    fuseOverride = -1f
                };

                childLocator.FindChild("RightShoulder").gameObject.SetActive(false);

                ProjectileManager.instance.FireProjectile(info);

                Util.PlaySound("CleaverThrow", base.gameObject);
            }
        }

        public override void OnExit()
        {
            childLocator.FindChild("RightShoulder").gameObject.SetActive(true);
            CoomerangProjectile.Returned -= setReturned;
            base.PlayAnimation("Gesture, Override", "AltPrimaryEnd");

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        private void setReturned()
        {
            hasReturned = true;
        }
    }
}