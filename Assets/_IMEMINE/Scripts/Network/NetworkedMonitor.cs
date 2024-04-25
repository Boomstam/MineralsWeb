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
    [SyncVar (OnChange = nameof(OnChangeIntroductionMode))] public bool introductionMode;
    [SyncVar (OnChange = nameof(OnChangeShouldShowOverlays))] public bool shouldShowOverlays;
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
        
        Instances.WebGLClientUI.SetVotingTags(votingHighTag, votingLowTag);
    }

    [ServerRpc (RequireOwnership = false)]
    public void ChangeShouldShowOverlays(bool newShouldShowOverlays)
    {
        shouldShowOverlays = newShouldShowOverlays;
    }
    
    private void OnChangeShouldShowOverlays(bool oldValue, bool newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Instances.WebGLClientUI.ToggleColorOverlayOverride(newValue);
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void ChangeIntroductionMode(bool newIntroductionMode)
    {
        introductionMode = newIntroductionMode;
    }
    
    private void OnChangeIntroductionMode(bool oldValue, bool newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Instances.WebGLClientUI.ToggleIntroductionMode(newValue);
    }
}
