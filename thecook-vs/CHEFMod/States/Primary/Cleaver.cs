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
        public static GameObject projectilePrefab;
        public static float damageCoefficient = 1.5f;
        public float baseDuration = 0.52f;
        public float throwTime = 0.38f; //attack comes out .1976s

        private float duration;
        private bool hasThrown;

        private ChildLocator childLocator;

        public override void OnEnter() {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;

            base.PlayCrossfade("Gesture, Override", "Primary", "PrimaryCleaver.playbackRate", duration, 0.05f);

            childLocator = base.GetModelChildLocator();

            base.StartAimMode(2f, false);

            //UnityEngine.Object.Instantiate<GameObject>(chefPlugin.firefab, characterBody.footPosition, Quaternion.identity);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(fixedAge > duration * throwTime && !hasThrown) {
                hasThrown = true;
                Throw();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }


        private void Throw() {

            if (base.isAuthority) {
                Ray aimRay = base.GetAimRay();
                Vector3 right = new Vector3(aimRay.direction.z, 0, -1 * aimRay.direction.x).normalized;

                /*var coom = projectilePrefab.GetComponent<CoomerangProjectile>();
                coom.fieldComponent = characterBody.GetComponent<FieldComponent>();
                coom.followRet = true;*/

                FireProjectileInfo info = new FireProjectileInfo() {
                    projectilePrefab = projectilePrefab,
                    position = aimRay.origin,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),// * Quaternion.FromToRotation(Vector3.left, Vector3.up),
                    owner = base.gameObject,
                    damage = base.characterBody.damage * Cleaver.damageCoefficient,
                    force = 50f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    target = null,
                    speedOverride = 16f,
                    fuseOverride = -1f
                };

                childLocator.FindChild("Cleaver").gameObject.SetActive(false);

                ProjectileManager.instance.FireProjectile(info);
            }

            Util.PlaySound("Play_ChefMod_Cleaver_Throw", base.gameObject);
        }

        public override void OnExit()
        {
            childLocator.FindChild("Cleaver").gameObject.SetActive(true);

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
