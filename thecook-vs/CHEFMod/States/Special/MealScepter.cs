using System;
using ChefMod;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EntityStates.Chef
{
    class MealScepter : Meal
    {
        public override void NextState()
        {
            this.outer.SetNextState(new Meal() { playSound = false });
        }
    }
}