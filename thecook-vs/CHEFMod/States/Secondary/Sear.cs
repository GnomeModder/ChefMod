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
        public static string prepSoundString = "DIng";
        private float duration;
        public bool specialBoosted = false;
    }

    public class FireSear : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = FireSear.baseDuration / this.attackSpeedStat;
            base.AddRecoil(-3f * FireSear.recoilAmplitude, -4f * FireSear.recoilAmplitude, -0.5f * FireSear.recoilAmplitude, 0.5f * FireSear.recoilAmplitude);
            Util.PlaySound(FireSear.attackSoundString, base.gameObject);

            if (!flamethrowerEffectPrefab)
            {
                Mage.Weapon.Flamethrower flameEffect = new Mage.Weapon.Flamethrower();
                flamethrowerEffectPrefab = flameEffect.flamethrowerEffectPrefab;
            }
            this.childLocator = base.GetModelChildLocator();
            if (this.childLocator)
            {
                Transform transform2 = this.childLocator.FindChild("Body");
                if (transform2)
                {
                    this.flamethrowerTransform = UnityEngine.Object.Instantiate<GameObject>(flamethrowerEffectPrefab, transform2).transform;
                    if (this.flamethrowerTransform)
                    {
                        this.flamethrowerTransform.localPosition += flameEffectOffset;
                        this.flamethrowerTransform.localScale = 0.07f * Vector3.one;
                        this.flamethrowerTransform.GetComponent<ScaleParticleSystemDuration>().newDuration = duration;
                    }
                }
            }
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                base.StartAimMode(aimRay, 2f, false);
                BulletAttack ba = new BulletAttack
                {
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
                    stopperMask = LayerIndex.world.mask
                };
                ModifyBullet(ba);
                ba.Fire();
            }
        }

        public virtual void ModifyBullet(BulletAttack ba)
        {
            ba.AddModdedDamageType(chefPlugin.chefSear);
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
        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/missileexplosionvfx");
        public static float damageCoefficient = 2.6f;
        public static float force = 2400f;
        public static int bulletCount;
        public static float baseDuration = 0.5f;
        public static string attackSoundString = "Fireball";
        public static float recoilAmplitude = 1f;
        private float duration;

        public static float procCoefficient = 1f;

        private static Vector3 flameEffectOffset = new Vector3(0f, 0.012f, 0.015f);
        public static GameObject flamethrowerEffectPrefab = null;
        private Transform flamethrowerTransform;
        private ChildLocator childLocator;
    }
}
