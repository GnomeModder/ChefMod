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

        private SkillStatus primaryStatus, secondaryStatus, utilityStatus;

        public bool playSound = true;

        private class SkillStatus
        {
            public int stock;
            public float stopwatch;
            public SkillStatus(int st, float sw)
            {
                this.stock = st;
                this.stopwatch = sw;
            }
        }

        public override void OnEnter()
        {
            if (playSound)
            {
                Util.PlaySound("Play_ChefMod_Special", base.gameObject);
            }
            if (base.isAuthority)
            {
                if (skillLocator.primary.baseSkill == chefPlugin.primaryDef)
                {
                    primaryStatus = new SkillStatus(skillLocator.primary.stock, skillLocator.primary.rechargeStopwatch);
                    base.skillLocator.primary.SetSkillOverride(this, chefPlugin.boostedPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                    boostPrimary = 1;
                }
                if (skillLocator.primary.baseSkill == chefPlugin.altPrimaryDef)
                {
                    primaryStatus = new SkillStatus(skillLocator.primary.stock, skillLocator.primary.rechargeStopwatch);
                    base.skillLocator.primary.SetSkillOverride(this, chefPlugin.boostedAltPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                    boostPrimary = 1;
                }
                if (skillLocator.secondary.baseSkill == chefPlugin.secondaryDef)
                {
                    secondaryStatus = new SkillStatus(skillLocator.secondary.stock, skillLocator.secondary.rechargeStopwatch);
                    base.skillLocator.secondary.SetSkillOverride(this, chefPlugin.boostedSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                    boostSecondary = 1;
                }
                if (skillLocator.secondary.baseSkill == chefPlugin.altSecondaryDef)
                {
                    secondaryStatus = new SkillStatus(skillLocator.secondary.stock, skillLocator.secondary.rechargeStopwatch);
                    base.skillLocator.secondary.SetSkillOverride(this, chefPlugin.boostedAltSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                    boostSecondary = 1;
                }
                if (skillLocator.utility.baseSkill == chefPlugin.utilityDef)
                {
                    utilityStatus = new SkillStatus(skillLocator.utility.stock, skillLocator.utility.rechargeStopwatch);
                    base.skillLocator.utility.SetSkillOverride(this, chefPlugin.boostedUtilityDef, GenericSkill.SkillOverridePriority.Contextual);
                    boostUtility = 1;
                }
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
            if (base.isAuthority)
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
                    base.skillLocator.primary.stock = primaryStatus.stock;
                    base.skillLocator.primary.rechargeStopwatch = primaryStatus.stopwatch;
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
                    base.skillLocator.secondary.stock = secondaryStatus.stock;
                    base.skillLocator.secondary.rechargeStopwatch = secondaryStatus.stopwatch;
                }
                if (boostUtility != 0)
                {
                    base.skillLocator.utility.UnsetSkillOverride(this, chefPlugin.boostedUtilityDef, GenericSkill.SkillOverridePriority.Contextual);
                    base.skillLocator.utility.stock = utilityStatus.stock;
                    base.skillLocator.utility.rechargeStopwatch = utilityStatus.stopwatch;
                }
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}