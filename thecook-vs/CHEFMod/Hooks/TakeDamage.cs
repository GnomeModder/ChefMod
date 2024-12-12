using RoR2;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using R2API;
using BepInEx.Configuration;

namespace ChefMod.Hooks
{
    public static class TakeDamage
    {
        public static ConfigEntry<bool> searScaleKnockback;
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
                        if (damageInfo.damageColorIndex == DamageColorIndex.Default) damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                    }

                    //Scale force to match mass
                    if (searScaleKnockback.Value)
                    {
                        float mass = 100f;
                        bool isGrounded = false;
                        Vector3 newForce = damageInfo.force;
                        CharacterBody body = self.body;
                        if (body.characterMotor)
                        {
                            mass = body.characterMotor.mass;
                            isGrounded = body.characterMotor.isGrounded;
                            /*if (body.characterMotor.isGrounded)
                            {
                                newForce.y = Mathf.Max(newForce.y, 1200f);
                            }*/
                        }
                        else if (body.rigidbody)
                        {
                            mass = body.rigidbody.mass;
                        }

                        float forceMult = Mathf.Max(mass / 100f, 1f);
                        if (body.isChampion && isGrounded)
                        {
                            forceMult *= 0.7f;
                        }
                        newForce *= forceMult;

                        damageInfo.force = newForce;
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
