using ChefMod.Modules;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace ChefMod
{
    public static class Assets
    {
        public static AssetBundle chefAssetBundle = LoadAssetBundle(Properties.Resources.chef);

        public static Texture chefIcon = chefAssetBundle.LoadAsset<Texture>("cheficon");
        public static Sprite chefIconSprite = chefAssetBundle.LoadAsset<Sprite>("unlockicon");
        public static Sprite chefDiceIcon = chefAssetBundle.LoadAsset<Sprite>("chef_primary");
        public static Sprite chefMinceIcon = chefAssetBundle.LoadAsset<Sprite>("chef_primary_boosted");
        public static Sprite chefSliceIcon = chefAssetBundle.LoadAsset<Sprite>("chef_alt_primary");
        public static Sprite chefJulienneIcon = chefAssetBundle.LoadAsset<Sprite>("chef_alt_primary_boosted");
        public static Sprite chefSearIcon = chefAssetBundle.LoadAsset<Sprite>("chef_secondary");
        public static Sprite chefFlambeIcon = chefAssetBundle.LoadAsset<Sprite>("chef_secondary_boosted");
        public static Sprite chefSauteeIcon = chefAssetBundle.LoadAsset<Sprite>("chef_alt_secondary");
        public static Sprite chefFryIcon = chefAssetBundle.LoadAsset<Sprite>("chef_alt_secondary_boosted");
        public static Sprite chefGlazeIcon = chefAssetBundle.LoadAsset<Sprite>("chef_utility");
        public static Sprite chefMarinateIcon = chefAssetBundle.LoadAsset<Sprite>("chef_utility_boosted");
        public static Sprite chefBHMIcon = chefAssetBundle.LoadAsset<Sprite>("chef_special");
        public static Sprite chefBHMScepterIcon = chefAssetBundle.LoadAsset<Sprite>("chef_special_scepter");
        public static Sprite chefBuffetIcon = chefAssetBundle.LoadAsset<Sprite>("chef_alt_special");
        public static Material armmat = chefAssetBundle.LoadAsset<Material>("matArm");

        public static Sprite defaultSkinIcon = LoadoutAPI.CreateSkinIcon(new Color(210f / 255f, 210f / 255f, 210f / 255f), new Color(150f / 255f, 74f / 255f, 77f / 255f), new Color(98f / 255f, 128 / 255f, 131f / 255f), new Color(27f / 255f, 45f / 255f, 45f / 255f));

        public static Material matChefDefault = CreateMaterial("matChefDefault");
        public static Material matChefDefaultKnife = CreateMaterial("matChefDefaultKnife");

        //public static UInt32 unloadingID = LoadSoundBank(ChefMod.Properties.Resources.ChefSounds);

        private static Material commandoMat;

        static AssetBundle LoadAssetBundle(Byte[] resourceBytes)
        {
            //Check to make sure that the byte array supplied is not null, and throw an appropriate exception if they are.
            if (resourceBytes == null) throw new ArgumentNullException(nameof(resourceBytes));

            //Actually load the bundle with a Unity function.
            var bundle = AssetBundle.LoadFromMemory(resourceBytes);

            return bundle;
        }

        /*static UInt32 LoadSoundBank(Byte[] resourceBytes)
        {
            //Check to make sure that the byte array supplied is not null, and throw an appropriate exception if they are.
            if (resourceBytes == null) throw new ArgumentNullException(nameof(resourceBytes));

            //Register the soundbank and return the ID
            return SoundAPI.SoundBanks.Add(resourceBytes);
        }*/

        #region materials(old)
        private const string obsolete = "use `Materials.CreateMaterial` instead, or use the extension `Material.SetHotpooMaterial` directly on a material";
        [Obsolete(obsolete)]
        public static Material CreateMaterial(string materialName) => Assets.CreateMaterial(materialName, 0f);
        [Obsolete(obsolete)]
        public static Material CreateMaterial(string materialName, float emission) => Assets.CreateMaterial(materialName, emission, Color.white);
        [Obsolete(obsolete)]
        public static Material CreateMaterial(string materialName, float emission, Color emissionColor) => Assets.CreateMaterial(materialName, emission, emissionColor, 0f);
        [Obsolete(obsolete)]
        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength) {
            return Materials.CreateHotpooMaterial(materialName)
                            .MakeUnique()
                            .SetEmission(emission, emissionColor)
                            .SetNormal(normalStrength);
        }
        #endregion materials(old)
    }
}
