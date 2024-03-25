using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetworkedVoting : NetworkBehaviour
{
    [SyncVar] public bool votingBlocked;
    [SyncVar] public ChoiceType currentChoice;
    
    [SyncVar] private float voteOffset;
    
    // Saved on the Monitor
    private Dictionary<int, float> votePerSeat = new Dictionary<int, float>();
    
    // Runs on the Monitor
    public void ResetVoting()
    {
        Debug.Log($"Reset Voting");
        
        votePerSeat = new Dictionary<int, float>();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void UpdateVoteOffset(float newVoteOffset)
    {
        voteOffset = newVoteOffset;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateVotingBlocked(bool blockVoting)
    {
        votingBlocked = blockVoting;
        
        SendBlockVotingToClients(blockVoting);
    }

    [ObserversRpc]
    private void SendBlockVotingToClients(bool blockVoting)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Instances.WebGLClientUI.SetBlockVoting(blockVoting);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SendVoteUpdate(float voteVal, int seatNumber, NetworkConnection conn = null)
    {
        if(votingBlocked)
            return;
        
        SendVoteUpdateToMonitor(voteVal, seatNumber);
    }

    [ObserversRpc] // Runs on the Monitor
    private void SendVoteUpdateToMonitor(float voteVal, int seatNumber)
    {
        if(Instances.BuildType != BuildType.Monitor)
            return;

        votePerSeat[seatNumber] = voteVal;

        float average = votePerSeat.Values.Average();
        
        OnVoteAverageUpdate(average);
    }

    [Client] // Runs on the Monitor
    public void OnVoteAverageUpdate(float voteAverage)
    {
        float offsettedAverage = Mathf.Min(1, voteAverage + voteOffset);
        
        Instances.MonitorUI.SetVoteAverage(voteAverage, offsettedAverage);
        
        Debug.Log($"voteAverage {voteAverage}, offsettedAverage {offsettedAverage}");
        
        ChoiceType choice = ChoiceType.B;
        
        if (offsettedAverage < Instances.MonitorUI.BThreshold)
            choice = ChoiceType.C;
        else if(offsettedAverage > Instances.MonitorUI.CThreshold)
            choice = ChoiceType.A;

        SetCurrentChoice(choice);
        Debug.Log($"CurrentChoice {currentChoice}");
        // Instances.MonitorUI.HighlightChoice(choice);
    }

    [ServerRpc (RequireOwnership = false)]
    private void SetCurrentChoice(ChoiceType choice)
    {
        currentChoice = choice;
    }

    [ServerRpc (RequireOwnership = false)]
    public void SendAverageToClientsViaServer(float voteAverage)
    {
        SendAverageToClients(voteAverage);
    }
    
    [ObserversRpc]
    private void SendAverageToClients(float voteAverage)
    {
        if (Instances.BuildType == BuildType.Voting)
            Instances.WebGLClientUI.SetVoteAverage(voteAverage);

        if(Instances.BuildType != BuildType.OSCClient)
            return;
        
        Debug.Log($"Will send message with average: {voteAverage}");
        
        Instances.OSCManager.SendOSCMessage("/minerals", $"{voteAverage:0.00}");
        Instances.OSCClientUI.SetMessage($"{voteAverage:0.00}");
    }

    [ServerRpc (RequireOwnership = false)]
    public void MuteSoundViaServer(bool playFadeClips)
    {
        MuteSoundOnClients(playFadeClips);
    }

    [ObserversRpc]
    private void MuteSoundOnClients(bool playFadeClips)
    {
        if (Instances.BuildType != BuildType.Voting)
            return;

        Instances.WebGLClientUI.PlayFadeClips = playFadeClips;
    }
}
