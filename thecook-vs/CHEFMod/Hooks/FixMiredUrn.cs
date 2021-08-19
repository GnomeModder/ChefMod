using RoR2;
using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using System.Text;

namespace ChefMod.Hooks
{
    public static class FixMiredUrn
    {
        public static void SearchForTargets(On.RoR2.SiphonNearbyController.orig_SearchForTargets orig, SiphonNearbyController self, List<HurtBox> dest)
        {
            orig(self, dest);
            List<HurtBox> oilBeetleList = new List<HurtBox>();
            foreach (HurtBox hb in dest)
            {
                if (hb.healthComponent && hb.healthComponent.body && hb.healthComponent.body.baseNameToken == "OilBeetle")
                {
                    oilBeetleList.Add(hb);
                }
            }

            foreach (HurtBox hb in oilBeetleList)
            {
                dest.Remove(hb);
            }
        }
    }
}
