using ChefMod;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ChefPlugin
{
    public class ChefInvasionManager
    {
        public static void PerformInvasion(Xoroshiro128Plus rng)
        {
            var master = ChefMod.ChefPlugin.invaderMaster;
            if (master) CreateNemesis(master, rng);
        }

        private static void CreateNemesis(CharacterMaster master, Xoroshiro128Plus rng)
        {
            SpawnCard spawnCard = NemesisSpawnCard.FromMaster(master);
            if (!spawnCard) return;

            CharacterMaster targetMaster = null;
            for (int i = CharacterMaster.readOnlyInstancesList.Count - 1; i >= 0; i--)
            {
                CharacterMaster tempMaster = CharacterMaster.readOnlyInstancesList[i];
                if (tempMaster.teamIndex == TeamIndex.Player && tempMaster.playerCharacterMasterController)
                {
                    targetMaster = tempMaster;
                }
            }

            if (!targetMaster.GetBody()) return;

            Transform spawnOnTarget = targetMaster.GetBody().coreTransform;
            DirectorCore.MonsterSpawnDistance input = DirectorCore.MonsterSpawnDistance.Far;

            DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule
            {
                spawnOnTarget = spawnOnTarget,
                placementMode = DirectorPlacementRule.PlacementMode.NearestNode
            };

            DirectorCore.GetMonsterSpawnDistance(input, out directorPlacementRule.minDistance, out directorPlacementRule.maxDistance);
            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, directorPlacementRule, rng);
            directorSpawnRequest.teamIndexOverride = new TeamIndex?(TeamIndex.Monster);
            directorSpawnRequest.ignoreTeamMemberLimit = true;

            CombatSquad combatSquad = null;

            DirectorSpawnRequest directorSpawnRequest2 = directorSpawnRequest;
            directorSpawnRequest2.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest2.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
            {
                if (!combatSquad)
                {
                    combatSquad = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Encounters/ShadowCloneEncounter")).GetComponent<CombatSquad>();
                }

                CharacterMaster characterMaster = result.spawnedInstance.GetComponent<CharacterMaster>();

                //fuck this
                // - I will
                if (ArenaMissionController.instance)
                {
                    Inventory arenaInventory = ArenaMissionController.instance.inventory;
                    characterMaster.inventory.AddItemsFrom(arenaInventory);
                }

                if (ChefMod.ChefPlugin.oldChefInvader.Value)
                {
                    //thanks man
                    //have some bullshit boss scaling~
                    float num = 1f;
                    float num2 = 1f;
                    num += Run.instance.difficultyCoefficient / 2.5f;
                    num2 += Run.instance.difficultyCoefficient / 30f;
                    int num3 = Mathf.Max(1, Run.instance.livingPlayerCount);
                    num *= Mathf.Pow((float)num3, 0.5f);
                    characterMaster.inventory.GiveItem(RoR2Content.Items.BoostHp, Mathf.RoundToInt((num - 1f) * 10f));
                    characterMaster.inventory.GiveItem(RoR2Content.Items.BoostDamage, Mathf.RoundToInt((num2 - 1f) * 10f));

                    //haha fuck you
                    //!
                    characterMaster.inventory.GiveItem(RoR2Content.Items.AdaptiveArmor, 1);
                }
                else
                {
                    //Swapping scaling to Vengeance-Like scaling and Scavenger items.
                    characterMaster.inventory.GiveItem(RoR2Content.Items.InvadingDoppelganger, 1);
                    characterMaster.inventory.GiveItem(RoR2Content.Items.UseAmbientLevel, 1);
                    List<PickupIndex> list = Run.instance.availableTier1DropList.Where(PickupIsNonBlacklistedItem).ToList();
                    List<PickupIndex> list2 = Run.instance.availableTier2DropList.Where(PickupIsNonBlacklistedItem).ToList();
                    List<PickupIndex> list3 = Run.instance.availableTier3DropList.Where(PickupIsNonBlacklistedItem).ToList();
                    List<PickupIndex> availableEquipmentDropList = Run.instance.availableEquipmentDropList;
                    GrantItems(characterMaster.inventory, list, 3, 9);
                    GrantItems(characterMaster.inventory, list2, 2, 4);
                    GrantItems(characterMaster.inventory, list3, 1, 1);
                }
                

                combatSquad.AddMember(characterMaster);
            }));

            DirectorCore.instance.TrySpawnObject(directorSpawnRequest);

            if (combatSquad)
            {
                NetworkServer.Spawn(combatSquad.gameObject);
            }

            UnityEngine.Object.Destroy(spawnCard);
        }

        private static void GrantItems(Inventory inventory, List<PickupIndex> list, int types, int stackSize)
        {
            for (int i = 0; i < types; i++)
            {
                PickupIndex pickupIndex = list[UnityEngine.Random.Range(0, list.Count)];
                PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
                inventory.GiveItem((pickupDef != null) ? pickupDef.itemIndex : ItemIndex.None, stackSize);
            }
        }

        private static bool PickupIsNonBlacklistedItem(PickupIndex pickupIndex)
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            if (pickupDef == null)
            {
                return false;
            }
            ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
            return !(itemDef == null) && itemDef.DoesNotContainTag(ItemTag.AIBlacklist);
        }
    }

    public class NemesisSpawnCard : CharacterSpawnCard
    {
        private CharacterMaster characterMaster;

        public static NemesisSpawnCard FromMaster(CharacterMaster master)
        {
            if (!master) return null;

            CharacterBody body = master.bodyPrefab.GetComponent<CharacterBody>();
            if (!body) return null;

            NemesisSpawnCard spawnCard = ScriptableObject.CreateInstance<NemesisSpawnCard>();
            spawnCard.hullSize = HullClassification.Human;
            spawnCard.nodeGraphType = (body.isFlying ? MapNodeGroup.GraphType.Air : MapNodeGroup.GraphType.Ground);
            spawnCard.prefab = MasterCatalog.GetMasterPrefab(MasterCatalog.FindAiMasterIndexForBody(body.bodyIndex));
            spawnCard.sendOverNetwork = true;
            spawnCard.runtimeLoadout = new Loadout();
            spawnCard.characterMaster = master;
            spawnCard.characterMaster.loadout.Copy(spawnCard.runtimeLoadout);

            return spawnCard;
        }
    }
}