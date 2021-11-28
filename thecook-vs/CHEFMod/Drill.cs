using System;
using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using ChefMod;

namespace EntityStates.Chef
{
    class Drill : BaseSkillState
    {
		public override void OnEnter()
		{
			base.OnEnter();
			this.fireFrequency = Drill.baseFireFrequency * this.attackSpeedStat;
			//Transform modelTransform = base.GetModelTransform();
			Util.PlaySound(Toolbot.FireBuzzsaw.spinUpSoundString, base.gameObject);
			Util.PlaySound(Toolbot.FireBuzzsaw.fireSoundString, base.gameObject);

			if (Toolbot.FireBuzzsaw.spinEffectPrefab)
			{
				this.spinEffectInstance = UnityEngine.Object.Instantiate<GameObject>(Toolbot.FireBuzzsaw.spinEffectPrefab, base.characterBody.transform.position, base.characterBody.transform.rotation);
				this.spinEffectInstance.transform.parent = base.characterBody.transform;
				this.spinEffectInstance.transform.localScale = Vector3.one;
			}
			if (Toolbot.FireBuzzsaw.spinImpactEffectPrefab)
			{
				this.spinImpactEffectInstance = UnityEngine.Object.Instantiate<GameObject>(Toolbot.FireBuzzsaw.spinImpactEffectPrefab, base.characterBody.transform.position, base.characterBody.transform.rotation);
				this.spinImpactEffectInstance.transform.parent = base.characterBody.transform;
				this.spinImpactEffectInstance.transform.localScale = Vector3.one;
				this.spinImpactEffectInstance.gameObject.SetActive(false);
			}
		}

		// Token: 0x06003B8F RID: 15247 RVA: 0x000EAC40 File Offset: 0x000E8E40
		public override void OnExit()
		{
			base.OnExit();
			Util.PlaySound(Toolbot.FireBuzzsaw.spinDownSoundString, base.gameObject);
			if (this.spinEffectInstance)
			{
				EntityState.Destroy(this.spinEffectInstance);
			}
			if (this.spinImpactEffectInstance)
			{
				EntityState.Destroy(this.spinImpactEffectInstance);
			}
		}

		// Token: 0x06003B90 RID: 15248 RVA: 0x000EACBC File Offset: 0x000E8EBC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.fireAge += Time.fixedDeltaTime;
			base.characterBody.SetAimTimer(2f);

			if (this.fireAge >= 1f / this.fireFrequency && base.isAuthority)
			{
				this.fireAge = 0;
				float speedScale = 0.3f * (Mathf.Sqrt(1 + characterMotor.velocity.magnitude));
				//if (characterBody.isSprinting) speedScale *= 1.5f;

				attack = new BlastAttack();
				attack.radius = 5f * Mathf.Sqrt(speedScale);
				attack.procCoefficient = Drill.procCoefficientPerSecond / Drill.baseFireFrequency;
				attack.position = characterBody.corePosition + 2 * characterDirection.moveVector.normalized;
				attack.attacker = base.gameObject;
				attack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
				attack.baseDamage = Mathf.Sqrt(Mathf.Sqrt(speedScale)) * Drill.damageCoefficientPerSecond * this.damageStat / Drill.baseFireFrequency;
				attack.falloffModel = BlastAttack.FalloffModel.None;
				attack.baseForce = 3f;
				attack.teamIndex = TeamComponent.GetObjectTeam(attack.attacker);
				attack.damageType = DamageType.Generic;
				attack.attackerFiltering = AttackerFiltering.NeverHit;
				var result = attack.Fire();
			}

			if (!base.IsKeyDownAuthority()) { this.outer.SetNextStateToMain(); }
			this.spinImpactEffectInstance.gameObject.SetActive(this.hitOverlapLastTick);
		}

		// Token: 0x06003B91 RID: 15249 RVA: 0x0006E59F File Offset: 0x0006C79F
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04003243 RID: 12867
		public static float damageCoefficientPerSecond = Toolbot.FireBuzzsaw.damageCoefficientPerSecond;

		// Token: 0x04003244 RID: 12868
		public static float procCoefficientPerSecond = 1f;

		// Token: 0x04003245 RID: 12869
		public static string fireSoundString = Toolbot.FireBuzzsaw.fireSoundString;

		// Token: 0x04003246 RID: 12870
		public static string impactSoundString = Toolbot.FireBuzzsaw.impactSoundString;

		// Token: 0x04003247 RID: 12871
		public static string spinUpSoundString = Toolbot.FireBuzzsaw.spinUpSoundString;

		// Token: 0x04003248 RID: 12872
		public static string spinDownSoundString = Toolbot.FireBuzzsaw.spinDownSoundString;

		// Token: 0x04003249 RID: 12873
		public static float spreadBloomValue = 0.2f;

		// Token: 0x0400324A RID: 12874
		public static float baseFireFrequency = Toolbot.FireBuzzsaw.baseFireFrequency;

		// Token: 0x0400324B RID: 12875
		public static GameObject spinEffectPrefab;

		// Token: 0x0400324C RID: 12876
		public static GameObject spinImpactEffectPrefab;

		// Token: 0x0400324D RID: 12877
		public static GameObject impactEffectPrefab;

		// Token: 0x0400324E RID: 12878
		public static float selfForceMagnitude = Toolbot.FireBuzzsaw.selfForceMagnitude;

		// Token: 0x0400324F RID: 12879
		private BlastAttack attack;

		// Token: 0x04003250 RID: 12880
		private float fireFrequency;

		// Token: 0x04003251 RID: 12881
		private float fireAge;

		// Token: 0x04003252 RID: 12882
		private GameObject spinEffectInstance;

		// Token: 0x04003253 RID: 12883
		private GameObject spinImpactEffectInstance;

		// Token: 0x04003254 RID: 12884
		private bool hitOverlapLastTick;
	}
}