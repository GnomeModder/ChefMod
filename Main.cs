using ChefMod;
using EntityStates;
using EntityStates.Engi.EngiMissilePainter;
using MonoMod;
using RoR2;
using UnityEngine;

namespace EntityStates.Chef
{
    public class Main : GenericCharacterMain
    {
        //public ChefMod.Trail oilTrail;
        //public float radius = 3f;
        ChefMod.FieldComponent fieldComponent;
        public override void OnEnter()
        {
            base.OnEnter();

            //GameObject fireTrail = new GameObject(); //UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/FireTrail"), this.transform);
            //fireTrail.AddComponent<Trail>();
            //this.oilTrail = fireTrail.GetComponent<ChefMod.Trail>();
            //this.oilTrail.transform.position = base.characterBody.footPosition;
            //this.oilTrail.owner = base.gameObject;
            //this.oilTrail.radius *= this.radius;
            //this.oilTrail.pointLifetime = 10f;
            //this.oilTrail.active = false;
            //this.oilTrail.segmentPrefab = ChefMod.Assets.chefAssetBundle.LoadAsset<GameObject>("Particle System");

            //DamageTrail damageTrail = fireTrail.GetComponent<DamageTrail>();
            //this.oilTrail.lineRenderer = damageTrail.lineRenderer;
            //this.oilTrail.segmentPrefab = damageTrail.segmentPrefab;

            fieldComponent = base.characterBody.GetComponent<ChefMod.FieldComponent>();
            fieldComponent.characterBody = base.characterBody;

            RoR2.GlobalEventManager.onServerDamageDealt += chefHeal;
        }

        public override void Update()
        {
            base.Update();

            //if (Input.GetKeyDown(KeyCode.K))
            //{
            //    characterBody.master.inventory.GiveItem(ItemIndex.SiphonOnLowHealth);
            //}

            //this.oilTrail.damagePerSecond = base.characterBody.damage * 1.5f;
            //this.oilTrail.active = fieldComponent.active;

            fieldComponent.aimRay = base.GetAimRay();
        }

        public override void OnExit()
        {
            RoR2.GlobalEventManager.onServerDamageDealt -= chefHeal;

            //Object.Destroy(this.oilTrail.gameObject);
            //this.oilTrail = null;
            base.OnExit();
        }

        private void chefHeal(DamageReport damage)
        {
            if (damage.dotType == DotController.DotIndex.Burn)
            {
                Vector3 distance = characterBody.corePosition - damage.victimBody.corePosition;
                if (distance.magnitude < 40f)
                {
                    characterBody.healthComponent.HealFraction(0.003f, damage.damageInfo.procChainMask);
                }
            }
        }
    }
}
