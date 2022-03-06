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
            bool isSear = damageInfo.HasModdedDamageType(ChefPlugin.chefSear);
            bool isBoostSear = damageInfo.HasModdedDamageType(ChefPlugin.chefFireballOnHit);
            if (self.body && self.body.baseNameToken == "OilBeetle")
            {
                damageInfo.procCoefficient = 0f;
            }
            if (isSear)
            {
                if (self.body)
                {
                    if (self.body.HasBuff(ChefPlugin.oilBuff))
                    {
                        hasGoo = true;
                        damageInfo.damage *= 2f;
                    }

                    //Scale force to match mass
                    Rigidbody rb = self.body.rigidbody;
                    if (rb)
                    {
                        if (rb.mass > 100f)
                        {
                            //damageInfo.force *= isBoostSear ? rb.mass / 100f: Mathf.Min(rb.mass / 100f, 10f);
                            damageInfo.force *= rb.mass / 100f;
                        }
                        else
                        {
                            damageInfo.force *=  100f/rb.mass;
                        }
                    }
                }
            }
            orig(self, damageInfo);
            if (!damageInfo.rejected && damageInfo.attacker)
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody)   // && attackerBody.bodyIndex == BodyCatalog.FindBodyIndex("ChefBody")
                {
                    //Does an additional check just in case the target did not have goo before but became goo'd after taking damage.
                    if (self.body && (hasGoo || self.body.HasBuff(ChefPlugin.oilBuff))) // || self.body.HasBuff(RoR2Content.Buffs.OnFire)
                    {
                        if (isSear)
                        {
                            if (damageInfo.force.magnitude > 0f && damageInfo.procCoefficient == 1f && self.body.baseNameToken != "OilBeetle")
                            {
                                EffectManager.SimpleImpactEffect(ChefPlugin.searBonusEffect, damageInfo.position, Vector3.up, true);
                            }
                            OilExplosion.Explode(attackerBody, self.body, damageInfo.crit, isBoostSear);
                        }
                    }
                }
            }
        }
    }
}
