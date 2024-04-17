using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetworkedAppState : NetworkBehaviour
{
    [SyncVar (OnChange = nameof(OnCurrentAuraTextIndexChange))] public int currentAuraTextIndex;
    // [SyncVar (OnChange = nameof(OnTutorialChange))] public bool tutorial = true;
    [SyncVar] public AppState appState;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        Debug.Log($"Start Client NetworkedAppState");
    }
    
    private void OnCurrentAuraTextIndexChange(int oldValue, int newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        if(appState != AppState.Introduction)
            return;
        
        Instances.WebGLClientUI.auraTextDisplay.GoToText(newValue);
    }
    
    // private void OnTutorialChange(bool oldValue, bool newValue, bool asServer)
    // {
    //     if(Instances.BuildType != BuildType.Voting)
    //         return;
    //     
    //     Debug.Log($"On Tutorial change: tutorial {newValue}");
    //  
    //     Instances.WebGLClientUI.ToggleTutorial(newValue);
    // }
    
    [Server]
    public void ChangeAppState(AppState newAppState)
    {
        this.appState = newAppState;
    }
}