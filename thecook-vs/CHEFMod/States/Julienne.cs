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
    class Julienne : BaseBoostedSkillState
    {
        public float baseDuration = 0.5f;
        public float throwTime = 0.36f;

        private float duration;
        private bool hasThrown;

        private ChildLocator childLocator;

        private List<CharacterBody> victimBodyList = new List<CharacterBody>();

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;

            base.PlayAnimation("Gesture, Override", "PrimaryBoosted", "PrimaryCleaver.playbackRate", duration);

            childLocator = base.GetModelChildLocator();

            base.StartAimMode(2f, false);
        }

        private void Throw()
        {
            if (base.isAuthority)
            {
                getHitList(base.characterBody.corePosition, 40f);
                
                Vector3 shoulderPos = childLocator.FindChild("RightShoulder").position;
                FireProjectileInfo info = new FireProjectileInfo()
                {
                    projectilePrefab = ChefMod.chefPlugin.knifePrefab,
                    position = shoulderPos,
                    owner = base.gameObject,
                    damage = base.characterBody.damage * 6f,
                    force = base.attackSpeedStat * 1.5f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    speedOverride = 16f,
                    fuseOverride = -1f
                };
                
                foreach (CharacterBody victim in victimBodyList)
                {
                    Vector3 difference = victim.corePosition - shoulderPos;

                    info.rotation = Util.QuaternionSafeLookRotation(difference);
                    info.target = victim.gameObject;

                    ProjectileManager.instance.FireProjectile(info);
                    Util.PlaySound("CleaverThrow", base.gameObject);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge > duration * throwTime && !hasThrown)
            {
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
            return InterruptPriority.Frozen;
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