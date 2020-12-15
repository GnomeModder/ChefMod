using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace ChefMod
{
    public class Spin : MonoBehaviour
    {
        private float time;

        void Start()
        {
            time = Time.fixedTime;
            transform.localScale *= 10f;
            transform.rotation = Util.QuaternionSafeLookRotation(Vector3.forward);
        }

        void FixedUpdate()
        {
            transform.Rotate(Vector3.right, Time.deltaTime);
        }
    }
}