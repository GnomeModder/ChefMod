using System;
using UnityEngine;

namespace ChefMod
{
    public static class Assets
    {
        public static AssetBundle chefAssetBundle = LoadAssetBundle(ChefMod.Properties.Resources.chef);

        public static Sprite chefDiceIcon = Assets.chefAssetBundle.LoadAsset<Sprite>("chef_primary");
        public static Sprite chefMinceIcon = Assets.chefAssetBundle.LoadAsset<Sprite>("chef_primary_boosted");
        public static Sprite chefSearIcon = Assets.chefAssetBundle.LoadAsset<Sprite>("chef_secondary");
        public static Sprite chefFlambeIcon = Assets.chefAssetBundle.LoadAsset<Sprite>("chef_secondary_boosted");
        public static Sprite chefSauteeIcon = Assets.chefAssetBundle.LoadAsset<Sprite>("chef_alt_secondary");
        public static Sprite chefFryIcon = Assets.chefAssetBundle.LoadAsset<Sprite>("chef_alt_secondary_boosted");
        public static Sprite chefGlazeIcon = Assets.chefAssetBundle.LoadAsset<Sprite>("chef_utility");
        public static Sprite chefMarinateIcon = Assets.chefAssetBundle.LoadAsset<Sprite>("chef_utility_boosted");
        public static Sprite chefBHMIcon = Assets.chefAssetBundle.LoadAsset<Sprite>("chef_special");
        public static Sprite chefBuffetIcon = Assets.chefAssetBundle.LoadAsset<Sprite>("chef_alt_special");

        static AssetBundle LoadAssetBundle(Byte[] resourceBytes)
        {
            //Check to make sure that the byte array supplied is not null, and throw an appropriate exception if they are.
            if (resourceBytes == null) throw new ArgumentNullException(nameof(resourceBytes));

            //Actually load the bundle with a Unity function.
            var bundle = AssetBundle.LoadFromMemory(resourceBytes);

            return bundle;
        }
    }
}
