using System;
using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using ChefMod;

namespace EntityStates.Chef
{
    class Mince2 : BaseState
    {
        public float baseDuration = 0.5f;
        public float throwTime = 0.36f;

        private float duration;
        private bool hasThrown;

        private int intensity = 2;
        public override void OnEnter() {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;

            base.PlayAnimation("Gesture, Override", "PrimaryBoosted", "PrimaryCleaver.playbackRate", duration);

            base.StartAimMode(2f, false);
        }

        private void Throw() {
            if (base.isAuthority) {
                var coom = chefPlugin.cleaverPrefab.GetComponent<CoomerangProjectile>();
                coom.fieldComponent = characterBody.GetComponent<FieldComponent>();
                coom.followRet = false;


                FireProjectileInfo info = new FireProjectileInfo() {
                    projectilePrefab = ChefMod.chefPlugin.cleaverPrefab,
                    position = characterBody.corePosition,
                    owner = base.gameObject,
                    damage = base.damageStat * Cleaver.damageCoefficient,
                    force = 50f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    target = null,
                    speedOverride = 16f,
                    fuseOverride = -1f
                };

                for (int i = -1 * intensity; i <= intensity; i++) {
                    float phi = 0;
                    if (intensity != 0) phi = i * (1f / (2f * intensity)) * Mathf.PI;
                    float r = Mathf.Cos(phi);
                    int circum = Mathf.Max(1, Mathf.FloorToInt(Mathf.PI * 2f * r));
                    for (int j = 0; j < circum; j++) {
                        float theta = 2 * Mathf.PI * ((float)j / (float)circum);
                        Vector3 direction = new Vector3(r * Mathf.Cos(theta), Mathf.Sin(phi), r * Mathf.Sin(theta));

                        info.rotation = Util.QuaternionSafeLookRotation(direction);

                        ProjectileManager.instance.FireProjectile(info);
                    }
                }
            }

            Util.PlaySound("CleaverThrow", base.gameObject);
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