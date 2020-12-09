using IL.RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;

namespace ChefMod
{
    class CleaverComponent : MonoBehaviour
    {
        public FieldComponent fieldComponent = new FieldComponent();
        float initialTime;
        Rigidbody rig;
        RoR2.CharacterBody characterBody;

        void Start()
        {
            initialTime = Time.fixedTime;
            rig = GetComponent<Rigidbody>();
            characterBody = fieldComponent.characterBody;
        }

        void Update()
        {
            Ray aimRay = fieldComponent.aimRay;
            Vector3 position = transform.position - characterBody.corePosition;
            float time = Time.fixedTime - initialTime;

            Vector3 cross = Vector3.Cross(position, aimRay.direction);
            Vector3 component2 = Vector3.Cross(position, cross);
            component2 = component2.normalized * -1f * Vector3.Angle(position, aimRay.direction);

            rig.velocity = component2;
        }
    }
}
