using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ChefMod
{
    public class FryLanding : NetworkBehaviour
    {
        public DamageInfo damageInfo;

        private CharacterBody characterBody;
        private float radius = 10;
        private float startTime;

        private static List<HurtBox> hurtBoxBuffer = new List<HurtBox>();
        private static SphereSearch sphereSearch = new SphereSearch();
        private static GameObject ExplosionEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/IgniteExplosionVFX");
        void Start()
        {
            characterBody = GetComponent<CharacterBody>();
            startTime = Time.fixedTime;
        }

        void FixedUpdate()
        {
            if (characterBody.characterMotor.isGrounded && Time.fixedTime - startTime > 0.25f)
            {
                damageInfo.position = transform.position;
                //characterBody.healthComponent.TakeDamage(damageInfo);
                esplode();

                BlastAttack blastAttack = new BlastAttack
                {
                    attacker = damageInfo.attacker,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    baseDamage = damageInfo.damage,
                    baseForce = 5f,
                    crit = damageInfo.crit,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.IgniteOnHit,
                    falloffModel = BlastAttack.FalloffModel.None,
                    position = transform.position,
                    procCoefficient = 1f,
                    radius = 1f
                };
                blastAttack.damageType.damageSource = DamageSource.Secondary;
                blastAttack.Fire();

                //RaycastHit[] array = Physics.SphereCastAll(characterBody.corePosition, 2f * radius, Vector3.up, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
                //for (int j = 0; j < array.Length; j++)
                //{
                //    Collider collider = array[j].collider;
                //    if (collider.gameObject)
                //    {
                //        RoR2.HurtBox component = collider.GetComponent<RoR2.HurtBox>();
                //        if (component)
                //        {
                //            RoR2.HealthComponent healthComponent = component.healthComponent;
                //            if (healthComponent)
                //            {
                //                Fireee fire = healthComponent.body.GetComponent<Fireee>();
                //                if (fire)
                //                {
                //                    fire.ignate();
                //                }
                //            }
                //        }
                //    }
                //}

                Destroy(this);
            }
        }

        private void esplode()
        {
            Vector3 corePosition = characterBody.corePosition;
            sphereSearch.origin = corePosition;
            sphereSearch.mask = LayerIndex.entityPrecise.mask;
            sphereSearch.radius = radius;
            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(TeamComponent.GetObjectTeam(damageInfo.attacker)));
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
    }
}