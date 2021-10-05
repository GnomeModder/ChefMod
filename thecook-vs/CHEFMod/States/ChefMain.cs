using ChefMod;
using ChefPlugin;
using EntityStates;
using EntityStates.Engi.EngiMissilePainter;
using MonoMod;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Chef
{
    public class ChefMain : GenericCharacterMain
    {
        //public ChefMod.Trail oilTrail;
        //public float radius = 3f;
        ChildLocator childLocator;
        //HuntressTracker tracker;
        public static RuntimeAnimatorController displayAnimatorController = ChefContent.survivorDefs[0].displayPrefab.GetComponentInChildren<Animator>().runtimeAnimatorController;
        RuntimeAnimatorController cachedAnimatorController;

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

            //fieldComponent = base.characterBody.GetComponent<ChefMod.FieldComponent>();
            //fieldComponent.characterBody = base.characterBody;

            childLocator = base.GetModelChildLocator();
            if (skillLocator.primary.baseSkill == ChefMod.ChefPlugin.altPrimaryDef || skillLocator.primary.baseSkill == ChefMod.ChefPlugin.boostedAltPrimaryDef)
            {
                childLocator.FindChild("Cleaver").gameObject.SetActive(false);
                childLocator.FindChild("Knife").gameObject.SetActive(true);
            }
            if (skillLocator.primary.baseSkill == ChefMod.ChefPlugin.primaryDef || skillLocator.primary.baseSkill == ChefMod.ChefPlugin.boostedPrimaryDef)
            {
                childLocator.FindChild("Cleaver").gameObject.SetActive(true);
                childLocator.FindChild("Knife").gameObject.SetActive(false);
            }

            cachedAnimatorController = modelAnimator.runtimeAnimatorController;

            //tracker = base.characterBody.GetComponent<HuntressTracker>();
            //if (skillLocator.primary.baseSkill == chefPlugin.altPrimaryDef || skillLocator.primary.baseSkill == chefPlugin.boostedAltPrimaryDef)
            //{
            //    tracker.enabled = true;
            //}

            //RoR2.GlobalEventManager.onServerDamageDealt += chefHeal;
        }

        public override void Update()
        {
            base.Update();

            //if (Input.GetKeyDown(KeyCode.K))
            //{
            //    //for (int i = 0; i < 10; i++) characterBody.master.inventory.GiveItem(RoR2Content.Items.Syringe);
            //    characterBody.inventory.GiveItem(RoR2Content.Items.Bandolier);
            //}

            //this.oilTrail.damagePerSecond = base.characterBody.damage * 1.5f;
            //this.oilTrail.active = fieldComponent.active;

            //fieldComponent.aimRay = base.GetAimRay();
        }

        public override void HandleMovements()
        {
            base.HandleMovements();
            if (!characterBody) return;

            var speedMultiplier = 5; //~16 hooves?
            if (characterBody.moveSpeed > characterBody.baseMoveSpeed * speedMultiplier)
                if (characterBody.isSprinting)
                {
                    modelAnimator.runtimeAnimatorController = displayAnimatorController;
                    modelAnimator.speed = Mathf.Max(characterBody.moveSpeed / characterBody.baseMoveSpeed * speedMultiplier / 4, 1);
                    modelLocator.normalizeToFloor = true;
                    return;
                }
            modelAnimator.runtimeAnimatorController = cachedAnimatorController;
            modelAnimator.speed = 1;
            modelLocator.normalizeToFloor = false;
        }

        public override void OnExit()
        {
            //RoR2.GlobalEventManager.onServerDamageDealt -= chefHeal;

            //Object.Destroy(this.oilTrail.gameObject);
            //this.oilTrail = null;

            //tracker.enabled = false;

            if (characterBody.healthComponent.health < 1)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = 0.25f;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = characterBody.corePosition;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = true;
                blastAttack.baseDamage = 1000000;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = 300f;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.damageType = DamageType.Generic;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                BlastAttack.Result result = blastAttack.Fire();

                GameObject effect = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");
                EffectData effectData = new EffectData
                {
                    scale = 15f,
                    origin = characterBody.corePosition
                };
                EffectManager.SpawnEffect(effect, effectData, true);

                var direction = characterBody.gameObject.GetComponentInChildren<CharacterDirection>();
                foreach (var thisItem in direction.modelAnimator.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    thisItem.gameObject.SetActive(false);
                }
                foreach (var thisItem in direction.modelAnimator.GetComponentsInChildren<MeshRenderer>())
                {
                    thisItem.gameObject.SetActive(false);
                }

                childLocator.FindChild("Cleaver").gameObject.SetActive(false);
                childLocator.FindChild("Knife").gameObject.SetActive(false);
            }

            base.OnExit();
        }

        //private void chefHeal(DamageReport damage)
        //{
        //    if (damage.dotType == DotController.DotIndex.Burn && damage.victimTeamIndex != TeamIndex.Neutral)
        //    {
        //        Vector3 distance = characterBody.corePosition - damage.victimBody.corePosition;
        //        if (distance.magnitude < 40f)
        //        {
        //            characterBody.healthComponent.HealFraction(0.003f, damage.damageInfo.procChainMask);
        //        }
        //    }
        //}
    }
}
