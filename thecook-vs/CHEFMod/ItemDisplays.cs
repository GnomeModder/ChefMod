using System;
using UnityEngine;
using R2API;
using RoR2;
using R2API.Utils;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ChefMod
{
    public static class ItemDisplays
    {
        public static List<ItemDisplayRuleSet.KeyAssetRuleGroup> list;
        public static List<ItemDisplayRuleSet.KeyAssetRuleGroup> list2;

        public static GameObject capacitorPrefab;
        public static GameObject gatDronePrefab;

        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

        public static void RegisterDisplays(GameObject bodyPrefab)
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            PopulateDisplays();

            ItemDisplayRuleSet itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();

            list = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();
            list2 = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            //add item displays here
            #region DisplayRules
            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Jetpack,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBugWings"),
                            limbMask = LimbFlags.None,
                            childName = "Hat",
                            localPos = new Vector3(0F, -0.0198F, -0.0186F),
                            localAngles = new Vector3(11.1031F, 0F, 0F),
                            localScale = new Vector3(0.0084F, 0.0084F, 0.0084F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.GoldGat,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldGat"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0248F, 0.0303F, -0.001F),
                            localAngles = new Vector3(318.5964F, 98.796F, 354.1569F),
                            localScale = new Vector3(0.0052F, 0.0052F, 0.0052F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.BFG,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBFG"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0093F, 0.0189F, -0.0003F),
                            localAngles = new Vector3(357.514F, 359.7857F, 24.8675F),
                            localScale = new Vector3(0.0148F, 0.0148F, 0.0148F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.CritGlasses,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(-0.0001F, 0.0038F, 0.0045F),
                            localAngles = new Vector3(10.04F, 359.548F, 359.9603F),
                            localScale = new Vector3(0.0136F, 0.0159F, 0.0112F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Syringe,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySyringeCluster"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0071F, -0.0005F, -0.0045F),
                            localAngles = new Vector3(323.9519F, 215.3434F, 92.5675F),
                            localScale = new Vector3(0.0048F, 0.0048F, 0.0048F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Behemoth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBehemoth"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0002F, -0.0048F, -0.0008F),
                            localAngles = new Vector3(83.7427F, 272.0482F, 94.0287F),
                            localScale = new Vector3(0.0032F, 0.003F, 0.0032F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Missile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileLauncher"),
                            limbMask = LimbFlags.None,
                            childName = "LeftShoulder",
                            localPos = new Vector3(-0.0047F, -0.0141F, -0.0004F),
                            localAngles = new Vector3(19.1176F, 96.9864F, 181.2133F),
                            localScale = new Vector3(0.0037F, 0.0037F, 0.0037F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Dagger,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDagger"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0125F, 0.0207F, 0.0019F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0473F, 0.0473F, 0.0473F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Hoof,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHoof"),
                            limbMask = LimbFlags.None,
                            childName = "LeftLeg",
                            localPos = new Vector3(0.0006F, 0.0174F, -0.0025F),
                            localAngles = new Vector3(79.3059F, 350.5624F, 347.8609F),
                            localScale = new Vector3(0.003F, 0.0032F, 0.0039F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ChainLightning,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayUkulele"),
                            limbMask = LimbFlags.None,
                            childName = "LeftHand",
                            localPos = new Vector3(-0.0092F, 0.0033F, -0.0175F),
                            localAngles = new Vector3(71.18F, 279.1243F, 235.7804F),
                            localScale = new Vector3(0.0363F, 0.0363F, 0.0363F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.GhostOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMask"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(-0.0001F, 0.0047F, 0.004F),
                            localAngles = new Vector3(14.8527F, 356.0418F, 357.6503F),
                            localScale = new Vector3(0.0224F, 0.0224F, 0.0224F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Mushroom,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMushroom"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0108F, 0.0054F, 0.0007F),
                            localAngles = new Vector3(42.7091F, 90.5678F, 0.706F),
                            localScale = new Vector3(0.0018F, 0.0018F, 0.0018F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AttackSpeedOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
                            limbMask = LimbFlags.None,
                            childName = "Hat",
                            localPos = new Vector3(-0.0039F, 0.0089F, 0.0073F),
                            localAngles = new Vector3(33.6649F, 4.6321F, 21.8836F),
                            localScale = new Vector3(0.0204F, 0.0204F, 0.0204F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BleedOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTriTip"),
                            limbMask = LimbFlags.None,
                            childName = "Cleaver",
                            localPos = new Vector3(-0.001F, 0.0233F, 0.0001F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0089F, 0.0089F, 0.0089F)
                        }
                    }
                }
            });

            //list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            //{
            //    keyAsset = RoR2Content.Items.BleedOnHit,
            //    displayRuleGroup = new DisplayRuleGroup
            //    {
            //        rules = new ItemDisplayRule[]
            //        {
            //            new ItemDisplayRule
            //            {
            //                ruleType = ItemDisplayRuleType.ParentedPrefab,
            //                followerPrefab = ItemDisplays.LoadDisplay("DisplayTriTip"),
            //                limbMask = LimbFlags.None,
            //                childName = "Knife",
            //                localPos = new Vector3(-0.001F, 0.0233F, 0.0001F),
            //                localAngles = new Vector3(0F, 0F, 0F),
            //                localScale = new Vector3(0.0089F, 0.0089F, 0.0089F)
            //            }
            //        }
            //    }
            //});

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.WardOnLevel,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarbanner"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0019F, 0.002F, -0.0102F),
                            localAngles = new Vector3(0F, 0F, 90F),
                            localScale = new Vector3(0.0227F, 0.0227F, 0.0227F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HealOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayScythe"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0.0136F, -0.0114F),
                            localAngles = new Vector3(325F, 272F, 86F),
                            localScale = new Vector3(0.0092F, 0.0092F, 0.0092F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HealWhileSafe,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySnail"),
                            limbMask = LimbFlags.None,
                            childName = "RightShoulder",
                            localPos = new Vector3(0.0015F, -0.0007F, -0.0019F),
                            localAngles = new Vector3(295.9211F, 158.2967F, 169.4749F),
                            localScale = new Vector3(0.0037F, 0.0037F, 0.0037F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Clover,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayClover"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0016F, 0.01F, 0.0018F),
                            localAngles = new Vector3(8.4173F, 0F, 0F),
                            localScale = new Vector3(0.0133F, 0.0133F, 0.0133F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BarrierOnOverHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAegis"),
                            limbMask = LimbFlags.None,
                            childName = "LeftArm3",
                            localPos = new Vector3(0.0018F, 0.009F, 0F),
                            localAngles = new Vector3(77.0525F, 280.131F, 257.4214F),
                            localScale = new Vector3(0.0127F, 0.0127F, 0.0127F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.GoldOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBoneCrown"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0F, 0.0048F, 0F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0449F, 0.0449F, 0.0461F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.WarCryOnMultiKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPauldron"),
                            limbMask = LimbFlags.None,
                            childName = "LeftShoulder",
                            localPos = new Vector3(0F, 0F, 0F),
                            localAngles = new Vector3(80.7989F, 158.242F, 341.6488F),
                            localScale = new Vector3(0.0431F, 0.0431F, 0.0431F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintArmor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBuckler"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0.0101F, 0.0005F),
                            localAngles = new Vector3(88.919F, -0.0005F, 269.9995F),
                            localScale = new Vector3(0.0054F, 0.0054F, 0.0054F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IceRing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayIceRing"),
                            limbMask = LimbFlags.None,
                            childName = "LeftHand",
                            localPos = new Vector3(0.0052F, -0.002F, -0.0045F),
                            localAngles = new Vector3(74.5752F, 114.7591F, 115.2486F),
                            localScale = new Vector3(0.0263F, 0.035F, 0.035F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FireRing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFireRing"),
                            limbMask = LimbFlags.None,
                            childName = "LeftHand",
                            localPos = new Vector3(0.0052F, -0.002F, -0.0045F),
                            localAngles = new Vector3(74.5752F, 114.7591F, 115.2486F),
                            localScale = new Vector3(0.0263F, 0.035F, 0.035F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.UtilitySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.005F, 0.015F, 0F),
                            localAngles = new Vector3(5.9633F, 346.6898F, 241.5504F),
                            localScale = new Vector3(0.05F, 0.05F, 0.05F)
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0048F, 0.0148F, 0F),
                            localAngles = new Vector3(7.5159F, 17.7374F, 119.9052F),
                            localScale = new Vector3(0.05F, 0.05F, 0.05F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.JumpBoost,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWaxBird"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0086F, -0.0051F, -0.0114F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0415F, 0.0415F, 0.0415F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ArmorReductionOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarhammer"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, -0.0047F, -0.0086F),
                            localAngles = new Vector3(61.7479F, 180F, 180F),
                            localScale = new Vector3(0.0108F, 0.0108F, 0.0108F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NearbyDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDiamond"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0F, 0.0027F, 0.005F),
                            localAngles = new Vector3(21.5282F, 0F, 0F),
                            localScale = new Vector3(0.0013F, 0.0019F, 0.0019F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ArmorPlate,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
                            limbMask = LimbFlags.None,
                            childName = "LeftLeg",
                            localPos = new Vector3(-0.0007F, -0.0002F, -0.0004F),
                            localAngles = new Vector3(70.1178F, 0F, 0F),
                            localScale = new Vector3(0.01F, 0.01F, 0.01F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CommandMissile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileRack"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0.0023F, 0.0109F),
                            localAngles = new Vector3(90F, 0F, 0F),
                            localScale = new Vector3(0.0166F, 0.0166F, 0.0166F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Feather,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFeather"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(-0.0046F, 0.0083F, -0.0012F),
                            localAngles = new Vector3(34.5417F, 218.6242F, 343.8996F),
                            localScale = new Vector3(0.0009F, 0.0009F, 0.0002F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Crowbar,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayCrowbar"),
                            limbMask = LimbFlags.None,
                            childName = "Door",
                            localPos = new Vector3(0.0061F, 0.011F, 0.0173F),
                            localAngles = new Vector3(2.1563F, 228.7778F, 98.6541F),
                            localScale = new Vector3(0.0175F, 0.0175F, 0.0175F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FallBoots,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
                            limbMask = LimbFlags.None,
                            childName = "LeftLeg",
                            localPos = new Vector3(0F, 0.0223F, -0.0002F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0069F, 0.0069F, 0.0069F)
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
                            limbMask = LimbFlags.None,
                            childName = "RightLeg",
                            localPos = new Vector3(0F, 0.0223F, -0.0002F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0069F, 0.0069F, 0.0069F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExecuteLowHealthElite,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGuillotine"),
                            limbMask = LimbFlags.None,
                            childName = "Cleaver",
                            localPos = new Vector3(0.0029F, 0.0155F, -0.0001F),
                            localAngles = new Vector3(1.1394F, 90.2069F, 269.7308F),
                            localScale = new Vector3(0.0211F, 0.0128F, 0.0138F)
                        }
                    }
                }
            });

            //list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            //{
            //    keyAsset = RoR2Content.Items.ExecuteLowHealthElite,
            //    displayRuleGroup = new DisplayRuleGroup
            //    {
            //        rules = new ItemDisplayRule[]
            //        {
            //            new ItemDisplayRule
            //            {
            //                ruleType = ItemDisplayRuleType.ParentedPrefab,
            //                followerPrefab = ItemDisplays.LoadDisplay("DisplayGuillotine"),
            //                limbMask = LimbFlags.None,
            //                childName = "Knife",
            //                localPos = new Vector3(0.0029F, 0.0155F, -0.0001F),
            //                localAngles = new Vector3(1.1394F, 90.2069F, 269.7308F),
            //                localScale = new Vector3(0.0211F, 0.0128F, 0.0138F)
            //            }
            //        }
            //    }
            //});

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.EquipmentMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBattery"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0139F, 0.0013F, 0.0001F),
                            localAngles = new Vector3(0F, 270F, 0F),
                            localScale = new Vector3(0.0059F, 0.0059F, 0.0059F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NovaOnHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(-0.0004F, 0.0012F, 0.0046F),
                            localAngles = new Vector3(320.8713F, 37.5248F, 351.3714F),
                            localScale = new Vector3(0.0166F, 0.0166F, 0.0166F)
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0.0006F, 0.0012F, 0.0045F),
                            localAngles = new Vector3(322.0167F, 317.9099F, 4.5252F),
                            localScale = new Vector3(-0.0166F, 0.0166F, 0.0166F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Infusion,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayInfusion"),
                            limbMask = LimbFlags.None,
                            childName = "Door",
                            localPos = new Vector3(0.006F, 0.0079F, 0.0011F),
                            localAngles = new Vector3(354.0444F, 347.3777F, 356.7757F),
                            localScale = new Vector3(0.0248F, 0.0248F, 0.0248F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Medkit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMedkit"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0121F, 0.0018F, 0F),
                            localAngles = new Vector3(270F, 90F, 0F),
                            localScale = new Vector3(0.0153F, 0.0153F, 0.0153F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Bandolier,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBandolier"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0016F, 0.0212F, 0F),
                            localAngles = new Vector3(86.5579F, 260.5948F, 261.6859F),
                            localScale = new Vector3(0.0209F, 0.0213F, 0.0321F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BounceNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHook"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0087F, 0.002F, -0.0093F),
                            localAngles = new Vector3(11.1082F, 335.0989F, 22.5401F),
                            localScale = new Vector3(0.01F, 0.01F, 0.01F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IgniteOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGasoline"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0115F, 0.0143F, 0.0003F),
                            localAngles = new Vector3(79.7958F, 215.1858F, 218.462F),
                            localScale = new Vector3(0.0286F, 0.0286F, 0.0286F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.StunChanceOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayStunGrenade"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(-0.0002F, 0.0013F, 0.0017F),
                            localAngles = new Vector3(90F, 90F, 0F),
                            localScale = new Vector3(0.048F, 0.0888F, 0.0219F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Firework,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFirework"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0118F, 0.0024F, -0.0102F),
                            localAngles = new Vector3(41.4698F, 183.0625F, 194.6253F),
                            localScale = new Vector3(0.0063F, 0.0063F, 0.0063F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarDagger,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLunarDagger"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0007F, 0.0187F, -0.0054F),
                            localAngles = new Vector3(3.9914F, 89.3518F, 177.9581F),
                            localScale = new Vector3(0.0016F, 0.0017F, 0.0016F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Knurl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayKnurl"),
                            limbMask = LimbFlags.None,
                            childName = "RightShoulder",
                            localPos = new Vector3(0.0007F, -0.0014F, -0.0004F),
                            localAngles = new Vector3(56.6026F, 57.9775F, 40.9218F),
                            localScale = new Vector3(0.0038F, 0.0038F, 0.0038F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BeetleGland,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBeetleGland"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(-0.0004F, 0.0019F, 0.0055F),
                            localAngles = new Vector3(8.8676F, 206.7793F, 353.8991F),
                            localScale = new Vector3(0.0004F, 0.0004F, 0.0004F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySoda"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0062F, 0.0102F, -0.0053F),
                            localAngles = new Vector3(270F, 0F, 0F),
                            localScale = new Vector3(0.0084F, 0.0084F, 0.0084F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SecondarySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
                            limbMask = LimbFlags.None,
                            childName = "Cleaver",
                            localPos = new Vector3(-0.0008F, -0.0009F, 0F),
                            localAngles = new Vector3(7.8622F, 270F, 0F),
                            localScale = new Vector3(0.0028F, 0.0047F, 0.0028F)
                        }
                    }
                }
            });

            //list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            //{
            //    keyAsset = RoR2Content.Items.SecondarySkillMagazine,
            //    displayRuleGroup = new DisplayRuleGroup
            //    {
            //        rules = new ItemDisplayRule[]
            //        {
            //            new ItemDisplayRule
            //            {
            //                ruleType = ItemDisplayRuleType.ParentedPrefab,
            //                followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
            //                limbMask = LimbFlags.None,
            //                childName = "Knife",
            //                localPos = new Vector3(-0.0008F, -0.0009F, 0F),
            //                localAngles = new Vector3(7.8622F, 270F, 0F),
            //                localScale = new Vector3(0.0028F, 0.0047F, 0.0028F)
            //            }
            //        }
            //    }
            //});

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.StickyBomb,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayStickyBomb"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0.0164F, 0F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0081F, -0.0078F, 0.0089F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TreasureCache,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayKey"),
                            limbMask = LimbFlags.None,
                            childName = "Door",
                            localPos = new Vector3(0.0141F, 0.0049F, 0.0043F),
                            localAngles = new Vector3(79.378F, 281.7222F, 20.4748F),
                            localScale = new Vector3(0.0207F, 0.0207F, 0.0207F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BossDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAPRound"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0F, -0.0088F),
                            localAngles = new Vector3(90F, 11.4108F, 0F),
                            localScale = new Vector3(0.0158F, 0.0158F, 0.0158F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SlowOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBauble"),
                            limbMask = LimbFlags.None,
                            childName = "Door",
                            localPos = new Vector3(0.0138F, 0.0168F, 0.0033F),
                            localAngles = new Vector3(17.4254F, 115.0501F, 143.0897F),
                            localScale = new Vector3(0.0093F, 0.0099F, 0.0103F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExtraLife,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHippo"),
                            limbMask = LimbFlags.None,
                            childName = "Hat",
                            localPos = new Vector3(0.00315F, 0.01512F, -0.00771F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0069F, 0.0069F, 0.0069F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.KillEliteFrenzy,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrainstalk"),
                            limbMask = LimbFlags.None,
                            childName = "Hat",
                            localPos = new Vector3(0F, 0.004F, 0.0024F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0176F, 0.0176F, 0.0176F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RepeatHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayCorpseFlower"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0127F, 0.0133F, 0.0049F),
                            localAngles = new Vector3(0F, 322F, 270F),
                            localScale = new Vector3(0.0058F, 0.0058F, 0.0058F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AutoCastEquipment,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFossil"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0108F, 0.0027F, -0.0003F),
                            localAngles = new Vector3(82.3217F, 192.0874F, 190.1216F),
                            localScale = new Vector3(0.0233F, 0.0233F, 0.0233F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IncreaseHealing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0.0043F, 0.0063F, 0.0024F),
                            localAngles = new Vector3(0F, 90F, 0F),
                            localScale = new Vector3(0.0098F, 0.0098F, 0.0098F)
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(-0.0044F, 0.006F, 0.0018F),
                            localAngles = new Vector3(0F, 270F, 0F),
                            localScale = new Vector3(-0.0098F, 0.0098F, 0.0098F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TitanGoldDuringTP,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldHeart"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0087F, 0.0011F, 0.0078F),
                            localAngles = new Vector3(13.7933F, 293.6213F, 275.9526F),
                            localScale = new Vector3(0.0042F, 0.0042F, 0.0042F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintWisp,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrokenMask"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0003F, 0.0128F, -0.012F),
                            localAngles = new Vector3(349.4339F, 178.573F, 314.7586F),
                            localScale = new Vector3(0.0134F, 0.0134F, 0.0134F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BarrierOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrooch"),
                            limbMask = LimbFlags.None,
                            childName = "Hat",
                            localPos = new Vector3(0.0001F, -0.0073F, 0.004F),
                            localAngles = new Vector3(77.1112F, 184.5349F, 182.6944F),
                            localScale = new Vector3(0.0147F, 0.0147F, 0.0147F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TPHealingNova,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGlowFlower"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0085F, 0.0102F, 0.0048F),
                            localAngles = new Vector3(4.5569F, 50.6902F, 0F),
                            localScale = new Vector3(0.0122F, 0.0122F, 0.0122F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarUtilityReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdFoot"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.007F, 0.02F, -0.0023F),
                            localAngles = new Vector3(6.7464F, 2.7068F, 291.9216F),
                            localScale = new Vector3(-0.0035F, 0.0083F, 0.0269F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Thorns,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRazorwireLeft"),
                            limbMask = LimbFlags.None,
                            childName = "LeftArm4",
                            localPos = new Vector3(-0.0005F, 0F, 0F),
                            localAngles = new Vector3(278.9942F, 310.0565F, 55.9508F),
                            localScale = new Vector3(0.0135F, 0.0203F, 0.0122F)
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRazorwireLeft"),
                            limbMask = LimbFlags.None,
                            childName = "LeftArm3",
                            localPos = new Vector3(-0.0005F, 0F, 0F),
                            localAngles = new Vector3(278.9942F, 310.0565F, 55.9508F),
                            localScale = new Vector3(0.0135F, 0.0203F, 0.0122F)
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRazorwireLeft"),
                            limbMask = LimbFlags.None,
                            childName = "LeftArm2",
                            localPos = new Vector3(-0.0005F, 0F, 0F),
                            localAngles = new Vector3(278.9942F, 310.0565F, 55.9508F),
                            localScale = new Vector3(0.0135F, 0.0203F, 0.0122F)
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRazorwireLeft"),
                            limbMask = LimbFlags.None,
                            childName = "LeftArm1",
                            localPos = new Vector3(-0.0005F, 0F, 0F),
                            localAngles = new Vector3(278.9942F, 310.0565F, 55.9508F),
                            localScale = new Vector3(0.0135F, 0.0203F, 0.0122F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarPrimaryReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdEye"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(-0.0001F, 0.0058F, 0.0053F),
                            localAngles = new Vector3(270F, 0F, 0F),
                            localScale = new Vector3(0.0048F, 0.0048F, 0.0048F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NovaOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayJellyGuts"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0F, 0.0065F, -0.0007F),
                            localAngles = new Vector3(307.964F, 10.3555F, 0F),
                            localScale = new Vector3(0.0048F, 0.0048F, 0.0048F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarTrinket,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBeads"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0067F, 0.0194F, 0.0006F),
                            localAngles = new Vector3(87.8274F, 301.1071F, 270.0035F),
                            localScale = new Vector3(0.0157F, 0.0179F, 0.0179F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Plant,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayInterstellarDeskPlant"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0098F, 0.01F, 0.0015F),
                            localAngles = new Vector3(9.3252F, 82.8658F, 7.8049F),
                            localScale = new Vector3(0.002F, 0.002F, 0.002F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Bear,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBear"),
                            limbMask = LimbFlags.None,
                            childName = "Hat",
                            localPos = new Vector3(0.0041F, 0.0106F, -0.0024F),
                            localAngles = new Vector3(288.9563F, 132.2334F, 46.1732F),
                            localScale = new Vector3(0.0069F, 0.0069F, 0.0069F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.DeathMark,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathMark"),
                            limbMask = LimbFlags.None,
                            childName = "LeftHand",
                            localPos = new Vector3(0.0031F, -0.0025F, -0.0046F),
                            localAngles = new Vector3(309.7751F, 346.5885F, 33.8053F),
                            localScale = new Vector3(0.0008F, 0.0008F, 0.0008F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExplodeOnDeath,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWilloWisp"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0026F, 0.0008F, -0.0091F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0013F, 0.0013F, 0.0013F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Seed,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySeed"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0102F, 0.0053F, 0.0042F),
                            localAngles = new Vector3(331.7216F, 179.9999F, 62.2966F),
                            localScale = new Vector3(0.0012F, 0.0012F, 0.0012F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintOutOfCombat,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWhip"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0005F, 0.0043F, 0F),
                            localAngles = new Vector3(0F, 0F, 90F),
                            localScale = new Vector3(0.027F, 0.027F, 0.027F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.CooldownOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySkull"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0003F, 0.0051F, 0.0097F),
                            localAngles = new Vector3(271.6967F, 0F, 0F),
                            localScale = new Vector3(0.0043F, 0.0043F, 0.0043F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Phasing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayStealthkit"),
                            limbMask = LimbFlags.None,
                            childName = "LeftLeg",
                            localPos = new Vector3(0.0006F, -0.0056F, 0.0032F),
                            localAngles = new Vector3(36.0807F, 0F, 0F),
                            localScale = new Vector3(0.0068F, 0.004F, 0.0068F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.PersonalShield,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldGenerator"),
                            limbMask = LimbFlags.None,
                            childName = "LeftLeg",
                            localPos = new Vector3(0.0005F, -0.0035F, 0.0106F),
                            localAngles = new Vector3(307.9786F, 182.1596F, 173.3755F),
                            localScale = new Vector3(0.006F, 0.006F, 0.006F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShockNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTeslaCoil"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0.0171F, -0.0101F),
                            localAngles = new Vector3(0F, 265.8423F, 90F),
                            localScale = new Vector3(0.012F, 0.012F, 0.012F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShieldOnly,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0.003F, 0.0053F, 0.0018F),
                            localAngles = new Vector3(0F, 324.4279F, 344.3555F),
                            localScale = new Vector3(0.0163F, 0.0163F, 0.0163F)
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(-0.0055F, 0.0069F, 0.001F),
                            localAngles = new Vector3(343.2643F, 67.6254F, 6.7602F),
                            localScale = new Vector3(-0.0163F, 0.0163F, 0.0163F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AlienHead,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAlienHead"),
                            limbMask = LimbFlags.None,
                            childName = "LeftArm1",
                            localPos = new Vector3(0F, 0.0002F, 0.0006F),
                            localAngles = new Vector3(60.8902F, 273.8373F, 205.3746F),
                            localScale = new Vector3(0.0486F, 0.0486F, 0.0486F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HeadHunter,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySkullCrown"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0F, 0.0049F, 0F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0186F, 0.0074F, 0.0074F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.EnergizedOnEquipmentUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarHorn"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0067F, 0.0048F, 0.0083F),
                            localAngles = new Vector3(0F, 305.394F, 0F),
                            localScale = new Vector3(0.0108F, 0.0108F, 0.0108F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FlatHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySteakCurved"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.003F, 0.0103F, 0.0013F),
                            localAngles = new Vector3(309.8688F, 54.7951F, 179.9999F),
                            localScale = new Vector3(0.0027F, 0.0027F, 0.0027F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Tooth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshLarge"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0004F, 0.005F, 0.0097F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.075F, 0.075F, 0.075F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Pearl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPearl"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0F, 0F),
                            localAngles = new Vector3(301.4987F, 66.0694F, 288.6561F),
                            localScale = new Vector3(0.0104F, 0.0104F, 0.0104F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShinyPearl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShinyPearl"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0F, 0F),
                            localAngles = new Vector3(305.3945F, 238.4917F, 117.2391F),
                            localScale = new Vector3(0.0113F, 0.0113F, 0.0113F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BonusGoldPackOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTome"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0099F, 0.0149F, -0.0062F),
                            localAngles = new Vector3(355.9361F, 298.4621F, 352.3327F),
                            localScale = new Vector3(0.0026F, 0.0026F, 0.0026F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Squid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySquidTurret"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0084F, 0.0002F, 0.0051F),
                            localAngles = new Vector3(0F, 147.8985F, 90F),
                            localScale = new Vector3(0.0017F, 0.0017F, 0.0017F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Icicle,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFrostRelic"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.021F, 0.0322F, -0.0162F),
                            localAngles = new Vector3(90F, 0F, 0F),
                            localScale = new Vector3(1F, 1F, 1F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Talisman,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTalisman"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0336F, 0.0394F, 0.0059F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(1F, 1F, 1F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LaserTurbine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLaserTurbine"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0307F, -0.01F, -0.023F),
                            localAngles = new Vector3(273.7555F, 180F, 180F),
                            localScale = new Vector3(0.02F, 0.02F, 0.02F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FocusConvergence,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFocusedConvergence"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.017F, 0.0327F, -0.0076F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0253F, 0.0253F, 0.0253F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Incubator,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAncestralIncubator"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0086F, 0.0195F, -0.0053F),
                            localAngles = new Vector3(0F, 25F, 0F),
                            localScale = new Vector3(0.0019F, 0.0019F, 0.0019F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FireballsOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFireballsOnHit"),
                            limbMask = LimbFlags.None,
                            childName = "Door",
                            localPos = new Vector3(0.0056F, 0.0057F, 0.0057F),
                            localAngles = new Vector3(342.6464F, 351.4756F, 164.878F),
                            localScale = new Vector3(0.0025F, 0.0025F, 0.0025F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SiphonOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySiphonOnLowHealth"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0064F, 0.0009F, 0.0099F),
                            localAngles = new Vector3(344.1552F, 318.8208F, 323.2846F),
                            localScale = new Vector3(0.0019F, 0.0019F, 0.0019F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BleedOnHitAndExplode,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBleedOnHitAndExplode"),
                            limbMask = LimbFlags.None,
                            childName = "Hat",
                            localPos = new Vector3(0F, -0.0004F, 0.0039F),
                            localAngles = new Vector3(14.2161F, 359.8377F, 359.9374F),
                            localScale = new Vector3(0.0101F, 0.0079F, 0.0126F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.MonstersOnShrineUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMonstersOnShrineUse"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0.019F, -0.0061F),
                            localAngles = new Vector3(0F, 270F, 90.2633F),
                            localScale = new Vector3(0.0007F, 0.0007F, 0.0007F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RandomDamageZone,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRandomDamageZone"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0036F, 0.0185F, -0.0047F),
                            localAngles = new Vector3(0F, 90F, 90F),
                            localScale = new Vector3(0.001F, 0.0005F, 0.0007F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Fruit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFruit"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0084F, -0.0069F, 0.0025F),
                            localAngles = new Vector3(339.9772F, 329.1383F, 22.8184F),
                            localScale = new Vector3(0.0096F, 0.0096F, 0.0096F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixRed,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0.0042F, 0.0075F, 0F),
                            localAngles = new Vector3(23.0159F, 347.9504F, 331.3674F),
                            localScale = new Vector3(0.0037F, 0.0037F, 0.0037F)
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(-0.0042F, 0.0075F, 0F),
                            localAngles = new Vector3(23.0159F, 12.0496F, 28.6326F),
                            localScale = new Vector3(-0.0037F, 0.0037F, 0.0037F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixBlue,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0F, 0.0062F, 0.0048F),
                            localAngles = new Vector3(315F, 0F, 0F),
                            localScale = new Vector3(0.0141F, 0.0141F, 0.0141F)
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0F, 0.0088F, 0.0046F),
                            localAngles = new Vector3(291F, 0F, 0F),
                            localScale = new Vector3(0.0086F, 0.0086F, 0.0086F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixWhite,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteIceCrown"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0F, 0.0136F, -0.0012F),
                            localAngles = new Vector3(270F, 0F, 0F),
                            localScale = new Vector3(0.0013F, 0.0013F, 0.0013F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixPoison,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteUrchinCrown"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0F, 0.0085F, 0F),
                            localAngles = new Vector3(270F, 0F, 0F),
                            localScale = new Vector3(0.0025F, 0.0025F, 0.0025F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixHaunted,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteStealthCrown"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0F, 0.0071F, 0F),
                            localAngles = new Vector3(270F, 0F, 0F),
                            localScale = new Vector3(0.0023F, 0.0023F, 0.0023F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CritOnUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayNeuralImplant"),
                            limbMask = LimbFlags.None,
                            childName = "Head",
                            localPos = new Vector3(0F, 0.0035F, 0.0088F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0087F, 0.007F, 0.007F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.DroneBackup,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRadio"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0114F, 0.0181F, 0.0039F),
                            localAngles = new Vector3(1.4382F, 276.6002F, 359.4386F),
                            localScale = new Vector3(0.0214F, 0.0214F, 0.0214F)
                        }
                    }
                }
            });

            //list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            //{
            //    keyAsset = RoR2Content.Equipment.Lightning,
            //    displayRuleGroup = new DisplayRuleGroup
            //    {
            //        rules = new ItemDisplayRule[]
            //        {
            //            new ItemDisplayRule
            //            {
            //                ruleType = ItemDisplayRuleType.ParentedPrefab,
            //                followerPrefab = ItemDisplays.capacitorPrefab,
            //                limbMask = LimbFlags.None,
            //                childName = "Body",
            //                localPos = new Vector3(-0.0247F, 0.0065F, 0.0011F),
            //                localAngles = new Vector3(53.2803F, 265.2803F, 39.4167F),
            //                localScale = new Vector3(0.0325F, 0.0325F, 0.0325F)
            //            }
            //        }
            //    }
            //});

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.BurnNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPotion"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0071F, 0.018F, 0F),
                            localAngles = new Vector3(0F, 0F, 315F),
                            localScale = new Vector3(0.0008F, 0.0008F, 0.0008F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CrippleWard,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEffigy"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0.0059F, 0.0185F, -0.0044F),
                            localAngles = new Vector3(0F, 180F, 0F),
                            localScale = new Vector3(0.0006F, 0.0006F, 0.0006F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.QuestVolatileBattery,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBatteryArray"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0.0146F, -0.0129F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.0085F, 0.0085F, 0.0085F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.GainArmor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayElephantFigure"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0076F, 0.0208F, -0.0014F),
                            localAngles = new Vector3(355.084F, 0F, 0F),
                            localScale = new Vector3(0.0159F, 0.0159F, 0.0159F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Recycle,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRecycler"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.012F, 0.0228F, 0.0006F),
                            localAngles = new Vector3(0F, 164.0281F, 0F),
                            localScale = new Vector3(0.0033F, 0.0033F, 0.0033F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.FireBallDash,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEgg"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0069F, 0.0199F, -0.0023F),
                            localAngles = new Vector3(281.0042F, 270.0001F, 179.9999F),
                            localScale = new Vector3(0.0065F, 0.0065F, 0.0065F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Cleanse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWaterPack"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0.0114F, -0.0105F),
                            localAngles = new Vector3(0F, 180F, 0F),
                            localScale = new Vector3(0.0034F, 0.0034F, 0.0034F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Tonic,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTonic"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0083F, 0.0196F, 0F),
                            localAngles = new Vector3(286.707F, 90.0001F, -0.0001F),
                            localScale = new Vector3(0.0054F, 0.0054F, 0.0054F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Gateway,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayVase"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0104F, 0.0229F, -0.0075F),
                            localAngles = new Vector3(315F, 0F, 0F),
                            localScale = new Vector3(-0.0071F, 0.0071F, 0.0071F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Meteor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMeteor"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0.0637F, 0.0019F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(1F, 1F, 1F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Saw,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySawmerang"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0185F, 0.0359F, 0F),
                            localAngles = new Vector3(0F, 90F, 0F),
                            localScale = new Vector3(0.2F, 0.2F, 0.2F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Blackhole,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravCube"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0222F, 0.05F, -0.0234F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.5048F, 0.5048F, 0.5048F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Scanner,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayScanner"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0108F, 0.0181F, 0F),
                            localAngles = new Vector3(304.6127F, 258.8317F, 92.298F),
                            localScale = new Vector3(0.0088F, 0.0088F, 0.0088F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.DeathProjectile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathProjectile"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0099F, 0.0181F, -0.0093F),
                            localAngles = new Vector3(0F, 213.0944F, 0F),
                            localScale = new Vector3(0.003F, 0.003F, 0.003F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.LifestealOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLifestealOnHit"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(-0.0107F, 0.0166F, -0.01F),
                            localAngles = new Vector3(15.0461F, 32.631F, 107.2616F),
                            localScale = new Vector3(0.003F, 0.003F, 0.003F)
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.TeamWarCry,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTeamWarCry"),
                            limbMask = LimbFlags.None,
                            childName = "Body",
                            localPos = new Vector3(0F, 0.0022F, 0.0127F),
                            localAngles = new Vector3(270F, 180F, 0F),
                            localScale = new Vector3(0.0041F, 0.0041F, 0.0041F)
                        }
                    }
                }
            });
            #endregion
            //apply displays here

            itemDisplayRuleSet.keyAssetRuleGroups = list.ToArray();
            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
            characterModel.itemDisplayRuleSet.GenerateRuntimeValues();
        }

        public static GameObject LoadDisplay(string name)
        {
            if (itemDisplayPrefabs.ContainsKey(name.ToLower()))
            {
                if (itemDisplayPrefabs[name.ToLower()]) return itemDisplayPrefabs[name.ToLower()];
            }
            return null;
        }

        private static void PopulateDisplays()
        {
            ItemDisplayRuleSet itemDisplayRuleSet = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;

            for (int i = 0; i < item.Length; i++)
            {
                ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;

                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if (followerPrefab)
                    {
                        string name = followerPrefab.name;
                        string key = (name != null) ? name.ToLower() : null;
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }
    }
}