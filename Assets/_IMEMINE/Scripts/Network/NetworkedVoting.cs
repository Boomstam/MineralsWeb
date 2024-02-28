using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class NetworkedVoting : NetworkBehaviour
{
    private Dictionary<int, float> votePerSeat = new Dictionary<int, float>();

    [ServerRpc(RequireOwnership = false)]
    public void SendVoteUpdate(float voteVal, int seatNumber, NetworkConnection conn = null)
    {
        SendVoteUpdateToMonitor(voteVal, seatNumber);
    }

    [ObserversRpc]
    private void SendVoteUpdateToMonitor(float voteVal, int seatNumber)
    {
        if(Instances.BuildType != BuildType.Monitor)
            return;

        votePerSeat[seatNumber] = voteVal;
        
        OnVoteAverageUpdate(votePerSeat.Values.Average());
    }

    private void OnVoteAverageUpdate(float voteAverage)
    {
        Instances.MonitorUI.SetVoteAverage(voteAverage);

        ChoiceType choice = ChoiceType.A;
        
        if (voteAverage is > 0.33f and < 0.66f)
            choice = ChoiceType.B;
        else if(voteAverage > 0.66f)
            choice = ChoiceType.C;
        
        Instances.MonitorUI.HighlightChoice(choice);
    }
}
