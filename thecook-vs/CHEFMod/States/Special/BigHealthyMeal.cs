using System;
using ChefMod;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EntityStates.Chef
{
    class Meal : BaseSkillState
    {
        public override void OnEnter()
        {
            if (skillLocator.primary.baseSkill == chefPlugin.primaryDef)
            {
                base.skillLocator.primary.SetSkillOverride(this, chefPlugin.boostedPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            if (skillLocator.primary.baseSkill == chefPlugin.altPrimaryDef)
            {
                base.skillLocator.primary.SetSkillOverride(this, chefPlugin.boostedAltPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            if (skillLocator.secondary.baseSkill == chefPlugin.secondaryDef)
            {
                base.skillLocator.secondary.SetSkillOverride(this, chefPlugin.boostedSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            if (skillLocator.secondary.baseSkill == chefPlugin.altSecondaryDef)
            {
                base.skillLocator.secondary.SetSkillOverride(this, chefPlugin.boostedAltSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            base.skillLocator.utility.SetSkillOverride(this, chefPlugin.boostedUtilityDef, GenericSkill.SkillOverridePriority.Contextual);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((skillLocator.primary.stock + skillLocator.secondary.stock + skillLocator.utility.stock) < 3 && base.isAuthority)
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
            if (skillLocator.primary.baseSkill == chefPlugin.primaryDef)
            {
                base.skillLocator.primary.UnsetSkillOverride(this, chefPlugin.boostedPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            if (skillLocator.primary.baseSkill == chefPlugin.altPrimaryDef)
            {
                base.skillLocator.primary.UnsetSkillOverride(this, chefPlugin.boostedAltPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            if (skillLocator.secondary.baseSkill == chefPlugin.secondaryDef)
            {
                base.skillLocator.secondary.UnsetSkillOverride(this, chefPlugin.boostedSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            if (skillLocator.secondary.baseSkill == chefPlugin.altSecondaryDef)
            {
                base.skillLocator.secondary.UnsetSkillOverride(this, chefPlugin.boostedAltSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            base.skillLocator.utility.UnsetSkillOverride(this, chefPlugin.boostedUtilityDef, GenericSkill.SkillOverridePriority.Contextual);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}