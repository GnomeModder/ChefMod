using System;
using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using ChefMod;

namespace EntityStates.Chef
{
    class Thrust : BaseSkillState
    {
        float duration = 0.2f;

        public GameObject largePrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXLarge");
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                BlastAttack attack = new BlastAttack();
                attack.radius = 10f;
                attack.procCoefficient = 1f;
                attack.position = characterBody.corePosition;
                attack.attacker = base.gameObject;
                attack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                attack.baseDamage = 2.5f * base.characterBody.damage;
                attack.falloffModel = BlastAttack.FalloffModel.None;
                attack.baseForce = 3f;
                attack.teamIndex = TeamComponent.GetObjectTeam(attack.attacker);
                attack.damageType = DamageType.Generic;
                attack.attackerFiltering = AttackerFiltering.NeverHit;
                attack.Fire();

                EffectData effectData = new EffectData();
                effectData.scale = 6;
                effectData.origin = characterBody.footPosition;
                EffectManager.SpawnEffect(largePrefab, effectData, false);

                Ray aimRay = base.GetAimRay();
                Vector3 push = new Vector3(aimRay.direction.x, 0.5f * aimRay.direction.y, aimRay.direction.z);
                base.characterMotor.velocity += 50 * push.normalized;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}