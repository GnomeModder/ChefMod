using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;
using EntityStates.ArtifactShell;
using EntityStates.Chef;
using System.Collections.Generic;
using EntityStates.Engi.EngiWeapon;
using UnityEngineInternal;

namespace ChefMod
{
	// Token: 0x02000602 RID: 1538
	[RequireComponent(typeof(ProjectileController))]
	public class CoomerangProjectile : NetworkBehaviour, IProjectileImpactBehavior
	{
		public static event Action CleaverCreated;

		public bool isKnife = false;
		private bool resetTargets = false;
		private bool hasFired = false;

		// Token: 0x060025A2 RID: 9634 RVA: 0x0009C7B0 File Offset: 0x0009A9B0
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.maxFlyStopwatch = this.charge * this.distanceMultiplier;
		}

		// Token: 0x060025A3 RID: 9635 RVA: 0x0009C82C File Offset: 0x0009AA2C
		private void Start()
		{
			float num = this.charge * 7f;
			if (num < 1f)
			{
				num = 1f;
			}

			if (isKnife)
			{
				this.travelSpeed *= projectileDamage.force;
				reelDistance *= projectileDamage.force;
				this.gameObject.layer = LayerIndex.projectile.intVal;
			}

			startTime = Time.fixedTime;

			if (CleaverCreated != null)
			{
				Action action = CleaverCreated;
				action();
			}
		}

		// Token: 0x060025A4 RID: 9636 RVA: 0x0009C8CC File Offset: 0x0009AACC
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo) //This only triggers on the knife
		{
			if (!this.canHitWorld || !NetworkServer.active)
			{
				return;
			}
			this.coomerangState = (int)CoomerangProjectile.CoomerangState.FlyBack;
			UnityEvent unityEvent = this.onFlyBack;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}

			if (!hasFired)
			{
				hasFired = true;
				this.gameObject.layer = LayerIndex.noCollision.intVal;
				
				if (impactInfo.collider)
                {
					var hurtbox = impactInfo.collider.GetComponent<HurtBox>();
					if (hurtbox && hurtbox.healthComponent)
					{
						var projdamg = this.gameObject.GetComponent<ProjectileDamage>();
						var teamfilt = this.gameObject.GetComponent<TeamFilter>();

						new BlastAttack
						{
							attacker = projectileController.owner,
							crit = projdamg.crit,
							baseDamage = projdamg.damage,
							damageColorIndex = projdamg.damageColorIndex,
							damageType = projdamg.damageType,
							inflictor = this.gameObject,
							position = impactInfo.estimatedPointOfImpact,
							attackerFiltering = AttackerFiltering.NeverHit,
							falloffModel = BlastAttack.FalloffModel.None,
							procCoefficient = 1f,
							teamIndex = teamfilt.teamIndex,
							radius = 0.5f
						}.Fire();
						EffectManager.SimpleImpactEffect(this.impactSpark, impactInfo.estimatedPointOfImpact, -base.transform.forward, true);
					}
				}
				
			}
		}

		// Token: 0x060025A5 RID: 9637 RVA: 0x0009C91C File Offset: 0x0009AB1C
		private bool Reel()
		{
			Vector3 vector = this.projectileController.owner.transform.position - base.transform.position;
			Vector3 normalized = vector.normalized;
			return vector.magnitude <= reelDistance;
		}

		// Token: 0x060025A6 RID: 9638 RVA: 0x0009C968 File Offset: 0x0009AB68
		public void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				if (!this.projectileController.owner)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				switch ((CoomerangProjectile.CoomerangState)this.coomerangState)
				{
					case CoomerangProjectile.CoomerangState.FlyOut:
						if (NetworkServer.active)
						{
							this.rigidbody.velocity = this.travelSpeed * base.transform.forward;
							//if (target) this.rigidbody.velocity = this.travelSpeed * (target.position - base.transform.position).normalized;
							this.stopwatch += Time.fixedDeltaTime;
							if (this.stopwatch >= this.maxFlyStopwatch)
							{
								this.stopwatch = 0f;
								this.coomerangState = (int)CoomerangProjectile.CoomerangState.Transition;

								return;
							}
						}
						break;
					case CoomerangProjectile.CoomerangState.Transition:
						{
							if (!resetTargets)
							{
								resetTargets = true;
								ProjectileOverlapAttack poa = base.GetComponent<ProjectileOverlapAttack>();
								if (poa)
								{
									poa.ResetOverlapAttack();
								}
							}
							this.stopwatch += Time.fixedDeltaTime;
							float num = this.stopwatch / this.transitionDuration;
							Vector3 a = this.CalculatePullDirection();
							this.rigidbody.velocity = Vector3.Lerp(this.travelSpeed * base.transform.forward, this.travelSpeed * a, num);
							if (num >= 1f)
							{
								if (isKnife) this.gameObject.layer = LayerIndex.noCollision.intVal;

								this.coomerangState = (int)CoomerangProjectile.CoomerangState.FlyBack;
								UnityEvent unityEvent = this.onFlyBack;
								if (unityEvent == null)
								{
									return;
								}
								unityEvent.Invoke();
								return;
							}
							break;
						}
					case CoomerangProjectile.CoomerangState.FlyBack:
						{
							bool flag = this.Reel();
							if (NetworkServer.active)
							{
								this.canHitWorld = false;
								Vector3 a2 = this.CalculatePullDirection();
								this.rigidbody.velocity = this.travelSpeed * a2;
								if (flag)
								{
									if (Returned != null)
									{
										Action action = Returned;
										action();
									}
									UnityEngine.Object.Destroy(base.gameObject);
								}
							}
							break;
						}
					default:
						return;
				}

				/*if (followRet)
				{
					Ray aimRay = fieldComponent.aimRay;
					Vector3 position = transform.position - fieldComponent.characterBody.corePosition - 2 * Vector3.up;

					Vector3 cross = Vector3.Cross(position, aimRay.direction);
					Vector3 component2 = Vector3.Cross(position, cross);
					component2 = component2.normalized * -1f * Vector3.Angle(position, aimRay.direction);
					float timefactor = Mathf.Max(2f * (Time.fixedTime - startTime), 1);
					this.rigidbody.velocity += component2 / timefactor;
				}*/
			}
		}

		// Token: 0x060025A8 RID: 9640 RVA: 0x0009CB04 File Offset: 0x0009AD04
		private Vector3 CalculatePullDirection()
		{
			if (this.projectileController.owner)
			{
				return (this.projectileController.owner.transform.position - base.transform.position).normalized;
			}
			return base.transform.forward;
		}


		public bool followRet = true;	//was false
		public FieldComponent fieldComponent;
		private float startTime;

		public static event Action Returned;
		private float reelDistance = 3;

		// Token: 0x04002055 RID: 8277
		public float travelSpeed = 40f;

		// Token: 0x04002056 RID: 8278
		public float charge;

		// Token: 0x04002057 RID: 8279
		public float transitionDuration;

		// Token: 0x04002058 RID: 8280
		private float maxFlyStopwatch;

		// Token: 0x04002059 RID: 8281
		public GameObject impactSpark;

		// Token: 0x0400205B RID: 8283
		public bool canHitCharacters;

		// Token: 0x0400205C RID: 8284
		public bool canHitWorld;

		// Token: 0x0400205D RID: 8285
		private ProjectileController projectileController;

		// Token: 0x0400205E RID: 8286
		[SyncVar]
		private int coomerangState;

		// Token: 0x04002060 RID: 8288
		private ProjectileDamage projectileDamage;

		// Token: 0x04002061 RID: 8289
		private Rigidbody rigidbody;

		// Token: 0x04002062 RID: 8290
		private float stopwatch;

		// Token: 0x04002063 RID: 8291
		//private float fireAge;

		// Token: 0x04002064 RID: 8292
		//private float fireFrequency;

		// Token: 0x04002065 RID: 8293
		public float distanceMultiplier = 2f;

		// Token: 0x04002066 RID: 8294
		public UnityEvent onFlyBack;

		// Token: 0x02000603 RID: 1539
		public enum CoomerangState
		{
			// Token: 0x04002069 RID: 8297
			FlyOut,
			// Token: 0x0400206A RID: 8298
			Transition,
			// Token: 0x0400206B RID: 8299
			FlyBack
		}
	}
}