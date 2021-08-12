using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChefMod.Hooks
{
    public static class CharacterBody_Update
    {
        public static void Update(On.RoR2.CharacterBody.orig_Update orig, CharacterBody self)
        {
            if (self.baseNameToken == "OilBeetle") return;
            orig(self);
        }
    }
}
