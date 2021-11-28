using EntityStates;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using RoR2.Projectile;
using ChefMod;
using System;

namespace EntityStates.Chef
{
    class Pound : BaseSkillState
    {
        Vector3 startpos;
        float dist;
        List<CharacterBody> victimBodyList = new List<CharacterBody>();

        public GameObject largePrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                dist = characterMotor.velocity.magnitude;

                base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                startpos = base.characterBody.corePosition;
                base.characterMotor.velocity *= 0.1f;
                base.characterMotor.velocity += 40 * Vector3.down;
            }
        }

        public override void OnExit()
        {
            BlastAttack attack = new BlastAttack();
            attack.radius = 7f * (1 + Mathf.Sqrt(dist));
            attack.procCoefficient = 1f;
            attack.position = characterBody.footPosition;
            attack.attacker = base.gameObject;
            attack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
            attack.baseDamage = 1f;
            attack.falloffModel = BlastAttack.FalloffModel.None;
            attack.baseForce = 3f;
            attack.teamIndex = TeamComponent.GetObjectTeam(attack.attacker);
            attack.damageType = DamageType.Stun1s;
            attack.attackerFiltering = AttackerFiltering.NeverHit;
            attack.Fire();

            getHitList(attack);
            victimBodyList.ForEach(Ground);

            attack.radius /= 10f;
            attack.damageType = DamageType.Generic;
            attack.baseDamage = 2.5f * Mathf.Sqrt(dist) * base.damageStat;
            attack.Fire();

            EffectData effectData = new EffectData();
            effectData.scale = 5 * Mathf.Sqrt(dist);
            effectData.origin = characterBody.footPosition;
            EffectManager.SpawnEffect(largePrefab, effectData, false);

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                base.characterMotor.velocity += 5 * Vector3.down;
                dist ++;
            }

            if (base.characterMotor.isGrounded)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void getHitList(BlastAttack ba)
        {
            Collider[] array = Physics.OverlapSphere(ba.position, ba.radius, LayerIndex.defaultLayer.mask);
            int num = 0;
            int num2 = 0;
            while (num < array.Length && num2 < 12)
            {
                HealthComponent component = array[num].GetComponent<HealthComponent>();
                if (component)
                {
                    TeamComponent component2 = component.GetComponent<TeamComponent>();
                    if (component2.teamIndex != TeamIndex.Player)
                    {
                        this.AddToList(component.gameObject);
                        num2++;
                    }
                }
                num++;
            }
        }

        private void AddToList(GameObject affectedObject)
        {
            CharacterBody component = affectedObject.GetComponent<CharacterBody>();
            if (!this.victimBodyList.Contains(component))
            {
                this.victimBodyList.Add(component);
            }
        }

        void Ground(CharacterBody charb)
        {
            if (charb.characterMotor)
            {
                if (charb.characterMotor.isGrounded)
                {
                    BlastAttack blast = new BlastAttack();
                    blast.radius =.1f;
                    blast.procCoefficient = 1f;
                    blast.position = charb.corePosition;
                    blast.attacker = base.gameObject;
                    blast.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                    blast.baseDamage = 2.5f * Mathf.Sqrt(dist) * base.damageStat;
                    blast.baseDamage /= Mathf.Sqrt((charb.corePosition - characterBody.corePosition).magnitude);
                    blast.falloffModel = BlastAttack.FalloffModel.None;
                    blast.baseForce = 3f;
                    blast.teamIndex = TeamComponent.GetObjectTeam(blast.attacker);
                    blast.damageType = DamageType.Generic;
                    blast.attackerFiltering = AttackerFiltering.NeverHit;
                    blast.Fire();
                }
                else
                {
                    charb.characterMotor.velocity += 25f * Vector3.down;
                }
            }
            else
            {
                Rigidbody component2 = charb.GetComponent<Rigidbody>();
                if (component2)
                {
                    component2.velocity += 25f * Vector3.down;
                }
            }
        }
    }
}