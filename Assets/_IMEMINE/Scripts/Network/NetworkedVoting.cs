using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using UnityEngine;

public class NetworkedVoting : NetworkBehaviour
{
    [SyncVar (OnChange = nameof(OnVotingBlockChanged))] public bool votingBlocked;
    [SyncVar (OnChange = nameof(OnVotingModeChanged))] public bool votingModeEnabled;
    [SyncVar (OnChange = nameof(OnChoiceChanged))] public ChoiceType currentChoice;
    [SyncVar] public int warningTime = 5;
    [SyncVar] private float voteOffset;
    [SerializeField] private float votingIntervalLength = 10;

    [SyncVar (OnChange = nameof(OnVoteProgressUpdate))] private float voteProgress;

    [SerializeField] private int maxNumSeats;
    [SerializeField] private int maxNumRows;
    

    // Saved on the Monitor
    private ChoiceType localCurrentChoice;
    private float[][] votePerSeat;

    private void Awake()
    {
        ResetVoting();
    }

    private bool hasStartedVoteInterval;

    [ServerRpc (RequireOwnership = false), Button]
    public void StartVotingInterval()
    {
        StartCoroutine(DoVotingInterval());
    }

    private IEnumerator DoVotingInterval()
    {
        float elapsedTime = 0;
        
        while (elapsedTime < votingIntervalLength)
        {
            float timeElapsedPercentage = elapsedTime / votingIntervalLength;

            voteProgress = timeElapsedPercentage;
            
            yield return 0;

            elapsedTime += Time.deltaTime;
        }

        voteProgress = 0;
    }

    private void OnVoteProgressUpdate(float prev, float next, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;

        if (prev <= 0 && next > 0)
        {
            Instances.WebGLClientUI.ShowStartVotingWarning();
        }
        if (prev > 0 && next <= 0)
        {
            Instances.WebGLClientUI.ShowStopVotingWarning();
        }
        Instances.WebGLClientUI.SetVotingProgress(next);
    }

    // Runs on the Monitor
    public void ResetVoting()
    {
        Debug.Log($"Reset Voting");
        votePerSeat = new float[maxNumSeats][];
        
        for (int i = 0; i < maxNumSeats; i++)
        {
            votePerSeat[i] = new float[maxNumRows];
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void UpdateVoteOffset(float newVoteOffset)
    {
        voteOffset = newVoteOffset;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void UpdateVotingMode(bool votingModeOn)
    {
        votingModeEnabled = votingModeOn;
    }
    
    private void OnVotingModeChanged(bool oldValue, bool newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Instances.WebGLClientUI.ToggleVotingMode(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateVotingBlocked(bool blockVoting)
    {
        votingBlocked = blockVoting;
    }
    
    private void OnVotingBlockChanged(bool oldValue, bool newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Instances.WebGLClientUI.SetBlockVoting(newValue);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SendVoteUpdate(float voteVal, int seat, int row, NetworkConnection conn = null)
    {
        if(votingBlocked)
            return;
        
        SendVoteUpdateToMonitor(voteVal, seat, row);
    }

    [ObserversRpc] // Runs on the Monitor
    private void SendVoteUpdateToMonitor(float voteVal, int seat, int row)
    {
        if(Instances.BuildType != BuildType.Monitor)
            return;

        votePerSeat[seat][row] = voteVal;

        List<float> vals = new List<float>();

        for (int seatToCheck = 0; seat < maxNumSeats; seat++)
        {
            for (int rowToCheck = 0; row < maxNumRows; row++)
            {
                float val = votePerSeat[seatToCheck][rowToCheck];

                if (val != 0)
                    vals.Add(val);
            }
        }

        float average = vals.Average();
        
        OnVoteAverageUpdate(average);
    }

    [Client] // Runs on the Monitor
    public void OnVoteAverageUpdate(float voteAverage)
    {
        float offsettedAverage = Mathf.Min(1, voteAverage + voteOffset);
        
        Instances.MonitorUI.SetVoteAverage(voteAverage);
        
        Debug.Log($"voteAverage {voteAverage}, offsettedAverage {offsettedAverage}");
        
        // ChoiceType choice = ChoiceType.B;
        //
        // if (offsettedAverage < Instances.MonitorUI.BThreshold)
        //     choice = ChoiceType.C;
        // else if(offsettedAverage > Instances.MonitorUI.CThreshold)
        //     choice = ChoiceType.A;
        //
        // localCurrentChoice = choice;
        // SetCurrentChoice(choice);
        Debug.Log($"CurrentChoice {currentChoice}");
        // Instances.MonitorUI.HighlightChoice(choice);
    }

    // [ServerRpc (RequireOwnership = false)]
    // private void SetCurrentChoice(ChoiceType choice)
    // {
    //     currentChoice = choice;
    // }

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
    
    [ServerRpc (RequireOwnership = false)]
    public void ChangeChoiceAfterWarning(ChoiceType choiceType)
    {
        ChoiceType choice = choiceType;
        
        this.RunDelayed(warningTime + 0.5f, () => currentChoice = choice);
        
        ChangeChoiceAfterWarningOnClients(choice);
    }

    [ObserversRpc]
    private void ChangeChoiceAfterWarningOnClients(ChoiceType choiceType)
    {
        if (Instances.BuildType != BuildType.Score)
            return;
        
        Instances.ScoreManager.StartHighlightChoiceRoutine(choiceType);
    }
    
    private void OnChoiceChanged(ChoiceType oldValue, ChoiceType newValue, bool asServer)
    {
        if(Instances.BuildType != BuildType.Score)
            return;
        
        Instances.ScoreManager.SetChoice(newValue);
    }

    [Button, ServerRpc(RequireOwnership = false)]
    public void SetWarningTime(int time)
    {
        warningTime = time;
    }
}
