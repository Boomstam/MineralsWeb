using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using UnityEngine;

public class NetworkedMonitor : NetworkBehaviour
{
    [SyncVar] public float minDelayTime;
    [SyncVar] public float maxDelayTime;
    [SyncVar] public float delayIntervalLength;

    [ServerRpc (RequireOwnership = false), Button]
    public void SetMinDelayTime(float delayTime)
    {
        minDelayTime = delayTime;
    }
    
    [ServerRpc (RequireOwnership = false), Button]
    public void SetMaxDelayTime(float delayTime)
    {
        maxDelayTime = delayTime;
    }
    
    [ServerRpc (RequireOwnership = false), Button]
    public void SetIntervalLength(float intervalLength)
    {
        delayIntervalLength = intervalLength;
    }
}
