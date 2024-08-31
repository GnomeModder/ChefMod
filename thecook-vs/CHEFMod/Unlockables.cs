using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Achievements;
using R2API;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using EntityStates.Chef;
using ChefPlugin;
using UnityEngine.Networking;
using R2API;

namespace ChefMod
{
    static class Unlockables
    {
        public static UnlockableDef chefUnlockDef;
        public static UnlockableDef sliceUnlockDef;

        public static int ingredientCount = 12;

        public static void RegisterUnlockables()
        {
            chefUnlockDef = UnlockableAPI.AddUnlockable<Achievements.ChefAchievement>();
            sliceUnlockDef = UnlockableAPI.AddUnlockable<Achievements.SliceAchievement>();
        }
    }
}

namespace ChefMod.Achievements
{
    internal class ChefAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier { get; } = "CHEF_CHEFUNLOCKABLE_ACHIEVEMENT_ID";
        public override string UnlockableIdentifier { get; } = "CHEF_CHEFUNLOCKABLE_REWARD_ID";
        public override string AchievementNameToken { get; } = "CHEF_CHEFUNLOCKABLE_ACHIEVEMENT_NAME";
        public override string PrerequisiteUnlockableIdentifier { get; } = "";
        public override string UnlockableNameToken { get; } = "CHEF_CHEFUNLOCKABLE_UNLOCKABLE_NAME";
        public override string AchievementDescToken { get; } = "CHEF_CHEFUNLOCKABLE_ACHIEVEMENT_DESC";
        public override Sprite Sprite { get; } = Assets.chefIconSprite;

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString("CHEF_CHEFUNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString("CHEF_CHEFUNLOCKABLE_ACHIEVEMENT_DESC")
                            }));
        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString("CHEF_CHEFUNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString("CHEF_CHEFUNLOCKABLE_ACHIEVEMENT_DESC")
                            }));

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("GnomeChefBody");
        }

        private bool warning = false;
        private bool spawned = false;
        private bool death = false;

        string[] chefwarnings = { "JE PEUX LE SENTIR", "JE VIENS À VOUS BIENTÔT", "MON FOUR PRÉCHAUFFE", "VOUS M'AVEZ AROUSÉ", "LE DÎNER EST TOUJOURS À L'HEURE" };

        private void CheckItem(On.RoR2.CharacterMaster.orig_OnInventoryChanged orig, CharacterMaster self)
        {
            orig(self);

            if (!ChefPlugin.arenaActive && self && self.teamIndex == TeamIndex.Player && self.inventory)
            {
                int count = getFoodCount(self.inventory);

                if (!warning && count >= Unlockables.ingredientCount - 1)
                {
                    warning = true;
                    int choice = UnityEngine.Random.Range(0, chefwarnings.Length);
                    Chat.AddMessage(Util.GenerateColoredString("CHEF: " + chefwarnings[choice], Color.red));
                }
                if (warning && !death && !spawned && count >= Unlockables.ingredientCount)
                {
                    ChefInvasionManager.PerformInvasion(new Xoroshiro128Plus(Run.instance.seed));
                    spawned = true;
                }
            }
        }

        private int getFoodCount(Inventory inventory)
        {
            int output = 0;
            output += inventory.GetItemCount(RoR2Content.Items.FlatHealth);
            output += inventory.GetItemCount(RoR2Content.Items.Mushroom);
            output += inventory.GetItemCount(RoR2Content.Items.HealWhileSafe);
            output += inventory.GetItemCount(RoR2Content.Items.Squid);
            output += inventory.GetItemCount(RoR2Content.Items.NovaOnLowHealth);
            output += inventory.GetItemCount(RoR2Content.Items.Seed);
            output += inventory.GetItemCount(RoR2Content.Items.TPHealingNova);
            output += inventory.GetItemCount(RoR2Content.Items.Clover);
            output += inventory.GetItemCount(RoR2Content.Items.Plant);
            output += inventory.GetItemCount(RoR2Content.Items.IncreaseHealing);
            output += inventory.GetItemCount(RoR2Content.Items.Hoof);
            output += inventory.GetItemCount(RoR2Content.Items.SprintBonus);
            output += inventory.GetItemCount(RoR2Content.Items.ParentEgg);
            output += inventory.GetItemCount(RoR2Content.Items.BeetleGland);
            output += inventory.GetItemCount(RoR2Content.Items.LunarPrimaryReplacement);
            output += inventory.GetItemCount(RoR2Content.Items.LunarSecondaryReplacement);
            output += inventory.GetItemCount(RoR2Content.Items.LunarUtilityReplacement);
            output += inventory.GetItemCount(RoR2Content.Items.LunarSpecialReplacement);
            output += inventory.GetItemCount(DLC1Content.Items.AttackSpeedAndMoveSpeed);
            output += inventory.GetItemCount(DLC1Content.Items.HealingPotion);
            output += inventory.GetItemCount(DLC1Content.Items.HealingPotionConsumed);
            output += inventory.GetItemCount(DLC1Content.Items.PermanentDebuffOnHit);
            output += inventory.GetItemCount(DLC1Content.Items.RandomEquipmentTrigger);
            output += inventory.GetItemCount(DLC1Content.Items.MushroomVoid);
            output += inventory.GetItemCount(DLC1Content.Items.BearVoid);
            output += inventory.GetItemCount(DLC1Content.Items.SlowOnHitVoid);
            output += inventory.GetItemCount(DLC1Content.Items.MissileVoid);
            output += inventory.GetItemCount(DLC1Content.Items.ExtraLifeVoid);
            output += inventory.GetItemCount(DLC1Content.Items.BleedOnHitVoid);
            output += inventory.GetItemCount(DLC1Content.Items.CloverVoid);
            output += inventory.GetItemCount(DLC1Content.Items.VoidMegaCrabItem);
            if (inventory.GetEquipmentIndex() == RoR2Content.Equipment.Fruit.equipmentIndex) output += 2;
            return output;
        }

        /*private void Death(DamageReport report)
        {
            if (report.victimMaster && report.victimMaster.name == "ChefInvader(Clone)")
            {
                if (report.isFallDamage)
                {
                    ChefInvasionManager.PerformInvasion(new Xoroshiro128Plus(Run.instance.seed));
                    return;
                }
                if (!death)
                {
                    death = true;
                    base.Grant();
                }
            }
        }*/


        private void Reset(Stage stage)
        {
            warning = false;
            spawned = false;
            death = false;
        }

        /*private void Check(Stage stage)
        {
            if (death) base.Grant();
        }
        */
        private void BuffChefInvader(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            if (ChefPlugin.oldChefInvader.Value && self.master && self.master.name == "ChefInvader(Clone)")
            {
                self.baseMaxHealth = 1000f;
                self.levelMaxHealth = 500f;
                self.baseMoveSpeed = 14f;
                self.levelDamage = 4.8f;
            }
            orig(self);
        }

        public override void OnInstall()
        {
            base.OnInstall();

            if (ChefPlugin.charUnlock.Value) base.Grant();

            On.RoR2.HealthComponent.ManagedFixedUpdate += ChefDeath;
            On.RoR2.CharacterMaster.OnInventoryChanged += CheckItem;
            //GlobalEventManager.onCharacterDeathGlobal += Death;
            Stage.onServerStageBegin += Reset;
            //Stage.onServerStageComplete += Check;
            On.RoR2.CharacterBody.RecalculateStats += BuffChefInvader;
        }

        private void ChefDeath(On.RoR2.HealthComponent.orig_ManagedFixedUpdate orig, HealthComponent self, float deltaTime)
        {
            if (!self.alive && self.wasAlive)
            {
                if (self.body && self.body.master && self.body.master.name == "ChefInvader(Clone)")
                {
                    if (self.body.master.inventory && self.body.master.inventory.GetItemCount(RoR2Content.Items.ExtraLife) <= 0)
                    {
                        base.Grant();
                    }
                }
            }
            orig(self, deltaTime);
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            On.RoR2.HealthComponent.ManagedFixedUpdate -= ChefDeath;
            On.RoR2.CharacterMaster.OnInventoryChanged -= CheckItem;
            //GlobalEventManager.onCharacterDeathGlobal -= Death;
            Stage.onServerStageBegin -= Reset;
            //Stage.onServerStageComplete -= Check;
            On.RoR2.CharacterBody.RecalculateStats -= BuffChefInvader;
        }
    }

    internal class SliceAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier { get; } = "CHEF_SLICEUNLOCKABLE_ACHIEVEMENT_ID";
        public override string UnlockableIdentifier { get; } = "CHEF_SLICEUNLOCKABLE_REWARD_ID";
        public override string AchievementNameToken { get; } = "CHEF_SLICEUNLOCKABLE_ACHIEVEMENT_NAME";
        public override string PrerequisiteUnlockableIdentifier { get; } = "CHEF_CHEFUNLOCKABLE_REWARD_ID";
        public override string UnlockableNameToken { get; } = "CHEF_SLICEUNLOCKABLE_UNLOCKABLE_NAME";
        public override string AchievementDescToken { get; } = "CHEF_SLICEUNLOCKABLE_ACHIEVEMENT_DESC";
        public override Sprite Sprite { get; } = Assets.chefSliceIcon;

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString("CHEF_SLICEUNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString("CHEF_SLICEUNLOCKABLE_ACHIEVEMENT_DESC")
                            }));
        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString("CHEF_SLICEUNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString("CHEF_SLICEUNLOCKABLE_ACHIEVEMENT_DESC")
                            }));

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("GnomeChefBody");
        }

        private int cleavercount = 0;

        private void increment()
        {
            cleavercount++;
        }

        private void decrement()
        {
            cleavercount--;
        }

        private void check(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            if (self.baseNameToken == "CHEF_NAME" && cleavercount >= 40)
            {
                base.Grant();
            }
            orig(self);
        }

        public override void OnInstall()
        {
            base.OnInstall();

            CoomerangProjectile.CleaverCreated += increment;
            CoomerangProjectile.Returned += decrement;
            On.RoR2.CharacterBody.FixedUpdate += check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            CoomerangProjectile.CleaverCreated -= increment;
            CoomerangProjectile.Returned -= decrement;
            On.RoR2.CharacterBody.FixedUpdate -= check;
        }
    }
}
