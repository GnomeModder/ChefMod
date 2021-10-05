using ChefMod;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EntityStates.Chef
{
    public class PrepBlaze : PrepSear
    {
        public override void NextState()
        {
            this.outer.SetNextState(new FireBlaze());
        }
    }

    public class FireBlaze : FireSear
    {
        public static new float damageCoefficient = 4.2f;
        public static GameObject ExplosionEffectBoosted;

        protected override GameObject explosion {
            get {
                return ExplosionEffectBoosted; 
            } 
        }

        public override void PlaySound()
        {
            Util.PlaySound("Play_Chefmod_Sear2", base.gameObject);
        }

        public override void ModifyBullet(BulletAttack ba)
        {
            ba.damage = base.damageStat * FireBlaze.damageCoefficient;
            ba.radius = 2.1f;
            ba.AddModdedDamageType(ChefMod.ChefPlugin.chefFireballOnHit);
            ba.AddModdedDamageType(ChefMod.ChefPlugin.chefSear);
        }
    }
}
