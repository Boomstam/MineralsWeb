using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetworkedAppState : NetworkBehaviour
{
    [SyncVar (OnChange = nameof(OnChangeShowAuraText))] public bool showAuraText;
    [SyncVar (OnChange = nameof(OnAuraTextChange))] public string currentAuraNLText;
    [SyncVar] public string currentAuraENText;
    [SyncVar (OnChange = nameof(OnAppStateChange))] public AppState appState;
    // [SyncVar (OnChange = nameof(OnQuadrantsModeChange))] private bool quadrantMode;
    [SyncVar (OnChange = nameof(OnCenterModeChange))] private bool centerMode;
    [SyncVar (OnChange = nameof(OnCenterModeValChange))] private float centerModeVal;
    [SyncVar (OnChange = nameof(OnEffectSlidersOnChange))] public bool effectSlidersOn;
    [SyncVar (OnChange = nameof(OnCirclesPosChange))] private Vector2Int circlesPos = new Vector2Int(41, 3);
    [SyncVar (OnChange = nameof(OnShouldPlayCirclesChange))] public bool shouldPlayCircles;
    [SyncVar (OnChange = nameof(OnShouldPlayMicroOrganismsChange))] public bool shouldPlayMicroOrganisms;
    [SyncVar] public bool shouldPlayDelays;
    [SyncVar (OnChange = nameof(OnQuadrantSeatMinMaxChange))] public Vector2Int quadrantSeatMinMax;
    [SyncVar(OnChange = nameof(OnShouldPlayStaticAudioChange))] public bool shouldPlayStaticAudio;
    [SyncVar(OnChange = nameof(OnShouldPlayStaticVideoChange))] public bool shouldPlayStaticVideo;
    [SyncVar(OnChange = nameof(OnChangeTheEnd))] public bool theEnd;
    [SyncVar] public Vector2Int quadrantRowMinMax;
    [SerializeField] private Vector2Int circlesSize = new Vector2Int(9, 5);
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        Debug.Log($"Start Client NetworkedAppState");
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void ChangeShowAuraText(bool newShowAuraText)
    {
        showAuraText = newShowAuraText;
    }
    
    private void OnChangeShowAuraText(bool oldValue, bool newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Instances.WebGLClientUI.ToggleAuraText(newValue);
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void SetCurrentAuraTexts(string nlText, string enText)
    {
        currentAuraENText = enText;
        currentAuraNLText = nlText;
    }
    
    private void OnAuraTextChange(string oldValue, string newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Instances.WebGLClientUI.auraTextDisplay.SetText(currentAuraNLText, currentAuraENText);
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
        StopALlMicroOrganismSounds();
    }

    [ObserversRpc]
    private void StopALlMicroOrganismSounds()
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Instances.AudioManager.StopMicroOrganisms();
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void EnableQuadrantRanges(Vector2Int newSeatMinMax, Vector2Int newRowMinMax)
    {
        Debug.Log($"EnableQuadrantRanges: {newSeatMinMax}, {newRowMinMax}");
        
        quadrantSeatMinMax = newSeatMinMax;
        quadrantRowMinMax = newRowMinMax;

        // quadrantMode = true;
    }

    // private void OnQuadrantsModeChange(bool oldValue, bool newValue, bool asServer)
    // {
    //     if (Instances.BuildType != BuildType.Voting)
    //         return;
    //     
    //     Debug.Log($"OnQuadrantsModeChange: {newValue}");
    //     
    //     if(newValue)
    //         Instances.AudioManager.OnQuadrantsModeDisabled();
    //     else
    //         Instances.AudioManager.OnQuadrantsModeEnabled(quadrantSeatMinMax, quadrantRowMinMax);
    // }

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
            bool playCirlcesAtThisSeat = PlayCirclesAtThisSeat(); 
            
            if(playCirlcesAtThisSeat)
                Instances.AudioManager.circlePlayer.StartPlayback();
            else
                Instances.AudioManager.circlePlayer.StopPlayback();
            // Instances.AudioManager.ResetCirclesVolume();
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
        bool shouldPlay = (Instances.SeatNumber <= circlesPos.x && Instances.SeatNumber >= circlesPos.x - circlesSize.x &&
                Instances.RowNumber >= circlesPos.y && Instances.RowNumber <= circlesPos.y + circlesSize.y);
        
        Debug.Log($"PlayCirclesAtThisSeat: {shouldPlay}; {Instances.SeatNumber}, {Instances.RowNumber}, circlesPos: {circlesPos}, circlesSize: {circlesSize}");
        
        return shouldPlay;
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
        {
            bool playMicroOrganismsAtThisSeat = PlayMicroOrganismsAtThisSeat(); 
            
            if(playMicroOrganismsAtThisSeat)
                Instances.AudioManager.PlayMicroOrganisms();
            else
                Instances.AudioManager.StopMicroOrganisms();
        }
        else
            Instances.AudioManager.StopMicroOrganisms();
    }
    
    private void OnQuadrantSeatMinMaxChange(Vector2Int oldVal, Vector2Int newVal, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"OnQuadrantSeatMinMaxChange: {newVal}");
        
        if(shouldPlayMicroOrganisms == false)
        {
            Debug.Log($"Don't continue with quadrant check because shouldPlayMicroOrganisms == false");
            return;
        }
        
        this.RunDelayed(0.5f, () =>
        {
            bool playMicroOrganismsAtThisSeat = PlayMicroOrganismsAtThisSeat();
            
            if(playMicroOrganismsAtThisSeat)
                Instances.AudioManager.PlayMicroOrganisms();
            else
                Instances.AudioManager.StopMicroOrganisms();
                // Instances.AudioManager.StopMicroOrganismsAfterDelay();
        });
    }
    
    public bool PlayMicroOrganismsAtThisSeat()
    {
        bool shouldPlay = (Instances.SeatNumber >= quadrantSeatMinMax.x && Instances.SeatNumber <= quadrantSeatMinMax.y &&
                Instances.RowNumber >= quadrantRowMinMax.x && Instances.RowNumber <= quadrantRowMinMax.y);
        
        Debug.Log($"PlayMicroOrganismsAtThisSeat: {shouldPlay}; {Instances.SeatNumber}, {Instances.RowNumber};" +
                  $"seatMinMax: {quadrantSeatMinMax}, rowMinMax: {quadrantRowMinMax}");
        
        return shouldPlay;
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void ChangeShouldPlayStaticAudio(bool newShouldPlayStatic)
    {
        shouldPlayStaticAudio = newShouldPlayStatic;
    }
    
    private void OnShouldPlayStaticAudioChange(bool oldValue, bool newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        if (newValue)
            Instances.AudioManager.StartRandomStaticVoice();
        else
            Instances.AudioManager.StopRandomStaticVoice();
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void ChangeShouldPlayStaticVideo(bool newShouldPlayStaticVideo)
    {
        shouldPlayStaticVideo = newShouldPlayStaticVideo;
    }
    
    private void OnShouldPlayStaticVideoChange(bool oldValue, bool newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        if (newValue)
            Instances.WebGLClientUI.ShowStaticVideo();
        else
            Instances.WebGLClientUI.StopStaticVideo();
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void ChangeTheEnd(bool newTheEnd)
    {
        theEnd = newTheEnd;
    }
    
    private void OnChangeTheEnd(bool oldValue, bool newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Instances.WebGLClientUI.ToggleTheEnd(newValue);
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