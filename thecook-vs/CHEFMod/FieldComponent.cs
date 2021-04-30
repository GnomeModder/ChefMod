using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace ChefMod
{
    public class FieldComponent : NetworkBehaviour
    {
        public CharacterBody characterBody;
        public Ray aimRay = new Ray();
        public GameObject oil;
    }
}