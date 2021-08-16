using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace ChefMod.Components
{
    public class NetworkAchievements : NetworkBehaviour
    {
        [ClientRpc]
        public void RpcGrantChefUnlock()
        {

        }
    }
}
