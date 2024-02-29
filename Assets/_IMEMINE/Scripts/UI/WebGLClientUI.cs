using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class WebGLClientUI : UIWithConnection
{
    [SerializeField] private ColorOverlay colorOverlay;
    [SerializeField] private Slider slider;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private RawImage backgroundVideo;
    [SerializeField] private TMP_InputField seatNumberInputField;
    [SerializeField] private Button seatNumberConfirmButton;
    [SerializeField] private Button seatNumberDisplayButton;

    [SerializeField] private TMP_InputField oscMessageInput;
    [SerializeField] private Button sendOSCButton;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private float maxProgressBarRight = 527;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI contentText;

    public bool VotingModeCurrentlyOn =>
        backgroundImage.gameObject.activeSelf && 
        colorOverlay.gameObject.activeSelf == false;

    public void Start()
    {
        slider.onValueChanged.AsObservable()
            .Subscribe(sliderVal =>
            {
                Instances.AudioManager.EnableLowPassFilter(500 + (4000 * sliderVal));
                Instances.NetworkedVoting.SendVoteUpdate(sliderVal, Instances.SeatNumber);
            });

        seatNumberConfirmButton.onClick.AsObservable().Subscribe(_ =>
        {
            Instances.SeatNumber = int.Parse(seatNumberInputField.text);
            
            Debug.Log($"Seat Number now: {Instances.SeatNumber}");
            
            seatNumberDisplayButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Seat: {Instances.SeatNumber}";
            
            ToggleEnterSeatDialog(false);
        });
        seatNumberDisplayButton.onClick.AsObservable().Subscribe(_ => ToggleEnterSeatDialog(true));
    }

    public void ToggleColorOverlay(bool show)
    {
        colorOverlay.gameObject.SetActive(show);
        
        slider.gameObject.SetActive(false);
        
        Instances.AudioManager.StopPlayback();
        Instances.AudioManager.ResetAllFx();
        
        Instances.AudioManager.PlayClip(ClipType.Chapter5);
    }
    
    public void ToggleVotingMode(bool votingModeOn)
    {
        // colorOverlay.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(votingModeOn);
        backgroundVideo.gameObject.SetActive(votingModeOn == false);
        
        slider.gameObject.SetActive(votingModeOn);
        
        SetStatusText(votingModeOn ? $"Intensity Slider" : "");
        
        Instances.AudioManager.StopPlayback();
        Instances.AudioManager.ResetAllFx();
        
        if(votingModeOn)
        {
            Instances.AudioManager.PlayClip(ClipType.Chapter4);
        }
    }

    private void ToggleEnterSeatDialog(bool show)
    {
        seatNumberConfirmButton.gameObject.SetActive(show);
        seatNumberInputField.gameObject.SetActive(show);
        
        SetStatusText(show ? $"Enter Your Seat Number" : "");
    }

    
    private void SetStatusText(string text)
    {
        statusText.text = text;
    }
/*
    private void OnChoiceClick(int choice)
    {
        Debug.Log(choice);
        
        ToggleChoiceButtons(false);
        
        // Instances.PerformanceManager.MakeChoice(choice);
        
        SetContentText($"You have chosen {choice}.{System.Environment.NewLine}" +
                       $"Enjoy the rest of this chapter!");
        SetStatusText($"Choice made");
    }*/
    /*
    [Button]
    private void SetProgress(float percentage)
    {
        float right = (1 - percentage) * maxProgressBarRight;
        
        progressBar.SetRight(right);
    }

    public void SetChapterText(int chapter)
    {
        chapterText.text = $"Chapter {chapter}";
    }
    
    public void SetContentText(string text)
    {
        contentText.text = text;
    }*/
/*
    public void ToggleChoiceButtons(bool showChoiceButtons, int chapter = -1)
    {
        choiceButtons.ForEach((button, i) =>
        {
            button.gameObject.SetActive(showChoiceButtons);
            
            if(InstanceFinder.IsOffline == false && chapter != -1)
            {
                // The current chapter currentChapterSyncVar is not updated here yet when coming from an OnNextChapter call.
                // That's why the values are 1 less than they should be.
                if (chapter == 1 && i > 1)
                    button.gameObject.SetActive(false);
                if (chapter == 2 && i > 2)
                    button.gameObject.SetActive(false);
            }
        });
        
        contentText.gameObject.SetActive(showChoiceButtons == false);
    }*/
}
