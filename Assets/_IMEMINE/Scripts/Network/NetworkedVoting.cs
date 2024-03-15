using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetworkedVoting : NetworkBehaviour
{
    private Dictionary<int, float> votePerSeat = new Dictionary<int, float>();

    [SyncVar] private float voteOffset;
    [SyncVar] private bool votingBlocked;

    [ServerRpc(RequireOwnership = false)]
    public void UpdateVoteOffset(float newVoteOffset)
    {
        voteOffset = newVoteOffset;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateVotingBlocked(bool blockVoting)
    {
        votingBlocked = blockVoting;
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
        float offsettedAverage = Mathf.Max(1, voteAverage + voteOffset);
        
        Instances.MonitorUI.SetVoteAverage(voteAverage, offsettedAverage);
        
        ChoiceType choice = ChoiceType.A;
        
        if (offsettedAverage < Instances.MonitorUI.BThreshold)
            choice = ChoiceType.B;
        else if(offsettedAverage > Instances.MonitorUI.CThreshold)
            choice = ChoiceType.C;
        
        Instances.MonitorUI.HighlightChoice(choice);
    }

    [ServerRpc (RequireOwnership = false)]
    public void SendAverageToOSCViaServer(float voteAverage)
    {
        SendAverageToOSCClient(voteAverage);
    }
    
    [ObserversRpc]
    private void SendAverageToOSCClient(float voteAverage)
    {
        if (Instances.BuildType == BuildType.Voting)
            Instances.WebGLClientUI.SetVoteAverage(voteAverage);

        if(Instances.BuildType != BuildType.OSCClient)
            return;
        
        Debug.Log($"Will send message with average: {voteAverage}");
        
        Instances.OSCManager.SendOSCMessage("/minerals", $"{voteAverage:0.00}");
        Instances.OSCClientUI.SetMessage($"{voteAverage:0.00}");
    }
}
