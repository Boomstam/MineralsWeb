using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using UnityEngine;

public class NetworkedMonitor : NetworkBehaviour
{
    [SyncVar] public bool playCircles;
    [SyncVar] public float volume;
    [SyncVar] public bool shouldSpatialize;
    [SyncVar] public float leftRightBalance;
    [SyncVar] public int seatsPerRow = 4;
    [SyncVar] public float minDelayTime = 0.1f;
    [SyncVar] public float maxDelayTime = 2f;
    [SyncVar] public float delayIntervalLength = 3f;

    private AppState AppState => Instances.NetworkedAppState.appState; 
    
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
}
