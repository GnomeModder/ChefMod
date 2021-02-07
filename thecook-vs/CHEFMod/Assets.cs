using R2API;
using System;
using UnityEngine;

namespace ChefMod
{
    public static class Assets
    {
        public static AssetBundle chefAssetBundle = LoadAssetBundle(ChefMod.Properties.Resources.chef);

        public static Texture chefIcon = chefAssetBundle.LoadAsset<Texture>("cheficon");
        public static Sprite chefDiceIcon = chefAssetBundle.LoadAsset<Sprite>("chef_primary");
        public static Sprite chefMinceIcon = chefAssetBundle.LoadAsset<Sprite>("chef_primary_boosted");
        public static Sprite chefSearIcon = chefAssetBundle.LoadAsset<Sprite>("chef_secondary");
        public static Sprite chefFlambeIcon = chefAssetBundle.LoadAsset<Sprite>("chef_secondary_boosted");
        public static Sprite chefSauteeIcon = chefAssetBundle.LoadAsset<Sprite>("chef_alt_secondary");
        public static Sprite chefFryIcon = chefAssetBundle.LoadAsset<Sprite>("chef_alt_secondary_boosted");
        public static Sprite chefGlazeIcon = chefAssetBundle.LoadAsset<Sprite>("chef_utility");
        public static Sprite chefMarinateIcon = chefAssetBundle.LoadAsset<Sprite>("chef_utility_boosted");
        public static Sprite chefBHMIcon = chefAssetBundle.LoadAsset<Sprite>("chef_special");
        public static Sprite chefBuffetIcon = chefAssetBundle.LoadAsset<Sprite>("chef_alt_special");

        public static Sprite defaultSkinIcon = LoadoutAPI.CreateSkinIcon(new Color(210f / 255f, 210f / 255f, 210f / 255f), new Color(150f / 255f, 74f / 255f, 77f / 255f), new Color(98f / 255f, 128 / 255f, 131f / 255f), new Color(27f / 255f, 45f / 255f, 45f / 255f));

        public static UInt32 unloadingID = LoadSoundBank(ChefMod.Properties.Resources.ChefSoundBank);

        static AssetBundle LoadAssetBundle(Byte[] resourceBytes)
        {
            //Check to make sure that the byte array supplied is not null, and throw an appropriate exception if they are.
            if (resourceBytes == null) throw new ArgumentNullException(nameof(resourceBytes));

            //Actually load the bundle with a Unity function.
            var bundle = AssetBundle.LoadFromMemory(resourceBytes);

            return bundle;
        }

        static UInt32 LoadSoundBank(Byte[] resourceBytes)
        {
            //Check to make sure that the byte array supplied is not null, and throw an appropriate exception if they are.
            if (resourceBytes == null) throw new ArgumentNullException(nameof(resourceBytes));

            //Register the soundbank and return the ID
            return SoundAPI.SoundBanks.Add(resourceBytes);
        }
    }
}
