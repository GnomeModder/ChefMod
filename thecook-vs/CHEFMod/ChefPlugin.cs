using BepInEx;
using BepInEx.Configuration;
using ChefMod.Components;
using ChefMod.Hooks;
using EntityStates;
using EntityStates.Chef;
using HG;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.ContentManagement;
using RoR2.Projectile;
using RoR2.Skills;
using Survariants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using static R2API.DamageAPI;

namespace ChefMod
{
    [BepInDependency(R2API.PrefabAPI.PluginGUID)]
    [BepInDependency(R2API.LoadoutAPI.PluginGUID)]
    [BepInDependency(R2API.RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency(R2API.UnlockableAPI.PluginGUID)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(R2API.Networking.NetworkingAPI.PluginGUID)]
    [BepInPlugin(
        "com.Gnome.ChefMod",
        "ChefMod",
        "2.3.4")]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Kingpinush.KingKombatArena", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("HIFU.Inferno", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("pseudopulse.Survariants", BepInDependency.DependencyFlags.SoftDependency)]
    public class ChefPlugin : BaseUnityPlugin
    {
        public static bool infernoPluginLoaded = false;
        public static PluginInfo pluginInfo;

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
        public static SkillDef specialDef;

        public static ConfigEntry<bool> useVariant;
        public static ConfigEntry<bool> swapVariant;

        public static ConfigEntry<bool> charUnlock;
        public static ConfigEntry<bool> altSkill;
        public static ConfigEntry<bool> altPodPrefab;

        public static ConfigEntry<bool> oldChefInvader;
        public static ConfigEntry<bool> unlockDisablesInvasion;

        public static ModdedDamageType chefSear;
        public static ModdedDamageType chefFireballOnHit;

        public static BuffDef foodBuff;
        public static BuffDef oilBuff;

        public static Color chefColor = new Color(189f / 255f, 190f / 255f, 194f / 255f);

        public static bool arenaPluginLoaded = false;
        public static bool arenaActive = false;

        public static bool riskOfOptionsLoaded = false;

        public static ConfigEntry<bool> enableCleaverTrails;
        public static bool useCursedDisplay = false;

        public void ReadConfig()
        {
            enableCleaverTrails = base.Config.Bind<bool>("01 - General Settings", "Enable Cleaver Trails", true, "Cleavers have a line trail like in RoR1.");
            charUnlock = base.Config.Bind<bool>("01 - General Settings", "Auto Unlock", false, "Automatically unlocks Chef");
            useCursedDisplay = base.Config.Bind<bool>("01 - General Settings", "Cursed Character Select Anim", false, "Does what it says.").Value;
            altPodPrefab = Config.Bind<bool>("01 - General Settings", "Alt Spawn Pod", true, "Makes the pod prefab more appetizing");
            oldChefInvader = base.Config.Bind<bool>("02 - Invasion Settings", "Old Chef Invader", false, "Use the old overpowered CHEF invasion bossfight.");

            useVariant = Config.Bind<bool>("Survariants Compat", "Enable Variants", true, "Use variants if plugin is installed.");
            swapVariant = Config.Bind<bool>("Survariants Compat", "Swap Variants", false, "Swaps variant order.");

            if (riskOfOptionsLoaded) RiskOfOptionsCompat();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void RiskOfOptionsCompat()
        {
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(enableCleaverTrails));
        }

        public void registerPodPrefabs()
        {
            GameObject robocratePrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/robocratepod");
            fruitPodPrefab = robocratePrefab.InstantiateClone("chef_fruitpod");
            Transform meshObject = fruitPodPrefab.transform.Find("Base/mdlRoboCrate/Base/RobotCrateMesh");
            RandomizeModelOnStartPod randomize = meshObject.gameObject.AddComponent<RandomizeModelOnStartPod>();
            randomize.meshFilter = meshObject.gameObject.GetComponent<MeshFilter>();
            randomize.meshRenderer = meshObject.gameObject.GetComponent<MeshRenderer>();
            //meshObject.transform.localScale = Vector3.one * 20f;

            GameObject impactEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/RoboCratePodGroundImpact");
            fruitPodImpactPrefab = PrefabAPI.InstantiateClone(impactEffect, "chef_fruitpod_impact", false);
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
            pluginInfo = Info;
            infernoPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
            riskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Kingpinush.KingKombatArena"))
            {
                arenaPluginLoaded = true;
            }

            //TODO: Commented out old sound loading code. I duplicated the ChefSounds.bnk from the resources folder and put it in the CHEFMod folder. Someone should clean this bit up later.
            Debug.Log(Assembly.GetExecutingAssembly().GetManifestResourceNames());
            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChefMod.ChefSounds.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            gameObject.AddComponent<TestValueManager>();

            ReadConfig();
            AddHooks();
            Unlockables.RegisterUnlockables();
            ItemDisplays.PopulateDisplayPrefabs();
            registerPodPrefabs();
            registerCharacter();
            registerSkills();
            RegisterProjectiles();
            registerBuff();
            LanguageTokens.RegisterLanguageTokens();
            BuildEffects();

            chefSear = ReserveDamageType();
            chefFireballOnHit = ReserveDamageType();

            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider) {
            addContentPackProvider(new ChefContent());
        }

        public void AddHooks()
        {
            On.RoR2.HealthComponent.TakeDamage += TakeDamage.HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.OnHitAll += OnHitAll.HitAll;
            On.RoR2.CharacterBody.Update += CharacterBody_Update.Update;
            On.RoR2.SiphonNearbyController.SearchForTargets += FixMiredUrn.SearchForTargets;
            if(ChefPlugin.arenaPluginLoaded)
            {
                //On.RoR2.Stage.Start += ArenaStage_Start.Stage_Start;
            }
            On.RoR2.Run.HandlePlayerFirstEntryAnimation += Run_HandlePlayerFirstEntryAnimation;

            RoR2.RoR2Application.onLoad += LateSetup;
            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += SetItemDisplays;
        }

        private void SetItemDisplays(ReadOnlyArray<ReadOnlyContentPack> array)
        {
            ItemDisplays.RegisterItemDisplays(chefPrefab);
        }

        private void LateSetup() {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("pseudopulse.Survariants"))
            {
                SurvariantsCompatInternal();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SurvariantsCompatInternal()
        {
            if (!useVariant.Value) return;
            SurvivorDef official = SurvivorCatalog.GetSurvivorDef(SurvivorCatalog.GetSurvivorIndexFromBodyIndex(BodyCatalog.FindBodyIndex("ChefBody")));
            SurvivorDef mod = SurvivorCatalog.GetSurvivorDef(SurvivorCatalog.GetSurvivorIndexFromBodyIndex(BodyCatalog.FindBodyIndex("GnomeChefBody")));
            if (!official || !mod) return;

            SurvivorDef mainSurvivor = swapVariant.Value ? mod : official;
            SurvivorDef variantSurvivor = swapVariant.Value ? official : mod;

            SurvivorVariantDef variant = ScriptableObject.CreateInstance<SurvivorVariantDef>();
            (variant as ScriptableObject).name = variantSurvivor.cachedName;
            variant.DisplayName = variantSurvivor.displayNameToken;
            variant.VariantSurvivor = variantSurvivor;
            variant.TargetSurvivor = mainSurvivor;
            variant.RequiredUnlock = variantSurvivor.unlockableDef;
            variant.Description = "";

            variantSurvivor.hidden = true;
            SurvivorVariantCatalog.AddSurvivorVariant(variant);
        }

        // REMOVE THIS HOOK ONCE THE PODPREFAB IS FIXED
        //  uh when
        private void Run_HandlePlayerFirstEntryAnimation(On.RoR2.Run.orig_HandlePlayerFirstEntryAnimation orig, Run self, CharacterBody body, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.Run::HandlePlayerFirstEntryAnimation(RoR2.CharacterBody,UnityEngine.Vector3,UnityEngine.Quaternion)' called on client");
                return;
            }
            if (body.preferredPodPrefab)
            {
                if (body.bodyIndex == BodyCatalog.FindBodyIndex("GnomeChefBody"))
                {
                    GameObject gg = UnityEngine.Object.Instantiate<GameObject>(fruitPodPrefab, body.transform.position, spawnRotation);
                    gg.GetComponent<VehicleSeat>().AssignPassenger(body.gameObject);
                    NetworkServer.Spawn(gg);
                    return;
                }
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(body.preferredPodPrefab, body.transform.position, spawnRotation);
                gameObject.GetComponent<VehicleSeat>().AssignPassenger(body.gameObject);
                NetworkServer.Spawn(gameObject);
                return;
            }
            body.SetBodyStateToPreferredInitialState();
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

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepterClassic()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(mealScepterDef, "GnomeChefBody", SkillSlot.Special, specialDef);
        }

        private void registerBuff()
        {
            foodBuff = ScriptableObject.CreateInstance<BuffDef>();
            foodBuff.name = "Mustard";
            foodBuff.iconSprite = LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffBleedingIcon");
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

            BuffDef clayGooDef = LegacyResourcesAPI.Load<BuffDef>("buffdefs/claygoo");

            oilBuff = ScriptableObject.CreateInstance<BuffDef>();
            oilBuff.name = "ChefOil";
            oilBuff.iconSprite = clayGooDef.iconSprite;
            oilBuff.buffColor = clayGooDef.buffColor;
            oilBuff.canStack = false;
            oilBuff.isDebuff = true;
            ChefContent.buffDefs.Add(oilBuff);

            //Steal Buff visuals for oil
            IL.RoR2.CharacterModel.UpdateOverlays += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "ClayGoo")
                    );
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, CharacterModel, bool>>((hasBuff, self) =>
                {
                    return hasBuff || self.body.HasBuff(oilBuff);
                });
            };

            R2API.RecalculateStatsAPI.GetStatCoefficients += (CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args) =>
            {
                if (sender.HasBuff(oilBuff))
                {
                    args.moveSpeedReductionMultAdd += 0.5f;
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
            //chefPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").InstantiateClone("GnomeChefBody");

            PrefabBuilder prefabBuilder = new PrefabBuilder();
            prefabBuilder.prefabName = "GnomeChefBody";
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

            //mealMachine wasn't in the networkstatemachine, but no one's noticed
            //idk if adding it now solves any problems or fuck it it actually introduces some
            NetworkStateMachine networkStateMachine = chefPrefab.GetComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = networkStateMachine.stateMachines.Concat(new EntityStateMachine[] { mealMachine }).ToArray();

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
            characterBody.name = "GnomeChefBody";
            characterBody.baseNameToken = "GNOMECHEF_NAME";
            characterBody.subtitleNameToken = "GNOMECHEF_SUBTITLE";
            characterBody.portraitIcon = Assets.chefIcon;
            characterBody.bodyColor = chefColor;
            characterBody.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Mechanical;

            characterBody.preferredPodPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/toolbotbody").GetComponent<CharacterBody>().preferredPodPrefab;

            EntityStateMachine stateMachine = characterBody.GetComponent<EntityStateMachine>();
            stateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Chef.ChefMain));

            SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
            survivorDef.bodyPrefab = chefPrefab;
            survivorDef.descriptionToken = "GNOMECHEF_DESCRIPTION";
            if (!useCursedDisplay)
            {
                survivorDef.displayPrefab = prefabBuilder.createDisplayPrefab("chefDisplay2");
                survivorDef.displayPrefab.AddComponent<DisplayMenuSoundComponent>();
            }
            else
            {
                survivorDef.displayPrefab = prefabBuilder.createDisplayPrefab("chefDisplay");
            }
            survivorDef.primaryColor = chefColor;
            survivorDef.displayNameToken = "GNOMECHEF_NAME";
            survivorDef.outroFlavorToken = "GNOMECHEF_OUTRO_FLAVOR";
            survivorDef.mainEndingEscapeFailureFlavorToken = "GNOMECHEF_OUTRO_FAILURE";
            survivorDef.desiredSortPosition = 99f;
            survivorDef.unlockableDef = Unlockables.chefUnlockDef;
            survivorDef.cachedName = "GnomeChef";

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
            primaryDef.skillDescriptionToken = "GNOMECHEF_PRIMARY_DESCRIPTION";
            primaryDef.skillName = "Primary";
            primaryDef.skillNameToken = "GNOMECHEF_PRIMARY_NAME";
            primaryDef.keywordTokens = new string[] { "KEYWORD_AGILE", "KEYWORD_GNOMECHEF_BOOST_DICE" };
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
            boostedPrimaryDef.skillDescriptionToken = "GNOMECHEF_BOOSTED_PRIMARY_DESCRIPTION";
            boostedPrimaryDef.skillName = "BoostedPrimary";
            boostedPrimaryDef.skillNameToken = "GNOMECHEF_BOOSTED_PRIMARY_NAME";
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
            altPrimaryDef.skillDescriptionToken = "GNOMECHEF_ALTPRIMARY_DESCRIPTION";
            altPrimaryDef.skillName = "Primary";
            altPrimaryDef.skillNameToken = "GNOMECHEF_ALTPRIMARY_NAME";
            altPrimaryDef.keywordTokens = new string[] { "KEYWORD_AGILE", "KEYWORD_GNOMECHEF_BOOST_SLICE" };
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
            boostedAltPrimaryDef.skillDescriptionToken = "GNOMECHEF_BOOSTED_ALTPRIMARY_DESCRIPTION";
            boostedAltPrimaryDef.skillName = "BoostedPrimary";
            boostedAltPrimaryDef.skillNameToken = "GNOMECHEF_BOOSTED_ALTPRIMARY_NAME";
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
            secondaryDef.skillDescriptionToken = "GNOMECHEF_SECONDARY_DESCRIPTION";
            secondaryDef.skillName = "Secondary";
            secondaryDef.skillNameToken = "GNOMECHEF_SECONDARY_NAME";
            secondaryDef.keywordTokens = new string[] { "KEYWORD_GNOMECHEF_BOOST_SEAR" };
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
            boostedSecondaryDef.skillDescriptionToken = "GNOMECHEF_BOOSTED_SECONDARY_DESCRIPTION";
            boostedSecondaryDef.skillName = "BoostedSecondary";
            boostedSecondaryDef.skillNameToken = "GNOMECHEF_BOOSTED_SECONDARY_NAME";
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
            altSecondaryDef.skillDescriptionToken = "GNOMECHEF_ALTSECONDARY_DESCRIPTION";
            altSecondaryDef.skillName = "AltSecondary";
            altSecondaryDef.skillNameToken = "GNOMECHEF_ALTSECONDARY_NAME";
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
            boostedAltSecondaryDef.skillDescriptionToken = "GNOMECHEF_BOOSTED_ALTSECONDARY_DESCRIPTION";
            boostedAltSecondaryDef.skillName = "BoostedAltSecondary";
            boostedAltSecondaryDef.skillNameToken = "GNOMECHEF_BOOSTED_ALTSECONDARY_NAME";
            ChefContent.skillDefs.Add(boostedAltSecondaryDef);

            utilityDef = ScriptableObject.CreateInstance<SkillDef>();
            utilityDef.activationState = new SerializableEntityStateType(typeof(OilSlick));
            utilityDef.activationStateMachineName = "Weapon";
            utilityDef.baseMaxStock = 1;
            utilityDef.baseRechargeInterval = 10f;
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
            utilityDef.skillDescriptionToken = "GNOMECHEF_UTILITY_DESCRIPTION";
            utilityDef.skillName = "Utility";
            utilityDef.skillNameToken = "GNOMECHEF_UTILITY_NAME";
            utilityDef.keywordTokens = new string[] { "KEYWORD_GNOMECHEF_BOOST_GLAZE" };
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
            boostedUtilityDef.skillDescriptionToken = "GNOMECHEF_BOOSTED_UTILITY_DESCRIPTION";
            boostedUtilityDef.skillName = "boostedUtilityDef";
            boostedUtilityDef.skillNameToken = "GNOMECHEF_BOOSTED_UTILITY_NAME";
            ChefContent.skillDefs.Add(boostedUtilityDef);

            

            specialDef = ScriptableObject.CreateInstance<SkillDef>();
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
            specialDef.skillDescriptionToken = "GNOMECHEF_SPECIAL_DESCRIPTION";
            specialDef.skillName = "Special";
            specialDef.skillNameToken = "GNOMECHEF_SPECIAL_NAME";
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
            specialScepterDef.skillDescriptionToken = "GNOMECHEF_SPECIAL_SCEPTER_DESCRIPTION";
            specialScepterDef.skillName = "SpecialScepter";
            specialScepterDef.skillNameToken = "GNOMECHEF_SPECIAL_SCEPTER_NAME";
            ChefContent.skillDefs.Add(specialScepterDef);
            mealScepterDef = specialScepterDef;
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter"))
            {
                SetupScepter();
            }
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems"))
            {
                SetupScepterClassic();
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
            altSpecialDef.skillDescriptionToken = "GNOMECHEF_ALT_SPECIAL_DESCRIPTION";
            altSpecialDef.skillName = "AltSpecial";
            altSpecialDef.skillNameToken = "GNOMECHEF_ALT_SPECIAL_NAME";
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

        private void RegisterProjectiles()
        {
            BuildProjectiles.BuildCleaver();
            BuildProjectiles.BuildKnife();
            BuildProjectiles.BuildOil();

            searBonusEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/omniimpactvfx").InstantiateClone("ChefSearBonusEffect", false);
            searBonusEffect.GetComponent<EffectComponent>().soundName = "Play_ChefMod_Ignite";
            ChefContent.effectDefs.Add(new EffectDef(searBonusEffect));

            var beegFire = LegacyResourcesAPI.Load<GameObject>("Prefabs/ProjectileGhosts/FireballGhost").InstantiateClone("FoirBallGhost", true);
            beegFire.AddComponent<NetworkIdentity>();
            beegFire.transform.localScale *= 4f;

            //var bbFire = beegFire.InstantiateClone("FlamBallGhost");
            //bbFire.GetComponentInChildren<ParticleSystemRenderer>().material.SetColor("_Color", Color.blue);
            //bbFire.GetComponentInChildren<Light>().color = Color.blue;

            foirballPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/Fireball").InstantiateClone("FoirBall", true);
            foirballPrefab.transform.localScale *= 2f;
            var coq = foirballPrefab.GetComponent<ProjectileController>();
            coq.ghostPrefab = beegFire;
            foirballPrefab.GetComponent<ProjectileDamage>().damageType = DamageType.IgniteOnHit;
            foirballPrefab.AddComponent<LightOnImpact>();

            flamballPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/Fireball").InstantiateClone("FlamBall", true);
            flamballPrefab.transform.localScale *= 2f;
            var cock = flamballPrefab.GetComponent<ProjectileController>();
            cock.ghostPrefab = beegFire;
            flamballPrefab.GetComponent<ProjectileDamage>().damageType = DamageType.IgniteOnHit;
            flamballPrefab.AddComponent<DripOnImpact>();
            //flamballPrefab.AddComponent<LightOnImpact>();
            flamballPrefab.AddComponent<EsplodeOnImpact>();

            drippingPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MagmaOrbProjectile").InstantiateClone("Dripping", true);
            //drippingPrefab.AddComponent<LightOnImpact>();
            drippingPrefab.AddComponent<EsplodeOnImpact>();

            OilExplosion.boostedSearProjectilePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MagmaOrbProjectile").InstantiateClone("BoostedSearProjectile", true);
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
            OilExplosion.explosionEffectPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/omniimpactvfx"), "ChefOilChainExplosion", false);
            EffectComponent ec = OilExplosion.explosionEffectPrefab.GetComponent<EffectComponent>();
            ec.soundName = "Play_engi_M2_explo";
            ec.applyScale = true;
            ChefContent.effectDefs.Add(new EffectDef(OilExplosion.explosionEffectPrefab));
        }

        private void BuildBoostedSearEffect() {

            GameObject ExplosionVFX = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX"), "ChefBlueOmniExplosionVFX", false);


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
            invaderMaster = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/MercMonsterMaster").InstantiateClone("ChefInvader", true).GetComponent<CharacterMaster>();
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