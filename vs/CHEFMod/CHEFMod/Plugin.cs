using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ChefMod
{
    [R2APISubmoduleDependency("SurvivorAPI")]
    [R2APISubmoduleDependency("PrefabAPI")]
    [R2APISubmoduleDependency("LoadoutAPI")]
    [R2APISubmoduleDependency("LanguageAPI")]
    [R2APISubmoduleDependency("SoundAPI")]
    [R2APISubmoduleDependency("AssetAPI")]
    [R2APISubmoduleDependency("BuffAPI")]
    [R2APISubmoduleDependency("EffectAPI")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(
        "com.Gnome.ChefMod",
        "ChefMod",
        "0.10.0")]
    public class chefPlugin : BaseUnityPlugin
    {
        public GameObject chefPrefab;
        public static GameObject cleaverPrefab;
        public static GameObject oilPrefab;
        public static GameObject segfab;
        public static GameObject foirballPrefab;
        public static GameObject flamballPrefab;
        public static GameObject drippingPrefab;

        public static SkillDef primaryDef;
        public static SkillDef boostedPrimaryDef;
        public static SkillDef secondaryDef;
        public static SkillDef boostedSecondaryDef;
        public static SkillDef altSecondaryDef;
        public static SkillDef boostedAltSecondaryDef;
        public static SkillDef utilityDef;
        public static SkillDef boostedUtilityDef;

        public static ConfigEntry<bool> classicMince;
        public static ConfigEntry<int> minceVerticalIntensity;
        public static ConfigEntry<float> minceHorizontalIntensity;
        public static ConfigEntry<float> oilProc;

        private static BuffDef foodDef = new BuffDef
        {
            name = "Mustard",
            iconPath = "Textures/BuffIcons/texBuffBleedingIcon",
            buffColor = Color.yellow,
            canStack = false,
            isDebuff = false,
            eliteIndex = EliteIndex.None
        };
        private static CustomBuff foodBuff = new CustomBuff(foodDef);
        public static BuffIndex foodBuffIndex = BuffAPI.Add(foodBuff);

        public void Awake()
        {
            classicMince = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Classic Mince"), false, new ConfigDescription("Makes Mince work more like ror1", null, Array.Empty<object>()));
            minceVerticalIntensity = base.Config.Bind<int>(new ConfigDefinition("01 - General Settings", "Mince Vertical Intensity"), 0, new ConfigDescription("controls how much you want mince to lag your game. Doesn't do anything unless you have classic mince as true", null, Array.Empty<object>()));
            minceHorizontalIntensity = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Mince Horizontal Intensity"), 5, new ConfigDescription("same as above", null, Array.Empty<object>()));
            oilProc = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Oil Proc"), 0, new ConfigDescription("proc coef on fire oil tick", null, Array.Empty<object>()));

            registerCharacter();
            registerSkills();
            registerProjectiles();
            registerBuff();

            On.RoR2.GlobalEventManager.OnTeamLevelUp += (orig, team) =>
            {
                if (team != TeamIndex.Neutral)
                {
                    orig(team);
                    return;
                }
                var hack = oilPrefab.GetComponent<TeamComponent>();
                hack.teamIndex = TeamIndex.Player;
                orig(team);
                hack.teamIndex = TeamIndex.Neutral;
            };
        }

        private void registerBuff()
        {
            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);
                if (self.HasBuff(foodBuffIndex))
                {
                    if (self.skillLocator.secondary)
                    {
                        self.skillLocator.secondary.cooldownScale = 0f;
                    }
                }
            };

            //On.RoR2.GenericSkill.CalculateFinalRechargeInterval += (orig, self) =>
            //{
            //    if (self.characterBody.HasBuff(foodBuffIndex) && self.characterBody.skillLocator.secondary == self)
            //    {
            //        return 0;
            //    }
            //    return orig(self);
            //};
        }

        private void registerCharacter()
        {
            //Load your base character body, from somewhere in the game.
            //chefPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").InstantiateClone("ChefBody");

            PrefabBuilder prefabBuilder = new PrefabBuilder();
            prefabBuilder.prefabName = "ChefBody";
            prefabBuilder.model = Assets.chefAssetBundle.LoadAsset<GameObject>("chefPrefab");
            prefabBuilder.model.transform.localScale *= 1.25f;
            prefabBuilder.defaultSkinIcon = Assets.defaultSkinIcon;
            prefabBuilder.masterySkinIcon = Assets.defaultSkinIcon;
            prefabBuilder.masteryAchievementUnlockable = "";
            chefPrefab = prefabBuilder.CreatePrefab();

            var fc = chefPrefab.AddComponent<FieldComponent>();
            var meshs = chefPrefab.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in meshs) { if (!mesh.enabled) fc.oil = mesh; }

            GameObject gameObject = chefPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject;

            foreach (GenericSkill skill in chefPrefab.GetComponentsInChildren<GenericSkill>())
            {
                DestroyImmediate(skill);
            }
            SkillLocator skillLocator = chefPrefab.GetComponent<SkillLocator>();
            skillLocator.SetFieldValue<GenericSkill[]>("allSkills", new GenericSkill[0]);

            //Do the following for each of primary, secondary, utility and special.

            skillLocator.primary = chefPrefab.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            skillLocator.primary.SetFieldValue("_skillFamily", newFamily);

            skillLocator.secondary = chefPrefab.AddComponent<GenericSkill>();
            SkillFamily nawFamily = ScriptableObject.CreateInstance<SkillFamily>();
            nawFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(nawFamily);
            skillLocator.secondary.SetFieldValue("_skillFamily", nawFamily);

            skillLocator.utility = chefPrefab.AddComponent<GenericSkill>();
            SkillFamily nowFamily = ScriptableObject.CreateInstance<SkillFamily>();
            nowFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(nowFamily);
            skillLocator.utility.SetFieldValue("_skillFamily", nowFamily);

            skillLocator.special = chefPrefab.AddComponent<GenericSkill>();
            SkillFamily nywFamily = ScriptableObject.CreateInstance<SkillFamily>();
            nywFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(nywFamily);
            skillLocator.special.SetFieldValue("_skillFamily", nywFamily);

            //BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            //{
            //    list.Add(chefPrefab);
            //};

            CharacterBody component = chefPrefab.GetComponent<CharacterBody>();
            component.baseDamage = 12f;
            component.levelDamage = 2.4f;
            component.baseMaxHealth = 100f;
            component.levelMaxHealth = 25f;
            component.baseArmor = 10f;
            component.baseRegen = 1f;
            component.levelRegen = 0.2f;
            component.baseMoveSpeed = 7f;
            component.levelMoveSpeed = 0f;
            component.baseAttackSpeed = 1f;
            component.name = "CHEF";
            component.baseNameToken = "CHEF_NAME";
            component.portraitIcon = Assets.chefIcon;

            LanguageAPI.Add("CHEF_NAME", "CHEF");

            chefPrefab.GetComponent<CharacterBody>().preferredPodPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/toolbotbody").GetComponent<CharacterBody>().preferredPodPrefab;

            var stateMachine = component.GetComponent<EntityStateMachine>();
            stateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Chef.Main));

            GameObject displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/CommandoDisplay");

            SurvivorDef survivorDef = new SurvivorDef
            {
                bodyPrefab = chefPrefab,
                descriptionToken = "CHEF_DESCRIPTION",
                displayPrefab = Assets.chefAssetBundle.LoadAsset<GameObject>("chefDisplayPrefab"),
                primaryColor = new Color(1, 1, 1),
                name = "CHEF",
                unlockableName = "",
                outroFlavorToken = "CHEF_OUTRO"
            };
            SurvivorAPI.AddSurvivor(survivorDef);

            LanguageAPI.Add("CHEF_DESCRIPTION", "Chef a bobbob" + "\r\n");
            LanguageAPI.Add("CHEF_OUTRO", "...and so it left, rock hard");
        }

        private void registerSkills()
        {
            primaryDef = ScriptableObject.CreateInstance<SkillDef>();
            primaryDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Chef.Cleaver));
            primaryDef.activationStateMachineName = "Weapon";
            primaryDef.baseMaxStock = 1;
            primaryDef.baseRechargeInterval = 0f;
            primaryDef.beginSkillCooldownOnSkillEnd = false;
            primaryDef.canceledFromSprinting = false;
            primaryDef.fullRestockOnAssign = false;
            primaryDef.interruptPriority = InterruptPriority.Any;
            primaryDef.isBullets = true;
            primaryDef.isCombatSkill = true;
            primaryDef.mustKeyPress = false;
            primaryDef.noSprint = false;
            primaryDef.rechargeStock = 1;
            primaryDef.requiredStock = 1;
            primaryDef.shootDelay = 0.5f;
            primaryDef.stockToConsume = 1;
            primaryDef.icon = Assets.chefDiceIcon;
            primaryDef.skillDescriptionToken = "CHEF_PRIMARY_DESCRIPTION";
            primaryDef.skillName = "Primary";
            primaryDef.skillNameToken = "CHEF_PRIMARY_NAME";

            LanguageAPI.Add("CHEF_PRIMARY_NAME", "Dice");
            LanguageAPI.Add("CHEF_PRIMARY_DESCRIPTION", "Toss a boomerang cleaver for 50% damage. Agile");
            LoadoutAPI.AddSkillDef(primaryDef);

            boostedPrimaryDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedPrimaryDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Chef.Mince));
            if (classicMince.Value) boostedPrimaryDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Chef.Sbince));
            boostedPrimaryDef.activationStateMachineName = "Weapon";
            boostedPrimaryDef.baseMaxStock = 1;
            boostedPrimaryDef.baseRechargeInterval = 0f;
            boostedPrimaryDef.beginSkillCooldownOnSkillEnd = false;
            boostedPrimaryDef.canceledFromSprinting = false;
            boostedPrimaryDef.fullRestockOnAssign = false;
            boostedPrimaryDef.interruptPriority = InterruptPriority.Any;
            boostedPrimaryDef.isBullets = true;
            boostedPrimaryDef.isCombatSkill = true;
            boostedPrimaryDef.mustKeyPress = false;
            boostedPrimaryDef.noSprint = false;
            boostedPrimaryDef.rechargeStock = 1;
            boostedPrimaryDef.requiredStock = 1;
            boostedPrimaryDef.shootDelay = 0.5f;
            boostedPrimaryDef.stockToConsume = 1;
            boostedPrimaryDef.icon = Assets.chefMinceIcon;
            boostedPrimaryDef.skillDescriptionToken = "CHEF_BOOSTED_PRIMARY_DESCRIPTION";
            boostedPrimaryDef.skillName = "BoostedPrimary";
            boostedPrimaryDef.skillNameToken = "CHEF_BOOSTED_PRIMARY_NAME";

            LanguageAPI.Add("CHEF_BOOSTED_PRIMARY_NAME", "Mince");
            LanguageAPI.Add("CHEF_BOOSTED_PRIMARY_DESCRIPTION", "Throw a cleaver at every nearby enemy");
            LoadoutAPI.AddSkillDef(boostedPrimaryDef);

            secondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            secondaryDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Chef.Sear));
            secondaryDef.activationStateMachineName = "Weapon";
            secondaryDef.baseMaxStock = 1;
            secondaryDef.baseRechargeInterval = 4f;
            secondaryDef.beginSkillCooldownOnSkillEnd = true;
            secondaryDef.canceledFromSprinting = false;
            secondaryDef.fullRestockOnAssign = false;
            secondaryDef.interruptPriority = InterruptPriority.Skill;
            secondaryDef.isBullets = false;
            secondaryDef.isCombatSkill = true;
            secondaryDef.mustKeyPress = false;
            secondaryDef.noSprint = false;
            secondaryDef.rechargeStock = 1;
            secondaryDef.requiredStock = 1;
            secondaryDef.shootDelay = 0.5f;
            secondaryDef.stockToConsume = 1;
            secondaryDef.icon = Assets.chefSearIcon;
            secondaryDef.skillDescriptionToken = "CHEF_SECONDARY_DESCRIPTION";
            secondaryDef.skillName = "Secondary";
            secondaryDef.skillNameToken = "CHEF_SECONDARY_NAME";

            LanguageAPI.Add("CHEF_SECONDARY_NAME", "Sear");
            LanguageAPI.Add("CHEF_SECONDARY_DESCRIPTION", "Shoot a fireball for 500% damage. Agile");
            LoadoutAPI.AddSkillDef(secondaryDef);

            boostedSecondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedSecondaryDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Chef.Flambe));
            boostedSecondaryDef.activationStateMachineName = "Weapon";
            boostedSecondaryDef.baseMaxStock = 1;
            boostedSecondaryDef.baseRechargeInterval = 4f;
            boostedSecondaryDef.beginSkillCooldownOnSkillEnd = true;
            boostedSecondaryDef.canceledFromSprinting = false;
            boostedSecondaryDef.fullRestockOnAssign = true;
            boostedSecondaryDef.interruptPriority = InterruptPriority.Skill;
            boostedSecondaryDef.isBullets = false;
            boostedSecondaryDef.isCombatSkill = true;
            boostedSecondaryDef.mustKeyPress = false;
            boostedSecondaryDef.noSprint = false;
            boostedSecondaryDef.rechargeStock = 1;
            boostedSecondaryDef.requiredStock = 1;
            boostedSecondaryDef.shootDelay = 0.5f;
            boostedSecondaryDef.stockToConsume = 1;
            boostedSecondaryDef.icon = Assets.chefFlambeIcon;
            boostedSecondaryDef.skillDescriptionToken = "CHEF_BOOSTED_SECONDARY_DESCRIPTION";
            boostedSecondaryDef.skillName = "BoostedSecondary";
            boostedSecondaryDef.skillNameToken = "CHEF_BOOSTED_SECONDARY_NAME";

            LanguageAPI.Add("CHEF_BOOSTED_SECONDARY_NAME", "Flambe");
            LanguageAPI.Add("CHEF_BOOSTED_SECONDARY_DESCRIPTION", "be a flaming homosexual for 500% dmage");
            LoadoutAPI.AddSkillDef(boostedSecondaryDef);

            altSecondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            altSecondaryDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Chef.Fry));
            altSecondaryDef.activationStateMachineName = "Weapon";
            altSecondaryDef.baseMaxStock = 1;
            altSecondaryDef.baseRechargeInterval = 4f;
            altSecondaryDef.beginSkillCooldownOnSkillEnd = true;
            altSecondaryDef.canceledFromSprinting = false;
            altSecondaryDef.fullRestockOnAssign = false;
            altSecondaryDef.interruptPriority = InterruptPriority.Skill;
            altSecondaryDef.isBullets = false;
            altSecondaryDef.isCombatSkill = true;
            altSecondaryDef.mustKeyPress = true;
            altSecondaryDef.noSprint = false;
            altSecondaryDef.rechargeStock = 1;
            altSecondaryDef.requiredStock = 1;
            altSecondaryDef.shootDelay = 0f;
            altSecondaryDef.stockToConsume = 1;
            altSecondaryDef.icon = Assets.chefSauteeIcon;
            altSecondaryDef.skillDescriptionToken = "CHEF_ALTSECONDARY_DESCRIPTION";
            altSecondaryDef.skillName = "AltSecondary";
            altSecondaryDef.skillNameToken = "CHEF_ALTSECONDARY_NAME";

            LanguageAPI.Add("CHEF_ALTSECONDARY_NAME", "Sautee");
            LanguageAPI.Add("CHEF_ALTSECONDARY_DESCRIPTION", "Launch small enemies in the air, dealing 500% damage on landing and igniting nearby enemies. Agile");
            LoadoutAPI.AddSkillDef(altSecondaryDef);

            boostedAltSecondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedAltSecondaryDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Chef.Roast));
            boostedAltSecondaryDef.activationStateMachineName = "Weapon";
            boostedAltSecondaryDef.baseMaxStock = 1;
            boostedAltSecondaryDef.baseRechargeInterval = 4f;
            boostedAltSecondaryDef.beginSkillCooldownOnSkillEnd = true;
            boostedAltSecondaryDef.canceledFromSprinting = false;
            boostedAltSecondaryDef.fullRestockOnAssign = true;
            boostedAltSecondaryDef.interruptPriority = InterruptPriority.Skill;
            boostedAltSecondaryDef.isBullets = false;
            boostedAltSecondaryDef.isCombatSkill = true;
            boostedAltSecondaryDef.mustKeyPress = false;
            boostedAltSecondaryDef.noSprint = false;
            boostedAltSecondaryDef.rechargeStock = 1;
            boostedAltSecondaryDef.requiredStock = 1;
            boostedAltSecondaryDef.shootDelay = 0f;
            boostedAltSecondaryDef.stockToConsume = 1;
            boostedAltSecondaryDef.icon = Assets.chefFryIcon;
            boostedAltSecondaryDef.skillDescriptionToken = "CHEF_BOOSTED_ALTSECONDARY_DESCRIPTION";
            boostedAltSecondaryDef.skillName = "BoostedAltSecondary";
            boostedAltSecondaryDef.skillNameToken = "CHEF_BOOSTED_ALTSECONDARY_NAME";

            LanguageAPI.Add("CHEF_BOOSTED_ALTSECONDARY_NAME", "Fry");
            LanguageAPI.Add("CHEF_BOOSTED_ALTSECONDARY_DESCRIPTION", "fcking obliterate");
            LoadoutAPI.AddSkillDef(boostedAltSecondaryDef);

            utilityDef = ScriptableObject.CreateInstance<SkillDef>();
            utilityDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Chef.OilSlick));
            utilityDef.activationStateMachineName = "Weapon";
            utilityDef.baseMaxStock = 1;
            utilityDef.baseRechargeInterval = 7f;
            utilityDef.beginSkillCooldownOnSkillEnd = true;
            utilityDef.canceledFromSprinting = false;
            utilityDef.fullRestockOnAssign = false;
            utilityDef.interruptPriority = InterruptPriority.Pain;
            utilityDef.isBullets = false;
            utilityDef.isCombatSkill = false;
            utilityDef.mustKeyPress = false;
            utilityDef.noSprint = false;
            utilityDef.rechargeStock = 1;
            utilityDef.requiredStock = 1;
            utilityDef.shootDelay = 0.5f;
            utilityDef.stockToConsume = 1;
            utilityDef.icon = Assets.chefGlazeIcon;
            utilityDef.skillDescriptionToken = "CHEF_UTILITY_DESCRIPTION";
            utilityDef.skillName = "Utility";
            utilityDef.skillNameToken = "CHEF_UTILITY_NAME";

            LanguageAPI.Add("CHEF_UTILITY_NAME", "Glaze");
            LanguageAPI.Add("CHEF_UTILITY_DESCRIPTION", "Dash forward leaving a trail of oil that slows enemies. Oil can be ignited");
            LoadoutAPI.AddSkillDef(utilityDef);

            boostedUtilityDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedUtilityDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Chef.Marinate));
            boostedUtilityDef.activationStateMachineName = "Weapon";
            boostedUtilityDef.baseMaxStock = 1;
            boostedUtilityDef.baseRechargeInterval = 7f;
            boostedUtilityDef.beginSkillCooldownOnSkillEnd = true;
            boostedUtilityDef.canceledFromSprinting = false;
            boostedUtilityDef.fullRestockOnAssign = true;
            boostedUtilityDef.interruptPriority = InterruptPriority.Pain;
            boostedUtilityDef.isBullets = false;
            boostedUtilityDef.isCombatSkill = false;
            boostedUtilityDef.mustKeyPress = false;
            boostedUtilityDef.noSprint = false;
            boostedUtilityDef.rechargeStock = 1;
            boostedUtilityDef.requiredStock = 1;
            boostedUtilityDef.shootDelay = 0.5f;
            boostedUtilityDef.stockToConsume = 1;
            boostedUtilityDef.icon = Assets.chefMarinateIcon;
            boostedUtilityDef.skillDescriptionToken = "CHEF_BOOSTED_UTILITY_DESCRIPTION";
            boostedUtilityDef.skillName = "boostedUtilityDef";
            boostedUtilityDef.skillNameToken = "CHEF_BOOSTED_UTILITY_NAME";

            LanguageAPI.Add("CHEF_BOOSTED_UTILITY_NAME", "Marinate");
            LanguageAPI.Add("CHEF_BOOSTED_UTILITY_DESCRIPTION", "cover yourself in oil");
            LoadoutAPI.AddSkillDef(boostedUtilityDef);

            var specialDef = ScriptableObject.CreateInstance<SkillDef>();
            specialDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Chef.Meal));
            specialDef.activationStateMachineName = "Weapon";
            specialDef.baseMaxStock = 1;
            specialDef.baseRechargeInterval = 15f;
            specialDef.beginSkillCooldownOnSkillEnd = true;
            specialDef.canceledFromSprinting = false;
            specialDef.fullRestockOnAssign = true;
            specialDef.interruptPriority = InterruptPriority.PrioritySkill;
            specialDef.isBullets = false;
            specialDef.isCombatSkill = false;
            specialDef.mustKeyPress = true;
            specialDef.noSprint = false;
            specialDef.rechargeStock = 1;
            specialDef.requiredStock = 1;
            specialDef.shootDelay = 0.5f;
            specialDef.stockToConsume = 1;
            specialDef.icon = Assets.chefBHMIcon;
            specialDef.skillDescriptionToken = "CHEF_SPECIAL_DESCRIPTION";
            specialDef.skillName = "Special";
            specialDef.skillNameToken = "CHEF_SPECIAL_NAME";

            LanguageAPI.Add("CHEF_SPECIAL_NAME", "Second Helping");
            LanguageAPI.Add("CHEF_SPECIAL_DESCRIPTION", "Boost your next skill");
            LoadoutAPI.AddSkillDef(specialDef);

            var altSpecialDef = ScriptableObject.CreateInstance<SkillDef>();
            altSpecialDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Chef.Special));
            altSpecialDef.activationStateMachineName = "Weapon";
            altSpecialDef.baseMaxStock = 1;
            altSpecialDef.baseRechargeInterval = 15f;
            altSpecialDef.beginSkillCooldownOnSkillEnd = true;
            altSpecialDef.canceledFromSprinting = false;
            altSpecialDef.fullRestockOnAssign = true;
            altSpecialDef.interruptPriority = InterruptPriority.PrioritySkill;
            altSpecialDef.isBullets = false;
            altSpecialDef.isCombatSkill = false;
            altSpecialDef.mustKeyPress = true;
            altSpecialDef.noSprint = false;
            altSpecialDef.rechargeStock = 1;
            altSpecialDef.requiredStock = 1;
            altSpecialDef.shootDelay = 0.5f;
            altSpecialDef.stockToConsume = 1;
            altSpecialDef.icon = Assets.chefBuffetIcon;
            altSpecialDef.skillDescriptionToken = "CHEF_ALT_SPECIAL_DESCRIPTION";
            altSpecialDef.skillName = "AltSpecial";
            altSpecialDef.skillNameToken = "CHEF_ALT_SPECIAL_NAME";

            LanguageAPI.Add("CHEF_ALT_SPECIAL_NAME", "Buffet");
            LanguageAPI.Add("CHEF_ALT_SPECIAL_DESCRIPTION", "Remove secondary cooldown for yourself and nearby allies");
            LoadoutAPI.AddSkillDef(altSpecialDef);

            SkillLocator skillLocator = chefPrefab.GetComponent<SkillLocator>();

            var skillFamily = skillLocator.primary.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = primaryDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(primaryDef.skillNameToken, false, null)
            };

            skillFamily = skillLocator.secondary.skillFamily;
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = secondaryDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(secondaryDef.skillNameToken, false, null)
            };

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[1] = new SkillFamily.Variant
            {
                skillDef = altSecondaryDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(altSecondaryDef.skillNameToken, false, null)
            };

            skillFamily = skillLocator.utility.skillFamily;
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = utilityDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(utilityDef.skillNameToken, false, null)
            };

            skillFamily = skillLocator.special.skillFamily;
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = specialDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(specialDef.skillNameToken, false, null)
            };

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[1] = new SkillFamily.Variant
            {
                skillDef = altSpecialDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(altSpecialDef.skillNameToken, false, null)
            };

            skillLocator.passiveSkill.skillNameToken = "Bon Apetit";
            skillLocator.passiveSkill.skillDescriptionToken = "Regen health for each nearby ignited enemy";
            skillLocator.passiveSkill.enabled = false;
        }

        private void registerProjectiles()
        {
            GameObject cleaverGhost = Assets.chefAssetBundle.LoadAsset<GameObject>("CleaverParent").InstantiateClone("CleaverGhost", true);
            cleaverGhost.AddComponent<NetworkIdentity>();
            var pog = cleaverGhost.AddComponent<ProjectileGhostController>();

            //var spin = cleaverGhost.GetComponentInChildren<MeshRenderer>().gameObject.AddComponent<Spin>();

            //foreach (Component comp in cleaverGhost.GetComponents<Component>()) Debug.Log(comp.GetType().Name);

            //var spin = cleaverGhost.GetComponentInChildren<MeshRenderer>().gameObject.AddComponent<RotateAroundAxis>();
            //spin.fastRotationSpeed = 10f;
            //spin.slowRotationSpeed = 5f;
            //spin.rotateAroundAxis = RotateAroundAxis.RotationAxis.X;
            //spin.SetSpeed(RotateAroundAxis.Speed.Fast);

            cleaverPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Sawmerang").InstantiateClone("CHEFCleaver", true);

            GameObject effect = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/omniimpactvfx").InstantiateClone("CleaverFX", true);
            effect.AddComponent<NetworkIdentity>();
            effect.GetComponent<EffectComponent>().soundName = "CleaverHit";
            EffectAPI.AddEffect(effect);

            BoomerangProjectile boo = cleaverPrefab.GetComponent<BoomerangProjectile>();
            CoomerangProjectile cum = cleaverPrefab.AddComponent<CoomerangProjectile>();
            cum.impactSpark = effect;
            cum.transitionDuration = boo.transitionDuration;
            cum.travelSpeed = boo.travelSpeed;
            cum.charge = boo.charge;
            cum.canHitCharacters = boo.canHitCharacters;
            cum.canHitWorld = boo.canHitWorld;
            cum.distanceMultiplier = boo.distanceMultiplier;
            Destroy(boo);

            ProjectileController projcont = cleaverPrefab.GetComponent<ProjectileController>(); 
            projcont.procCoefficient = 1f;

            projcont.ghostPrefab = cleaverGhost;
            ProjectileOverlapAttack poa = cleaverPrefab.GetComponent<ProjectileOverlapAttack>();
            poa.impactEffect = effect;
            poa.resetInterval = 0.5f;

            Destroy(cleaverPrefab.GetComponent<ProjectileDotZone>());
            //ProjectileDotZone pdz = cleaverPrefab.GetComponent<ProjectileDotZone>();
            //pdz.impactEffect = effect;
            //pdz.resetFrequency *= 0.25f;

            cleaverPrefab.layer = LayerIndex.noCollision.intVal;

            //GameObject stunGrenadeModel = Assets.chefAssetBundle.LoadAsset<GameObject>("Cleaver").InstantiateClone("CleaverGhost", true);
            //stunGrenadeModel.AddComponent<UnityEngine.Networking.NetworkIdentity>();
            //stunGrenadeModel.AddComponent<ProjectileGhostController>();

            //bollPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/VagrantTrackingBomb").InstantiateClone("OilPrefab");
            //Destroy(bollPrefab.GetComponent<ProjectileSimple>());
            //Destroy(bollPrefab.GetComponent<ProjectileDamage>());
            //Destroy(bollPrefab.GetComponent<ProjectileImpactExplosion>());
            //Destroy(bollPrefab.GetComponent<ProjectileDirectionalTargetFinder>());
            //Destroy(bollPrefab.GetComponent<ProjectileTargetComponent>());
            //Destroy(bollPrefab.GetComponent<ProjectileSteerTowardTarget>());
            //Destroy(bollPrefab.GetComponent<AssignTeamFilterToTeamComponent>());
            //Destroy(bollPrefab.GetComponent<TeamFilter>());
            //bollPrefab.GetComponent<TeamFilter>().teamIndex = TeamIndex.Neutral;
            //bollPrefab.GetComponent<HealthComponent>().dontShowHealthbar = false;
            //bollPrefab.GetComponent<Rigidbody>().useGravity = true;
            //bollPrefab.AddComponent<CharacterMotor>();
            //bollPrefab.AddComponent<Fireee>();

            //GameObject acid = Resources.Load<GameObject>("Prefabs/Projectiles/CrocoLeapAcid");
            //cumStain = acid.GetComponentInChildren<ThreeEyedGames.Decal>().gameObject.InstantiateClone("OilCum", true);
            //Material oilcum = new Material(Resources.Load<Material>("Materials/matClayGooDebuff"));
            //var decal = cumStain.GetComponent<ThreeEyedGames.Decal>();
            ////decal.Material = Resources.Load<Material>("Materials/matBeetleJuice");
            ////Material acidcum = decal.Material;
            ////oilcum.mainTexture = acidcum.mainTexture;
            ////oilcum.mainTextureOffset = acidcum.mainTextureOffset;
            ////oilcum.mainTextureScale = acidcum.mainTextureScale;
            ////oilcum.shaderKeywords = acidcum.shaderKeywords;
            ////oilcum.
            ////oilcum.shader = acidcum.shader;
            ////decal.Material = oilcum;
            //cumStain.transform.localScale *= 7f;

            //Debug.Log("------------------------------------------------");
            //foreach (Component comp in cumStain.GetComponents<Component>()) Debug.Log(comp.GetType().Name);

            oilPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/BeetleCrystalBody").InstantiateClone("OilSlick", true);
            oilPrefab.transform.localScale *= 3f;

            var hc = oilPrefab.GetComponent<HealthComponent>();
            hc.dontShowHealthbar = true;
            //hc.godMode = true;

            oilPrefab.GetComponent<CharacterBody>().baseNameToken = "OilBeetle";
            oilPrefab.GetComponent<TeamComponent>().teamIndex = TeamIndex.Neutral;
            oilPrefab.layer = LayerIndex.debris.intVal;
            oilPrefab.name = "OilBeetle";

            ModelLocator modelLocator = oilPrefab.GetComponent<ModelLocator>();
            foreach (HurtBox hbox in modelLocator.modelBaseTransform.gameObject.GetComponentsInChildren<HurtBox>())
            {
                hbox.damageModifier = HurtBox.DamageModifier.Barrier;
                foreach (HurtBox hhbox in hbox.hurtBoxGroup.hurtBoxes)
                {
                    hhbox.damageModifier = HurtBox.DamageModifier.Barrier;
                }
            }

            oilPrefab.AddComponent<Fireee>();
            //oilPrefab.AddComponent<NetworkIdentity>();
            oilPrefab.AddComponent<ProjectileController>();

            On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victim) =>
            {
                if (victim.name == "OilBeetle(Clone)")
                {
                    bool flag5 = (damageInfo.damageType & DamageType.IgniteOnHit) > DamageType.Generic;
                    bool flag6 = (damageInfo.damageType & DamageType.PercentIgniteOnHit) != DamageType.Generic;
                    bool flag7 = false;
                    if (damageInfo.attacker) flag7 = damageInfo.attacker.GetComponent<CharacterBody>().HasBuff(BuffIndex.AffixRed);
                    if (flag5 || flag6 || flag7)
                    {
                        DotController.InflictDot(victim, damageInfo.attacker, flag6 ? DotController.DotIndex.PercentBurn : DotController.DotIndex.Burn, 4f * damageInfo.procCoefficient, 1f);
                    }
                    damageInfo.procCoefficient = 0f;
                }

                orig(self, damageInfo, victim);
            };

            On.RoR2.CharacterBody.Update += (orig, self) =>
            {
                if (self.baseNameToken == "OilBeetle") return;
                orig(self);
            };

            var firetrail = Resources.Load<GameObject>("Prefabs/FireTrail");
            segfab = firetrail.GetComponent<DamageTrail>().segmentPrefab;
            //var ups = segfab.GetComponent<ParticleSystem>();
            //var man = ups.main;
            //man.maxParticles *= 10;
            //man.duration *= 10;

            //fireMat = segfab.GetComponent<LineRenderer>().sharedMaterial;
            //On.RoR2.Orbs.LightningOrb.PickNextTarget += (orig, self, position) =>
            //{
            //    HurtBox output = orig(self, position);

            //    while (output)
            //    {
            //        Chat.AddMessage("1");
            //        RoR2.HealthComponent healthComponent = output.healthComponent;
            //        if (healthComponent)
            //        {
            //            Fireee fire = healthComponent.body.GetComponent<Fireee>();
            //            if (!fire)
            //            {
            //                break;
            //            }
            //        }
            //        output = self.PickNextTarget(position);
            //        Chat.AddMessage("2");
            //    }

            //    return output;
            //};

            //oilMaster = Resources.Load<GameObject>("Prefabs/CharacterMasters/BeetleCrystalMaster").InstantiateClone("OilMaster", true);
            //var mister = oilMaster.GetComponent<CharacterMaster>().bodyPrefab = oilPrefab;

            var beegFire = Resources.Load<GameObject>("Prefabs/ProjectileGhosts/FireballGhost").InstantiateClone("FoirBallGhost", true);
            beegFire.AddComponent<NetworkIdentity>();
            beegFire.transform.localScale *= 5f;

            foirballPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Fireball").InstantiateClone("FoirBall", true);
            foirballPrefab.transform.localScale *= 2f;
            var coq = foirballPrefab.GetComponent<ProjectileController>();
            coq.ghostPrefab = beegFire;
            foirballPrefab.GetComponent<ProjectileDamage>().damageType = DamageType.IgniteOnHit;
            foirballPrefab.AddComponent<LightOnImpact>();

            flamballPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Fireball").InstantiateClone("FoirBall", true);
            flamballPrefab.transform.localScale *= 2f;
            var cock = flamballPrefab.GetComponent<ProjectileController>();
            cock.ghostPrefab = beegFire;
            flamballPrefab.GetComponent<ProjectileDamage>().damageType = DamageType.IgniteOnHit;
            flamballPrefab.AddComponent<DripOnImpact>();
            //flamballPrefab.AddComponent<LightOnImpact>();
            flamballPrefab.AddComponent<EsplodeOnImpact>();

            drippingPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/MagmaOrbProjectile").InstantiateClone("Dripping", true);
            //drippingPrefab.AddComponent<LightOnImpact>();
            drippingPrefab.AddComponent<EsplodeOnImpact>();

            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(cleaverPrefab);
                list.Add(oilPrefab);
                list.Add(foirballPrefab);
                list.Add(flamballPrefab);
                list.Add(drippingPrefab);
            };
        }
    }
}