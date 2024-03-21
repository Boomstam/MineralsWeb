using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetworkedAppState : NetworkBehaviour
{
    [SyncVar] public AppState appState;

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        Debug.Log($"Start Client NetworkedAppState");
    }
}

public enum AppState
{
    Introduction,
    Voting,
    ColorOverlay,
    EffectSliders,
}