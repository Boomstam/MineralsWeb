using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetworkedAppState : NetworkBehaviour
{
    [SyncVar (OnChange = nameof(OnCurrentAuraTextIndexChange))] public int currentAuraTextIndex;
    [SyncVar (OnChange = nameof(OnAppStateChange))] public AppState appState;
    [SyncVar (OnChange = nameof(OnQuadrantsModeChange))] private bool quadrantMode;
    [SyncVar (OnChange = nameof(OnCenterModeChange))] private bool centerMode;
    [SyncVar (OnChange = nameof(OnCenterModeValChange))] private float centerModeVal;
    [SyncVar] public Vector2 quadrantSeatMinMax;
    [SyncVar] public Vector2 quadrantRowMinMax;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        Debug.Log($"Start Client NetworkedAppState");
    }

    [ServerRpc (RequireOwnership = false)]
    public void GoToNextAuraTextIndex(bool next)
    {
        int newIndex = currentAuraTextIndex + (next ? 1 : -1);
        
        newIndex = Mathf.Clamp(newIndex, 0, int.MaxValue);
        
        currentAuraTextIndex = newIndex;
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
    public void ChangeAppState(AppState newAppState)
    {
        this.appState = newAppState;
    }
    
    private void OnAppStateChange(AppState oldValue, AppState newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"On Tutorial change: tutorial {newValue}");
        
        Instances.WebGLClientUI.SetToAppState(newValue);
    }

    [ServerRpc (RequireOwnership = false)]
    public void DisableQuadrantsMode()
    {
        quadrantMode = false;
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void EnableQuadrantRanges(Vector2 newSeatMinMax, Vector2 newRowMinMax)
    {
        quadrantSeatMinMax = newSeatMinMax;
        quadrantRowMinMax = newRowMinMax;

        quadrantMode = true;
    }

    private void OnQuadrantsModeChange(bool oldValue, bool newValue, bool asServer)
    {
        if (Instances.BuildType != BuildType.Voting)
            return; 
        
        if(newValue)
            Instances.AudioManager.OnQuadrantsModeDisabled();
        else
            Instances.AudioManager.OnQuadrantsModeEnabled(quadrantSeatMinMax, quadrantRowMinMax);
    }

    [ServerRpc (RequireOwnership = false)]
    public void ToggleCenterMode(bool centerModeOn)
    {
        
    }
    
    private void OnCenterModeChange(bool oldValue, bool newValue, bool asServer)
    {
        Debug.Log($"OnCenterModeChange: {newValue} NOT IMPLEMENTED");
    }

    [ServerRpc (RequireOwnership = false)]
    public void ChangeCenterModeVal(float val)
    {
        
    }
        
    private void OnCenterModeValChange(float oldValue, float newValue, bool asServer)
    {
        Debug.Log($"OnCenterModeValChange: {newValue} NOT IMPLEMENTED");
    }
}

public enum AppState
{
    Tutorial,
    Introduction,
    MicroOrganisms,
    WaysOfWater,
    Magma,
    AboutCrystals,
    ColorOverlay,
}