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

namespace ChefMod
{
    static class Unlockables
    {
        public static UnlockableDef chefUnlockDef;
        public static UnlockableDef sliceUnlockDef;

        public static void RegisterUnlockables()
        {
            LanguageAPI.Add("CHEF_CHEFUNLOCKABLE_ACHIEVEMENT_NAME", "Mise en Place");
            LanguageAPI.Add("CHEF_CHEFUNLOCKABLE_ACHIEVEMENT_DESC", "Gather Ingredients");
            LanguageAPI.Add("CHEF_CHEFUNLOCKABLE_UNLOCKABLE_NAME", "Mise en Place");

            LanguageAPI.Add("CHEF_SLICEUNLOCKABLE_ACHIEVEMENT_NAME", "Full Set");
            LanguageAPI.Add("CHEF_SLICEUNLOCKABLE_ACHIEVEMENT_DESC", "Have 40 cleavers in the air at once");
            LanguageAPI.Add("CHEF_SLICEUNLOCKABLE_UNLOCKABLE_NAME", "Full Set");

            chefUnlockDef = Unlockable.AddUnlockable<Achievements.ChefAchievement>(true);
            sliceUnlockDef = Unlockable.AddUnlockable<Achievements.SliceAchievement>(true);
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
            return BodyCatalog.FindBodyIndex("ChefBody");
        }

        private bool warning = false;
        private bool spawned = false;
        private bool death = false;

        string[] chefwarnings = { "JE PEUX LE SENTIR", "JE VIENS À VOUS BIENTÔT", "MON FOUR PRÉCHAUFFE", "VOUS M'AVEZ AROUSÉ", "LE DÎNER EST TOUJOURS À L'HEURE" };

        private void CheckItem(On.RoR2.CharacterMaster.orig_OnInventoryChanged orig, CharacterMaster self)
        {
            orig(self);

            if (self && self.teamIndex == TeamIndex.Player && self.inventory)
            {
                int count = getFoodCount(self.inventory);

                if (!warning && count > 8)
                {
                    warning = true;
                    int choice = UnityEngine.Random.Range(0, chefwarnings.Length);
                    Chat.AddMessage(Util.GenerateColoredString("CHEF: " + chefwarnings[choice], Color.red));
                }
                if (warning && !death && !spawned && count > 9)
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
            if (inventory.GetEquipmentIndex() == RoR2Content.Equipment.Fruit.equipmentIndex) output += 2;
            return output;
        }

        private void Death(DamageReport report)
        {
            if (report.victimMaster && report.victimMaster.name == "ChefInvader(Clone)")
            {
                if (report.isFallDamage)
                {
                    ChefInvasionManager.PerformInvasion(new Xoroshiro128Plus(Run.instance.seed));
                    return;
                }
                death = true;
            }
        }

        private void Reset(Stage stage)
        {
            warning = false;
            spawned = false;
            death = false;
        }

        private void Check(Stage stage)
        {
            if (death) base.Grant();
        }

        private void BuffChefInvader(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            if (chefPlugin.oldChefInvader.Value && self.master && self.master.name == "ChefInvader(Clone)")
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

            if (chefPlugin.charUnlock.Value) base.Grant();

            On.RoR2.CharacterMaster.OnInventoryChanged += CheckItem;
            GlobalEventManager.onCharacterDeathGlobal += Death;
            Stage.onServerStageBegin += Reset;
            Stage.onServerStageComplete += Check;
            On.RoR2.CharacterBody.RecalculateStats += BuffChefInvader;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            On.RoR2.CharacterMaster.OnInventoryChanged -= CheckItem;
            GlobalEventManager.onCharacterDeathGlobal -= Death;
            Stage.onServerStageBegin -= Reset;
            Stage.onServerStageComplete -= Check;
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
            return BodyCatalog.FindBodyIndex("ChefBody");
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

namespace ChefMod
{
    internal static class Unlockable
    {
        private static readonly HashSet<string> usedRewardIds = new HashSet<string>();
        internal static List<AchievementDef> achievementDefs = new List<AchievementDef>();
        internal static List<UnlockableDef> unlockableDefs = new List<UnlockableDef>();
        private static readonly List<(AchievementDef achDef, UnlockableDef unlockableDef, String unlockableName)> moddedUnlocks = new List<(AchievementDef achDef, UnlockableDef unlockableDef, string unlockableName)>();

        private static bool addingUnlockables;
        public static bool ableToAdd { get; private set; } = false;

        internal static UnlockableDef CreateNewUnlockable(UnlockableInfo unlockableInfo)
        {
            UnlockableDef newUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();

            newUnlockableDef.nameToken = unlockableInfo.Name;
            newUnlockableDef.cachedName = unlockableInfo.Name;
            newUnlockableDef.getHowToUnlockString = unlockableInfo.HowToUnlockString;
            newUnlockableDef.getUnlockedString = unlockableInfo.UnlockedString;
            newUnlockableDef.sortScore = unlockableInfo.SortScore;

            return newUnlockableDef;
        }

        public static UnlockableDef AddUnlockable<TUnlockable>(bool serverTracked) where TUnlockable : BaseAchievement, IModdedUnlockableDataProvider, new()
        {
            TUnlockable instance = new TUnlockable();

            string unlockableIdentifier = instance.UnlockableIdentifier;

            if (!usedRewardIds.Add(unlockableIdentifier)) throw new InvalidOperationException($"The unlockable identifier '{unlockableIdentifier}' is already used by another mod or the base game.");

            AchievementDef achievementDef = new AchievementDef
            {
                identifier = instance.AchievementIdentifier,
                unlockableRewardIdentifier = instance.UnlockableIdentifier,
                prerequisiteAchievementIdentifier = instance.PrerequisiteUnlockableIdentifier,
                nameToken = instance.AchievementNameToken,
                descriptionToken = instance.AchievementDescToken,
                achievedIcon = instance.Sprite,
                type = instance.GetType(),
                serverTrackerType = (serverTracked ? instance.GetType() : null),
            };

            UnlockableDef unlockableDef = CreateNewUnlockable(new UnlockableInfo
            {
                Name = instance.UnlockableIdentifier,
                HowToUnlockString = instance.GetHowToUnlock,
                UnlockedString = instance.GetUnlocked,
                SortScore = 200
            });

            unlockableDefs.Add(unlockableDef);
            achievementDefs.Add(achievementDef);

            moddedUnlocks.Add((achievementDef, unlockableDef, instance.UnlockableIdentifier));

            if (!addingUnlockables)
            {
                addingUnlockables = true;
                IL.RoR2.AchievementManager.CollectAchievementDefs += CollectAchievementDefs;
                IL.RoR2.UnlockableCatalog.Init += Init_Il;
            }

            return unlockableDef;
        }

        public static ILCursor CallDel_<TDelegate>(this ILCursor cursor, TDelegate target, out Int32 index)
where TDelegate : Delegate
        {
            index = cursor.EmitDelegate<TDelegate>(target);
            return cursor;
        }
        public static ILCursor CallDel_<TDelegate>(this ILCursor cursor, TDelegate target)
            where TDelegate : Delegate => cursor.CallDel_(target, out _);

        private static void Init_Il(ILContext il) => new ILCursor(il)
    .GotoNext(MoveType.AfterLabel, x => x.MatchCallOrCallvirt(typeof(UnlockableCatalog), nameof(UnlockableCatalog.SetUnlockableDefs)))
    .CallDel_(ArrayHelper.AppendDel(unlockableDefs));

        private static void CollectAchievementDefs(ILContext il)
        {
            var f1 = typeof(AchievementManager).GetField("achievementIdentifiers", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            if (f1 is null) throw new NullReferenceException($"Could not find field in {nameof(AchievementManager)}");
            var cursor = new ILCursor(il);
            _ = cursor.GotoNext(MoveType.After,
                x => x.MatchEndfinally(),
                x => x.MatchLdloc(1)
            );

            void EmittedDelegate(List<AchievementDef> list, Dictionary<String, AchievementDef> map, List<String> identifiers)
            {
                ableToAdd = false;
                for (Int32 i = 0; i < moddedUnlocks.Count; ++i)
                {
                    var (ach, unl, unstr) = moddedUnlocks[i];
                    if (ach is null) continue;
                    identifiers.Add(ach.identifier);
                    list.Add(ach);
                    map.Add(ach.identifier, ach);
                }
            }

            _ = cursor.Emit(OpCodes.Ldarg_0);
            _ = cursor.Emit(OpCodes.Ldsfld, f1);
            _ = cursor.EmitDelegate<Action<List<AchievementDef>, Dictionary<String, AchievementDef>, List<String>>>(EmittedDelegate);
            _ = cursor.Emit(OpCodes.Ldloc_1);
        }

        internal struct UnlockableInfo
        {
            internal string Name;
            internal Func<string> HowToUnlockString;
            internal Func<string> UnlockedString;
            internal int SortScore;
        }
    }

    internal interface IModdedUnlockableDataProvider
    {
        string AchievementIdentifier { get; }
        string UnlockableIdentifier { get; }
        string AchievementNameToken { get; }
        string PrerequisiteUnlockableIdentifier { get; }
        string UnlockableNameToken { get; }
        string AchievementDescToken { get; }
        Sprite Sprite { get; }
        Func<string> GetHowToUnlock { get; }
        Func<string> GetUnlocked { get; }
    }

    internal abstract class ModdedUnlockable : BaseAchievement, IModdedUnlockableDataProvider
    {
        #region Implementation
        public void Revoke()
        {
            if (base.userProfile.HasAchievement(this.AchievementIdentifier))
            {
                base.userProfile.RevokeAchievement(this.AchievementIdentifier);
            }

            base.userProfile.RevokeUnlockable(UnlockableCatalog.GetUnlockableDef(this.UnlockableIdentifier));
        }
        #endregion

        #region Contract
        public abstract string AchievementIdentifier { get; }
        public abstract string UnlockableIdentifier { get; }
        public abstract string AchievementNameToken { get; }
        public abstract string PrerequisiteUnlockableIdentifier { get; }
        public abstract string UnlockableNameToken { get; }
        public abstract string AchievementDescToken { get; }
        public abstract Sprite Sprite { get; }
        public abstract Func<string> GetHowToUnlock { get; }
        public abstract Func<string> GetUnlocked { get; }
        #endregion

        #region Virtuals
        public override void OnGranted() => base.OnGranted();
        public override void OnInstall()
        {
            base.OnInstall();
        }
        public override void OnUninstall()
        {
            base.OnUninstall();
        }
        public override Single ProgressForAchievement() => base.ProgressForAchievement();
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return base.LookUpRequiredBodyIndex();
        }
        public override void OnBodyRequirementBroken() => base.OnBodyRequirementBroken();
        public override void OnBodyRequirementMet() => base.OnBodyRequirementMet();
        public override bool wantsBodyCallbacks { get => base.wantsBodyCallbacks; }
        #endregion
    }

    internal static class ArrayHelper
    {
        public static T[] Append<T>(ref T[] array, List<T> list)
        {
            var orig = array.Length;
            var added = list.Count;
            Array.Resize<T>(ref array, orig + added);
            list.CopyTo(array, orig);
            return array;
        }

        public static Func<T[], T[]> AppendDel<T>(List<T> list) => (r) => Append(ref r, list);
    }
}
