using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using RoR2;

namespace ChefMod.Components
{
    public class KnifeHandler : NetworkBehaviour
    {
        public bool knifeThrown { get; private set; }
        public Transform shoulder;

        public void Awake()
        {
            knifeThrown = false;
        }

        public void Start()
        {
            ChildLocator childLocator = null;
            ModelLocator ml = base.GetComponent<ModelLocator>();
            if (ml && ml.modelTransform)
            {
                childLocator = ml.modelTransform.GetComponent<ChildLocator>();
            }
            shoulder = childLocator.FindChild("RightShoulder");
        }

        //Client sets knife thrown state to true, then notifies other players.
        //This is not a syncvar because the client needs the most control on whether it's thrown or not since skills are clientside, and SyncVars are server-side.
        [Client]
        public void ThrowKnife()
        {
            if (this.hasAuthority)
            {
                knifeThrown = true;
                CmdThrowKnife();
            }
        }
        [Command]
        public void CmdThrowKnife()
        {
            RpcThrowKnife();
        }
        [ClientRpc]
        public void RpcThrowKnife()
        {
            if (!this.hasAuthority)
            {
                knifeThrown = true;
            }
        }
    
        //The server has control of projectiles, so it is the one notifying others when the knife has finished its job.
        [Server]
        public void ReturnKnife()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            RpcReturnKnife();
        }
        [ClientRpc]
        public void RpcReturnKnife()
        {
            knifeThrown = false;
        }
    }
}
