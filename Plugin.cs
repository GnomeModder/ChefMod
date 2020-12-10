using System;
using System.Collections.Generic;
using BepInEx;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using RoR2.Projectile;
using UnityEngine;
using EntityStates.Chef;
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
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(
        "com.Gnome.ChefMod",
        "ChefMod",
        "0.1.0")]
    public class chefPlugin : BaseUnityPlugin
    {
        public GameObject chefPrefab;
        public static GameObject cleaverPrefab;
        public static GameObject oilPrefab;
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
            registerCharacter();
            registerSkills();
            registerProjectiles();
            registerBuff();
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
            chefPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").InstantiateClone("ChefBody");
            ///Get the model without any components.
            chefPrefab.AddComponent<FieldComponent>();

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

            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(chefPrefab);
            };

            CharacterBody component = chefPrefab.GetComponent<CharacterBody>();
            component.baseDamage = 12f;
            component.levelDamage = 2.4f;
            component.baseMaxHealth = 80f;
            component.levelMaxHealth = 20f;
            component.baseArmor = 1f;
            component.baseRegen = 1f;
            component.levelRegen = 0.2f;
            component.baseMoveSpeed = 7f;
            component.levelMoveSpeed = 0f;
            component.baseAttackSpeed = 1f;
            component.name = "CHEF";
            component.baseNameToken = "CHEF_NAME";
            //component.portraitIcon = Assets.chefIcon;

            LanguageAPI.Add("CHEF_NAME", "CHEF");

            chefPrefab.GetComponent<CharacterBody>().preferredPodPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/toolbotbody").GetComponent<CharacterBody>().preferredPodPrefab;

            var stateMachine = component.GetComponent<EntityStateMachine>();
            stateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Chef.Main));

            GameObject displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/CommandoDisplay");

            SurvivorDef survivorDef = new SurvivorDef
            {
                bodyPrefab = chefPrefab,
                descriptionToken = "likes to bbq" + "\r\n",
                displayPrefab = displayPrefab,
                primaryColor = new Color(1, 1, 1),
                name = "CHEF",
                unlockableName = ""
            };
            SurvivorAPI.AddSurvivor(survivorDef);
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
            //primaryDef.icon = Assets.chefprimaryIconSprite;
            primaryDef.skillDescriptionToken = "Toss a boomerang cleaver for 20% damage. Agile";
            primaryDef.skillName = "Primary";
            primaryDef.skillNameToken = "CHEF_PRIMARY";

            LanguageAPI.Add("CHEF_PRIMARY", "Dice");
            LoadoutAPI.AddSkillDef(primaryDef);

            boostedPrimaryDef = ScriptableObject.CreateInstance<SkillDef>();
            boostedPrimaryDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Chef.Mince));
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
            //boostedPrimaryDef.icon = Assets.chefprimaryIconSprite;
            boostedPrimaryDef.skillDescriptionToken = "Throw a cleaver at every nearby enemy";
            boostedPrimaryDef.skillName = "BoostedPrimary";
            boostedPrimaryDef.skillNameToken = "CHEF_BOOSTED_PRIMARY";

            LanguageAPI.Add("CHEF_BOOSTED_PRIMARY", "Mince");
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
            //secondaryDef.icon = Assets.chefsecondaryIconSprite;
            secondaryDef.skillDescriptionToken = "Shoot a fireball for 500% damage. Agile";
            secondaryDef.skillName = "Secondary";
            secondaryDef.skillNameToken = "CHEF_SECONDARY";

            LanguageAPI.Add("CHEF_SECONDARY", "Sear");
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
            // boostedSecondaryDef.icon = Assets.chefsecondaryIconSprite;
            boostedSecondaryDef.skillDescriptionToken = "be a flaming homosexual for 500% dmage";
            boostedSecondaryDef.skillName = "BoostedSecondary";
            boostedSecondaryDef.skillNameToken = "CHEF_BOOSTED_SECONDARY";

            LanguageAPI.Add("CHEF_BOOSTED_SECONDARY", "Flambe");
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
            //altSecondaryDef.icon = Assets.chefsecondaryIconSprite;
            altSecondaryDef.skillDescriptionToken = "Launch small enemies in the air, dealing 500% damage on landing and igniting nearby enemies. Agile";
            altSecondaryDef.skillName = "AltSecondary";
            altSecondaryDef.skillNameToken = "CHEF_ALTSECONDARY";

            LanguageAPI.Add("CHEF_ALTSECONDARY", "Sautee");
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
            //boostedAltSecondaryDef.icon = Assets.chefsecondaryIconSprite;
            boostedAltSecondaryDef.skillDescriptionToken = "fcking obliterate";
            boostedAltSecondaryDef.skillName = "BoostedAltSecondary";
            boostedAltSecondaryDef.skillNameToken = "CHEF_BOOSTED_ALTSECONDARY";

            LanguageAPI.Add("CHEF_BOOSTED_ALTSECONDARY", "Fry");
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
            //boostedPrimaryDef.icon = Assets.chefprimaryIconSprite;
            utilityDef.skillDescriptionToken = "Dash forward leaving a trail of oil that slows enemies. Oil can be ignited";
            utilityDef.skillName = "Utility";
            utilityDef.skillNameToken = "CHEF_UTILITY";

            LanguageAPI.Add("CHEF_UTILITY", "Glaze");
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
            //boostedPrimaryDef.icon = Assets.chefprimaryIconSprite;
            boostedUtilityDef.skillDescriptionToken = "cover yourself in oil";
            boostedUtilityDef.skillName = "boostedUtilityDef";
            boostedUtilityDef.skillNameToken = "CHEF_BOOSTED_UTILITY";

            LanguageAPI.Add("CHEF_BOOSTED_UTILITY", "Marinate");
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
            //boostedPrimaryDef.icon = Assets.chefprimaryIconSprite;
            specialDef.skillDescriptionToken = "Boost your next skill";
            specialDef.skillName = "Special";
            specialDef.skillNameToken = "CHEF_SPECIAL";

            LanguageAPI.Add("CHEF_SPECIAL", "Second Helping");
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
            //boostedPrimaryDef.icon = Assets.chefprimaryIconSprite;
            altSpecialDef.skillDescriptionToken = "Remove secondary cooldown for yourself and nearby allies";
            altSpecialDef.skillName = "AltSpecial";
            altSpecialDef.skillNameToken = "CHEF_ALT_SPECIAL";

            LanguageAPI.Add("CHEF_ALT_SPECIAL", "Buffet");
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
            skillLocator.passiveSkill.enabled = true;
        }

        private void registerProjectiles()
        {
            cleaverPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Sawmerang").InstantiateClone("CHEFCleaver", true);
            //cleaverPrefab.AddComponent<CleaverComponent>();
            //cleaverPrefab.GetComponent<CleaverComponent>().fieldComponent = chefPrefab.GetComponent<FieldComponent>();

            //ProjectileController CleaverController = cleaverPrefab.GetComponent<ProjectileController>();
            //CleaverController.ghostPrefab = stunGrenadeModel;

            BoomerangProjectile boo = cleaverPrefab.GetComponent<BoomerangProjectile>();
            CoomerangProjectile cum = cleaverPrefab.AddComponent<CoomerangProjectile>();
            cum.impactSpark = boo.impactSpark;
            cum.transitionDuration = boo.transitionDuration;
            cum.travelSpeed = boo.travelSpeed;
            cum.charge = boo.charge;
            cum.canHitCharacters = boo.canHitCharacters;
            cum.canHitWorld = boo.canHitWorld;
            cum.distanceMultiplier = boo.distanceMultiplier;
            Destroy(boo);

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

            oilPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/BeetleCrystalBody").InstantiateClone("OilSlick", true);
            oilPrefab.transform.localScale *= 3f;

            var hc = oilPrefab.GetComponent<HealthComponent>();
            hc.dontShowHealthbar = true;
            //hc.godMode = true;

            oilPrefab.GetComponent<TeamComponent>().teamIndex = TeamIndex.Neutral;

            oilPrefab.layer = LayerIndex.fakeActor.intVal;
            oilPrefab.AddComponent<Fireee>();

            oilPrefab.AddComponent<NetworkIdentity>();
            oilPrefab.AddComponent<ProjectileController>();

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
        }
    }
}