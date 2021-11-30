using BepInEx;
using BepInEx.Configuration;
using ChefMod.Components;
using ChefMod.Hooks;
using EntityStates;
using EntityStates.Chef;
using R2API;
using R2API.Networking;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.ContentManagement;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ThreeEyedGames;
using UnityEngine;
using UnityEngine.Networking;
using static R2API.DamageAPI;
using System.Linq;

namespace ChefMod
{
    [R2APISubmoduleDependency("PrefabAPI")]
    [R2APISubmoduleDependency("LanguageAPI")]
    [R2APISubmoduleDependency("LoadoutAPI")]
    [R2APISubmoduleDependency("SoundAPI")]
    [R2APISubmoduleDependency("AssetAPI")]
    [R2APISubmoduleDependency("DamageAPI")]
    [R2APISubmoduleDependency(nameof(NetworkingAPI))]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(
        "com.Gnome.ChefMod",
        "ChefMod",
        "2.0.15")]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Kingpinush.KingKombatArena", BepInDependency.DependencyFlags.SoftDependency)]
    public class ChefPlugin : BaseUnityPlugin
    {
        public GameObject chefPrefab;
        public static CharacterMaster invaderMaster;
        public static GameObject foirballPrefab;
        public static GameObject flamballPrefab;
        public static GameObject drippingPrefab;
        public static GameObject searBonusEffect;
        public static GameObject fruitPodPrefab;

        public static GameObject fruitPodImpactPrefab;

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
        public static SkillDef mealScepterDef;

        public static ConfigEntry<bool> altVictoryMessage;
        public static ConfigEntry<bool> charUnlock;
        public static ConfigEntry<bool> altSkill;
        public static ConfigEntry<bool> altPodPrefab;
        public static ConfigEntry<bool> OilDropCombine;

        public static ConfigEntry<bool> oldChefInvader;
        public static ConfigEntry<bool> unlockDisablesInvasion;

        public static ModdedDamageType chefSear;
        public static ModdedDamageType chefFireballOnHit;

        public static BuffDef foodBuff;

        public static Color chefColor = new Color(189f / 255f, 190f / 255f, 194f / 255f);

        public static bool arenaPluginLoaded = false;
        public static bool arenaActive = false;

        public static PluginInfo pluginInfo;

        public static BepInEx.Logging.ManualLogSource logger = null;

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new ChefContent());
        }

        public void Start()
        {
            ItemDisplays.RegisterDisplays(chefPrefab);
        }

        public void RegisterLanguageTokens()
        {



            //Don't need these since they're already shown as keywords.
            /*chefDesc += "\n\n<style=cIsDamage>Boosted Dice</style>: Throw cleavers in all directions.";
            chefDesc += "\n<style=cIsDamage>Boosted Sear</style>: Glazed customers burst into fireballs.";
            chefDesc += "\n<style=cIsDamage>Boosted Glaze</style>: Leave a longer trail of oil.";
            chefDesc += "\n<style=cIsDamage>Boosted Slice</style>: Stab many times.";*/

            //LanguageAPI.Add("KEYWORD_CHEF_BOOST_SEAR", "<style=cKeywordName>Flambe</style><style=cSub>Ricochet explosive grease balls on impact.</style>");
            
            //LanguageAPI.Add("CHEF_SECONDARY_DESCRIPTION", "Cook customers for <style=cIsDamage>500% damage</style> or until golden brown.");

            //LanguageAPI.Add("CHEF_BOOSTED_SECONDARY_NAME", "Flambe");
            //LanguageAPI.Add("CHEF_BOOSTED_SECONDARY_DESCRIPTION", "Ricochet explosive grease balls on impact.");
            //WiP alts

            LanguageAPI.Add("CHEF_ALTSECONDARY_NAME", "Sautee");
            LanguageAPI.Add("CHEF_ALTSECONDARY_DESCRIPTION", "Launch small enemies in the air, dealing 500% damage on landing and igniting nearby enemies. Agile");
            LanguageAPI.Add("CHEF_BOOSTED_ALTSECONDARY_NAME", "Fry");
            LanguageAPI.Add("CHEF_BOOSTED_ALTSECONDARY_DESCRIPTION", "fcking obliterate");
        }

        public void ReadConfig()
        {
            charUnlock = base.Config.Bind<bool>("01 - General Settings", "Auto Unlock", false, "Automatically unlocks Chef");
            altVictoryMessage = base.Config.Bind<bool>("01 - General Settings", "Alt Victory Message", false, "Makes the victory message and lore more in-line with the game's tone.");
            //unlockDisablesInvasion = base.Config.Bind<bool>(new ConfigDefinition("02 - Invasion Settings", "Disable Invasion after Unlock"), true, new ConfigDescription("Disables the CHEF invasion bossfight once CHEF is unlocked.", null, Array.Empty<object>()));
            oldChefInvader = base.Config.Bind<bool>("02 - Invasion Settings", "Old Chef Invader", false, "Use the old overpowered CHEF invasion bossfight.");
            altPodPrefab = Config.Bind<bool>("01 - General Settings", "Alt Spawn Pod", true, "Makes the pod prefab more appetizing");
            OilDropCombine = Config.Bind<bool>("02 - Performance", "Oil Combining (beta)", false, "Pretty buggy. Combines the large oil drops from the Utilty when they're too close to each other, removing the amount of oil drops in one place, hopefully improving performance.\ndo me a favor and test games with and without this to see how much this actually improves, thanks thanks.");
        }

        public void registerPodPrefabs()
        {
            GameObject robocratePrefab = Resources.Load<GameObject>("prefabs/networkedobjects/robocratepod");
            fruitPodPrefab = robocratePrefab.InstantiateClone("chef_fruitpod");
            Transform meshObject = fruitPodPrefab.transform.Find("Base/mdlRoboCrate/Base/RobotCrateMesh");
            RandomizeModelOnStartPod randomize = meshObject.gameObject.AddComponent<RandomizeModelOnStartPod>();
            randomize.meshFilter = meshObject.gameObject.GetComponent<MeshFilter>();
            randomize.meshRenderer = meshObject.gameObject.GetComponent<MeshRenderer>();
            //meshObject.transform.localScale = Vector3.one * 20f;

            GameObject impactEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/RoboCratePodGroundImpact");
            fruitPodImpactPrefab = PrefabAPI.InstantiateClone(impactEffect, "chef_fruitpod_impact");
            Destroy(fruitPodImpactPrefab.GetComponent<EffectComponent>());
            foreach (Transform child in fruitPodImpactPrefab.transform.Find("Particles").transform)
            {
                if (child.name != "Chunks, Solid")
                    Destroy(child.gameObject);
            }
            RandomizeModelOnStartPod.impactEffect = fruitPodImpactPrefab;
            PrefabAPI.RegisterNetworkPrefab(fruitPodPrefab);
        }

        public void Awake()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Kingpinush.KingKombatArena"))
            {
                arenaPluginLoaded = true;
            }

            pluginInfo = Info;

            logger = Logger;

            gameObject.AddComponent<TestValueManager>();

            ReadConfig();
            AddHooks();
            Unlockables.RegisterUnlockables();
            registerPodPrefabs();
            registerCharacter();
            registerSkills();
            registerProjectiles();
            registerBuff();
            RegisterLanguageTokens();
            CHEFLanguage.Initialize();
            BuildEffects();

            chefSear = ReserveDamageType();
            chefFireballOnHit = ReserveDamageType();

            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        public void AddHooks()
        {
            On.RoR2.HealthComponent.TakeDamage += TakeDamage.HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.OnHitAll += OnHitAll.HitAll;
            On.RoR2.CharacterBody.Update += CharacterBody_Update.Update;
            On.RoR2.SiphonNearbyController.SearchForTargets += FixMiredUrn.SearchForTargets;
            if(ChefPlugin.arenaPluginLoaded)
            {
                On.RoR2.Stage.Start += ArenaStage_Start.Stage_Start;
            }
            Inventory.onServerItemGiven += Inventory_onServerItemGiven;
            //EquipmentSlot.onServerEquipmentActivated += EquipmentSlot_onServerEquipmentActivated;
        }

        public static ItemDef[] itemDefs = new ItemDef[]
        {
            RoR2Content.Items.FlatHealth,
            RoR2Content.Items.Mushroom,
            RoR2Content.Items.HealWhileSafe,
            RoR2Content.Items.Squid,
            RoR2Content.Items.NovaOnLowHealth,
            RoR2Content.Items.Seed,
            RoR2Content.Items.TPHealingNova,
            RoR2Content.Items.Clover,
            RoR2Content.Items.Plant,
            RoR2Content.Items.IncreaseHealing,
            RoR2Content.Items.Hoof,
            RoR2Content.Items.SprintBonus
        };
        readonly string[] responses = new string[]
        {
            "CHEF_EAT_RESPONSE_0",
            "CHEF_EAT_RESPONSE_1",
            "CHEF_EAT_RESPONSE_2",
            "CHEF_EAT_RESPONSE_3",
            "CHEF_EAT_RESPONSE_4",
            "CHEF_EAT_RESPONSE_5",
            "CHEF_EAT_RESPONSE_6"
        };

        /* base strings
            string[] responsesEigo = new string[]
            {
                "Mmm.. Tasty!",
                "I want some more!",
                "Delicious.",
                "Not bad.",
                "Hey.. that's pretty good.",
                "Tasty.",
                "That's good."
            };
         */

        private void Inventory_onServerItemGiven(Inventory inventory, ItemIndex itemIndex, int amount)
        {
            if (inventory.GetComponent<CharacterMaster>().GetBody()?.baseNameToken == "CHEF_NAME")
            {
                if (itemDefs.Contains(ItemCatalog.GetItemDef(itemIndex)))
                {
                    int choice = UnityEngine.Random.Range(0, responses.Length);
                    var charMaster = inventory.GetComponent<CharacterMaster>();
                    string prefix = "";
                    string formatToken;
                    if (charMaster.GetBody().GetUserName().IsNullOrWhiteSpace())
                    { //so no username
                        formatToken = "CHEF_PREFIX_CHAT";
                    } else
                    {
                        prefix = charMaster.GetBody().GetUserName();
                        formatToken = "CHEF_PREFIX_PLAYER_CHAT";
                    }
                    var output = string.Format(formatToken, prefix, responses[choice]);
                    Chat.AddMessage(output);
                }
            }
        }

        public void BuildEffects()
        {
            BuildOilChainExplosionEffect();
            BuildBoostedSearEffect();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(mealScepterDef, chefPrefab.name, SkillSlot.Special, 0);
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
            prefabBuilder.model.transform.localScale *= 1;// *= 1.25f;
            prefabBuilder.defaultCustomRendererInfos = new CustomRendererInfo[] {
                new CustomRendererInfo("Chef", Assets.matChefDefault),
                new CustomRendererInfo("Cleaver", Assets.matChefDefaultKnife),
            };
            prefabBuilder.defaultSkinIcon = Assets.defaultSkinIcon;
            prefabBuilder.masterySkinIcon = Assets.defaultSkinIcon;
            prefabBuilder.masteryAchievementUnlockable = "";
            prefabBuilder.preferredPodPrefab = fruitPodPrefab;
            chefPrefab = prefabBuilder.CreatePrefab();

            //var tracker = chefPrefab.AddComponent<HuntressTracker>();
            //tracker.maxTrackingDistance = 30f;
            //tracker.maxTrackingAngle = 90f;
            ////tracker.indicator = ;
            //tracker.enabled = false;
            chefPrefab.AddComponent<KnifeHandler>();    //Makes Slice work in MP

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

            EntityStateMachine mealMachine = chefPrefab.AddComponent<EntityStateMachine>();
            mealMachine.customName = "MealPrep";
            mealMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.BaseBodyAttachmentState));
            mealMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.BaseBodyAttachmentState));

            //BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            //{
            //    list.Add(chefPrefab);
            //};

            CharacterBody characterBody = chefPrefab.GetComponent<CharacterBody>();
            characterBody.baseDamage = 12f;
            characterBody.levelDamage = characterBody.baseDamage * 0.2f;
            characterBody.baseMaxHealth = 110f;
            characterBody.levelMaxHealth = characterBody.baseMaxHealth * 0.3f;
            characterBody.baseArmor = 12f;
            characterBody.baseRegen = 1f;
            characterBody.levelRegen = characterBody.baseRegen * 0.2f;
            characterBody.baseMoveSpeed = 7f;
            characterBody.levelMoveSpeed = 0f;
            characterBody.baseAttackSpeed = 1f;
            characterBody.name = "CHEF";
            characterBody.baseNameToken = "CHEF_NAME";
            characterBody.subtitleNameToken = "CHEF_SUBTITLE";
            characterBody.portraitIcon = Assets.chefIcon;
            characterBody.bodyColor = chefColor;
            characterBody.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Mechanical;

            EntityStateMachine stateMachine = characterBody.GetComponent<EntityStateMachine>();
            stateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Chef.ChefMain));

            SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
            survivorDef.bodyPrefab = chefPrefab;
            survivorDef.descriptionToken = "CHEF_DESCRIPTION";
            survivorDef.displayPrefab = prefabBuilder.createDisplayPrefab("CHEFDisplay");
            survivorDef.primaryColor = chefColor;
            survivorDef.displayNameToken = altVictoryMessage.Value ? "CHEF_NAME_ALT" : "CHEF_NAME";
            survivorDef.outroFlavorToken = altVictoryMessage.Value ? "CHEF_OUTRO_FLAVOR_ALT" : "CHEF_OUTRO_FLAVOR";
            survivorDef.mainEndingEscapeFailureFlavorToken = altVictoryMessage.Value ? "CHEF_OUTRO_FAILURE_ALT" : "CHEF_OUTRO_FAILURE";
            survivorDef.desiredSortPosition = 99f;
            survivorDef.unlockableDef = Unlockables.chefUnlockDef;

            ChefContent.survivorDefs.Add(survivorDef);
            BuildChefAI();
        }

        private void registerSkills()
        {
            ChefContent.entityStates.Add(typeof(Cleaver));
            ChefContent.entityStates.Add(typeof(MinceHoming));
            ChefContent.entityStates.Add(typeof(Mince));

            ChefContent.entityStates.Add(typeof(Slice));
            ChefContent.entityStates.Add(typeof(Julienne));

            ChefContent.entityStates.Add(typeof(SearOld));
            ChefContent.entityStates.Add(typeof(Flambe));

            ChefContent.entityStates.Add(typeof(PrepSear));
            ChefContent.entityStates.Add(typeof(FireSear));

            ChefContent.entityStates.Add(typeof(PrepBlaze));
            ChefContent.entityStates.Add(typeof(FireBlaze));

            ChefContent.entityStates.Add(typeof(Fry));
            ChefContent.entityStates.Add(typeof(Roast));

            ChefContent.entityStates.Add(typeof(OilSlick));
            ChefContent.entityStates.Add(typeof(Marinate));

            ChefContent.entityStates.Add(typeof(Special));
            ChefContent.entityStates.Add(typeof(Meal));
            ChefContent.entityStates.Add(typeof(MealScepter));

            ChefContent.entityStates.Add(typeof(ChefMain));

            primaryDef = ScriptableObject.CreateInstance<SkillDef>();
            primaryDef.activationState = new SerializableEntityStateType(typeof(Cleaver));
            primaryDef.activationStateMachineName = "Weapon";
            primaryDef.baseMaxStock = 1;
            primaryDef.baseRechargeInterval = 0f;
            primaryDef.beginSkillCooldownOnSkillEnd = false;
            primaryDef.canceledFromSprinting = false;
            primaryDef.fullRestockOnAssign = true;
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
            primaryDef.keywordTokens = new string[] { "KEYWORD_AGILE", "KEYWORD_CHEF_BOOST_DICE" };
            ChefContent.skillDefs.Add(primaryDef);

            boostedPrimaryDef = ScriptableObject.CreateInstance<SkillDef>();
            //boostedPrimaryDef.activationState = new SerializableEntityStateType(typeof(MinceHoming));
            //if (classicMince.Value) boostedPrimaryDef.activationState = new SerializableEntityStateType(typeof(Mince));
            boostedPrimaryDef.activationState = new SerializableEntityStateType(typeof(Mince));
            boostedPrimaryDef.activationStateMachineName = "Weapon";
            boostedPrimaryDef.baseMaxStock = 1;
            boostedPrimaryDef.baseRechargeInterval = 0f;
            boostedPrimaryDef.beginSkillCooldownOnSkillEnd = false;
            boostedPrimaryDef.canceledFromSprinting = false;
            boostedPrimaryDef.fullRestockOnAssign = true;
            boostedPrimaryDef.interruptPriority = InterruptPriority.Any;
            boostedPrimaryDef.isCombatSkill = true;
            boostedPrimaryDef.mustKeyPress = false;
            boostedPrimaryDef.cancelSprintingOnActivation = false;
            boostedPrimaryDef.rechargeStock = 0;
            boostedPrimaryDef.requiredStock = 1;
            boostedPrimaryDef.stockToConsume = 1;
            boostedPrimaryDef.icon = Assets.chefMinceIcon;
            boostedPrimaryDef.skillDescriptionToken = "CHEF_BOOSTED_PRIMARY_DESCRIPTION";
            boostedPrimaryDef.skillName = "BoostedPrimary";
            boostedPrimaryDef.skillNameToken = "CHEF_BOOSTED_PRIMARY_NAME";
            boostedPrimaryDef.keywordTokens = new string[] { "KEYWORD_AGILE" };
            ChefContent.skillDefs.Add(boostedPrimaryDef);

            altPrimaryDef = ScriptableObject.CreateInstance<SkillDef>();
            altPrimaryDef.activationState = new SerializableEntityStateType(typeof(Slice));
            altPrimaryDef.activationStateMachineName = "Weapon";
            altPrimaryDef.baseMaxStock = 1;
            altPrimaryDef.baseRechargeInterval = 0f;
            altPrimaryDef.beginSkillCooldownOnSkillEnd = false;
            altPrimaryDef.canceledFromSprinting = false;
            altPrimaryDef.fullRestockOnAssign = true;
            altPrimaryDef.interruptPriority = InterruptPriority.Any;
            altPrimaryDef.isCombatSkill = true;
            altPrimaryDef.mustKeyPress = false;
            altPrimaryDef.cancelSprintingOnActivation = false;
            altPrimaryDef.rechargeStock = 1;
            altPrimaryDef.requiredStock = 1;
            altPrimaryDef.stockToConsume = 1;
            altPrimaryDef.icon = Assets.chefSliceIcon;
            altPrimaryDef.skillDescriptionToken = "CHEF_ALTPRIMARY_DESCRIPTION";
            altPrimaryDef.skillName = "Primary";
            altPrimaryDef.skillNameToken = "CHEF_ALTPRIMARY_NAME";
            altPrimaryDef.keywordTokens = new string[] { "KEYWORD_AGILE","KEYWORD_CHEF_BOOST_SLICE" };
            ChefContent.skillDefs.Add(altPrimaryDef);

            boostedAltPrimaryDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedAltPrimaryDef.activationState = new SerializableEntityStateType(typeof(Julienne));
            boostedAltPrimaryDef.activationStateMachineName = "Weapon";
            boostedAltPrimaryDef.baseMaxStock = 1;
            boostedAltPrimaryDef.baseRechargeInterval = 0f;
            boostedAltPrimaryDef.beginSkillCooldownOnSkillEnd = false;
            boostedAltPrimaryDef.canceledFromSprinting = false;
            boostedAltPrimaryDef.fullRestockOnAssign = true;
            boostedAltPrimaryDef.interruptPriority = InterruptPriority.Any;
            boostedAltPrimaryDef.isCombatSkill = true;
            boostedAltPrimaryDef.mustKeyPress = false;
            boostedAltPrimaryDef.cancelSprintingOnActivation = false;
            boostedAltPrimaryDef.rechargeStock = 0;
            boostedAltPrimaryDef.requiredStock = 1;
            boostedAltPrimaryDef.stockToConsume = 1;
            boostedAltPrimaryDef.icon = Assets.chefJulienneIcon;
            boostedAltPrimaryDef.skillDescriptionToken = "CHEF_BOOSTED_ALTPRIMARY_DESCRIPTION";
            boostedAltPrimaryDef.skillName = "BoostedPrimary";
            boostedAltPrimaryDef.skillNameToken = "CHEF_BOOSTED_ALTPRIMARY_NAME";
            ChefContent.skillDefs.Add(boostedAltPrimaryDef);

            secondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            secondaryDef.activationState = new SerializableEntityStateType(typeof(PrepSear));
            secondaryDef.activationStateMachineName = "Weapon";
            secondaryDef.baseMaxStock = 1;
            secondaryDef.baseRechargeInterval = 4f;
            secondaryDef.beginSkillCooldownOnSkillEnd = true;
            secondaryDef.canceledFromSprinting = false;
            secondaryDef.fullRestockOnAssign = true;
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
            secondaryDef.keywordTokens = new string[] { "KEYWORD_CHEF_BOOST_SEAR" };
            ChefContent.skillDefs.Add(secondaryDef);

            boostedSecondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedSecondaryDef.activationState = new SerializableEntityStateType(typeof(PrepBlaze));
            boostedSecondaryDef.activationStateMachineName = "Weapon";
            boostedSecondaryDef.baseMaxStock = 1;
            boostedSecondaryDef.baseRechargeInterval = secondaryDef.baseRechargeInterval;
            boostedSecondaryDef.beginSkillCooldownOnSkillEnd = true;
            boostedSecondaryDef.canceledFromSprinting = false;
            boostedSecondaryDef.fullRestockOnAssign = true;
            boostedSecondaryDef.interruptPriority = InterruptPriority.Skill;
            boostedSecondaryDef.isCombatSkill = true;
            boostedSecondaryDef.mustKeyPress = false;
            boostedSecondaryDef.cancelSprintingOnActivation = false;
            boostedSecondaryDef.rechargeStock = 0;
            boostedSecondaryDef.requiredStock = 1;
            boostedSecondaryDef.stockToConsume = 1;
            boostedSecondaryDef.icon = Assets.chefFlambeIcon;
            boostedSecondaryDef.skillDescriptionToken = "CHEF_BOOSTED_SECONDARY_DESCRIPTION";
            boostedSecondaryDef.skillName = "BoostedSecondary";
            boostedSecondaryDef.skillNameToken = "CHEF_BOOSTED_SECONDARY_NAME";
            ChefContent.skillDefs.Add(boostedSecondaryDef);

            altSecondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            altSecondaryDef.activationState = new SerializableEntityStateType(typeof(Fry));
            altSecondaryDef.activationStateMachineName = "Weapon";
            altSecondaryDef.baseMaxStock = 1;
            altSecondaryDef.baseRechargeInterval = 4f;
            altSecondaryDef.beginSkillCooldownOnSkillEnd = true;
            altSecondaryDef.canceledFromSprinting = false;
            altSecondaryDef.fullRestockOnAssign = true;
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
            ChefContent.skillDefs.Add(altSecondaryDef);

            boostedAltSecondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedAltSecondaryDef.activationState = new SerializableEntityStateType(typeof(Roast));
            boostedAltSecondaryDef.activationStateMachineName = "Weapon";
            boostedAltSecondaryDef.baseMaxStock = 1;
            boostedAltSecondaryDef.baseRechargeInterval = altSecondaryDef.baseRechargeInterval;
            boostedAltSecondaryDef.beginSkillCooldownOnSkillEnd = true;
            boostedAltSecondaryDef.canceledFromSprinting = false;
            boostedAltSecondaryDef.fullRestockOnAssign = true;
            boostedAltSecondaryDef.interruptPriority = InterruptPriority.Skill;
            boostedAltSecondaryDef.isCombatSkill = true;
            boostedAltSecondaryDef.mustKeyPress = false;
            boostedAltSecondaryDef.cancelSprintingOnActivation = false;
            boostedAltSecondaryDef.rechargeStock = 0;
            boostedAltSecondaryDef.requiredStock = 1;
            boostedAltSecondaryDef.stockToConsume = 1;
            boostedAltSecondaryDef.icon = Assets.chefFryIcon;
            boostedAltSecondaryDef.skillDescriptionToken = "CHEF_BOOSTED_ALTSECONDARY_DESCRIPTION";
            boostedAltSecondaryDef.skillName = "BoostedAltSecondary";
            boostedAltSecondaryDef.skillNameToken = "CHEF_BOOSTED_ALTSECONDARY_NAME";
            ChefContent.skillDefs.Add(boostedAltSecondaryDef);

            utilityDef = ScriptableObject.CreateInstance<SkillDef>();
            utilityDef.activationState = new SerializableEntityStateType(typeof(OilSlick));
            utilityDef.activationStateMachineName = "Weapon";
            utilityDef.baseMaxStock = 1;
            utilityDef.baseRechargeInterval = 7f;
            utilityDef.beginSkillCooldownOnSkillEnd = true;
            utilityDef.canceledFromSprinting = false;
            utilityDef.fullRestockOnAssign = true;
            utilityDef.interruptPriority = InterruptPriority.Skill;
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
            utilityDef.keywordTokens = new string[] { "KEYWORD_CHEF_BOOST_GLAZE" };
            ChefContent.skillDefs.Add(utilityDef);

            boostedUtilityDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedUtilityDef.activationState = new SerializableEntityStateType(typeof(Marinate));
            boostedUtilityDef.activationStateMachineName = "Weapon";
            boostedUtilityDef.baseMaxStock = 1;
            boostedUtilityDef.baseRechargeInterval = utilityDef.baseRechargeInterval;
            boostedUtilityDef.beginSkillCooldownOnSkillEnd = true;
            boostedUtilityDef.canceledFromSprinting = false;
            boostedUtilityDef.fullRestockOnAssign = true;
            boostedUtilityDef.interruptPriority = InterruptPriority.Skill;
            boostedUtilityDef.isCombatSkill = false;
            boostedUtilityDef.mustKeyPress = false;
            boostedUtilityDef.cancelSprintingOnActivation = false;
            boostedUtilityDef.rechargeStock = 0;
            boostedUtilityDef.requiredStock = 1;
            boostedUtilityDef.stockToConsume = 1;
            boostedUtilityDef.icon = Assets.chefMarinateIcon;
            boostedUtilityDef.skillDescriptionToken = "CHEF_BOOSTED_UTILITY_DESCRIPTION";
            boostedUtilityDef.skillName = "boostedUtilityDef";
            boostedUtilityDef.skillNameToken = "CHEF_BOOSTED_UTILITY_NAME";
            ChefContent.skillDefs.Add(boostedUtilityDef);

            

            var specialDef = ScriptableObject.CreateInstance<SkillDef>();
            specialDef.activationState = new SerializableEntityStateType(typeof(Meal));
            specialDef.activationStateMachineName = "MealPrep";
            specialDef.baseMaxStock = 1;
            specialDef.baseRechargeInterval = 12f;
            specialDef.beginSkillCooldownOnSkillEnd = true;
            specialDef.canceledFromSprinting = false;
            specialDef.fullRestockOnAssign = true;
            specialDef.interruptPriority = InterruptPriority.Any;
            specialDef.isCombatSkill = false;
            specialDef.mustKeyPress = false;
            specialDef.cancelSprintingOnActivation = false;
            specialDef.rechargeStock = 1;
            specialDef.requiredStock = 1;
            specialDef.stockToConsume = 1;
            specialDef.icon = Assets.chefBHMIcon;
            specialDef.skillDescriptionToken = "CHEF_SPECIAL_DESCRIPTION";
            specialDef.skillName = "Special";
            specialDef.skillNameToken = "CHEF_SPECIAL_NAME";
            ChefContent.skillDefs.Add(specialDef);

            var specialScepterDef = ScriptableObject.CreateInstance<SkillDef>();
            specialScepterDef.activationState = new SerializableEntityStateType(typeof(MealScepter));
            specialScepterDef.activationStateMachineName = "MealPrep";
            specialScepterDef.baseMaxStock = 1;
            specialScepterDef.baseRechargeInterval = specialDef.baseRechargeInterval;
            specialScepterDef.beginSkillCooldownOnSkillEnd = true;
            specialScepterDef.canceledFromSprinting = false;
            specialScepterDef.fullRestockOnAssign = true;
            specialScepterDef.interruptPriority = InterruptPriority.Any;
            specialScepterDef.isCombatSkill = false;
            specialScepterDef.mustKeyPress = false;
            specialScepterDef.cancelSprintingOnActivation = false;
            specialScepterDef.rechargeStock = 1;
            specialScepterDef.requiredStock = 1;
            specialScepterDef.stockToConsume = 1;
            specialScepterDef.icon = Assets.chefBHMScepterIcon;
            specialScepterDef.skillDescriptionToken = "CHEF_SPECIAL_SCEPTER_DESCRIPTION";
            specialScepterDef.skillName = "SpecialScepter";
            specialScepterDef.skillNameToken = "CHEF_SPECIAL_SCEPTER_NAME";
            ChefContent.skillDefs.Add(specialScepterDef);
            mealScepterDef = specialScepterDef;
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter"))
            {
                SetupScepter();
            }

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

            if (false && altSkill.Value)
            {
                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                skillFamily.variants[1] = new SkillFamily.Variant
                {
                    skillDef = altSecondaryDef,
                    viewableNode = new ViewablesCatalog.Node(altSecondaryDef.skillNameToken, false, null)
                };
            }

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

            if (false && altSkill.Value)
            {
                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                skillFamily.variants[1] = new SkillFamily.Variant
                {
                    skillDef = altSpecialDef,
                    viewableNode = new ViewablesCatalog.Node(altSpecialDef.skillNameToken, false, null)
                };
            }

            skillLocator.passiveSkill.skillNameToken = "Bon Apetit";
            skillLocator.passiveSkill.skillDescriptionToken = "Regen health for each nearby ignited enemy";
            skillLocator.passiveSkill.enabled = false;
        }

        private void registerProjectiles()
        {
            BuildProjectiles.BuildCleaver();
            BuildProjectiles.BuildKnife();
            BuildProjectiles.BuildOil();

            searBonusEffect = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/omniimpactvfx").InstantiateClone("ChefSearBonusEffect", false);
            searBonusEffect.GetComponent<EffectComponent>().soundName = "Play_bandit2_R_kill";
            ChefContent.effectDefs.Add(new EffectDef(searBonusEffect));

            var beegFire = Resources.Load<GameObject>("Prefabs/ProjectileGhosts/FireballGhost").InstantiateClone("FoirBallGhost", true);
            beegFire.AddComponent<NetworkIdentity>();
            beegFire.transform.localScale *= 4f;

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

            OilExplosion.boostedSearProjectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/MagmaOrbProjectile").InstantiateClone("BoostedSearProjectile", true);
            ProjectileDamage bspd = OilExplosion.boostedSearProjectilePrefab.GetComponent<ProjectileDamage>();
            bspd.damageType = DamageType.Stun1s | DamageType.IgniteOnHit;
            ModdedDamageTypeHolderComponent mdthc = OilExplosion.boostedSearProjectilePrefab.AddComponent<ModdedDamageTypeHolderComponent>();
            mdthc.Add(chefSear);
            mdthc.Add(chefFireballOnHit);
            ProjectileImpactExplosion bspie = OilExplosion.boostedSearProjectilePrefab.GetComponent<ProjectileImpactExplosion>();
            bspie.blastProcCoefficient = 0.4f;

            ChefContent.projectilePrefabs.Add(OilExplosion.boostedSearProjectilePrefab);
            ChefContent.projectilePrefabs.Add(foirballPrefab);
            ChefContent.projectilePrefabs.Add(flamballPrefab);
            ChefContent.projectilePrefabs.Add(drippingPrefab);
        }

        private void BuildOilChainExplosionEffect()
        {
            OilExplosion.explosionEffectPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/omnieffect/omniimpactvfx"), "ChefOilChainExplosion", false);
            EffectComponent ec = OilExplosion.explosionEffectPrefab.GetComponent<EffectComponent>();
            ec.soundName = "Play_engi_M2_explo";
            ec.applyScale = true;
            ChefContent.effectDefs.Add(new EffectDef(OilExplosion.explosionEffectPrefab));
        }

        private void BuildBoostedSearEffect() {

            GameObject ExplosionVFX = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX"), "ChefBlueOmniExplosionVFX", false);


            ParticleSystemRenderer particleRenderer = ExplosionVFX.transform.Find("Unscaled Flames").GetComponent<ParticleSystemRenderer>();
            Material mat = new Material(particleRenderer.material);
            mat.SetColor("_TintColor", new Color(0, 0.32f, 1));
            particleRenderer.material = mat;

            ExplosionVFX.transform.Find("Point Light").GetComponent<Light>().color = new Color(0.05f, 0.38f, 1);
            ExplosionVFX.transform.Find("AreaIndicatorRing, Billboard").gameObject.SetActive(false);

            FireBlaze.ExplosionEffectBoosted = ExplosionVFX;
            ChefContent.effectDefs.Add(new EffectDef(FireBlaze.ExplosionEffectBoosted));
        }

        private void BuildChefAI()
        {
            invaderMaster = Resources.Load<GameObject>("Prefabs/CharacterMasters/MercMonsterMaster").InstantiateClone("ChefInvader", true).GetComponent<CharacterMaster>();
            invaderMaster.bodyPrefab = chefPrefab;
            invaderMaster.name = "ChefInvader";

            Component[] toDelete = invaderMaster.GetComponents<AISkillDriver>();
            foreach (AISkillDriver asd in toDelete)
            {
                Destroy(asd);
            }

            AISkillDriver oilSkillNear = invaderMaster.gameObject.AddComponent<AISkillDriver>();
            oilSkillNear.skillSlot = SkillSlot.Utility;
            oilSkillNear.requireSkillReady = true;
            oilSkillNear.requireEquipmentReady = false;
            oilSkillNear.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            oilSkillNear.minDistance = 0f;
            oilSkillNear.maxDistance = Mathf.Infinity;
            oilSkillNear.selectionRequiresTargetLoS = false;
            oilSkillNear.activationRequiresTargetLoS = false;
            oilSkillNear.activationRequiresAimConfirmation = false;
            oilSkillNear.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            oilSkillNear.aimType = AISkillDriver.AimType.MoveDirection;
            oilSkillNear.ignoreNodeGraph = false;
            oilSkillNear.noRepeat = true;
            oilSkillNear.shouldSprint = true;
            oilSkillNear.shouldFireEquipment = false;
            oilSkillNear.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver specialSkill = invaderMaster.gameObject.AddComponent<AISkillDriver>();
            specialSkill.skillSlot = SkillSlot.Special;
            specialSkill.requireSkillReady = true;
            specialSkill.requireEquipmentReady = false;
            specialSkill.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            specialSkill.minDistance = 0f;
            specialSkill.maxDistance = 30f;
            specialSkill.selectionRequiresTargetLoS = false;
            specialSkill.activationRequiresTargetLoS = false;
            specialSkill.activationRequiresAimConfirmation = false;
            specialSkill.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            specialSkill.aimType = AISkillDriver.AimType.MoveDirection;
            specialSkill.ignoreNodeGraph = false;
            specialSkill.noRepeat = true;
            specialSkill.shouldSprint = true;
            specialSkill.shouldFireEquipment = false;
            specialSkill.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver searSkill = invaderMaster.gameObject.AddComponent<AISkillDriver>();
            searSkill.skillSlot = SkillSlot.Secondary;
            searSkill.requireSkillReady = true;
            searSkill.requireEquipmentReady = false;
            searSkill.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            searSkill.minDistance = 0f;
            searSkill.maxDistance = 20f;
            searSkill.selectionRequiresTargetLoS = true;
            searSkill.activationRequiresTargetLoS = false;
            searSkill.activationRequiresAimConfirmation = false;
            searSkill.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            searSkill.aimType = AISkillDriver.AimType.AtMoveTarget;
            searSkill.ignoreNodeGraph = false;
            searSkill.noRepeat = false;
            searSkill.shouldSprint = true;
            searSkill.shouldFireEquipment = false;
            searSkill.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver cleaverStrafeSkill = invaderMaster.gameObject.AddComponent<AISkillDriver>();
            cleaverStrafeSkill.skillSlot = SkillSlot.Primary;
            cleaverStrafeSkill.requireSkillReady = true;
            cleaverStrafeSkill.requireEquipmentReady = false;
            cleaverStrafeSkill.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            cleaverStrafeSkill.minDistance = 0f;
            cleaverStrafeSkill.maxDistance = 10f;
            cleaverStrafeSkill.selectionRequiresTargetLoS = true;
            cleaverStrafeSkill.activationRequiresTargetLoS = false;
            cleaverStrafeSkill.activationRequiresAimConfirmation = false;
            cleaverStrafeSkill.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            cleaverStrafeSkill.aimType = AISkillDriver.AimType.AtMoveTarget;
            cleaverStrafeSkill.ignoreNodeGraph = false;
            cleaverStrafeSkill.noRepeat = false;
            cleaverStrafeSkill.shouldSprint = true;
            cleaverStrafeSkill.shouldFireEquipment = false;
            cleaverStrafeSkill.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver cleaverSkill = invaderMaster.gameObject.AddComponent<AISkillDriver>();
            cleaverSkill.skillSlot = SkillSlot.Primary;
            cleaverSkill.requireSkillReady = true;
            cleaverSkill.requireEquipmentReady = false;
            cleaverSkill.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            cleaverSkill.minDistance = 10f;
            cleaverSkill.maxDistance = 30f;
            cleaverSkill.selectionRequiresTargetLoS = true;
            cleaverSkill.activationRequiresTargetLoS = false;
            cleaverSkill.activationRequiresAimConfirmation = false;
            cleaverSkill.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            cleaverSkill.aimType = AISkillDriver.AimType.AtMoveTarget;
            cleaverSkill.ignoreNodeGraph = false;
            cleaverSkill.noRepeat = false;
            cleaverSkill.shouldSprint = true;
            cleaverSkill.shouldFireEquipment = false;
            cleaverSkill.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver chase = invaderMaster.gameObject.AddComponent<AISkillDriver>();
            chase.skillSlot = SkillSlot.None;
            chase.requireSkillReady = false;
            chase.requireEquipmentReady = false;
            chase.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chase.minDistance = 0f;
            chase.maxDistance = Mathf.Infinity;
            chase.selectionRequiresTargetLoS = false;
            chase.activationRequiresTargetLoS = false;
            chase.activationRequiresAimConfirmation = false;
            chase.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chase.aimType = AISkillDriver.AimType.MoveDirection;
            chase.ignoreNodeGraph = false;
            chase.noRepeat = false;
            chase.shouldSprint = true;
            chase.shouldFireEquipment = false;
            chase.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            ChefContent.masterPrefabs.Add(invaderMaster.gameObject);
        }
    }
}