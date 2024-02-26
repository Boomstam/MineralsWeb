using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using Sirenix.OdinInspector;
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
    [SerializeField] private TMP_InputField chapterLenghtInput;
    
    [SerializeField] private int currentMeasure;

    private void Start()
    {
        startButton.onClick.AsObservable().Subscribe(_ => Instances.PerformanceManager.StartPerformance());
        stopButton.onClick.AsObservable().Subscribe(_ => Instances.PerformanceManager.StopPerformance());

        chapterLenghtInput.onSubmit.AsObservable().Subscribe(text =>
        {
            int val = int.Parse(text);
            Instances.PerformanceManager.SetChapterLength(val);
            Debug.Log($"Set chapter length to {val}");
        });
    }
    
    public void SetChapter(int chapter)
    {
        chapterText.text = $"Chapter: {chapter}";
    }
    
    public void SetTime(float time)
    {
        timeText.text = $"Running time: {(int)time}";
    }
    
    public void SetOSCConnections(int connections)
    {
        oscConnectionsText.text = $"OSC clients: {connections}";
    }
    
    public void SetClientConnections(int connections)
    {
        clientConnectionsText.text = $"Client connections: {connections}";
    }

    public void UpdateChoices(int choice1, int choice2, int choice3, int choice4)
    {
        float totalNumChoices = choice1 + choice2 + choice3 + choice4;
        
        float percentage1 = totalNumChoices == 0 ? 0 : choice1 / totalNumChoices * 100;
        float percentage2 = totalNumChoices == 0 ? 0 : choice2 / totalNumChoices * 100;
        float percentage3 = totalNumChoices == 0 ? 0 : choice3 / totalNumChoices * 100;
        float percentage4 = totalNumChoices == 0 ? 0 : choice4 / totalNumChoices * 100;
        
        SetChoiceText(1, choice1, percentage1);
        SetChoiceText(2, choice2, percentage2);
        SetChoiceText(3, choice3, percentage3);
        SetChoiceText(4, choice4, percentage4);
    }

    private void SetChoiceText(int choice, int numberOfChoices, float percentage)
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
        // Debug.Log($"choice {choice}, numberOfChoices {numberOfChoices}, percentage {percentage}, comp {textComp}");

        textComp.text = $"Choice {choice}: {numberOfChoices}, {percentage}%";
    }

    [Button]
    private void HighlightChoice(ChoiceType choiceType)
    {
        Debug.Log($"Highlight choice: {choiceType}");
    }

    [Button]
    private void IncrementMeasure()
    {
        currentMeasure++;
        
        FocusOnCurrentMeasure();
    }

    [Button]
    private void FocusOnCurrentMeasure()
    {
        Debug.Log($"Focus on current measure: {currentMeasure}");
    }
}
