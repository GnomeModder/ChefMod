using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace ChefMod
{
    public static class OilExplosion
    {
        public static void Explode (CharacterBody attackerBody, CharacterBody victimBody, bool crit, bool boosted)
        {
            Explode(attackerBody, victimBody, crit, boosted, victimBody.corePosition);
        }

        public static void Explode(CharacterBody attackerBody, CharacterBody victimBody, bool crit, bool boosted, Vector3 position)
        {
            if (victimBody != null)
            {
                if (victimBody.baseNameToken == "OilBeetle")
                {
                    return;
                }
                victimBody.ClearTimedBuffs(ChefPlugin.oilBuff);
                //Debug.Log("Triggering chain explosion with radius " + 10f * Mathf.Max(1f, victimBody.radius));

                float radius = 10f * Mathf.Max(1f, victimBody ? victimBody.radius : 1f) * (boosted ? 1.5f : 1f);

                if (boosted)
                {
                    Vector3 normalized = Vector3.ProjectOnPlane(Vector3.forward, Vector3.up).normalized;
                    Vector3 point = Vector3.RotateTowards(Vector3.up, normalized, 15f * 0.017453292f, float.PositiveInfinity);
                    float num = 360f / (float)boostedFireballCount;
                    for (int i = 0; i < boostedFireballCount; i++)
                    {
                        Vector3 forward2 = Quaternion.AngleAxis(num * (float)i, Vector3.up) * point;
                        ProjectileManager.instance.FireProjectile(ChefPlugin.drippingPrefab, position + 5f * Vector3.up, Util.QuaternionSafeLookRotation(forward2),
                            attackerBody.gameObject, attackerBody.damage * 2f, 0f, crit, DamageColorIndex.Default, null, -1f);
                    }
                }

                var ba = new BlastAttack
                {
                    radius = radius,
                    procCoefficient = 0.4f,
                    position = position,
                    attacker = attackerBody.gameObject,
                    crit = crit,
                    baseDamage = attackerBody.damage * 0.78f,
                    falloffModel = BlastAttack.FalloffModel.None,
                    baseForce = 0f,
                    teamIndex = attackerBody.teamComponent.teamIndex,
                    damageType = DamageType.Stun1s | DamageType.IgniteOnHit,
                    attackerFiltering = AttackerFiltering.NeverHitSelf
                };
                ba.damageType.damageSource = DamageSource.Utility;
                ba.Fire();

                EffectManager.SpawnEffect(explosionEffectPrefab, new EffectData
                {
                    origin = position,
                    scale = 12f
                }, true);
            }
        }

        public static int boostedFireballCount = 5;
        public static GameObject explosionEffectPrefab;
        public static GameObject boostedSearProjectilePrefab;
    }
}
