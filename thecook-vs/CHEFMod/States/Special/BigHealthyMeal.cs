using System;
using ChefMod;
using EntityStates;
using IL.RoR2.Skills;
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
            public SkillStatus(int stock, float stopwatch)
            {
                this.stock = stock;
                this.stopwatch = stopwatch;
            }
        }

        public override void OnEnter()
        {
            GetModelChildLocator().FindChild("Hat").localScale = Vector3.one * 1.4f;

            if (playSound)
            {
                Util.PlaySound("Play_ChefMod_Special", base.gameObject);
            }

            if (base.isAuthority)
            {
                if (skillLocator.primary.baseSkill == ChefMod.ChefPlugin.primaryDef)
                {
                    primaryStatus = new SkillStatus(skillLocator.primary.stock, skillLocator.primary.rechargeStopwatch);
                    base.skillLocator.primary.SetSkillOverride(this, ChefMod.ChefPlugin.boostedPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                    boostPrimary = 1;
                }
                else if (skillLocator.primary.baseSkill == ChefMod.ChefPlugin.altPrimaryDef)
                {
                    primaryStatus = new SkillStatus(skillLocator.primary.stock, skillLocator.primary.rechargeStopwatch);
                    base.skillLocator.primary.SetSkillOverride(this, ChefMod.ChefPlugin.boostedAltPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                    boostPrimary = 1;
                }

                if (skillLocator.secondary.baseSkill == ChefMod.ChefPlugin.secondaryDef)
                {
                    secondaryStatus = new SkillStatus(skillLocator.secondary.stock, skillLocator.secondary.rechargeStopwatch);
                    base.skillLocator.secondary.SetSkillOverride(this, ChefMod.ChefPlugin.boostedSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                    boostSecondary = 1;
                }
                else if (skillLocator.secondary.baseSkill == ChefMod.ChefPlugin.altSecondaryDef)
                {
                    secondaryStatus = new SkillStatus(skillLocator.secondary.stock, skillLocator.secondary.rechargeStopwatch);
                    base.skillLocator.secondary.SetSkillOverride(this, ChefMod.ChefPlugin.boostedAltSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                    boostSecondary = 1;
                }

                if (skillLocator.utility.baseSkill == ChefMod.ChefPlugin.utilityDef)
                {
                    utilityStatus = new SkillStatus(skillLocator.utility.stock, skillLocator.utility.rechargeStopwatch);
                    base.skillLocator.utility.SetSkillOverride(this, ChefMod.ChefPlugin.boostedUtilityDef, GenericSkill.SkillOverridePriority.Contextual);
                    boostUtility = 1;
                }

                //Fix skill stocks
                base.skillLocator.primary.stock = base.skillLocator.primary.maxStock;
                base.skillLocator.secondary.stock = base.skillLocator.secondary.maxStock;
                base.skillLocator.utility.stock = base.skillLocator.utility.maxStock;
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
            GetModelChildLocator().FindChild("Hat").localScale = Vector3.one * 1f;

            if (base.isAuthority)
            {
                if (boostPrimary != 0)
                {
                    if (skillLocator.primary.baseSkill == ChefMod.ChefPlugin.primaryDef)
                    {
                        base.skillLocator.primary.UnsetSkillOverride(this, ChefMod.ChefPlugin.boostedPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                    }
                    else if (skillLocator.primary.baseSkill == ChefMod.ChefPlugin.altPrimaryDef)
                    {
                        base.skillLocator.primary.UnsetSkillOverride(this, ChefMod.ChefPlugin.boostedAltPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                    }
                    base.skillLocator.primary.stock = primaryStatus.stock;
                    base.skillLocator.primary.rechargeStopwatch = primaryStatus.stopwatch;
                }
                if (boostSecondary != 0)
                {
                    if (skillLocator.secondary.baseSkill == ChefMod.ChefPlugin.secondaryDef)
                    {
                        base.skillLocator.secondary.UnsetSkillOverride(this, ChefMod.ChefPlugin.boostedSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                    }
                    else if (skillLocator.secondary.baseSkill == ChefMod.ChefPlugin.altSecondaryDef)
                    {
                        base.skillLocator.secondary.UnsetSkillOverride(this, ChefMod.ChefPlugin.boostedAltSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                    }
                    base.skillLocator.secondary.stock = secondaryStatus.stock;
                    base.skillLocator.secondary.rechargeStopwatch = secondaryStatus.stopwatch;
                }
                if (boostUtility != 0)
                {
                    base.skillLocator.utility.UnsetSkillOverride(this, ChefMod.ChefPlugin.boostedUtilityDef, GenericSkill.SkillOverridePriority.Contextual);
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