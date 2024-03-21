using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetworkedMonitor : NetworkBehaviour
{
    [SyncVar] public AppState appState;
    
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (Instances.BuildType == BuildType.Voting)
        {
            Debug.Log($"Start Client NetworkedMonitor");
            
            if(appState == AppState.Introduction)
                Instances.WebGLClientUI.EnableIntroductionMode();
            else if(appState == AppState.Voting)
                Instances.WebGLClientUI.EnableIntroductionMode();
            else if(appState == AppState.ColorOverlay)
                Instances.WebGLClientUI.ToggleColorOverlay(true);
            else if(appState == AppState.EffectSliders)
                Instances.WebGLClientUI.EnableEffectSlidersMode();
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
    // FOOOOOOOOOOOOOOORk
    // 1 trigger these
        /* 2 import samples
         * 3 connect sliders and use big enum to trigger samples
         * 4 implement recency bias for voting => drop if too little time
         * 5 clear voting sessions => drop if too little time
         * 6 Test clear number of clients 
         * build? 
         *
         * GODspeed future Menno
         * 
         * check webclient ui on build!
         *
         * 
         */

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
}

public enum AppState
{
    Introduction,
    Voting,
    ColorOverlay,
    EffectSliders,
}