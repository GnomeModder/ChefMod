using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ChefMod
{
	// Token: 0x02000225 RID: 549
	public class Trail : MonoBehaviour
	{
		// Token: 0x06000C0F RID: 3087 RVA: 0x00031F70 File Offset: 0x00030170
		private void Awake()
		{
			this.pointsList = new List<Trail.TrailPoint>();
			this.transform = base.transform;
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x00031F89 File Offset: 0x00030189
		private void Start()
		{
			this.localTime = 0f;
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x00031FA4 File Offset: 0x000301A4
		private void FixedUpdate()
		{
			this.localTime += Time.fixedDeltaTime;
			if (this.localTime >= this.nextUpdate)
			{
				this.nextUpdate += this.updateInterval;
				this.UpdateTrail(this.active);
			}
			if (this.pointsList.Count > 0)
			{
				Trail.TrailPoint trailPoint = this.pointsList[this.pointsList.Count - 1];
				trailPoint.position = this.transform.position;
				trailPoint.localEndTime = this.localTime + this.pointLifetime;
				this.pointsList[this.pointsList.Count - 1] = trailPoint;
				if (trailPoint.segmentTransform)
				{
					trailPoint.segmentTransform.position = this.transform.position;
				}
				if (this.lineRenderer)
				{
					this.lineRenderer.SetPosition(this.pointsList.Count - 1, trailPoint.position);
				}
			}
			if (this.segmentPrefab)
			{
				Vector3 position = this.transform.position;
				for (int i = this.pointsList.Count - 1; i >= 0; i--)
				{
					Transform segmentTransform = this.pointsList[i].segmentTransform;
					segmentTransform.LookAt(position, Vector3.up);
					Vector3 a = this.pointsList[i].position - position;
					segmentTransform.position = position + a * 0.5f;
					float num = Mathf.Clamp01(Mathf.InverseLerp(this.pointsList[i].localStartTime, this.pointsList[i].localEndTime, this.localTime));
					Vector3 localScale = new Vector3(this.radius * (1f - num), this.radius * (1f - num), a.magnitude);
					segmentTransform.localScale = localScale;
					position = this.pointsList[i].position;
				}
			}
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x000321A8 File Offset: 0x000303A8
		private void UpdateTrail(bool addPoint)
		{
			while (this.pointsList.Count > 0 && this.pointsList[0].localEndTime <= this.localTime)
			{
				this.RemovePoint(0);
			}
			if (addPoint)
			{
				this.AddPoint();
			}
			if (NetworkServer.active)
			{
				this.DoDamage();
			}
			if (this.lineRenderer)
			{
				this.UpdateLineRenderer(this.lineRenderer);
			}
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x00032214 File Offset: 0x00030414
		private void DoDamage()
		{
			if (this.pointsList.Count == 0)
			{
				return;
			}
			float damage = this.damagePerSecond * this.updateInterval;
			Vector3 vector = this.pointsList[this.pointsList.Count - 1].position;
			HashSet<GameObject> hashSet = new HashSet<GameObject>();
			RoR2.TeamIndex attackerTeamIndex = RoR2.TeamIndex.Neutral;
			if (this.owner)
			{
				hashSet.Add(this.owner);
				attackerTeamIndex = RoR2.TeamComponent.GetObjectTeam(this.owner);
			}
			for (int i = this.pointsList.Count - 2; i >= 0; i--)
			{
				Vector3 position = this.pointsList[i].position;
				Vector3 direction = position - vector;
				RaycastHit[] array = Physics.SphereCastAll(new Ray(vector, direction), this.radius, direction.magnitude, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
				for (int j = 0; j < array.Length; j++)
				{
					Collider collider = array[j].collider;
					if (collider.gameObject)
					{
						RoR2.HurtBox component = collider.GetComponent<RoR2.HurtBox>();
						if (component)
						{
							RoR2.HealthComponent healthComponent = component.healthComponent;
							if (healthComponent)
							{
								GameObject gameObject = healthComponent.gameObject;
								if (!hashSet.Contains(gameObject))
								{
									hashSet.Add(gameObject);
									if (RoR2.FriendlyFireManager.ShouldSplashHitProceed(healthComponent, attackerTeamIndex))
									{
										healthComponent.TakeDamage(new RoR2.DamageInfo
										{
											position = array[j].point,
											attacker = this.owner,
											inflictor = base.gameObject,
											crit = false,
											damage = damage,
											damageColorIndex = RoR2.DamageColorIndex.Default,
											damageType = RoR2.DamageType.Generic,
											force = Vector3.zero,
											procCoefficient = 0f
										});
									}
								}
							}
						}
					}
				}
				vector = position;
			}
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x00032400 File Offset: 0x00030600
		private void UpdateLineRenderer(LineRenderer lineRenderer)
		{
			lineRenderer.positionCount = this.pointsList.Count;
			for (int i = 0; i < this.pointsList.Count; i++)
			{
				lineRenderer.SetPosition(i, this.pointsList[i].position);
			}
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x0003244C File Offset: 0x0003064C
		private void AddPoint()
		{
			Trail.TrailPoint item = new Trail.TrailPoint
			{
				position = this.transform.position,
				localStartTime = this.localTime,
				localEndTime = this.localTime + this.pointLifetime
			};
			if (this.segmentPrefab)
			{
				item.segmentTransform = UnityEngine.Object.Instantiate<GameObject>(this.segmentPrefab, this.transform).transform;
			}
			this.pointsList.Add(item);
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x000324CC File Offset: 0x000306CC
		private void RemovePoint(int pointIndex)
		{
			if (this.destroyTrailSegments && this.pointsList[pointIndex].segmentTransform)
			{
				UnityEngine.Object.Destroy(this.pointsList[pointIndex].segmentTransform.gameObject);
			}
			this.pointsList.RemoveAt(pointIndex);
		}

		// Token: 0x04000BA3 RID: 2979
		[Tooltip("How often to drop a new point onto the trail and do damage.")]
		public float updateInterval = 0.2f;

		// Token: 0x04000BA4 RID: 2980
		[Tooltip("How large the radius of the damage detection should be.")]
		public float radius = 0.5f;

		// Token: 0x04000BA5 RID: 2981
		[Tooltip("How long a point on the trail should last.")]
		public float pointLifetime = 3f;

		// Token: 0x04000BA6 RID: 2982
		[Tooltip("The line renderer to use for display.")]
		public LineRenderer lineRenderer;

		// Token: 0x04000BA7 RID: 2983
		public bool active = true;

		// Token: 0x04000BA8 RID: 2984
		[Tooltip("Prefab to use per segment.")]
		public GameObject segmentPrefab;

		// Token: 0x04000BA9 RID: 2985
		public bool destroyTrailSegments;

		// Token: 0x04000BAA RID: 2986
		public float damagePerSecond;

		// Token: 0x04000BAB RID: 2987
		public GameObject owner;

		// Token: 0x04000BAC RID: 2988
		private new Transform transform;

		// Token: 0x04000BAD RID: 2989
		public List<Trail.TrailPoint> pointsList;

		// Token: 0x04000BAE RID: 2990
		private float localTime;

		// Token: 0x04000BAF RID: 2991
		private float nextUpdate;

		// Token: 0x02000226 RID: 550
		public struct TrailPoint
		{
			// Token: 0x04000BB0 RID: 2992
			public Vector3 position;

			// Token: 0x04000BB1 RID: 2993
			public float localStartTime;

			// Token: 0x04000BB2 RID: 2994
			public float localEndTime;

			// Token: 0x04000BB3 RID: 2995
			public Transform segmentTransform;
		}
	}
}
