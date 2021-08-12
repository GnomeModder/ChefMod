using System;
using ChefMod;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EntityStates.Chef
{
    class Meal : BaseSkillState
    {
        private int boostPrimary = 0;
        private int boostSecondary = 0;
        private int boostUtility = 0;
        public override void OnEnter()
        {
            if (skillLocator.primary.baseSkill == chefPlugin.primaryDef)
            {
                base.skillLocator.primary.SetSkillOverride(this, chefPlugin.boostedPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                boostPrimary = 1;
            }
            if (skillLocator.primary.baseSkill == chefPlugin.altPrimaryDef)
            {
                base.skillLocator.primary.SetSkillOverride(this, chefPlugin.boostedAltPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                boostPrimary = 1;
            }
            if (skillLocator.secondary.baseSkill == chefPlugin.secondaryDef)
            {
                base.skillLocator.secondary.SetSkillOverride(this, chefPlugin.boostedSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                boostSecondary = 1;
            }
            if (skillLocator.secondary.baseSkill == chefPlugin.altSecondaryDef)
            {
                base.skillLocator.secondary.SetSkillOverride(this, chefPlugin.boostedAltSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                boostSecondary = 1;
            }
            if (skillLocator.utility.baseSkill == chefPlugin.utilityDef)
            {
                base.skillLocator.utility.SetSkillOverride(this, chefPlugin.boostedUtilityDef, GenericSkill.SkillOverridePriority.Contextual);
                boostUtility = 1;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            int maxStock = (boostPrimary * skillLocator.primary.maxStock) + (boostSecondary * skillLocator.secondary.maxStock) + (boostUtility * skillLocator.utility.maxStock);
            int currentStock = (boostPrimary * skillLocator.primary.stock) + (boostSecondary * skillLocator.secondary.stock) + (boostUtility * skillLocator.utility.stock);
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
            if (boostPrimary != 0)
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
            if (boostSecondary != 0)
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
            if (boostUtility != 0)
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