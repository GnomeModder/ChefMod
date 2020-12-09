using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;

namespace ChefMod
{
	// Token: 0x02000602 RID: 1538
	[RequireComponent(typeof(ProjectileController))]
	public class CoomerangProjectile : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x060025A2 RID: 9634 RVA: 0x0009C7B0 File Offset: 0x0009A9B0
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			if (this.projectileController && this.projectileController.owner)
			{
				this.ownerTransform = this.projectileController.owner.transform;
			}
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
			Vector3 localScale = new Vector3(num * base.transform.localScale.x, num * base.transform.localScale.y, num * base.transform.localScale.z);
			base.transform.localScale = localScale;
			base.gameObject.GetComponent<ProjectileController>().ghost.transform.localScale = localScale;
			base.GetComponent<ProjectileDotZone>().damageCoefficient *= num;

			startTime = Time.fixedTime;
		}

		// Token: 0x060025A4 RID: 9636 RVA: 0x0009C8CC File Offset: 0x0009AACC
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!this.canHitWorld)
			{
				return;
			}
			this.NetworkCoomerangState = CoomerangProjectile.CoomerangState.FlyBack;
			UnityEvent unityEvent = this.onFlyBack;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			EffectManager.SimpleImpactEffect(this.impactSpark, impactInfo.estimatedPointOfImpact, -base.transform.forward, true);
		}

		// Token: 0x060025A5 RID: 9637 RVA: 0x0009C91C File Offset: 0x0009AB1C
		private bool Reel()
		{
			Vector3 vector = this.projectileController.owner.transform.position - base.transform.position;
			Vector3 normalized = vector.normalized;
			return vector.magnitude <= 2f;
		}

		// Token: 0x060025A6 RID: 9638 RVA: 0x0009C968 File Offset: 0x0009AB68
		public void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				if (!this.setScale)
				{
					this.setScale = true;
				}
				if (!this.projectileController.owner)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				switch (this.coomerangState)
				{
					case CoomerangProjectile.CoomerangState.FlyOut:
						if (NetworkServer.active)
						{
							this.rigidbody.velocity = this.travelSpeed * base.transform.forward;
							this.stopwatch += Time.fixedDeltaTime;
							if (this.stopwatch >= this.maxFlyStopwatch)
							{
								this.stopwatch = 0f;
								this.NetworkCoomerangState = CoomerangProjectile.CoomerangState.Transition;
								return;
							}
						}
						break;
					case CoomerangProjectile.CoomerangState.Transition:
						{
							this.stopwatch += Time.fixedDeltaTime;
							float num = this.stopwatch / this.transitionDuration;
							Vector3 a = this.CalculatePullDirection();
							this.rigidbody.velocity = Vector3.Lerp(this.travelSpeed * base.transform.forward, this.travelSpeed * a, num);
							if (num >= 1f)
							{
								this.NetworkCoomerangState = CoomerangProjectile.CoomerangState.FlyBack;
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
									UnityEngine.Object.Destroy(base.gameObject);
								}
							}
							break;
						}
					default:
						return;
				}

				Ray aimRay = fieldComponent.aimRay;
				Vector3 position = transform.position - fieldComponent.characterBody.corePosition;

				Vector3 cross = Vector3.Cross(position, aimRay.direction);
				Vector3 component2 = Vector3.Cross(position, cross);
				component2 = component2.normalized * -1f * Vector3.Angle(position, aimRay.direction);
				this.rigidbody.velocity += component2 / Mathf.Max(2f * (Time.fixedTime - startTime), 1);
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

		// Token: 0x060025A9 RID: 9641 RVA: 0x00004379 File Offset: 0x00002579
		private void UNetVersion()
		{
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x060025AA RID: 9642 RVA: 0x0009CB5C File Offset: 0x0009AD5C
		// (set) Token: 0x060025AB RID: 9643 RVA: 0x0009CB6F File Offset: 0x0009AD6F
		public CoomerangProjectile.CoomerangState NetworkCoomerangState
		{
			get
			{
				return this.coomerangState;
			}
			[param: In]
			set
			{
				base.SetSyncVar<CoomerangProjectile.CoomerangState>(value, ref this.coomerangState, 1U);
			}
		}

		// Token: 0x060025AC RID: 9644 RVA: 0x0009CB84 File Offset: 0x0009AD84
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write((int)this.coomerangState);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write((int)this.coomerangState);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060025AD RID: 9645 RVA: 0x0009CBF0 File Offset: 0x0009ADF0
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.coomerangState = (CoomerangProjectile.CoomerangState)reader.ReadInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.coomerangState = (CoomerangProjectile.CoomerangState)reader.ReadInt32();
			}
		}

		public FieldComponent fieldComponent;
		private float startTime;
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

		// Token: 0x0400205A RID: 8282
		public GameObject crosshairPrefab;

		// Token: 0x0400205B RID: 8283
		public bool canHitCharacters;

		// Token: 0x0400205C RID: 8284
		public bool canHitWorld;

		// Token: 0x0400205D RID: 8285
		private ProjectileController projectileController;

		// Token: 0x0400205E RID: 8286
		[SyncVar]
		private CoomerangProjectile.CoomerangState coomerangState;

		// Token: 0x0400205F RID: 8287
		private Transform ownerTransform;

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

		// Token: 0x04002067 RID: 8295
		private bool setScale;

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