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
    [SerializeField] private Toggle colorOverlayToggle;
    [SerializeField] private Toggle votingModeToggle;
    [SerializeField] private Toggle blockVotingToggle;
    [SerializeField] private Button sendChoiceButton;
    [SerializeField] private TextMeshProUGUI resultVoteText;
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI oscConnectionsText;
    [SerializeField] private TextMeshProUGUI clientConnectionsText;
    [SerializeField] private TextMeshProUGUI choice1Text;
    [SerializeField] private TextMeshProUGUI choice2Text;
    [SerializeField] private TextMeshProUGUI choice3Text;
    [SerializeField] private TextMeshProUGUI choice4Text;
    [SerializeField] private TMP_InputField chapterLenghtInput;
    
    [SerializeField] private TextMeshProUGUI voteAverageText;
    [SerializeField] private Slider voteAverageSlider;
    [SerializeField] private TextMeshProUGUI voteOffsetText;
    [SerializeField] private Slider voteOffsetSlider;
    [SerializeField] private TextMeshProUGUI BThresholdText;
    [SerializeField] private Slider BThresholdSlider;
    [SerializeField] private TextMeshProUGUI CThresholdText;
    [SerializeField] private Slider CThresholdSlider;
    
    [SerializeField] private TMP_InputField measureInputField;
    [SerializeField] private Button sendMeasureButton;
    [SerializeField] private Button incrementMeasureButton;
    
    [SerializeField] private int currentMeasure;

    public float BThreshold => BThresholdSlider.value;
    public float CThreshold => CThresholdSlider.value;

    private void Start()
    {
        startButton.onClick.AsObservable().Subscribe(_ => Instances.PerformanceManager.StartPerformance());
        stopButton.onClick.AsObservable().Subscribe(_ => Instances.PerformanceManager.StopPerformance());
        
        colorOverlayToggle.onValueChanged.AsObservable()
            .Subscribe(toggleVal => Instances.NetworkedMonitor.ToggleColorOverlay(toggleVal));
        votingModeToggle.onValueChanged.AsObservable()
            .Subscribe(toggleVal => { Instances.NetworkedMonitor.ToggleVotingMode(toggleVal); });
        blockVotingToggle.onValueChanged.AsObservable()
            .Subscribe(ToggleBlockVoting);

        sendChoiceButton.onClick.AsObservable().Subscribe(_ => SendChoice());

        voteAverageSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal => Instances.NetworkedVoting.OnVoteAverageUpdate(sliderVal));
        voteOffsetSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal =>
            {
                voteOffsetText.text = $"Vote offset: {sliderVal}";
                Instances.NetworkedVoting.UpdateVoteOffset(sliderVal);
            });
        
        BThresholdSlider.onValueChanged.AsObservable().Subscribe(sliderVal => BThresholdText.text = $"B Threshold: {sliderVal:0.00}");
        CThresholdSlider.onValueChanged.AsObservable().Subscribe(sliderVal => CThresholdText.text = $"C Threshold: {sliderVal:0.00}");

        sendMeasureButton.onClick.AsObservable().Subscribe(_ =>
        {
            currentMeasure = int.Parse(measureInputField.text);
            
            FocusOnCurrentMeasure();
        });
        
        incrementMeasureButton.onClick.AsObservable().Subscribe(_ => IncrementMeasure());
    }

    public void SetVoteAverage(float average, float offsettedAverage)
    {
        voteAverageText.text = $"Vote Average: {average:0.00}";
        voteAverageSlider.SetValueWithoutNotify(average);

        Instances.NetworkedVoting.SendAverageToClientsViaServer(average);

        resultVoteText.text = $"Result vote: {average}";
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

    private void SendChoice()
    {
        Debug.Log($"Send Choice {Instances.NetworkedVoting.currentChoice}");
        
        HighlightChoice(Instances.NetworkedVoting.currentChoice);
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

    private void ToggleBlockVoting(bool blockVoting)
    {
        Instances.NetworkedVoting.UpdateVotingBlocked(blockVoting);
    }

    [Button]
    public void HighlightChoice(ChoiceType choiceType)
    {
        // Debug.Log($"Highlight choice: {choiceType}");
        
        Instances.MyMessageBroker.SendMessageToBuildType(BuildType.Score, $"HighlightChoice {(int)choiceType}");
    }

    [Button]
    private void IncrementMeasure()
    {
        currentMeasure++;
        
        FocusOnCurrentMeasure();
        
        measureInputField.SetTextWithoutNotify(currentMeasure.ToString());
    }

    [Button]
    private void FocusOnCurrentMeasure()
    {
        Debug.Log($"Focus on current measure: {currentMeasure}");

        Instances.MyMessageBroker.SendMessageToBuildType(BuildType.Score, $"GoToMeasure {currentMeasure}");
    }
}
