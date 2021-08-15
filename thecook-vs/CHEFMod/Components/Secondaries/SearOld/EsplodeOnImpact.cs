using System.Collections.Generic;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace ChefMod
{
    public class EsplodeOnImpact : MonoBehaviour, RoR2.Projectile.IProjectileImpactBehavior
    {
        private bool impacted = false;
        private float radius = 6;
        private DamageInfo damageInfo;
        private TeamIndex teamIndex;
        private Vector3 position;
        ProjectileDamage damage;
        private GameObject owner;

        private static List<HurtBox> hurtBoxBuffer = new List<HurtBox>();
        private static SphereSearch sphereSearch = new SphereSearch();
        private static GameObject ExplosionEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/IgniteExplosionVFX");

        void Start()
        {
            ProjectileController controller = GetComponent<ProjectileController>();
            owner = controller.owner;
            damage = GetComponent<ProjectileDamage>();
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
            if (impacted || !NetworkServer.active) return;
            impacted = true;
            position = impactInfo.estimatedPointOfImpact;
            damageInfo.position = position;
            esplode();
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
                    DotController.InflictDot(hurtBox.healthComponent.gameObject, damageInfo.attacker, DotController.DotIndex.Burn);
                }
            }
            hurtBoxBuffer.Clear();
        }
    }
}