using System;
using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using ChefMod;

namespace EntityStates.Chef
{
    class Rail : BaseSkillState
    {
		public float speedMultiplier = 2f;
		private Vector3 idealDirection;
		GameObject spinEffectInstance;
		float fireAge;
		float fireFrequency;

		public override void OnEnter()
		{
			base.OnEnter();

			if (base.isAuthority)
			{
				this.fireFrequency = Drill.baseFireFrequency * this.attackSpeedStat;

				base.gameObject.layer = LayerIndex.fakeActor.intVal;
				base.characterMotor.Motor.RebuildCollidableLayers();

				if (Toolbot.FireBuzzsaw.spinEffectPrefab)
				{
					this.spinEffectInstance = UnityEngine.Object.Instantiate<GameObject>(Toolbot.FireBuzzsaw.spinEffectPrefab, base.characterBody.transform.position, base.characterBody.transform.rotation);
					this.spinEffectInstance.transform.parent = base.characterBody.transform;
					this.spinEffectInstance.transform.localScale = Vector3.one;
					spinEffectInstance.SetActive(false);
				}
			}

			base.PlayAnimation("Fullbody, Override", "UtilityStart");
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.inputBank)
			{
				spinEffectInstance.SetActive(base.inputBank.skill1.down);

				if (base.inputBank.skill1.down && this.fireAge >= 1f / this.fireFrequency && base.isAuthority)
				{
					//this.fireAge = 0;
					//float speedScale = 0.3f * (Mathf.Sqrt(1 + characterMotor.velocity.magnitude));
					////if (characterBody.isSprinting) speedScale *= 1.5f;

					//var attack = new BlastAttack();
					//attack.radius = 5f * Mathf.Sqrt(speedScale);
					//attack.procCoefficient = Drill.procCoefficientPerSecond / Drill.baseFireFrequency;
					//attack.position = characterBody.corePosition + 2 * characterDirection.moveVector.normalized;
					//attack.attacker = base.gameObject;
					//attack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
					//attack.baseDamage = Mathf.Sqrt(Mathf.Sqrt(speedScale)) * Drill.damageCoefficientPerSecond * this.damageStat / Drill.baseFireFrequency;
					//attack.falloffModel = BlastAttack.FalloffModel.None;
					//attack.baseForce = 3f;
					//attack.teamIndex = TeamComponent.GetObjectTeam(attack.attacker);
					//attack.damageType = DamageType.Generic;
					//attack.attackerFiltering = AttackerFiltering.NeverHit;
					//var result = attack.Fire();
				}

				if (base.inputBank.skill3.down)
				{
					this.outer.SetNextStateToMain();
					return;
				}

			}
			if (base.isAuthority)
			{
				if (base.characterBody)
				{
					base.characterBody.isSprinting = true;
				}
				//if (base.skillLocator.special && base.inputBank.skill4.down)
				//{
				//	base.skillLocator.special.ExecuteIfReady();
				//}
				this.UpdateDirection();
				if (base.characterDirection)
				{
					base.characterDirection.moveVector = this.idealDirection;
					if (base.characterMotor && !base.characterMotor.disableAirControlUntilCollision)
					{
						base.characterMotor.rootMotion += this.GetIdealVelocity() * Time.fixedDeltaTime;
					}
				}
			}
		}

		public override void OnExit()
		{
			base.PlayAnimation("Fullbody, Override", "UtilityEnd");

			base.OnExit();
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		private void UpdateDirection()
		{
			if (base.inputBank)
			{
				Vector2 vector = Util.Vector3XZToVector2XY(base.inputBank.moveVector);
				if (vector != Vector2.zero)
				{
					vector.Normalize();
					this.idealDirection = new Vector3(vector.x, 0f, vector.y).normalized;
				}
			}
		}

		private Vector3 GetIdealVelocity()
		{
			return base.characterDirection.forward * base.characterBody.moveSpeed * this.speedMultiplier;// Mathf.Pow((base.characterBody.moveSpeed * base.characterBody.moveSpeed) + 300f, 0.5f); //base.characterBody.moveSpeed * this.speedMultiplier;
		}
	}
}
