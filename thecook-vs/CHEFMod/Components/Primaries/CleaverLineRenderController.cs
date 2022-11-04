using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using RoR2;
using RoR2.Projectile;

namespace ChefMod.Components
{
    public class CleaverLineRenderController : MonoBehaviour
	{
		private LineRenderer lineRenderer;
		public Transform lineOrigin;
		private GameObject owner;
		private ProjectileController projectileController;
		public void Awake()
		{
			this.lineRenderer = base.GetComponent<LineRenderer>();
			this.projectileController = base.GetComponent<ProjectileController>();
		}

		public void Start()
		{
			owner = this.projectileController.owner;
			if (owner)
            {
				lineOrigin = owner.transform;

				KnifeHandler knifeHandler = owner.GetComponent<KnifeHandler>();
				if (knifeHandler)
				{
					lineOrigin = knifeHandler.cleaverOrigin;
				}

				if (ChefPlugin.enableCleaverTrails.Value) this.lineRenderer.enabled = true;
			}
			else
            {
				Destroy(this);
            }				
		}

		public void OnDestroy()
		{
			this.lineRenderer.enabled = false;
		}

		public void Update()
		{
			if (this.lineRenderer.enabled && lineOrigin)
			{
				this.lineRenderer.SetPositions(new Vector3[2] { this.transform.position, lineOrigin.position });
			}
		}
	}
}
