using ChefMod;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Chef
{
	public class Marinate : BaseState
	{
		public static float buffOverlapDuration = 0.1f;
		public static float baseDuration = 1.5f;
		private float duration;
		public float speedMultiplier = 2f;
		//private float radius = 3f;

		private float buffOverlapStopwatch;
		private Vector3 idealDirection;
		private ChildLocator childLocator;
		private ParticleSystem.EmissionModule emissionator;
		//DamageTrail oilTrail;

		//ChefMod.FieldComponent trailComponent;

		private float lastUpdateTime;
        private float oilSpillTimer;
        private bool isCrit;

        public override void OnEnter()
		{
			base.OnEnter();
			isCrit = base.RollCrit();
			oilSpillTimer = 0f;
			lastUpdateTime = Time.time;
			this.duration = baseDuration;
			childLocator = base.GetModelChildLocator();
			emissionator = childLocator.FindChild("OilParticles").GetComponent<ParticleSystem>().emission;

			if (base.isAuthority) {
				emissionator.enabled = true;

				base.gameObject.layer = LayerIndex.fakeActor.intVal;
				base.characterMotor.Motor.RebuildCollidableLayers();

				//var fire = chefPlugin.oilPrefab.GetComponent<Fireee>();
				//fire.owner = characterBody.gameObject;
				//fire.teamIndex = characterBody.teamComponent.teamIndex;
				//fire.damagePerFrame = characterBody.damage * 0.25f;
				//fire.critStat = base.critStat;
				//fire.master = characterBody.master;

				//trailComponent = base.characterBody.GetComponent<ChefMod.FieldComponent>();
				//trailComponent.active = true;
				//this.oilTrail = UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/FireTrail"), this.transform).GetComponent<DamageTrail>();
				//this.oilTrail.transform.position = base.characterBody.footPosition;
				//this.oilTrail.owner = base.gameObject;
				//this.oilTrail.radius *= this.radius;
				//this.oilTrail.pointLifetime = 10f;

				//TrailController.Slick slick = new TrailController.Slick(oilTrail);
				//base.characterBody.GetComponent<TrailController>().slicks.Add(slick);

				//Util.PlaySound("Marinate", base.gameObject);

				if (!base.characterMotor.isGrounded) base.characterMotor.velocity.y = 15f;

				base.PlayAnimation("Fullbody, Override", "UtilityStart");
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			float deltaTime = Time.time - lastUpdateTime;
			lastUpdateTime = Time.time;

			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
				return;
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
						base.characterMotor.rootMotion += this.GetIdealVelocity() * deltaTime;
					}
				}

                //this.oilTrail.damagePerSecond = base.characterBody.damage * 1.5f;

                if (oilSpillTimer <= 0f)
                {
                    float ratio = GetIdealVelocity().magnitude / characterBody.moveSpeed;
                    int frequency = Mathf.FloorToInt(4f * ratio);
                    oilSpillTimer = frequency / 60f;

                    FireProjectileInfo info = new FireProjectileInfo()
                    {
                        projectilePrefab = OilSlick.projectilePrefab,
                        position = characterBody.corePosition,
                        rotation = Quaternion.identity,
                        owner = base.gameObject,
                        damage = characterBody.damage * 0.25f,
                        crit = isCrit
                    };

                    ProjectileManager.instance.FireProjectile(info);
                }
                else
                {
                    oilSpillTimer -= deltaTime;
                }

			}

			if (NetworkServer.active)
            {
				buffOverlapStopwatch += deltaTime;
				if (buffOverlapStopwatch >= OilSlick.buffOverlapDuration)
				{
					buffOverlapStopwatch -= OilSlick.buffOverlapDuration;
					ApplyOilServer();
				}
			}
		}

		private void ApplyOilServer()
		{
			List<HealthComponent> hcList = new List<HealthComponent>();
			Collider[] array = Physics.OverlapSphere(base.transform.position, 12f, LayerIndex.entityPrecise.mask);
			for (int i = 0; i < array.Length; i++)
			{
				HurtBox hurtBox = array[i].GetComponent<HurtBox>();
				if (hurtBox && hurtBox.healthComponent && !hcList.Contains(hurtBox.healthComponent))
				{
					hcList.Add(hurtBox.healthComponent);
					if (hurtBox.healthComponent.body.teamComponent && hurtBox.healthComponent.body.teamComponent.teamIndex != base.GetTeam())
					{
						hurtBox.healthComponent.body.AddTimedBuff(ChefMod.ChefPlugin.oilBuff, 4f);
					}
				}
			}
		}

		public override void OnExit()
		{
			//UnityEngine.Object.Destroy(this.oilTrail.gameObject);
			//this.oilTrail = null;
			//this.oilTrail.active = false;

			//skillLocator.primary.SetBaseSkill(chefPlugin.primaryDef);
			//if (skillLocator.secondary.baseSkill == chefPlugin.boostedSecondaryDef)
			//{
			//	skillLocator.secondary.SetBaseSkill(chefPlugin.secondaryDef);
			//}
			//if (skillLocator.secondary.baseSkill == chefPlugin.boostedAltSecondaryDef)
			//{
			//	skillLocator.secondary.SetBaseSkill(chefPlugin.altSecondaryDef);
			//}
			//skillLocator.utility.SetBaseSkill(chefPlugin.utilityDef);

			//skillLocator.utility.RunRecharge(chefPlugin.utilityDef.baseRechargeInterval);

			base.gameObject.layer = LayerIndex.defaultLayer.intVal;
			base.characterMotor.Motor.RebuildCollidableLayers();

			//trailComponent.active = false;

			emissionator.enabled = false;

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
			return base.characterDirection.forward * (10f + Mathf.Sqrt((base.characterBody.moveSpeed * base.characterBody.moveSpeed) + 300f)); //base.characterBody.moveSpeed * this.speedMultiplier;
		}
	}
}