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
    [SerializeField] private TMP_Dropdown appStateDropdown;
    [SerializeField] private Button sendChoiceButton;
    [SerializeField] private Button resetVotingButton;
    [SerializeField] private Button startVotingIntervalButton;
    [SerializeField] private TextMeshProUGUI oscConnectionsText;
    [SerializeField] private TextMeshProUGUI clientConnectionsText;
    
    [SerializeField] private Button previousAuraTextButton;
    [SerializeField] private Button nextAuraTextButton;
    
    [SerializeField] private TextMeshProUGUI voteAverageText;
    [SerializeField] private Slider voteAverageSlider;
    [SerializeField] private TextMeshProUGUI voteOverwriteText;
    [SerializeField] private Slider voteOverwriteSlider;
    [SerializeField] private TextMeshProUGUI BThresholdText;
    [SerializeField] private Slider BThresholdSlider;
    [SerializeField] private TextMeshProUGUI CThresholdText;
    [SerializeField] private Slider CThresholdSlider;

    [SerializeField] private Toggle votingModeToggle;
    [SerializeField] private Toggle blockVotingToggle;
    [SerializeField] private Toggle effectSlidersToggle;
    [SerializeField] private Toggle delaysToggle;
    [SerializeField] private Toggle microOrganismsToggle;
    [SerializeField] private Toggle circlesToggle;
    [SerializeField] private Toggle auraTextToggle;
    [SerializeField] private Toggle staticAudioToggle;
    [SerializeField] private Toggle staticVideoToggle;
    [SerializeField] private Toggle theEndToggle;
    
    [SerializeField] private Toggle centerModeToggle;
    [SerializeField] private Slider centerModeSlider;
    [SerializeField] private Button shiftLeftButton;
    [SerializeField] private Button shiftRightButton;
    [SerializeField] private Button shiftDownButton;
    [SerializeField] private Button shiftUpButton;
    [SerializeField] private Button cycleVotingTagsButton;
    [SerializeField] private TextMeshProUGUI cycleVotingTagsText;
    [SerializeField] private TextMeshProUGUI circlesPositionText;
    [SerializeField] private TMP_InputField warningTimeInputField;
    [SerializeField] private string[] votingHighTags;
    [SerializeField] private string[] votingLowTags;

    public float BThreshold => BThresholdSlider.value;
    public float CThreshold => CThresholdSlider.value;

    private int currentVotingTag;

    private void Start()
    {
        sendChoiceButton.onClick.AsObservable().Subscribe(_ => SendChoice());
        resetVotingButton.onClick.AsObservable().Subscribe(_ => Instances.NetworkedVoting.ResetVoting());
        startVotingIntervalButton.onClick.AsObservable().Subscribe(_ => Instances.NetworkedVoting.StartVotingInterval());
        
        voteAverageSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal => Instances.NetworkedVoting.OnVoteAverageUpdate(sliderVal));
        voteOverwriteSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal =>
            {
                voteOverwriteText.text = $"Vote overwrite: {sliderVal}";
                Instances.NetworkedVoting.SendAverageToClientsViaServer(sliderVal);
            });
        
        votingModeToggle.onValueChanged.AsObservable().Subscribe(votingModeOn => Instances.NetworkedVoting.UpdateVotingMode(votingModeOn));
        blockVotingToggle.onValueChanged.AsObservable().Subscribe(blockVoting => Instances.NetworkedVoting.UpdateVotingBlocked(blockVoting));
        
        effectSlidersToggle.onValueChanged.AsObservable().Subscribe(effectSlidersOn => Instances.NetworkedAppState.ChangeEffectSlidersOn(effectSlidersOn));
        delaysToggle.onValueChanged.AsObservable().Subscribe(playDelays => Instances.NetworkedAppState.ChangeShouldPlayDelays(playDelays));
        circlesToggle.onValueChanged.AsObservable().Subscribe(playCircles => Instances.NetworkedAppState.ChangeShouldPlayCircles(playCircles));
        microOrganismsToggle.onValueChanged.AsObservable().Subscribe(playMicroOrganisms => Instances.NetworkedAppState.ChangeShouldPlayMicroOrganisms(playMicroOrganisms));
        theEndToggle.onValueChanged.AsObservable().Subscribe(theEnd => Instances.NetworkedAppState.ChangeTheEnd(theEnd));
        
        auraTextToggle.onValueChanged.AsObservable().Subscribe(showAuraText => Instances.NetworkedAppState.ChangeShowAuraText(showAuraText));
        staticAudioToggle.onValueChanged.AsObservable().Subscribe(playStaticAudio => Instances.NetworkedAppState.ChangeShouldPlayStaticAudio(playStaticAudio));
        staticVideoToggle.onValueChanged.AsObservable().Subscribe(playStaticVideo => Instances.NetworkedAppState.ChangeShouldPlayStaticVideo(playStaticVideo));
        
        shiftLeftButton.onClick.AsObservable().Subscribe(_ => Instances.NetworkedAppState.ShiftCirclesPos(Vector2Int.right));
        shiftRightButton.onClick.AsObservable().Subscribe(_ => Instances.NetworkedAppState.ShiftCirclesPos(Vector2Int.left));
        shiftDownButton.onClick.AsObservable().Subscribe(_ => Instances.NetworkedAppState.ShiftCirclesPos(Vector2Int.down));
        shiftUpButton.onClick.AsObservable().Subscribe(_ => Instances.NetworkedAppState.ShiftCirclesPos(Vector2Int.up));
        
        previousAuraTextButton.onClick.AsObservable().Subscribe(_ => GoToNextAuraText(false));
        nextAuraTextButton.onClick.AsObservable().Subscribe(_ => GoToNextAuraText(true));
        
        cycleVotingTagsButton.onClick.AsObservable().Subscribe(_ => CycleVotingTags());
        
        BThresholdSlider.onValueChanged.AsObservable().Subscribe(sliderVal => BThresholdText.text = $"B Threshold: {sliderVal:0.00}");
        CThresholdSlider.onValueChanged.AsObservable().Subscribe(sliderVal => CThresholdText.text = $"C Threshold: {sliderVal:0.00}");
        
        centerModeToggle.onValueChanged.AsObservable().Subscribe(OnCenterModeToggled);
        centerModeSlider.onValueChanged.AsObservable().Subscribe(OnCenterModeSliderChanged);
        
        warningTimeInputField.onValueChanged.AsObservable()
            .Subscribe(val => Instances.NetworkedVoting.SetWarningTime(int.Parse(val)));
    }

    public void OnAppStateDropdownChanged(int newStateIndex)
    {
        AppState newState = (AppState)newStateIndex;
        
        Instances.NetworkedAppState.ChangeAppState(newState);
    }
    
    public void SetVoteAverage(float average)
    {
        voteAverageText.text = $"Vote Average: {average:0.00}";
        voteAverageSlider.SetValueWithoutNotify(average);
        
        // Instances.NetworkedVoting.SendAverageToClientsViaServer(average);
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
        
        float sliderVal = voteOverwriteSlider.value;
        
        ChoiceType choice = ChoiceType.B;
        
        if (sliderVal < Instances.MonitorUI.BThreshold)
            choice = ChoiceType.C;
        else if(sliderVal > Instances.MonitorUI.CThreshold)
            choice = ChoiceType.A;
        
        Debug.Log($"Send Choice {Instances.NetworkedVoting.currentChoice}");
        
        Instances.NetworkedVoting.ChangeChoiceAfterWarning(choice);
        
        // HighlightChoice(Instances.NetworkedVoting.currentChoice);
    }
    
    private void GoToNextAuraText(bool next)
    {
        Instances.NetworkedAppState.GoToNextAuraTextIndex(next);
    }
    
    private void CycleVotingTags()
    {
        currentVotingTag++;
        
        if (currentVotingTag >= votingHighTags.Length)
            currentVotingTag = 0;
        
        string highTag = votingHighTags[currentVotingTag];
        string lowTag = votingLowTags[currentVotingTag];
        
        cycleVotingTagsText.text = $"{highTag} - {lowTag}";
        
        Instances.NetworkedMonitor.SetVotingTags(highTag, lowTag);
    }
    
    private void OnCenterModeToggled(bool centerModeOn)
    {
        centerModeSlider.interactable = centerModeOn;
        
        
    }
    
    private void OnCenterModeSliderChanged(float centerModeSliderVal)
    {
        
    }

    public void SetCirclesPos(Vector2Int pos)
    {
        circlesPositionText.text = $"Circles: {pos.x}, {pos.y}";
    }
}
