using RoR2;
using System;
using UnityEngine;

namespace ChefMod
{
    public class FieldComponent : MonoBehaviour
    {
        public CharacterBody characterBody;
        public Ray aimRay = new Ray();
    }
}