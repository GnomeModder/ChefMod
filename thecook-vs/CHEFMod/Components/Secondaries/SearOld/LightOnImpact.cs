using System.Collections.Generic;
using ChefMod.Components;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace ChefMod
{
    public class LightOnImpact : MonoBehaviour, RoR2.Projectile.IProjectileImpactBehavior
    {
        private bool impacted = false;
        private float radius = 10;
        private Vector3 position;

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (impacted) return;
            impacted = true;
            position = impactInfo.estimatedPointOfImpact;
            light();
        }

        private void light()
        {
            RaycastHit[] array = Physics.SphereCastAll(position, radius, Vector3.up, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
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
                                fire.Ignite();
                            }
                        }
                    }
                }
            }
        }
    }
}