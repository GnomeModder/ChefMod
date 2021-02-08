using System.Collections.Generic;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace ChefMod
{
    public class DripOnImpact : MonoBehaviour, RoR2.Projectile.IProjectileImpactBehavior
    {
        private float radius = 10;
        private DamageInfo damageInfo;
        private TeamIndex teamIndex;
        private Vector3 position;

        private static List<HurtBox> hurtBoxBuffer = new List<HurtBox>();
        private static SphereSearch sphereSearch = new SphereSearch();
        private static GameObject ExplosionEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/IgniteExplosionVFX");

        void Start()
        {
            ProjectileController controller = GetComponent<ProjectileController>();
            ProjectileDamage damage = GetComponent<ProjectileDamage>();
            damageInfo = new DamageInfo
            {
                attacker = controller.owner,
                crit = damage.crit,
                damage = damage.damage / 2f,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.IgniteOnHit,
                force = Vector3.up,
                inflictor = this.gameObject,
                procCoefficient = 1f
            };
            teamIndex = controller.teamFilter.teamIndex;
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            position = impactInfo.estimatedPointOfImpact;
            damageInfo.position = position;
            esplode();
            spawntheballs(impactInfo.estimatedImpactNormal, impactInfo.estimatedPointOfImpact, Vector3.forward, 8, 20f, 1f);
        }

        private void esplode()
        {
            Vector3 corePosition = position;
            sphereSearch.origin = corePosition;
            sphereSearch.mask = LayerIndex.entityPrecise.mask;
            sphereSearch.radius = 2f * radius;
            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(teamIndex));
            sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            sphereSearch.OrderCandidatesByDistance();
            sphereSearch.GetHurtBoxes(hurtBoxBuffer);
            sphereSearch.ClearCandidates();
            for (int i = 0; i < hurtBoxBuffer.Count; i++)
            {
                HurtBox hurtBox = hurtBoxBuffer[i];
                if (hurtBox.healthComponent)
                {
                    DotController.InflictDot(hurtBox.healthComponent.gameObject, damageInfo.attacker, DotController.DotIndex.Burn, 1.5f + 1.5f, 0.5f);
                }
            }
            hurtBoxBuffer.Clear();
            EffectManager.SpawnEffect(ExplosionEffectPrefab, new EffectData
            {
                origin = corePosition,
                scale = radius,
                rotation = Util.QuaternionSafeLookRotation(Vector3.up)
            }, true);
        }

        private void spawntheballs(Vector3 impactNormal, Vector3 impactPosition, Vector3 forward, int meatballCount, float meatballAngle, float meatballForce)
        {
            float num = 360f / (float)meatballCount;
            Vector3 normalized = Vector3.ProjectOnPlane(forward, impactNormal).normalized;
            Vector3 point = Vector3.RotateTowards(impactNormal, normalized, meatballAngle * 0.017453292f, float.PositiveInfinity);
            for (int i = 0; i < meatballCount; i++)
            {
                Vector3 forward2 = Quaternion.AngleAxis(num * (float)i, impactNormal) * point;
                ProjectileManager.instance.FireProjectile(chefPlugin.drippingPrefab, impactPosition + 5f * impactNormal.normalized, Util.QuaternionSafeLookRotation(forward2), damageInfo.attacker, damageInfo.damage, meatballForce, damageInfo.crit, DamageColorIndex.Default, null, -1f);
            }
        }
    }
}