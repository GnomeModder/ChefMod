using System;
using System.Collections.Generic;
using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using ChefMod;
using System.Runtime.InteropServices;

namespace EntityStates.Chef
{
    class MinceHoming : BaseState
    {
        public float baseDuration = 0.5f;   
        public float throwTime = 0.36f;

        private float duration;
        private bool hasThrown;

        private List<CharacterBody> victimBodyList = new List<CharacterBody>();

        public override void OnEnter() {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;

            base.PlayAnimation("Gesture, Override", "PrimaryBoosted", "PrimaryCleaver.playbackRate", duration);

            base.StartAimMode(2f, false);
        }

        private void Throw() 
        {
            if (base.isAuthority) {
                Ray aimRay = base.GetAimRay();
                Vector3 right = new Vector3(aimRay.direction.z, 0, -1 * aimRay.direction.x).normalized;

                var coom = Cleaver.projectilePrefab.GetComponent<CoomerangProjectile>();
                coom.fieldComponent = characterBody.GetComponent<FieldComponent>();
                coom.followRet = false;

                getHitList(characterBody.corePosition, 40f);

                foreach (CharacterBody victim in victimBodyList) {
                    Vector3 direction = victim.corePosition - characterBody.corePosition;
                    direction = direction.normalized;

                    FireProjectileInfo info = new FireProjectileInfo() {
                        projectilePrefab = Cleaver.projectilePrefab,
                        position = characterBody.corePosition + 1.5f * direction,// + 1.5f * Vector3.up + 2 * right,
                        rotation = Util.QuaternionSafeLookRotation(direction),// * Quaternion.FromToRotation(Vector3.left, Vector3.up),
                        owner = base.gameObject,
                        damage = base.characterBody.damage * 1.5f,
                        force = 50f,
                        crit = base.RollCrit(),
                        damageColorIndex = DamageColorIndex.Default,
                        target = null,
                        speedOverride = 16f,
                        fuseOverride = -1f
                    };

                    ProjectileManager.instance.FireProjectile(info);
                    Util.PlaySound("CleaverThrow", base.gameObject);
                }

                //int dale = 3 - victimBodyList.Count;
                //Vector3 split = new Vector3(aimRay.direction.z, aimRay.direction.y, -1 * aimRay.direction.x).normalized;
                //Vector3 aimer = 21 * aimRay.direction - 3 * split;

                //for (int i = 0; i < dale; i++) {
                //    float numbedr = 12 / (dale + 1);
                //    Vector3 direction = (i * numbedr * split) + aimer;
                //    direction = direction.normalized;

                //    FireProjectileInfo info = new FireProjectileInfo() {
                //        projectilePrefab = ChefMod.chefPlugin.cleaverPrefab,
                //        position = characterBody.corePosition + 1.5f * direction,// + 1.5f * Vector3.up + 2 * right,
                //        rotation = Util.QuaternionSafeLookRotation(direction),// * Quaternion.FromToRotation(Vector3.left, Vector3.up),
                //        owner = base.gameObject,
                //        damage = base.characterBody.damage * 1.5f,
                //        force = 50f,
                //        crit = base.RollCrit(),
                //        damageColorIndex = DamageColorIndex.Default,
                //        target = null,
                //        speedOverride = 16f,
                //        fuseOverride = -1f
                //    };

                //    ProjectileManager.instance.FireProjectile(info);
                //    Util.PlaySound("CleaverThrow", base.gameObject);
                //}
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

        private void getHitList(Vector3 position, float radius)
        {
            Collider[] array = Physics.OverlapSphere(position, radius, LayerIndex.defaultLayer.mask);
            int num = 0;
            int num2 = 0;
            while (num < array.Length && num2 < 12)
            {
                HealthComponent component = array[num].GetComponent<HealthComponent>();
                if (component)
                {
                    TeamComponent component2 = component.GetComponent<TeamComponent>();
                    if (component2.teamIndex != characterBody.teamComponent.teamIndex)
                    {
                        this.AddToList(component.body);
                        num2++;
                    }
                }
                num++;
            }
        }

        private void AddToList(CharacterBody component)
        {
            if (!this.victimBodyList.Contains(component))
            {
                this.victimBodyList.Add(component);
            }
        }
    }
}