using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class PerformanceManager : NetworkBehaviour
{
    [SerializeField] private int numChapters;
    [SerializeField] private float chapterLength;
    [SerializeField] private int timeToMakeChoice;

    [SyncVar, ReadOnly] public float runningTime;
    [SyncVar, ReadOnly] public int currentChapterSyncVar;

    private IDisposable timeUpdater;
    private IDisposable choiceCountdown;

    private float serverStartTime = 0;

    private ReactiveProperty<int> currentChapterReactiveProperty;

    private int localChoice;
    private int[] choices;

    [ServerRpc(RequireOwnership = false)]
    public void StartPerformance()
    {
        Debug.Log("StartPerformance");

        serverStartTime = Time.time;

        choiceCountdown?.Dispose();
        
        timeUpdater?.Dispose();
        timeUpdater = Observable.EveryUpdate().Subscribe(_ => OnTimeUpdate());

        currentChapterReactiveProperty = new();

        currentChapterReactiveProperty.Skip(1).Subscribe(OnNextChapter);

        ResetChoices();

        StartPerformanceOnClients();
    }

    [ObserversRpc]
    private void StartPerformanceOnClients()
    {
        if (Instances.BuildType == BuildType.WebGLClient)
        {
            Instances.WebGLClientUI.ToggleChoiceButtons(true);
            Instances.WebGLClientUI.SetStatusText($"Welcome to Minerals");
        }
    }
    
    [Server]
    private void OnTimeUpdate()
    {
        runningTime = Time.time - serverStartTime;

        int chapter = Mathf.CeilToInt(runningTime / chapterLength);

        if (chapter > numChapters)
        {
            StopPerformanceOnServer();
            return;
        }

        currentChapterReactiveProperty.Value = chapter;
        SendTimeAndChapterToMonitor(runningTime, chapter);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StopPerformance()
    {
        StopPerformanceOnServer();
        
        StopPerformanceOnClients();
    }
    
    [Server]
    private void StopPerformanceOnServer()
    {
        Debug.Log("StopPerformance");

        timeUpdater?.Dispose();
        choiceCountdown?.Dispose();
    }
    
    [ObserversRpc]
    private void StopPerformanceOnClients()
    {
        if (Instances.BuildType == BuildType.WebGLClient)
        {
            Instances.AudioManager.StopPlayback();
        }
    }

    [ObserversRpc]
    private void SendTimeAndChapterToMonitor(float time, int chapter)
    {
        if (Instances.BuildType != BuildType.Monitor)
            return;

        Instances.MonitorUI.SetTime(time);
        Instances.MonitorUI.SetChapter(chapter);
    }

    [Server]
    private void OnNextChapter(int chapter)
    {
        currentChapterSyncVar = chapter;
        Debug.Log($"Next chapter on server: {currentChapterSyncVar}");

        ResetChoices();
        
        NextChapterOnClients(chapter);
    }

    [ObserversRpc]
    private void NextChapterOnClients(int chapter)
    {
        Debug.Log($"Next chapter on clients: {chapter}");

        if (Instances.BuildType == BuildType.WebGLClient)
        {
            ClipType clipType = (ClipType)chapter;

            Instances.AudioManager.PlayClip(clipType);
            Instances.AudioManager.ResetAllFx();

            localChoice = -1;

            Instances.WebGLClientUI.ToggleChoiceButtons(true, chapter);
            Instances.WebGLClientUI.SetChapterText(chapter);
            Instances.WebGLClientUI.SetStatusText($"Make your choice for this chapter");

            choiceCountdown?.Dispose();
            choiceCountdown = Observable.FromCoroutine(DoChoiceCountdown).Subscribe();
        }
        if (Instances.BuildType == BuildType.OSCClient)
        {
            Instances.NetworkOSCManager.OnNextChapter(chapter);
        }
    }

    private IEnumerator DoChoiceCountdown()
    {
        for (int i = 0; i < timeToMakeChoice; i++)
        {
            int timeLeft = timeToMakeChoice - i;
            Instances.WebGLClientUI.SetStatusText($"Make your choice in {timeLeft} seconds");
            
            yield return new WaitForSeconds(1);
        }
        Instances.WebGLClientUI.SetStatusText($"No choice made");
        Instances.WebGLClientUI.ToggleChoiceButtons(false);
        Instances.WebGLClientUI.SetContentText($"Enjoy the rest of this chapter");
    }

    [Client]
    public void MakeChoice(int choice)
    {
        localChoice = choice;
        
        if(choice == 1)
            Instances.AudioManager.EnableReverb(AudioReverbPreset.User);
        if(choice == 2)
            Instances.AudioManager.EnableChorus();
        if(choice == 3)
            Instances.AudioManager.EnableLowPassFilter(1000);
        if(choice == 4)
            Instances.AudioManager.EnableDistortion(0.666f);

        SendChoiceToServer(choice);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChoiceToServer(int choice, NetworkConnection connection = null)
    {
        Debug.Log($"Received choice {choice} from client {connection.ClientId}");

        choices[choice - 1]++;
        
        SendChoicesToClients(choices[0], choices[1], choices[2], choices[3]);
    }

    [ObserversRpc]
    private void SendChoicesToClients(int choice1, int choice2, int choice3, int choice4)
    {
        if (Instances.BuildType == BuildType.Monitor)
        {
            Instances.MonitorUI.UpdateChoices(choice1, choice2, choice3, choice4);
        }
        if (Instances.BuildType == BuildType.OSCClient)
        {
            Instances.NetworkOSCManager.OnChoicesChanged(choice1, choice2, choice3, choice4);
        }
    }
    
    [Server]
    private void ResetChoices()
    {
        choices = new[] { 0, 0, 0, 0 };

        SendChoicesToClients(choices[0], choices[1], choices[2], choices[3]);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetChapterLength(int length)
    {
        Debug.Log($"Set chapter length to {length}");
        chapterLength = length;
    }
}
