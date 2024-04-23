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
    [SyncVar (OnChange = nameof(OnEffectSlidersOnChange))] private bool effectSlidersOn;
    [SyncVar (OnChange = nameof(OnCirclesPosChange))] private Vector2Int circlesPos;
    [SyncVar] public bool shouldPlayDelays;
    [SyncVar] public Vector2 quadrantSeatMinMax;
    [SyncVar] public Vector2 quadrantRowMinMax;
    [SerializeField] private Vector2Int circlesSize;
    
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
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"OnCenterModeChange: {newValue} NOT IMPLEMENTED");
    }

    [ServerRpc (RequireOwnership = false)]
    public void ChangeCenterModeVal(float val)
    {
        
    }
        
    private void OnCenterModeValChange(float oldValue, float newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"OnCenterModeValChange: {newValue} NOT IMPLEMENTED");
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void ChangeEffectSlidersOn(bool newEffectSlidersOn)
    {
        Debug.Log($"On server ChangeEffectSlidersOn: {newEffectSlidersOn}");
        
        effectSlidersOn = newEffectSlidersOn;
    }
    
    private void OnEffectSlidersOnChange(bool oldValue, bool newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"OnEffectSlidersOnChange: {newValue}");

        Instances.WebGLClientUI.ToggleEffectSlidersMode(newValue);
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void ChangeCirclesPos(Vector2Int newCirclesPos)
    {
        circlesPos = newCirclesPos;
    }
    
    private void OnCirclesPosChange(Vector2Int oldValue, Vector2Int newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"OnCirclesPosChange: {newValue} NOT IMPLEMENTED");

        // Instances.WebGLClientUI.ToggleEffectSlidersMode(newValue);
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void ChangeShouldPlayDelays(bool newShouldPlayDelays)
    {
        shouldPlayDelays = newShouldPlayDelays;
    }
    
    // private void OnShouldPlayDelaysChange(bool oldValue, bool newValue, bool asServer)
    // {
    //     if(Instances.BuildType != BuildType.Voting)
    //         return;
    //     
    //     Debug.Log($"OnCirclesPosChange: {newValue} NOT IMPLEMENTED");
    //
    //     if()
    //     // Instances.WebGLClientUI.ToggleEffectSlidersMode(newValue);
    // }
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