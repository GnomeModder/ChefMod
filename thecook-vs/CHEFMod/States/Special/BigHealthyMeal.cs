using System;
using ChefMod;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EntityStates.Chef
{
    class Meal : BaseSkillState
    {
        private bool boostPrimary = false;
        private bool boostSecondary = false;
        private bool boostUtility = false;
        public override void OnEnter()
        {
            if (skillLocator.primary.baseSkill == chefPlugin.primaryDef)
            {
                base.skillLocator.primary.SetSkillOverride(this, chefPlugin.boostedPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                boostPrimary = true;
            }
            if (skillLocator.primary.baseSkill == chefPlugin.altPrimaryDef)
            {
                base.skillLocator.primary.SetSkillOverride(this, chefPlugin.boostedAltPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                boostPrimary = true;
            }
            if (skillLocator.secondary.baseSkill == chefPlugin.secondaryDef)
            {
                base.skillLocator.secondary.SetSkillOverride(this, chefPlugin.boostedSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                boostSecondary = true;
            }
            if (skillLocator.secondary.baseSkill == chefPlugin.altSecondaryDef)
            {
                base.skillLocator.secondary.SetSkillOverride(this, chefPlugin.boostedAltSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                boostSecondary = true;
            }
            if (skillLocator.utility.baseSkill == chefPlugin.utilityDef)
            {
                base.skillLocator.utility.SetSkillOverride(this, chefPlugin.boostedUtilityDef, GenericSkill.SkillOverridePriority.Contextual);
                boostUtility = true;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            int maxStock = (boostPrimary ? skillLocator.primary.maxStock : 0) + (boostSecondary ? skillLocator.secondary.maxStock : 0) + (boostUtility ? skillLocator.utility.maxStock : 0);
            int currentStock = (boostPrimary ? skillLocator.primary.stock : 0) + (boostSecondary ? skillLocator.secondary.stock : 0) + (boostUtility ? skillLocator.utility.stock : 0);
            if ((currentStock) < maxStock && base.isAuthority)
            {
                NextState();
                return;
            }
        }

        public virtual void NextState()
        {
            this.outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            if (boostPrimary)
            {
                if (skillLocator.primary.baseSkill == chefPlugin.primaryDef)
                {
                    base.skillLocator.primary.UnsetSkillOverride(this, chefPlugin.boostedPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                }
                if (skillLocator.primary.baseSkill == chefPlugin.altPrimaryDef)
                {
                    base.skillLocator.primary.UnsetSkillOverride(this, chefPlugin.boostedAltPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                }
            }
            if (boostSecondary)
            {
                if (skillLocator.secondary.baseSkill == chefPlugin.secondaryDef)
                {
                    base.skillLocator.secondary.UnsetSkillOverride(this, chefPlugin.boostedSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                }
                if (skillLocator.secondary.baseSkill == chefPlugin.altSecondaryDef)
                {
                    base.skillLocator.secondary.UnsetSkillOverride(this, chefPlugin.boostedAltSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                }
            }
            if (boostUtility)
            {
                base.skillLocator.utility.UnsetSkillOverride(this, chefPlugin.boostedUtilityDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}