using System;
using System.Collections.Generic;
using ChefMod.Components;
using R2API;
using RoR2;
using UnityEngine;

namespace ChefMod.Hooks
{
    public static class OnHitAll
    {
        public static void HitAll(On.RoR2.GlobalEventManager.orig_OnHitAll orig, GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            orig(self, damageInfo, hitObject);
            if(damageInfo.HasModdedDamageType(ChefPlugin.chefSear))
            {
                RaycastHit[] array = Physics.SphereCastAll(damageInfo.position, 15f, Vector3.up, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
                for (int j = 0; j < array.Length; j++)
                {
                    Collider collider = array[j].collider;
                    if (collider.gameObject)
                    {
                        RoR2.HurtBox component = collider.GetComponent<RoR2.HurtBox>();
                        if (component)
                        {
                            RoR2.HealthComponent healthComponent = component.healthComponent;
                            if (healthComponent)
                            {
                                OilController fire = healthComponent.body.GetComponent<OilController>();
                                if (fire)
                                {
                                    if (damageInfo.HasModdedDamageType(ChefPlugin.chefFireballOnHit))
                                    {
                                        fire.boosted = true;
                                    }
                                    fire.Ignite();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
