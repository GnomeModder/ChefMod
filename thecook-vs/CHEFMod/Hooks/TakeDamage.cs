using RoR2;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using R2API;

namespace ChefMod.Hooks
{
    public static class TakeDamage
    {
        public static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            bool hasGoo = false;
            if(damageInfo.HasModdedDamageType(chefPlugin.chefSear))
            {
                if (self.body && self.body.HasBuff(RoR2Content.Buffs.ClayGoo))
                {
                    hasGoo = true;
                    damageInfo.damage *= 2f;
                }
            }
            orig(self, damageInfo);
            if (!damageInfo.rejected && damageInfo.attacker)
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody)   // && attackerBody.bodyIndex == BodyCatalog.FindBodyIndex("ChefBody")
                {
                    //Does an additional check just in case the target did not have goo before but became goo'd after taking damage.
                    if (self.body && (hasGoo || self.body.HasBuff(RoR2Content.Buffs.ClayGoo))) // || self.body.HasBuff(RoR2Content.Buffs.OnFire)
                    {
                        if (damageInfo.HasModdedDamageType(chefPlugin.chefSear))
                        {
                            OilExplosion.Explode(attackerBody, self.body, damageInfo.crit, damageInfo.HasModdedDamageType(chefPlugin.chefFireballOnHit));
                        }
                    }
                }
            }
        }
    }
}
