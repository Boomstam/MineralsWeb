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
    [SyncVar (OnChange = nameof(OnCirclesPosChange))] private Vector2Int circlesPos = new Vector2Int(41, 3);
    [SyncVar (OnChange = nameof(OnShouldPlayCirclesChange))] public bool shouldPlayCircles;
    [SyncVar (OnChange = nameof(OnShouldPlayMicroOrganismsChange))] public bool shouldPlayMicroOrganisms;
    [SyncVar] public bool shouldPlayDelays;
    [SyncVar] public Vector2 quadrantSeatMinMax;
    [SyncVar] public Vector2 quadrantRowMinMax;
    
    [SerializeField] private Vector2Int circlesSize = new Vector2Int(9, 5);
    
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
        
        Debug.Log($"OnQuadrantsModeChange: {newValue}");
        
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
    
    [ServerRpc (RequireOwnership = false)]
    public void ChangeShouldPlayCircles(bool newShouldPlayCircles)
    {
        shouldPlayCircles = newShouldPlayCircles;
    }
    
    private void OnShouldPlayCirclesChange(bool oldValue, bool newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        if(newValue)
        {
            // Instances.AudioManager.ResetCirclesVolume();
            Instances.AudioManager.circlePlayer.StartPlayback();
        }
        else
        {
            Instances.AudioManager.circlePlayer.StopPlayback();
        }
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void ShiftCirclesPos(Vector2Int offset)
    {
        circlesPos += offset;
    }
    
    private void OnCirclesPosChange(Vector2Int oldValue, Vector2Int newValue, bool asServer)
    {
        if (Instances.BuildType == BuildType.Monitor)
        {
            Instances.MonitorUI.SetCirclesPos(newValue);
        }
        
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        if(appState != AppState.WaysOfWater)
        {
            Instances.AudioManager.circlePlayer.StopPlayback();
            return;
        }
        
        bool playCirclesAtThisSeat = PlayCirclesAtThisSeat();
        
        Debug.Log($"OnCirclesPosChange: {newValue}, playCirclesAtThisSeat: {playCirclesAtThisSeat}");
        
        // Instances.AudioManager.MuteCircles(playCirclesAtThisSeat == false);
        if(playCirclesAtThisSeat)
            Instances.AudioManager.circlePlayer.StartPlayback();
        else
            Instances.AudioManager.circlePlayer.StopPlayback();
    }
    
    private bool PlayCirclesAtThisSeat()
    {
        Debug.Log($"PlayCirclesAtThisSeat:  {Instances.SeatNumber} - {Instances.RowNumber} {circlesPos}, circlesSize: {circlesSize}");
        
        return (Instances.SeatNumber <= circlesPos.x && Instances.SeatNumber >= circlesPos.x - circlesSize.x &&
                Instances.RowNumber >= circlesPos.y && Instances.RowNumber <= circlesPos.y + circlesSize.y);
    }

    [ServerRpc (RequireOwnership = false)]
    public void ChangeShouldPlayMicroOrganisms(bool newShouldPlayMicroOrganisms)
    {
        shouldPlayMicroOrganisms = newShouldPlayMicroOrganisms;
    }

    private void OnShouldPlayMicroOrganismsChange(bool oldValue, bool newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
    
        if (newValue)
            Instances.AudioManager.PlayMicroOrganisms();
        else
            Instances.AudioManager.StopMicroOrganisms();
    }
    
    // private bool PlayMicroOrganismsAtThisSeat()
    // {
    //     Debug.Log($"PlayMicroOrganismsAtThisSeat: {Instances.SeatNumber} - {Instances.RowNumber}, circlesSize: {circlesSize}");
    //     
    //     return (Instances.SeatNumber <= circlesPos.x && Instances.SeatNumber >= circlesPos.x - circlesSize.x &&
    //             Instances.RowNumber >= circlesPos.y && Instances.RowNumber <= circlesPos.y + circlesSize.y);
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