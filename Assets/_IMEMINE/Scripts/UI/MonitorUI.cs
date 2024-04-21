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
    [SerializeField] private TMP_Dropdown appStateDropdowm;
    [SerializeField] private Button sendChoiceButton;
    [SerializeField] private Button resetVotingButton;
    [SerializeField] private TextMeshProUGUI resultVoteText;
    [SerializeField] private TextMeshProUGUI oscConnectionsText;
    [SerializeField] private TextMeshProUGUI clientConnectionsText;
    
    [SerializeField] private TextMeshProUGUI voteAverageText;
    [SerializeField] private Slider voteAverageSlider;
    [SerializeField] private TextMeshProUGUI voteOffsetText;
    [SerializeField] private Slider voteOffsetSlider;
    [SerializeField] private TextMeshProUGUI BThresholdText;
    [SerializeField] private Slider BThresholdSlider;
    [SerializeField] private TextMeshProUGUI CThresholdText;
    [SerializeField] private Slider CThresholdSlider;
    
    [SerializeField] private TMP_InputField warningTimeInputField;
    
    public float BThreshold => BThresholdSlider.value;
    public float CThreshold => CThresholdSlider.value;

    private void Start()
    {
        sendChoiceButton.onClick.AsObservable().Subscribe(_ => SendChoice());
        resetVotingButton.onClick.AsObservable().Subscribe(_ => Instances.NetworkedVoting.ResetVoting());

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

        warningTimeInputField.onValueChanged.AsObservable()
            .Subscribe(val => Instances.NetworkedMonitor.SetWarningTime(int.Parse(val)));
    }

    public void OnAppStateDropdownChanged(int newStateIndex)
    {
        AppState newState = (AppState)newStateIndex;
        
        Instances.NetworkedAppState.ChangeAppState(newState);
    }
    
    public void SetVoteAverage(float average, float offsettedAverage)
    {
        voteAverageText.text = $"Vote Average: {average:0.00}";
        voteAverageSlider.SetValueWithoutNotify(average);

        Instances.NetworkedVoting.SendAverageToClientsViaServer(average);

        resultVoteText.text = $"Result vote: {average}";
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
}
