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

    [SyncVar (OnChange = nameof(OnChangeVotingTags))] private string votingHighTag;
    [SyncVar] private string votingLowTag;

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

    [ServerRpc (RequireOwnership = false)]
    public void SetVotingTags(string newHighTag, string newLowTag)
    {
        votingHighTag = newHighTag;
        votingLowTag = newLowTag;
    }

    private void OnChangeVotingTags(string oldValue, string newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        this.RunDelayed(0.5f, () =>
        {
            Instances.WebGLClientUI.SetVotingTags(votingHighTag, votingLowTag);
        });
    }
}
