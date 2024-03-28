using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using UnityEngine;

public class NetworkedMonitor : NetworkBehaviour
{
    [SyncVar] public AppState appState;

    [SyncVar] public int warningTime = 5;
    [SyncVar] public bool playCircles;
    [SyncVar] public float volume;
    [SyncVar] public bool shouldSpatialize;
    [SyncVar] public float leftRightBalance;
    [SyncVar] public int seatsPerRow = 4;
    [SyncVar] public bool shouldPlayDelays;
    [SyncVar] public float minDelayTime = 0.1f;
    [SyncVar] public float maxDelayTime = 2f;
    [SyncVar] public float delayIntervalLength = 3f;

    [ServerRpc (RequireOwnership = false), Button]
    public void SetPlayCircles(bool shouldPlayCircles)
    {
        playCircles = shouldPlayCircles;
    }
    
    [ServerRpc (RequireOwnership = false), Button]
    public void SetVolume(float volumeVal)
    {
        volume = volumeVal;
    }
    
    [ServerRpc (RequireOwnership = false), Button]
    public void SetSpatialize(bool spatialize)
    {
        shouldSpatialize = spatialize;
    }
    
    [ServerRpc (RequireOwnership = false), Button]
    public void SetLeftRightBalance(float leftRight)
    {
        leftRightBalance = leftRight;
    }

    [ServerRpc (RequireOwnership = false), Button]
    public void SetSeatsPerRow(int seats)
    {
        if(seats == 0)
            return;
        
        seatsPerRow = seats;
    }
    
    [ServerRpc (RequireOwnership = false), Button]
    public void SetPlayDelays(bool playDelays)
    {
        shouldPlayDelays = playDelays;
    }
    
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
    
    // TODO: Implement recency bias
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (Instances.BuildType == BuildType.Voting)
        {
            Debug.Log($"Start Client NetworkedMonitor with state {appState}");
            
            if(appState == AppState.Introduction)
                Instances.WebGLClientUI.EnableIntroductionMode();
            else if(appState == AppState.Voting)
                Instances.WebGLClientUI.ToggleVotingMode(true);
            else if(appState == AppState.ColorOverlay)
                Instances.WebGLClientUI.ToggleColorOverlay(true);
            else if(appState == AppState.EffectSliders)
                Instances.WebGLClientUI.EnableEffectSlidersMode();
            else if(appState == AppState.WaysOfWater)
                Instances.WebGLClientUI.EnableWaysOfWaterMode();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ToggleColorOverlay(bool show)
    {
        ToggleColorOverlayOnVotingClients(show);
        
        appState = AppState.ColorOverlay;
        
        appState = show ? AppState.Voting : AppState.Introduction;
    }

    [ObserversRpc]
    private void ToggleColorOverlayOnVotingClients(bool show)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"ToggleColorOverlayOnVotingClients: {show}");
        
        Instances.WebGLClientUI.ToggleColorOverlay(show);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ToggleVotingMode(bool votingModeOn)
    {
        ToggleVotingModeOnVotingClients(votingModeOn);

        appState = votingModeOn ? AppState.Voting : AppState.Introduction;
    }
    
    [ObserversRpc]
    private void ToggleVotingModeOnVotingClients(bool votingModeOn)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"ToggleColorOverlayOnVotingClients votingModeOn: {votingModeOn}");
        
        Instances.WebGLClientUI.ToggleVotingMode(votingModeOn);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void EnableIntroductionMode()
    {
        EnableIntroductionModeOnVotingClients();
        
        appState = AppState.Introduction;
    }
    
    [ObserversRpc]
    private void EnableIntroductionModeOnVotingClients()
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"ToggleIntroductionModeOnVotingClients");
        
        Instances.WebGLClientUI.EnableIntroductionMode();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void EnableEffectSlidersMode()
    {
        EnableEffectSlidersModeOnVotingClients();
        
        appState = AppState.EffectSliders;
    }
    
    [ObserversRpc]
    private void EnableEffectSlidersModeOnVotingClients()
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"ToggleEffectSlidersModeOnVotingClients");
        
        Instances.WebGLClientUI.EnableEffectSlidersMode();
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    public void EnableWaysOfWaterMode()
    {
        EnableWaysOfWaterModeOnVotingClients();
        
        appState = AppState.WaysOfWater;
    }
    
    [ObserversRpc]
    private void EnableWaysOfWaterModeOnVotingClients()
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"EnableWaysOfWaterModeOnVotingClients");
        
        Instances.WebGLClientUI.EnableWaysOfWaterMode();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ToggleSound1(bool sound1)
    {
        ToggleSound1OnVotingClients(sound1);
    }
    
    [ObserversRpc]
    private void ToggleSound1OnVotingClients(bool sound1)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"ToggleSound1OnVotingClients: {sound1}");

        Instances.AudioManager.doubleFader.sound1 = sound1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetWarningTime(int time)
    {
        warningTime = time;
    }
}

public enum AppState
{
    Introduction,
    Voting,
    ColorOverlay,
    EffectSliders,
    WaysOfWater,
}