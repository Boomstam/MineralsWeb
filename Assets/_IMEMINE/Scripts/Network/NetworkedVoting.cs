using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class NetworkedVoting : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void SendVoteUpdate(float newVal, NetworkConnection conn = null)
    {
        
    }
}
