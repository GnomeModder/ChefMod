using ChefMod;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EntityStates.Chef
{
	public class OilSlick : BaseSkillState
	{
		public static float baseDuration = 1.5f;
		private float duration;
		public float speedMultiplier = 2f;
		//private float radius = 3f;

		private Vector3 idealDirection;
		//DamageTrail oilTrail;

		//ChefMod.FieldComponent trailComponent;

		private int counter = 0;

		public override void OnEnter()
		{
			base.OnEnter();

			this.duration = baseDuration;

			if (base.isAuthority)
			{
				base.gameObject.layer = LayerIndex.fakeActor.intVal;
				base.characterMotor.Motor.RebuildCollidableLayers();

				//trailComponent = base.characterBody.GetComponent<ChefMod.FieldComponent>();
				//trailComponent.active = true;
				//this.oilTrail = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/FireTrail"), this.transform).GetComponent<DamageTrail>();
				//this.oilTrail.transform.position = base.characterBody.footPosition;
				//this.oilTrail.owner = base.gameObject;
				//this.oilTrail.radius *= this.radius;
				//this.oilTrail.pointLifetime = 10f;

				//TrailController.Slick slick = new TrailController.Slick(oilTrail);
				//base.characterBody.GetComponent<TrailController>().slicks.Add(slick);
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
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
						base.characterMotor.rootMotion += this.GetIdealVelocity() * Time.fixedDeltaTime;
					}
				}

				float ratio = characterBody.baseMoveSpeed / characterBody.moveSpeed;
				int frequency = Mathf.FloorToInt(24f * ratio);
				if (counter % frequency == 0)
				{
					//CharacterMaster characterMaster = new MasterSummon
					//{
					//	masterPrefab = chefPlugin.oilPrefab,
					//	position = characterBody.corePosition,
					//	rotation = Quaternion.identity,
					//	summonerBodyObject = null,
					//	teamIndexOverride = TeamIndex.Neutral,
					//	ignoreTeamMemberLimit = true
					//}.Perform();

					//GameObject obj = characterMaster.bodyPrefab;

					GameObject obj = Object.Instantiate(ChefMod.chefPlugin.oilPrefab, characterBody.corePosition, Quaternion.identity);

					Fireee fire = obj.GetComponent<Fireee>();
					fire.owner = characterBody.gameObject;
					fire.teamIndex = characterBody.teamComponent.teamIndex;
					fire.damagePerFrame = characterBody.damage * 0.5f;
				}
				counter++;
			}
		}

		public override void OnExit()
		{
			//UnityEngine.Object.Destroy(this.oilTrail.gameObject);
			//this.oilTrail = null;
			//this.oilTrail.active = false;

			base.gameObject.layer = LayerIndex.defaultLayer.intVal;
			base.characterMotor.Motor.RebuildCollidableLayers();

			//trailComponent.active = false;

			base.OnExit();
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
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
			return base.characterDirection.forward * base.characterBody.moveSpeed * this.speedMultiplier;
		}
	}
}