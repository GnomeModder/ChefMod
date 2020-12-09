using System;
using ChefMod;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EntityStates.Chef
{
    class Special : BaseSkillState
    {
        public float baseDuration = 0.1f;
        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            if (base.isAuthority)
            {
                RaycastHit[] array = Physics.SphereCastAll(characterBody.corePosition, 20f, Vector3.up, 5f, RoR2.LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
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
                                if (healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex == characterBody.teamComponent.teamIndex)
                                {
                                    if (!healthComponent.body.HasBuff(chefPlugin.foodBuffIndex))
                                    {
                                        healthComponent.body.AddTimedBuff(chefPlugin.foodBuffIndex, 4f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}