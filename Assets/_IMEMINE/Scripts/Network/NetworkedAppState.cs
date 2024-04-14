using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetworkedAppState : NetworkBehaviour
{
    [SyncVar (OnChange = nameof(OnCurrentAuraTextIndexChange))] public int currentAuraTextIndex;

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        Debug.Log($"Start Client NetworkedAppState");
    }

    private void OnCurrentAuraTextIndexChange(int oldValue, int newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Instances.WebGLClientUI.auraTextDisplay.GoToText(newValue);
    }
}