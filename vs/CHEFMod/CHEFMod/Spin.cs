using RoR2;
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
            transform.localScale *= 20f;
        }

        void FixedUpdate()
        {

        }
    }
}