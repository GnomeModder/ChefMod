using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System.Linq;
using RoR2;

namespace ChefMod.Components
{
    public class RandomizeModelOnStartPod : NetworkBehaviour
    {
        [SyncVar]
        public int index = 0;

        private GameObject chosenGameObject;

        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;

        public GameObject chefBody;

        public static Dictionary<GameObject, Vector3> keyValuePairs = new Dictionary<GameObject, Vector3>()
        {
            { Resources.Load<GameObject>("prefabs/pickupmodels/PickupFruit"), Vector3.one * 20f },
            { Resources.Load<GameObject>("prefabs/pickupmodels/PickupSteak"), Vector3.one * 10f },
            //Resources.Load<GameObject>("prefabs/pickupmodels/PickupMushroom"), //bungus errors?
            { Resources.Load<GameObject>("prefabs/pickupmodels/PickupInterstellarDeskPlant"), Vector3.one * 8f }
        };

        public static GameObject impactEffect = ChefPlugin.fruitPodImpactPrefab;

        public void Start()
        {
            if (!ChefPlugin.altPodPrefab.Value)
                return;

            if (NetworkServer.active)
                index = UnityEngine.Random.Range(0, keyValuePairs.Count);

            //Debug.Log("Chosen Index:" + index);
            var choice = keyValuePairs.ElementAt(index);
            chosenGameObject = choice.Key;
            meshRenderer.material = chosenGameObject.GetComponentInChildren<MeshRenderer>().material;
            meshFilter.sharedMesh = chosenGameObject.GetComponentInChildren<MeshFilter>().sharedMesh;
            gameObject.transform.localScale = choice.Value;
            chefBody = gameObject.transform.root.GetComponent<VehicleSeat>().passengerBodyObject;
        }

        public void OnDestroy()
        {
            RoR2.Util.PlaySound("Play_ChefMod_Ding", chefBody);
            var impact = Object.Instantiate<GameObject>(impactEffect);
            impact.transform.position = transform.position;

            var com = impact.transform.Find("Particles/Chunks, Solid").GetComponent<ParticleSystemRenderer>();
            com.mesh = meshFilter.sharedMesh;
            com.material = meshRenderer.material;
            /*try
            {
                ChefPlugin.logger.LogMessage("15a");
                com.SetMeshes(new Mesh[] { meshFilter.sharedMesh });
            }
            catch
            {
                ChefPlugin.logger.LogMessage("14b");
                com.mesh = chosenGameObject.GetComponentInChildren<MeshFilter>().sharedMesh;
            }
            ChefPlugin.logger.LogMessage("15");
            com.SetMaterials(new List<Material>() { meshRenderer.material });*/
            //ChefPlugin.logger.LogMessage("16");
        }
    }
}
