using ChefMod;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EntityStates.Chef
{
    public class PrepSear : BaseState
    {

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = PrepSear.baseDuration / this.attackSpeedStat;
            Util.PlaySound(PrepSear.prepSoundString, base.gameObject);
            base.PlayAnimation("Fullbody, Override", "Secondary", "Secondary.playbackRate", duration * 4f);
            base.PlayAnimation("Gesture, Override", "Secondary", "Secondary.playbackRate", duration * 4f);
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                NextState();
                return;
            }
        }

        public virtual void NextState()
        {
            this.outer.SetNextState(new FireSear());
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public static float baseDuration = 0.5f;
        public static string prepSoundString = "Play_ChefMod_Ding";
        private float duration;
        public bool specialBoosted = false;
    }

    public class FireSear : BaseState {

        public static float blastEffectRadius = 2;
        public static float blastSpacing;
        public static GameObject ExplosionEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");

        protected virtual GameObject explosion {
            get => ExplosionEffectPrefab;
        }

        public virtual void PlaySound()
        {
            Util.PlaySound("Play_Chefmod_Sear", base.gameObject);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = FireSear.baseDuration / this.attackSpeedStat;
            base.AddRecoil(-3f * FireSear.recoilAmplitude, -5f * FireSear.recoilAmplitude, -0.5f * FireSear.recoilAmplitude, 0.5f * FireSear.recoilAmplitude);

            PlaySound();

            //if (!flamethrowerEffectPrefab)
            //{
            //    Mage.Weapon.Flamethrower flameEffect = new Mage.Weapon.Flamethrower();
            //    flamethrowerEffectPrefab = flameEffect.flamethrowerEffectPrefab;
            //}

            //this.childLocator = base.GetModelChildLocator();
            //if (this.childLocator)
            //{
            //    Transform transform2 = this.childLocator.FindChild("Body");
            //    if (transform2)
            //    {
            //        this.flamethrowerTransform = UnityEngine.Object.Instantiate<GameObject>(flamethrowerEffectPrefab, transform2).transform;
            //        if (this.flamethrowerTransform)
            //        {
            //            this.flamethrowerTransform.localPosition += flameEffectOffset;
            //            this.flamethrowerTransform.localScale = flameScale;
            //            this.flamethrowerTransform.GetComponent<ScaleParticleSystemDuration>().newDuration = duration*0.25f;
            //        }
            //    }
            //}

            Ray aimRay = base.GetAimRay();

            EffectData effectData = new EffectData();
            effectData.scale = 2.9f;

            for (float f = 0; f <= 48;) {
                //start spacing little in front of him
                f += blastEffectRadius + blastSpacing;

                Vector3 point = aimRay.origin + aimRay.direction * f;
                effectData.origin = point;
                EffectManager.SpawnEffect(explosion, effectData, false);
            }

            if (base.isAuthority)
            {
                base.StartAimMode(aimRay, 2f, false);
                BulletAttack ba = new BulletAttack {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = 0f,
                    bulletCount = 1,
                    damage = FireSear.damageCoefficient * this.damageStat,
                    damageType = DamageType.IgniteOnHit | DamageType.Stun1s,
                    procCoefficient = FireSear.procCoefficient,
                    force = FireSear.force,
                    falloffModel = BulletAttack.FalloffModel.None,
                    hitEffectPrefab = FireSear.hitEffectPrefab,
                    tracerEffectPrefab = null,
                    isCrit = base.RollCrit(),
                    radius = 2f,
                    smartCollision = true,
                    maxDistance = 48f,
                    stopperMask = LayerIndex.world.collisionMask,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                };
                ModifyBullet(ba);
                ba.Fire();

                if (base.characterBody && base.characterMotor)
                {
                    base.characterBody.characterMotor.ApplyForce(-2000f * aimRay.direction, true, false);
                }
            }
        }

        public virtual void ModifyBullet(BulletAttack ba)
        {
            ba.AddModdedDamageType(ChefMod.ChefPlugin.chefSear);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdateFlamethrowerEffect();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            if (this.flamethrowerTransform)
            {
                EntityState.Destroy(this.flamethrowerTransform.gameObject);
            }
            base.OnExit();
        }

        private void UpdateFlamethrowerEffect()
        {
            if (this.flamethrowerTransform)
            {
                this.flamethrowerTransform.forward = base.GetAimRay().direction;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
        public static GameObject hitEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/missileexplosionvfx");
        public static float damageCoefficient = 2.6f;
        public static float force = 2400f;
        public static int bulletCount;
        public static float baseDuration = 0.5f;
        public static float recoilAmplitude = 1f;
        private float duration;

        public static float procCoefficient = 1f;

        public static Vector3 flameScale = new Vector3(0.14f, 0.14f, 0.07f);
        private static Vector3 flameEffectOffset = new Vector3(0f, 0.012f, 0.015f);
        public static GameObject flamethrowerEffectPrefab = null;
        private Transform flamethrowerTransform;
        private ChildLocator childLocator;
    }
}
