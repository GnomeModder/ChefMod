using System;
using System.Collections.Generic;
using ChefMod;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Chef
{
    class Fry : BaseSkillState
    {
        public float baseDuration = 0.1f;
        private float duration;
        private Vector3 direction;

        Tuple<CharacterBody, float> victim = new Tuple<CharacterBody, float>(null, 100f);
        Ray aimRay;
        public override void OnEnter()
        {
            base.OnEnter();
            aimRay = base.GetAimRay();
            this.duration = this.baseDuration;
            if (base.isAuthority)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = 5f;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = aimRay.origin + aimRay.direction * 2.5f;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blastAttack.baseDamage = 0.1f;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = 3f;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                //blastAttack.Fire();

                Vector3 horizontal = new Vector3(aimRay.direction.x, 0, aimRay.direction.z);
                horizontal = horizontal.normalized;
                direction = new Vector3(horizontal.x, 2, horizontal.z);

                getVictim(blastAttack);

                if (victim.Item1)
                {
                    DamageInfo damInfo = new DamageInfo
                    {
                        attacker = base.gameObject,
                        crit = base.RollCrit(),
                        damage = 5f * base.damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.IgniteOnHit,
                        force = Vector3.forward,
                        inflictor = base.gameObject,
                        procCoefficient = 1f
                    };

                    launch(victim.Item1);
                    if (victim.Item1.characterMotor)
                    {
                        var fl = victim.Item1.gameObject.AddComponent<FryLanding>();
                        fl.damageInfo = damInfo;
                    }
                }
                //else
                //{
                //    FireProjectileInfo info = new FireProjectileInfo()
                //    {
                //        projectilePrefab = ChefMod.chefPlugin.drippingPrefab,
                //        position = aimRay.origin + 1.5f * aimRay.direction,
                //        rotation = Util.QuaternionSafeLookRotation(direction),
                //        owner = base.gameObject,
                //        damage = 2 * base.damageStat,
                //        force = 5f,
                //        crit = base.RollCrit(),
                //        damageColorIndex = DamageColorIndex.Default,
                //        target = null,
                //        fuseOverride = -1f
                //    };
                //    ProjectileManager.instance.FireProjectile(info);
                //}
            }
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

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void getVictim(BlastAttack ba)
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
                    if (component2.teamIndex != TeamComponent.GetObjectTeam(base.gameObject))
                    {
                        this.Compare(component.body);
                        num2++;
                    }
                }
                num++;
            }
        }

        private void Compare(CharacterBody candidate)
        {
            Vector3 distance = candidate.corePosition - characterBody.corePosition;
            if (distance.magnitude < victim.Item2)
            {
                victim = new Tuple<CharacterBody, float>(candidate, distance.magnitude);
            }
        }

        private void launch(CharacterBody charB)
        {
            float speed = 35f;
            if (charB.characterMotor)
            {
                if (charB.characterMotor.mass > 300f) speed = 1f;
                charB.characterMotor.rootMotion.y += 1f;
                charB.characterMotor.velocity += speed * direction.normalized;
            }
        }
    }
}