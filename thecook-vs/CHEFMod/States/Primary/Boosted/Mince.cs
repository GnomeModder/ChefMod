using System;
using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using ChefMod;

namespace EntityStates.Chef
{
    class Mince : BaseState
    {
        public float baseDuration = 0.5f;
        public float throwTime = 0.36f;

        private float duration;
        private bool hasThrown;

        public static int verticalIntensity = 2;
        public static float horizontalIntensity = 1f;

        public override void OnEnter() {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;

            base.PlayAnimation("Gesture, Override", "PrimaryBoosted", "PrimaryCleaver.playbackRate", duration);

            base.StartAimMode(2f, false);
        }

        private void Throw() {
            if (base.isAuthority) {
                /*var coom = Cleaver.projectilePrefab.GetComponent<CoomerangProjectile>();
                coom.fieldComponent = characterBody.GetComponent<FieldComponent>();
                coom.followRet = false;*/


                FireProjectileInfo info = new FireProjectileInfo() {
                    projectilePrefab = Cleaver.projectilePrefab,
                    position = characterBody.corePosition,
                    owner = base.gameObject,
                    //damage = base.characterBody.damage * (4f / (chefPlugin.minceHorizontolIntensity.Value + intensity)),
                    damage = base.damageStat * Cleaver.damageCoefficient,
                    force = 50f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    target = null,
                    speedOverride = 16f,
                    fuseOverride = -1f
                };

                Vector3 aimDirection = base.GetAimRay().direction;
                aimDirection.y = 0f;
                aimDirection.Normalize();
                float orientation = Mathf.Atan2(aimDirection.z, aimDirection.x);

                for (int i = -1 * verticalIntensity; i <= verticalIntensity; i++) {
                    float phi = 0f;
                    if (verticalIntensity != 0) phi = i * (1f / (2f * verticalIntensity)) * Mathf.PI;
                    float r = Mathf.Cos(phi);
                    int circum = Mathf.Max(1, Mathf.FloorToInt(horizontalIntensity * Mathf.PI * 2 * r));
                    for (int j = 0; j < circum; j++) {
                        float theta = orientation + 2 * Mathf.PI * ((float)j / (float)circum);
                        Vector3 direction = new Vector3(r * Mathf.Cos(theta), Mathf.Sin(phi), r * Mathf.Sin(theta));

                        info.rotation = Util.QuaternionSafeLookRotation(direction);

                        ProjectileManager.instance.FireProjectile(info);
                    }
                }
            }

            Util.PlaySound("Play_ChefMod_Cleaver_Throw", base.gameObject);
        }

        public override void FixedUpdate() 
        {

            if (fixedAge > duration * throwTime && !hasThrown) {
                hasThrown = true;
                Throw();
            }

            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            //skillLocator.primary.SetBaseSkill(chefPlugin.primaryDef);
            //if (skillLocator.secondary.baseSkill == chefPlugin.boostedSecondaryDef)
            //{
            //    skillLocator.secondary.SetBaseSkill(chefPlugin.secondaryDef);
            //}
            //if (skillLocator.secondary.baseSkill == chefPlugin.boostedAltSecondaryDef)
            //{
            //    skillLocator.secondary.SetBaseSkill(chefPlugin.altSecondaryDef);
            //}
            //skillLocator.utility.SetBaseSkill(chefPlugin.utilityDef);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}