using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PerformanceManager : NetworkBehaviour
{
    [SyncVar] public float runningTime;
    
    [ServerRpc (RequireOwnership = false)]
    public void StartPerformance()
    {
        Debug.Log("StartPerformance");
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void StopPerformance()
    {
        Debug.Log("StopPerformance");
    }
}
