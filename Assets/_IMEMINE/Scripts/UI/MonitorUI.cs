using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MonitorUI : UIWithConnection
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI oscConnectionsText;
    [SerializeField] private TextMeshProUGUI clientConnectionsText;
    [SerializeField] private TextMeshProUGUI choice1Text;
    [SerializeField] private TextMeshProUGUI choice2Text;
    [SerializeField] private TextMeshProUGUI choice3Text;
    [SerializeField] private TextMeshProUGUI choice4Text;

    private void Start()
    {
        startButton.onClick.AsObservable().Subscribe(_ => Instances.PerformanceManager.StartPerformance());
        stopButton.onClick.AsObservable().Subscribe(_ => Instances.PerformanceManager.StopPerformance());
    }

    public void SetChapter(int chapter)
    {
        chapterText.text = $"Chapter: {chapter}";
    }
    
    public void SetTime(float time)
    {
        timeText.text = $"Running time: {time}";
    }
    
    public void SetOSCConnections(int connections)
    {
        oscConnectionsText.text = $"OSC clients: {connections}";
    }
    
    public void SetClientConnections(int connections)
    {
        clientConnectionsText.text = $"Client connections: {connections}";
    }

    public void SetChoiceText(int choice, int numberOfChoices, int percentage)
    {
        TextMeshProUGUI textComp;
        switch (choice)
        {
            case 1:
                textComp = choice1Text;
                break;
            case 2:
                textComp = choice2Text;
                break;
            case 3:
                textComp = choice3Text;
                break;
            case 4:
                textComp = choice4Text;
                break;
            default:
                throw new Exception($"No text component for choice {choice}");
        }

        textComp.text = $"Choice {choice}: {numberOfChoices}, {percentage}%";
    }
}
