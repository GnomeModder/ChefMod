using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.Chef;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using ThreeEyedGames;
using UnityEngine;
using UnityEngine.Networking;

namespace ChefMod
{
    [R2APISubmoduleDependency("PrefabAPI")]
    [R2APISubmoduleDependency("LanguageAPI")]
    [R2APISubmoduleDependency("LoadoutAPI")]
    [R2APISubmoduleDependency("SoundAPI")]
    [R2APISubmoduleDependency("AssetAPI")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(
        "com.Gnome.ChefMod",
        "ChefMod",
        "0.13.0")]
    public class chefPlugin : BaseUnityPlugin
    {
        public GameObject chefPrefab;
        public static CharacterMaster invaderMaster;
        public static GameObject cleaverPrefab;
        public static GameObject knifePrefab;
        public static GameObject oilPrefab;
        public static GameObject oilfab;
        public static GameObject firefab;
        public static GameObject foirballPrefab;
        public static GameObject flamballPrefab;
        public static GameObject drippingPrefab;

        public static SkillDef primaryDef;
        public static SkillDef boostedPrimaryDef;
        public static SkillDef altPrimaryDef;
        public static SkillDef boostedAltPrimaryDef;
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

        public static BuffDef foodBuff;

        public static Color chefColor = new Color(189f / 255f, 190f / 255f, 194f / 255f);

        public void Start()
        {
            ItemDisplays.RegisterDisplays(chefPrefab);
        }

        public void Awake()
        {
            classicMince = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Sbince"), true, new ConfigDescription("Makes Mince work more like ror1. Turn off if it's hurting performance too much, there's an alternate version that's less costly", null, Array.Empty<object>()));
            minceVerticalIntensity = base.Config.Bind<int>(new ConfigDefinition("01 - General Settings", "Mince Vertical Density"), 3, new ConfigDescription("controls how much you want mince to lag your game. Doesn't do anything unless you have classic mince (sbince) as true", null, Array.Empty<object>()));
            minceHorizontalIntensity = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Mince Horizontal Density"), 3, new ConfigDescription("same as above", null, Array.Empty<object>()));
            oilProc = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Oil Proc"), 0, new ConfigDescription("proc coef on fire oil tick", null, Array.Empty<object>()));

            Unlockables.RegisterUnlockables();
            registerCharacter();
            registerSkills();
            registerProjectiles();
            registerBuff();
            AddLore();

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
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }
        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new ChefContent());
        }

        private void registerBuff()
        {
            foodBuff = ScriptableObject.CreateInstance<BuffDef>();
            foodBuff.name = "Mustard";
            foodBuff.iconSprite = Resources.Load<Sprite>("Textures/BuffIcons/texBuffBleedingIcon");
            foodBuff.buffColor = Color.yellow;
            foodBuff.canStack = false;
            foodBuff.isDebuff = false;
            ChefContent.buffDefs.Add(foodBuff);

            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);
                if (self.HasBuff(foodBuff))
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
            prefabBuilder.model = Assets.chefAssetBundle.LoadAsset<GameObject>("mdlCHEF");
            prefabBuilder.model.transform.localScale *= 1.25f;
            prefabBuilder.defaultCustomRendererInfos = new CustomRendererInfo[] {
                new CustomRendererInfo("Chef", Assets.matChefDefault),
                new CustomRendererInfo("Cleaver", Assets.matChefDefaultKnife),
            };
            prefabBuilder.defaultSkinIcon = Assets.defaultSkinIcon;
            prefabBuilder.masterySkinIcon = Assets.defaultSkinIcon;
            prefabBuilder.masteryAchievementUnlockable = "";
            chefPrefab = prefabBuilder.CreatePrefab();

            //var tracker = chefPrefab.AddComponent<HuntressTracker>();
            //tracker.maxTrackingDistance = 30f;
            //tracker.maxTrackingAngle = 90f;
            ////tracker.indicator = ;
            //tracker.enabled = false;

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
            ChefContent.skillFamilies.Add(newFamily);
            skillLocator.primary.SetFieldValue("_skillFamily", newFamily);

            skillLocator.secondary = chefPrefab.AddComponent<GenericSkill>();
            SkillFamily nawFamily = ScriptableObject.CreateInstance<SkillFamily>();
            nawFamily.variants = new SkillFamily.Variant[1];
            ChefContent.skillFamilies.Add(nawFamily);
            skillLocator.secondary.SetFieldValue("_skillFamily", nawFamily);

            skillLocator.utility = chefPrefab.AddComponent<GenericSkill>();
            SkillFamily nowFamily = ScriptableObject.CreateInstance<SkillFamily>();
            nowFamily.variants = new SkillFamily.Variant[1];
            ChefContent.skillFamilies.Add(nowFamily);
            skillLocator.utility.SetFieldValue("_skillFamily", nowFamily);

            skillLocator.special = chefPrefab.AddComponent<GenericSkill>();
            SkillFamily nywFamily = ScriptableObject.CreateInstance<SkillFamily>();
            nywFamily.variants = new SkillFamily.Variant[1];
            ChefContent.skillFamilies.Add(nywFamily);
            skillLocator.special.SetFieldValue("_skillFamily", nywFamily);

            //BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            //{
            //    list.Add(chefPrefab);
            //};

            CharacterBody characterBody = chefPrefab.GetComponent<CharacterBody>();
            characterBody.baseDamage = 12f;
            characterBody.levelDamage = 2.4f;
            characterBody.baseMaxHealth = 100f;
            characterBody.levelMaxHealth = 25f;
            characterBody.baseArmor = 20f;
            characterBody.baseRegen = 1f;
            characterBody.levelRegen = 0.2f;
            characterBody.baseMoveSpeed = 7f;
            characterBody.levelMoveSpeed = 0f;
            characterBody.baseAttackSpeed = 1f;
            characterBody.name = "CHEF";
            characterBody.baseNameToken = "CHEF_NAME";
            characterBody.subtitleNameToken = "CHEF_SUBTITLE";
            characterBody.portraitIcon = Assets.chefIcon;
            characterBody.bodyColor = chefColor;

            LanguageAPI.Add("CHEF_NAME", "CHEF");
            LanguageAPI.Add("CHEF_SUBTITLE", "The Cook");

            characterBody.preferredPodPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/toolbotbody").GetComponent<CharacterBody>().preferredPodPrefab;

            EntityStateMachine stateMachine = characterBody.GetComponent<EntityStateMachine>();
            stateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Chef.Main));

            SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
            survivorDef.bodyPrefab = chefPrefab;
            survivorDef.descriptionToken = "CHEF_DESCRIPTION";
            survivorDef.displayPrefab = prefabBuilder.createDisplayPrefab("CHEFDisplay");
            survivorDef.primaryColor = chefColor;
            survivorDef.displayNameToken = "CHEF_NAME";
            survivorDef.outroFlavorToken = "CHEF_OUTRO";
            survivorDef.desiredSortPosition = 99f;
            survivorDef.unlockableDef = Unlockables.chefUnlockDef;

            ChefContent.survivorDefs.Add(survivorDef);

            LanguageAPI.Add("CHEF_DESCRIPTION", "*sizzle* \n'You're not cooking' \n'yeah, dude' \n'eeh, aahh' \n*fire* \n'babbabababaa' \n'porkchop sandwiches' \n*fire alarm* \n'oh shit get the fuck out of here what are you doing go get  the  fuck  out  of here you stupid idiot fuck were all dead get the fuck out' \n*firetruck sidewalk* \n'my god did that smell good'\n'detector no goin and you tell me do things I done runnin'" + "\r\n");
            LanguageAPI.Add("CHEF_OUTRO", "...and so it left, rock hard");

            invaderMaster = Resources.Load<GameObject>("Prefabs/CharacterMasters/MercMonsterMaster").InstantiateClone("ChefInvader", true).GetComponent<CharacterMaster>();
            invaderMaster.bodyPrefab = chefPrefab;
            invaderMaster.name = "ChefInvader";
            ChefContent.masterPrefabs.Add(invaderMaster.gameObject);
        }

        private void registerSkills()
        {
            ChefContent.entityStates.Add(typeof(Cleaver));
            ChefContent.entityStates.Add(typeof(Mince));
            ChefContent.entityStates.Add(typeof(Sbince));

            ChefContent.entityStates.Add(typeof(Slice));
            ChefContent.entityStates.Add(typeof(Julienne));

            ChefContent.entityStates.Add(typeof(Sear));
            ChefContent.entityStates.Add(typeof(Flambe));

            ChefContent.entityStates.Add(typeof(Fry));
            ChefContent.entityStates.Add(typeof(Roast));

            ChefContent.entityStates.Add(typeof(OilSlick));
            ChefContent.entityStates.Add(typeof(Marinate));

            ChefContent.entityStates.Add(typeof(Special));
            ChefContent.entityStates.Add(typeof(Meal));

            ChefContent.entityStates.Add(typeof(Main));

            primaryDef = ScriptableObject.CreateInstance<SkillDef>();
            primaryDef.activationState = new SerializableEntityStateType(typeof(Cleaver));
            primaryDef.activationStateMachineName = "Weapon";
            primaryDef.baseMaxStock = 1;
            primaryDef.baseRechargeInterval = 0f;
            primaryDef.beginSkillCooldownOnSkillEnd = false;
            primaryDef.canceledFromSprinting = false;
            primaryDef.fullRestockOnAssign = false;
            primaryDef.interruptPriority = InterruptPriority.Any;
            primaryDef.isCombatSkill = true;
            primaryDef.mustKeyPress = false;
            primaryDef.cancelSprintingOnActivation = false;
            primaryDef.rechargeStock = 1;
            primaryDef.requiredStock = 1;
            primaryDef.stockToConsume = 1;
            primaryDef.icon = Assets.chefDiceIcon;
            primaryDef.skillDescriptionToken = "CHEF_PRIMARY_DESCRIPTION";
            primaryDef.skillName = "Primary";
            primaryDef.skillNameToken = "CHEF_PRIMARY_NAME";

            LanguageAPI.Add("CHEF_PRIMARY_NAME", "Dice");
            LanguageAPI.Add("CHEF_PRIMARY_DESCRIPTION", "Toss a boomerang cleaver for 25% damage per hit. Agile");
            ChefContent.skillDefs.Add(primaryDef);

            boostedPrimaryDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedPrimaryDef.activationState = new SerializableEntityStateType(typeof(Mince));
            if (classicMince.Value) boostedPrimaryDef.activationState = new SerializableEntityStateType(typeof(Sbince));
            boostedPrimaryDef.activationStateMachineName = "Weapon";
            boostedPrimaryDef.baseMaxStock = 1;
            boostedPrimaryDef.baseRechargeInterval = 0f;
            boostedPrimaryDef.beginSkillCooldownOnSkillEnd = false;
            boostedPrimaryDef.canceledFromSprinting = false;
            boostedPrimaryDef.fullRestockOnAssign = false;
            boostedPrimaryDef.interruptPriority = InterruptPriority.Frozen;
            boostedPrimaryDef.isCombatSkill = true;
            boostedPrimaryDef.mustKeyPress = false;
            boostedPrimaryDef.cancelSprintingOnActivation = false;
            boostedPrimaryDef.rechargeStock = 1;
            boostedPrimaryDef.requiredStock = 1;
            boostedPrimaryDef.stockToConsume = 1;
            boostedPrimaryDef.icon = Assets.chefMinceIcon;
            boostedPrimaryDef.skillDescriptionToken = "CHEF_BOOSTED_PRIMARY_DESCRIPTION";
            boostedPrimaryDef.skillName = "BoostedPrimary";
            boostedPrimaryDef.skillNameToken = "CHEF_BOOSTED_PRIMARY_NAME";

            LanguageAPI.Add("CHEF_BOOSTED_PRIMARY_NAME", "Mince");
            LanguageAPI.Add("CHEF_BOOSTED_PRIMARY_DESCRIPTION", "Throw a cleaver in every direction");
            ChefContent.skillDefs.Add(boostedPrimaryDef);

            altPrimaryDef = ScriptableObject.CreateInstance<SkillDef>();
            altPrimaryDef.activationState = new SerializableEntityStateType(typeof(Slice));
            altPrimaryDef.activationStateMachineName = "Weapon";
            altPrimaryDef.baseMaxStock = 1;
            altPrimaryDef.baseRechargeInterval = 0f;
            altPrimaryDef.beginSkillCooldownOnSkillEnd = false;
            altPrimaryDef.canceledFromSprinting = false;
            altPrimaryDef.fullRestockOnAssign = false;
            altPrimaryDef.interruptPriority = InterruptPriority.Any;
            altPrimaryDef.isCombatSkill = true;
            altPrimaryDef.mustKeyPress = false;
            altPrimaryDef.cancelSprintingOnActivation = false;
            altPrimaryDef.rechargeStock = 1;
            altPrimaryDef.requiredStock = 1;
            altPrimaryDef.stockToConsume = 1;
            altPrimaryDef.icon = Assets.chefDiceIcon;
            altPrimaryDef.skillDescriptionToken = "CHEF_ALTPRIMARY_DESCRIPTION";
            altPrimaryDef.skillName = "Primary";
            altPrimaryDef.skillNameToken = "CHEF_ALTPRIMARY_NAME";

            LanguageAPI.Add("CHEF_ALTPRIMARY_NAME", "Slice");
            LanguageAPI.Add("CHEF_ALTPRIMARY_DESCRIPTION", "Stab your target for 100% damage. Agile");
            ChefContent.skillDefs.Add(altPrimaryDef);

            boostedAltPrimaryDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedAltPrimaryDef.activationState = new SerializableEntityStateType(typeof(Julienne));
            boostedAltPrimaryDef.activationStateMachineName = "Weapon";
            boostedAltPrimaryDef.baseMaxStock = 1;
            boostedAltPrimaryDef.baseRechargeInterval = 0f;
            boostedAltPrimaryDef.beginSkillCooldownOnSkillEnd = false;
            boostedAltPrimaryDef.canceledFromSprinting = false;
            boostedAltPrimaryDef.fullRestockOnAssign = false;
            boostedAltPrimaryDef.interruptPriority = InterruptPriority.Frozen;
            boostedAltPrimaryDef.isCombatSkill = true;
            boostedAltPrimaryDef.mustKeyPress = false;
            boostedAltPrimaryDef.cancelSprintingOnActivation = false;
            boostedAltPrimaryDef.rechargeStock = 1;
            boostedAltPrimaryDef.requiredStock = 1;
            boostedAltPrimaryDef.stockToConsume = 1;
            boostedAltPrimaryDef.icon = Assets.chefMinceIcon;
            boostedAltPrimaryDef.skillDescriptionToken = "CHEF_BOOSTED_ALTPRIMARY_DESCRIPTION";
            boostedAltPrimaryDef.skillName = "BoostedPrimary";
            boostedAltPrimaryDef.skillNameToken = "CHEF_BOOSTED_ALTPRIMARY_NAME";

            LanguageAPI.Add("CHEF_BOOSTED_ALTPRIMARY_NAME", "Julienne");
            LanguageAPI.Add("CHEF_BOOSTED_ALTPRIMARY_DESCRIPTION", "Stab every nearby enemy");
            ChefContent.skillDefs.Add(boostedAltPrimaryDef);

            secondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            secondaryDef.activationState = new SerializableEntityStateType(typeof(Sear));
            secondaryDef.activationStateMachineName = "Weapon";
            secondaryDef.baseMaxStock = 1;
            secondaryDef.baseRechargeInterval = 4f;
            secondaryDef.beginSkillCooldownOnSkillEnd = true;
            secondaryDef.canceledFromSprinting = false;
            secondaryDef.fullRestockOnAssign = false;
            secondaryDef.interruptPriority = InterruptPriority.Skill;
            secondaryDef.isCombatSkill = true;
            secondaryDef.mustKeyPress = false;
            secondaryDef.cancelSprintingOnActivation = false;
            secondaryDef.rechargeStock = 1;
            secondaryDef.requiredStock = 1;
            secondaryDef.stockToConsume = 1;
            secondaryDef.icon = Assets.chefSearIcon;
            secondaryDef.skillDescriptionToken = "CHEF_SECONDARY_DESCRIPTION";
            secondaryDef.skillName = "Secondary";
            secondaryDef.skillNameToken = "CHEF_SECONDARY_NAME";

            LanguageAPI.Add("CHEF_SECONDARY_NAME", "Sear");
            LanguageAPI.Add("CHEF_SECONDARY_DESCRIPTION", "Shoot a fireball for 500% damage. Agile");
            ChefContent.skillDefs.Add(secondaryDef);

            boostedSecondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedSecondaryDef.activationState = new SerializableEntityStateType(typeof(Flambe));
            boostedSecondaryDef.activationStateMachineName = "Weapon";
            boostedSecondaryDef.baseMaxStock = 1;
            boostedSecondaryDef.baseRechargeInterval = 4f;
            boostedSecondaryDef.beginSkillCooldownOnSkillEnd = true;
            boostedSecondaryDef.canceledFromSprinting = false;
            boostedSecondaryDef.fullRestockOnAssign = true;
            boostedSecondaryDef.interruptPriority = InterruptPriority.Skill;
            boostedSecondaryDef.isCombatSkill = true;
            boostedSecondaryDef.mustKeyPress = false;
            boostedSecondaryDef.cancelSprintingOnActivation = false;
            boostedSecondaryDef.rechargeStock = 1;
            boostedSecondaryDef.requiredStock = 1;
            boostedSecondaryDef.stockToConsume = 1;
            boostedSecondaryDef.icon = Assets.chefFlambeIcon;
            boostedSecondaryDef.skillDescriptionToken = "CHEF_BOOSTED_SECONDARY_DESCRIPTION";
            boostedSecondaryDef.skillName = "BoostedSecondary";
            boostedSecondaryDef.skillNameToken = "CHEF_BOOSTED_SECONDARY_NAME";

            LanguageAPI.Add("CHEF_BOOSTED_SECONDARY_NAME", "Flambe");
            LanguageAPI.Add("CHEF_BOOSTED_SECONDARY_DESCRIPTION", "be a flaming homosexual for 500% dmage");
            ChefContent.skillDefs.Add(boostedSecondaryDef);

            altSecondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            altSecondaryDef.activationState = new SerializableEntityStateType(typeof(Fry));
            altSecondaryDef.activationStateMachineName = "Weapon";
            altSecondaryDef.baseMaxStock = 1;
            altSecondaryDef.baseRechargeInterval = 4f;
            altSecondaryDef.beginSkillCooldownOnSkillEnd = true;
            altSecondaryDef.canceledFromSprinting = false;
            altSecondaryDef.fullRestockOnAssign = false;
            altSecondaryDef.interruptPriority = InterruptPriority.Skill;
            altSecondaryDef.isCombatSkill = true;
            altSecondaryDef.mustKeyPress = true;
            altSecondaryDef.cancelSprintingOnActivation = false;
            altSecondaryDef.rechargeStock = 1;
            altSecondaryDef.requiredStock = 1;
            altSecondaryDef.stockToConsume = 1;
            altSecondaryDef.icon = Assets.chefSauteeIcon;
            altSecondaryDef.skillDescriptionToken = "CHEF_ALTSECONDARY_DESCRIPTION";
            altSecondaryDef.skillName = "AltSecondary";
            altSecondaryDef.skillNameToken = "CHEF_ALTSECONDARY_NAME";

            LanguageAPI.Add("CHEF_ALTSECONDARY_NAME", "Sautee");
            LanguageAPI.Add("CHEF_ALTSECONDARY_DESCRIPTION", "Launch small enemies in the air, dealing 500% damage on landing and igniting nearby enemies. Agile");
            ChefContent.skillDefs.Add(altSecondaryDef);

            boostedAltSecondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedAltSecondaryDef.activationState = new SerializableEntityStateType(typeof(Roast));
            boostedAltSecondaryDef.activationStateMachineName = "Weapon";
            boostedAltSecondaryDef.baseMaxStock = 1;
            boostedAltSecondaryDef.baseRechargeInterval = 4f;
            boostedAltSecondaryDef.beginSkillCooldownOnSkillEnd = true;
            boostedAltSecondaryDef.canceledFromSprinting = false;
            boostedAltSecondaryDef.fullRestockOnAssign = true;
            boostedAltSecondaryDef.interruptPriority = InterruptPriority.Skill;
            boostedAltSecondaryDef.isCombatSkill = true;
            boostedAltSecondaryDef.mustKeyPress = false;
            boostedAltSecondaryDef.cancelSprintingOnActivation = false;
            boostedAltSecondaryDef.rechargeStock = 1;
            boostedAltSecondaryDef.requiredStock = 1;
            boostedAltSecondaryDef.stockToConsume = 1;
            boostedAltSecondaryDef.icon = Assets.chefFryIcon;
            boostedAltSecondaryDef.skillDescriptionToken = "CHEF_BOOSTED_ALTSECONDARY_DESCRIPTION";
            boostedAltSecondaryDef.skillName = "BoostedAltSecondary";
            boostedAltSecondaryDef.skillNameToken = "CHEF_BOOSTED_ALTSECONDARY_NAME";

            LanguageAPI.Add("CHEF_BOOSTED_ALTSECONDARY_NAME", "Fry");
            LanguageAPI.Add("CHEF_BOOSTED_ALTSECONDARY_DESCRIPTION", "fcking obliterate");
            ChefContent.skillDefs.Add(boostedAltSecondaryDef);

            utilityDef = ScriptableObject.CreateInstance<SkillDef>();
            utilityDef.activationState = new SerializableEntityStateType(typeof(OilSlick));
            utilityDef.activationStateMachineName = "Weapon";
            utilityDef.baseMaxStock = 1;
            utilityDef.baseRechargeInterval = 7f;
            utilityDef.beginSkillCooldownOnSkillEnd = true;
            utilityDef.canceledFromSprinting = false;
            utilityDef.fullRestockOnAssign = false;
            utilityDef.interruptPriority = InterruptPriority.Pain;
            utilityDef.isCombatSkill = false;
            utilityDef.mustKeyPress = false;
            utilityDef.cancelSprintingOnActivation = false;
            utilityDef.rechargeStock = 1;
            utilityDef.requiredStock = 1;
            utilityDef.stockToConsume = 1;
            utilityDef.icon = Assets.chefGlazeIcon;
            utilityDef.skillDescriptionToken = "CHEF_UTILITY_DESCRIPTION";
            utilityDef.skillName = "Utility";
            utilityDef.skillNameToken = "CHEF_UTILITY_NAME";

            LanguageAPI.Add("CHEF_UTILITY_NAME", "Glaze");
            LanguageAPI.Add("CHEF_UTILITY_DESCRIPTION", "Dash forward leaving a trail of oil that slows enemies. Oil can be ignited");
            ChefContent.skillDefs.Add(utilityDef);

            boostedUtilityDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedUtilityDef.activationState = new SerializableEntityStateType(typeof(Marinate));
            boostedUtilityDef.activationStateMachineName = "Weapon";
            boostedUtilityDef.baseMaxStock = 1;
            boostedUtilityDef.baseRechargeInterval = 7f;
            boostedUtilityDef.beginSkillCooldownOnSkillEnd = true;
            boostedUtilityDef.canceledFromSprinting = false;
            boostedUtilityDef.fullRestockOnAssign = true;
            boostedUtilityDef.interruptPriority = InterruptPriority.Pain;
            boostedUtilityDef.isCombatSkill = false;
            boostedUtilityDef.mustKeyPress = false;
            boostedUtilityDef.cancelSprintingOnActivation = false;
            boostedUtilityDef.rechargeStock = 1;
            boostedUtilityDef.requiredStock = 1;
            boostedUtilityDef.stockToConsume = 1;
            boostedUtilityDef.icon = Assets.chefMarinateIcon;
            boostedUtilityDef.skillDescriptionToken = "CHEF_BOOSTED_UTILITY_DESCRIPTION";
            boostedUtilityDef.skillName = "boostedUtilityDef";
            boostedUtilityDef.skillNameToken = "CHEF_BOOSTED_UTILITY_NAME";

            LanguageAPI.Add("CHEF_BOOSTED_UTILITY_NAME", "Marinate");
            LanguageAPI.Add("CHEF_BOOSTED_UTILITY_DESCRIPTION", "cover yourself in oil");
            ChefContent.skillDefs.Add(boostedUtilityDef);

            var specialDef = ScriptableObject.CreateInstance<SkillDef>();
            specialDef.activationState = new SerializableEntityStateType(typeof(Meal));
            specialDef.activationStateMachineName = "Weapon";
            specialDef.baseMaxStock = 1;
            specialDef.baseRechargeInterval = 15f;
            specialDef.beginSkillCooldownOnSkillEnd = true;
            specialDef.canceledFromSprinting = false;
            specialDef.fullRestockOnAssign = true;
            specialDef.interruptPriority = InterruptPriority.PrioritySkill;
            specialDef.isCombatSkill = false;
            specialDef.mustKeyPress = true;
            specialDef.cancelSprintingOnActivation = false;
            specialDef.rechargeStock = 1;
            specialDef.requiredStock = 1;
            specialDef.stockToConsume = 1;
            specialDef.icon = Assets.chefBHMIcon;
            specialDef.skillDescriptionToken = "CHEF_SPECIAL_DESCRIPTION";
            specialDef.skillName = "Special";
            specialDef.skillNameToken = "CHEF_SPECIAL_NAME"; 

            LanguageAPI.Add("CHEF_SPECIAL_NAME", "Second Helping");
            LanguageAPI.Add("CHEF_SPECIAL_DESCRIPTION", "Boost your next skill");
            ChefContent.skillDefs.Add(specialDef);

            var altSpecialDef = ScriptableObject.CreateInstance<SkillDef>();
            altSpecialDef.activationState = new SerializableEntityStateType(typeof(Special));
            altSpecialDef.activationStateMachineName = "Weapon";
            altSpecialDef.baseMaxStock = 1;
            altSpecialDef.baseRechargeInterval = 15f;
            altSpecialDef.beginSkillCooldownOnSkillEnd = true;
            altSpecialDef.canceledFromSprinting = false;
            altSpecialDef.fullRestockOnAssign = true;
            altSpecialDef.interruptPriority = InterruptPriority.PrioritySkill;
            altSpecialDef.isCombatSkill = false;
            altSpecialDef.mustKeyPress = true;
            altSpecialDef.cancelSprintingOnActivation = false;
            altSpecialDef.rechargeStock = 1;
            altSpecialDef.requiredStock = 1;
            altSpecialDef.stockToConsume = 1;
            altSpecialDef.icon = Assets.chefBuffetIcon;
            altSpecialDef.skillDescriptionToken = "CHEF_ALT_SPECIAL_DESCRIPTION";
            altSpecialDef.skillName = "AltSpecial";
            altSpecialDef.skillNameToken = "CHEF_ALT_SPECIAL_NAME";

            LanguageAPI.Add("CHEF_ALT_SPECIAL_NAME", "Buffet");
            LanguageAPI.Add("CHEF_ALT_SPECIAL_DESCRIPTION", "Remove secondary cooldown for yourself and nearby allies");
            ChefContent.skillDefs.Add(altSpecialDef);

            SkillLocator skillLocator = chefPrefab.GetComponent<SkillLocator>();

            var skillFamily = skillLocator.primary.skillFamily;
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = primaryDef,
                viewableNode = new ViewablesCatalog.Node(primaryDef.skillNameToken, false, null)
            };

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[1] = new SkillFamily.Variant
            {
                skillDef = altPrimaryDef,
                unlockableDef = Unlockables.sliceUnlockDef,
                viewableNode = new ViewablesCatalog.Node(altPrimaryDef.skillNameToken, false, null)
            };

            skillFamily = skillLocator.secondary.skillFamily;
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = secondaryDef,
                viewableNode = new ViewablesCatalog.Node(secondaryDef.skillNameToken, false, null)
            };

            //Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            //skillFamily.variants[1] = new SkillFamily.Variant
            //{
            //    skillDef = altSecondaryDef,
            //    viewableNode = new ViewablesCatalog.Node(altSecondaryDef.skillNameToken, false, null)
            //};

            skillFamily = skillLocator.utility.skillFamily;
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = utilityDef,
                viewableNode = new ViewablesCatalog.Node(utilityDef.skillNameToken, false, null)
            };

            skillFamily = skillLocator.special.skillFamily;
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = specialDef,
                viewableNode = new ViewablesCatalog.Node(specialDef.skillNameToken, false, null)
            };

            //Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            //skillFamily.variants[1] = new SkillFamily.Variant
            //{
            //    skillDef = altSpecialDef,
            //    unlockableName = "",
            //    viewableNode = new ViewablesCatalog.Node(altSpecialDef.skillNameToken, false, null)
            //};

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
            ChefContent.effectDefs.Add(new EffectDef(effect));

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

            HitBox hit = cleaverPrefab.GetComponentInChildren<HitBox>();
            hit.transform.localScale = new Vector3(hit.transform.localScale.x, 0.69f, hit.transform.localScale.z);

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

            knifePrefab = cleaverPrefab.InstantiateClone("CHEFKnife", true);
            //knifePrefab.AddComponent<ProjectileTargetComponent>();
            var kum = knifePrefab.GetComponent<CoomerangProjectile>();
            kum.target = true;
            kum.distanceMultiplier *= 0.2f;
            knifePrefab.layer = LayerIndex.projectile.intVal;

            Destroy(knifePrefab.GetComponent<ProjectileOverlapAttack>());


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

            GameObject acid = Resources.Load<GameObject>("Prefabs/CharacterBodies/commandobody").GetComponent<CharacterBody>().preferredPodPrefab;
            oilfab = acid.GetComponentInChildren<ThreeEyedGames.Decal>().gameObject.InstantiateClone("OilCum", true);
            var dekal = oilfab.GetComponent<ThreeEyedGames.Decal>();

            //dekal.Material.SetTexture("_MainTex", Assets.chefIcon);
            //dekal.Material.SetTexture("_Cloud1Tex", Assets.chefIcon);
            //dekal.Material.SetTexture("_Cloud2Tex", Assets.chefIcon);
            //dekal.Material.SetTexture("_RemapTex", Assets.chefIcon);
            //dekal.Material.SetTexture("_NormalTex", Assets.chefIcon);
            //dekal.Material.SetTexture("_MaskTex", Assets.chefIcon);

            //decal.Material = Resources.Load<Material>("Materials/matBeetleJuice");
            //Material acidcum = decal.Material;
            //oilcum.mainTexture = acidcum.mainTexture;
            //oilcum.mainTextureOffset = acidcum.mainTextureOffset;
            //oilcum.mainTextureScale = acidcum.mainTextureScale;
            //oilcum.shaderKeywords = acidcum.shaderKeywords;
            //oilcum.
            //oilcum.shader = acidcum.shader;
            //decal.Material = oilcum;
            oilfab.transform.localScale *= 4f;

            var firetrail = Resources.Load<GameObject>("Prefabs/FireTrail");
            Material firepart = firetrail.GetComponent<DamageTrail>().segmentPrefab.GetComponent<ParticleSystemRenderer>().material;

            var chumStain = Resources.Load<GameObject>("prefabs/projectiles/LunarExploderProjectileDotZone");
            firefab = chumStain.GetComponentInChildren<AlignToNormal>().gameObject.InstantiateClone("ChefFire", true);

            Destroy(firefab.GetComponentInChildren<TeamAreaIndicator>().gameObject);
            var decal = firefab.GetComponentInChildren<Decal>();
            Material fireMat = new Material(decal.Material);
            fireMat.SetColor("_Color", Color.red);
            decal.Material = fireMat;

            var systems = firefab.GetComponentsInChildren<ParticleSystemRenderer>();
            Destroy(systems[0].gameObject);
            Material bluefire = new Material(systems[1].material);
            systems[1].material = firepart;
            systems[2].material = firepart;

            firefab.GetComponentInChildren<Light>().color = Color.red;

            //Debug.Log("------------------------------------------------");
            //foreach (Component comp in firefab.GetComponentsInChildren<Component>()) Debug.Log(comp.GetType().Name);

            oilPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/BeetleCrystalBody").InstantiateClone("OilSlick", true);
            oilPrefab.transform.localScale *= 3f;

            var hc = oilPrefab.GetComponent<HealthComponent>();
            hc.dontShowHealthbar = true;
            //hc.godMode = true;

            oilPrefab.GetComponent<CharacterBody>().baseNameToken = "OilBeetle";
            oilPrefab.GetComponent<TeamComponent>().teamIndex = TeamIndex.Neutral;
            oilPrefab.layer = LayerIndex.debris.intVal;
            oilPrefab.name = "OilBeetle";

            foreach (Component comp in oilPrefab.GetComponents<Component>()) if (comp.GetType().Name == "KinematicCharacterMotor")
                {
                    Destroy(comp);
                }
            Destroy(oilPrefab.GetComponent<CharacterMotor>());
            var rig = oilPrefab.GetComponent<Rigidbody>();
            rig.isKinematic = false;
            rig.useGravity = true;
            rig.freezeRotation = true;

            ModelLocator modelLocator = oilPrefab.GetComponent<ModelLocator>();
            modelLocator.modelBaseTransform.localScale *= 0.1f;
            //foreach (HurtBox hbox in modelLocator.modelBaseTransform.gameObject.GetComponentsInChildren<HurtBox>())
            //{
            //    hbox.gameObject.transform.localScale *= .01f;
            //}

            var cap = oilPrefab.GetComponent<CapsuleCollider>();
            cap.radius *= 0.6f;
            cap.height = 0;
            cap.material.staticFriction = 1;

            Destroy(oilPrefab.GetComponent<InteractionDriver>());
            Destroy(oilPrefab.GetComponent<InputBankTest>());
            Destroy(oilPrefab.GetComponent<CameraTargetParams>());
            Destroy(oilPrefab.GetComponent<EntityStateMachine>());
            foreach (GenericSkill skill in oilPrefab.GetComponents<GenericSkill>()) Destroy(skill);
            Destroy(oilPrefab.GetComponent<NetworkStateMachine>());
            Destroy(oilPrefab.GetComponent<Interactor>());
            Destroy(oilPrefab.GetComponent<EquipmentSlot>());
            Destroy(oilPrefab.GetComponent<CharacterDeathBehavior>());
            Destroy(oilPrefab.GetComponent<DeathRewards>());
            Destroy(oilPrefab.GetComponent<CharacterEmoteDefinitions>());
            Destroy(oilPrefab.GetComponent<SfxLocator>());

            oilPrefab.AddComponent<Fireee>();
            oilPrefab.AddComponent<ProjectileController>();
            oilPrefab.AddComponent<TeamFilter>();
            oilPrefab.AddComponent<ProjectileDamage>();
            //oilPrefab.AddComponent<ProjectileDotZone>().enabled = false;

            On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victim) =>
            {
                if (victim.name == "OilBeetle(Clone)")
                {
                    bool flag5 = (damageInfo.damageType & DamageType.IgniteOnHit) > DamageType.Generic;
                    bool flag6 = (damageInfo.damageType & DamageType.PercentIgniteOnHit) != DamageType.Generic;
                    bool flag7 = false;
                    if (damageInfo.attacker) flag7 = damageInfo.attacker.GetComponent<CharacterBody>().HasBuff(RoR2Content.Buffs.AffixRed);
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

            //var ups = segfab.GetComponent<ParticleSystem>();
            //var man = ups.main;
            //man.loop = true;
            //man.duration *= 10f;
            //man.simulationSpeed *= 5f;
            //man.startSizeMultiplier *= 5f;
            //segfab.transform.localScale = new Vector3(10, 100, 10);
            //var em = ups.emission;
            //em.enabled = true;
            //var x = em.rateOverTime; 
            //x.curveMultiplier *= 100f;

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

            //var bbFire = beegFire.InstantiateClone("FlamBallGhost");
            //bbFire.GetComponentInChildren<ParticleSystemRenderer>().material.SetColor("_Color", Color.blue);
            //bbFire.GetComponentInChildren<Light>().color = Color.blue;

            foirballPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Fireball").InstantiateClone("FoirBall", true);
            foirballPrefab.transform.localScale *= 2f;
            var coq = foirballPrefab.GetComponent<ProjectileController>();
            coq.ghostPrefab = beegFire;
            foirballPrefab.GetComponent<ProjectileDamage>().damageType = DamageType.IgniteOnHit;
            foirballPrefab.AddComponent<LightOnImpact>();

            flamballPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Fireball").InstantiateClone("FlamBall", true);
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

            ChefContent.projectilePrefabs.Add(cleaverPrefab);
            ChefContent.projectilePrefabs.Add(knifePrefab);
            ChefContent.projectilePrefabs.Add(oilPrefab);
            ChefContent.projectilePrefabs.Add(foirballPrefab);
            ChefContent.projectilePrefabs.Add(flamballPrefab);
            ChefContent.projectilePrefabs.Add(drippingPrefab);
        }

        private void AddLore()
        {
            LanguageAPI.Add("CHEF_LORE", "A few months ago, I was driving along the streets of Petricor V with my friend, Commando, until we realize that we were running out of gas.\n\nWe were trying to park somewhere to give our car a rest, but no luck. Then something caught our eyes. It was a McDonald's building with Ronald McDonald at the top of it. \n\nWhen we parked by the parking lot, we noticed that they was no customers inside. It was weird, considering that McDonald's is open 24/7. Also. there were no cars either. I could've swore that the Ronald McDonald statue turned its head against me. \n\nI told Commando about it, but when he he saw it, it was in normal position. He told me I was going nuts, but the statue DID turn it's head. I then heard a faint laugh coming from the inside. We got terrified. I tried opening the door, but it was locked by a rusty Master combination lock. We didn't have time to figure out the combination, so Commando pulled out his gun and shot at it. it was finally unlocked. \n\nSo we opened the door to the inside. What we saw was so horrible. There were dead corpses all over the tables and chairs, and lots of blood in the soda machine. We puked in the trash can that was next to us, but before we did that, I saw mutilated arms and legs inside, which made us puke even more. How did McDonald's end up like this? We were so hungry, so we ran into the kitchen. There were fries and a few burgers, I thought we finally found food, until we saw more corpses. \n\nThis time, they had no eyeballs, juts blood coming from the sockets. Their stomachs have been ripped open with the organs ripped out. I tried ignoring them, but they still bother me. I didn't even have time to eat fries. We tried escaping through the main door, but it mysteriously locked by itself. We were now prisoners inside the building. We got scared just by staring at the corpses. Commando and I spitted up, trying to look for a exit. He went to a door that was covered in oil. \n\nI didn't want to enter with him, because I felt that danger was lurking behind the door. That's when I saw a small bomb in one of the dining tables. I picked up, fused with my flamethrower, and placed in the door. After 10 seconds, it finally exploded. I was free to go wherever I want. I tried calling him to come back, but he still didn't answer. I filled up my car with my backup gas supply that I stored in the trunk. I waited for him to go back to my car with me, it was now 7:00 pm. That's when I saw a tall figure coming from the door with a meat cleaver. I drove away as fast as I could. I managed to escaped to my house. Commando never came back at all... \n\nTwo days later, I received a newspaper with the most disturbing headline of all, it said: \n\n'2 boys went to a abandoned McDonald's restaurant in the far side of Petricor V. One manged to get away, with the other one nowhere in sight. Police are still trying to locate the man, but they never found proof. Then they mysteriously disappeared by entering a oil covered door.' \n\nWhat was behind that door? How did they all went missing? I hope someone would this mystery anytime soon...... And who is the strange figure?");
        }

    }
}