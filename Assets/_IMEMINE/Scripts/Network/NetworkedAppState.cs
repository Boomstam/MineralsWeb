using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetworkedAppState : NetworkBehaviour
{
    [SyncVar (OnChange = nameof(OnCurrentAuraTextIndexChange))] public int currentAuraTextIndex;
    [SyncVar  (OnChange = nameof(OnAppStateChange))] public AppState appState;
    
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
    
    [ServerRpc (RequireOwnership = false)]
    public void GoToNextAuraTextIndex(bool next)
    {
        int newIndex = currentAuraTextIndex + (next ? 1 : -1);
        
        newIndex = Mathf.Clamp(newIndex, 0, int.MaxValue);
        
        currentAuraTextIndex = newIndex;
    }
    
    private void OnAppStateChange(AppState oldValue, AppState newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"On Tutorial change: tutorial {newValue}");
        
        Instances.WebGLClientUI.SetToAppState(newValue);
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void ChangeAppState(AppState newAppState)
    {
        this.appState = newAppState;
    }
}

public enum AppState
{
    Tutorial,
    Introduction,
    MicroOrganisms,
    Magma,
    WaysOfWater,
    AboutCrystals,
    ColorOverlay,
}