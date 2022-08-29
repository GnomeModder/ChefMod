using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using RoR2;
using RoR2.Projectile;

namespace ChefMod.Components
{
    public class KnifeProjectileController : MonoBehaviour
    {
		private LineRenderer lineRenderer;
		public Transform shoulder;
		public bool notifyKnifeHandler = true;
		private GameObject owner;
		private KnifeHandler knifeHandler;
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
				this.knifeHandler = owner.GetComponent<KnifeHandler>();
				if (knifeHandler)
				{
					shoulder = knifeHandler.shoulder;
					this.lineRenderer.enabled = true;
				}
			}
		}

		public void OnDestroy()
        {
			this.lineRenderer.enabled = false;
			if (notifyKnifeHandler && knifeHandler && NetworkServer.active)
            {
				knifeHandler.ReturnKnife();
            }
		}

		public void Update()
		{
			if (this.lineRenderer.enabled && shoulder)
			{
				this.lineRenderer.SetPositions(new Vector3[2] { this.transform.position, shoulder.position });
			}
		}
	}
}
