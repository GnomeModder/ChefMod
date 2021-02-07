using ChefMod;

namespace EntityStates.Chef {
    public class BaseBoostedSkillState : BaseSkillState {

        public override void OnEnter() {
            base.OnEnter();

            skillLocator.primary.SetBaseSkill(chefPlugin.primaryDef);
            if (skillLocator.secondary.baseSkill == chefPlugin.boostedSecondaryDef) {
                skillLocator.secondary.SetBaseSkill(chefPlugin.secondaryDef);
            }
            if (skillLocator.secondary.baseSkill == chefPlugin.boostedAltSecondaryDef) {
                skillLocator.secondary.SetBaseSkill(chefPlugin.altSecondaryDef);
            }
            skillLocator.utility.SetBaseSkill(chefPlugin.utilityDef);

        }
    }
}