using System;
using ChefMod;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EntityStates.Chef
{
    class Meal : BaseSkillState
    {
        public float duration = 0.1f;
        public override void OnEnter()
        {
            if (skillLocator.primary.baseSkill == chefPlugin.primaryDef)
            {
                skillLocator.primary.SetBaseSkill(chefPlugin.boostedPrimaryDef);
            }
            if (skillLocator.primary.baseSkill == chefPlugin.altPrimaryDef)
            {
                skillLocator.primary.SetBaseSkill(chefPlugin.boostedAltPrimaryDef);
            }
            if (skillLocator.secondary.baseSkill == chefPlugin.secondaryDef)
            {
                skillLocator.secondary.SetBaseSkill(chefPlugin.boostedSecondaryDef);
            }
            if (skillLocator.secondary.baseSkill == chefPlugin.altSecondaryDef)
            {
                skillLocator.secondary.SetBaseSkill(chefPlugin.boostedAltSecondaryDef);
            }
            skillLocator.utility.SetBaseSkill(chefPlugin.boostedUtilityDef);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            skillLocator.special.enabled = false;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}