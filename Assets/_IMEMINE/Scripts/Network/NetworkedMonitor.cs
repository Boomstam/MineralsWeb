using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class NetworkedMonitor : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void ToggleColorOverlay(bool show)
    {
        ToggleColorOverlayOnVotingClients(show);
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
    }

    [ObserversRpc]
    private void ToggleVotingModeOnVotingClients(bool votingModeOn)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Debug.Log($"ToggleColorOverlayOnVotingClients votingModeOn: {votingModeOn}");
        
        Instances.WebGLClientUI.ToggleVotingMode(votingModeOn);
    }
}
