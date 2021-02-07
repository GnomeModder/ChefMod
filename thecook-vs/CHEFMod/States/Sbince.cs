using System;
using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using ChefMod;

namespace EntityStates.Chef
{
    class Sbince : BaseBoostedSkillState
    {
        public float baseDuration = 0.5f;
        public float throwTime = 0.36f;

        private float duration;
        private bool hasThrown;

        private float pi = 3.14159f;
        private int intensity = chefPlugin.minceVerticalIntensity.Value;
        public override void OnEnter() {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;

            base.PlayAnimation("Gesture, Override", "Primary", "PrimaryCleaver.playbackRate", duration);

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
                    damage = base.characterBody.damage * (1f / (chefPlugin.minceHorizontalIntensity.Value + intensity)),
                    force = 50f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    target = null,
                    speedOverride = 16f,
                    fuseOverride = -1f
                };

                for (int i = -1 * intensity; i <= intensity; i++) {
                    float phi = 0;
                    if (intensity != 0) phi = i * (1f / (2f * intensity)) * pi;
                    float r = Mathf.Cos(phi);
                    int circum = Mathf.Max(1, Mathf.FloorToInt(chefPlugin.minceHorizontalIntensity.Value * pi * 2 * r));
                    for (int j = 0; j < circum; j++) {
                        float theta = 2 * pi * ((float)j / (float)circum);
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
            return InterruptPriority.Skill;
        }
    }
}