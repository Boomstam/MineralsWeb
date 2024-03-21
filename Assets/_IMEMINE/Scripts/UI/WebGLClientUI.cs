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
    [SerializeField] private Slider voteSlider;
    [SerializeField] private Slider averageSlider;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite[] fadeSprites;
    [SerializeField] private ImageFader imageFader;
    [SerializeField] private RawImage backgroundVideo;
    [SerializeField] private TMP_InputField seatNumberInputField;
    [SerializeField] private Button seatNumberConfirmButton;
    [SerializeField] private Button seatNumberDisplayButton;
    [SerializeField] private Button seatNumberDecreaseButton;
    [SerializeField] private Button seatNumberIncreaseButton;
    [SerializeField] private TextMeshProUGUI seatNumberText;
    [SerializeField] private GameObject effectsSliders;
    [SerializeField] private Slider highLowSlider;
    [SerializeField] private Slider distortionSlider;

    [SerializeField] private TMP_InputField oscMessageInput;
    [SerializeField] private Button sendOSCButton;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private float maxProgressBarRight = 527;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI contentText;

    public bool PlayFadeClips { get; set; } = true;

    private const string seatPlayerPrefsKey = "Seat";

    public void Start()
    {
        voteSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal =>
            {
                // Instances.AudioManager.EnableLowPassFilter(300 + (7200 * sliderVal));
                Instances.AudioManager.SetFadeVal(1 - sliderVal);
                imageFader.SetFadeVal(1 - sliderVal);
                
                Instances.NetworkedVoting.SendVoteUpdate(sliderVal, Instances.SeatNumber);
            });
        
        seatNumberConfirmButton.onClick.AsObservable().Subscribe(_ =>
        {
            Instances.SeatNumber = int.Parse(seatNumberText.text);

            OnConfirmSeatNumber();
        });
        seatNumberDisplayButton.onClick.AsObservable().Subscribe(_ => ToggleEnterSeatDialog(true));
        
        seatNumberInputField.onSelect.AsObservable().Subscribe(_ =>
        {
            Debug.Log($"Select input field");
            TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        });
        seatNumberDecreaseButton.onClick.AsObservable().Subscribe(_ =>
        {
            int newSeatVal = Mathf.Max(1, int.Parse(seatNumberText.text) - 1);
            UpdateSeatVal(newSeatVal);
        });
        seatNumberIncreaseButton.onClick.AsObservable().Subscribe(_ =>
        {
            int newSeatVal = int.Parse(seatNumberText.text) + 1;
            UpdateSeatVal(newSeatVal);
        });
        
        if(PlayerPrefs.HasKey(seatPlayerPrefsKey))
        {
            int savedSeat = PlayerPrefs.GetInt(seatPlayerPrefsKey);
            Instances.SeatNumber = savedSeat; 
            
            UpdateSeatVal(savedSeat);
            
            OnConfirmSeatNumber();
        }
    }

    private void OnConfirmSeatNumber()
    {
        Debug.Log($"Seat Number now: {Instances.SeatNumber}");

        seatNumberDisplayButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Seat: {Instances.SeatNumber}";

        ToggleEnterSeatDialog(false);
    }

    private void UpdateSeatVal(int newVal)
    {
        PlayerPrefs.SetInt(seatPlayerPrefsKey, newVal);
        
        seatNumberText.text = newVal.ToString();
    }

    public void ToggleColorOverlay(bool show)
    {
        // this.RunDelayed(Instances.SeatNumber, () => colorOverlay.gameObject.SetActive(show));
        colorOverlay.StartDelay = Instances.SeatNumber;
        colorOverlay.gameObject.SetActive(true);
        
        voteSlider.gameObject.SetActive(false);
        
        Instances.AudioManager.StopAllPlayback();
        Instances.AudioManager.ResetAllFx();
        
        if (show)
        {
            Instances.AudioManager.PlayClip(ClipType.Chapter5);
            ToggleEnterSeatDialog(false);
        }
    }
    
    public void ToggleVotingMode(bool votingModeOn)
    {
        imageFader.DisplayFadeImages(fadeSprites);
        
        SetStatusText(votingModeOn ? $"Intensity" : "");
        
        Instances.AudioManager.StopAllPlayback();
        Instances.AudioManager.ResetAllFx();

        if (votingModeOn)
        {
            voteSlider.gameObject.SetActive(true);
            averageSlider.gameObject.SetActive(true);
            
            ToggleEnterSeatDialog(false);
            
            if(PlayFadeClips)
                Instances.AudioManager.PlayFadeClips(new [] { ClipType.MineralsA, ClipType.MineralsB, ClipType.MineralsC });
        }
        else
        {
            ToggleIntroductionMode();
        }
    }

    public void ToggleEffectSlidersMode()
    {
        DisableAllModes();
        
        effectsSliders.SetActive(true);
        colorOverlay.gameObject.SetActive(true);
    }
    
    public void ToggleIntroductionMode()
    {
        DisableAllModes();
        
        backgroundVideo.gameObject.SetActive(true);
    }

    private void DisableAllModes()
    {
        colorOverlay.gameObject.SetActive(false);
        backgroundVideo.gameObject.SetActive(false);
        
        voteSlider.gameObject.SetActive(false);
        averageSlider.gameObject.SetActive(false);
        
        Instances.AudioManager.StopAllPlayback();
        Instances.AudioManager.ResetAllFx();

        SetStatusText("");
        
        effectsSliders.SetActive(false);
    }
    
    public void SetVoteAverage(float voteAverage)
    {
        averageSlider.value = voteAverage;
    }
    
    private void ToggleEnterSeatDialog(bool show)
    {
        // seatNumberInputField.gameObject.SetActive(show);
        
        seatNumberConfirmButton.gameObject.SetActive(show);
        
        seatNumberDecreaseButton.gameObject.SetActive(show);
        seatNumberIncreaseButton.gameObject.SetActive(show);
        seatNumberText.gameObject.SetActive(show);
        
        SetStatusText(show ? $"Enter Your Seat Number" : "");
    }
    
    private void SetStatusText(string text)
    {
        statusText.text = text;
    }
    
    public void SetBlockVoting(bool blockVoting)
    {
        voteSlider.interactable = blockVoting == false;
    }
}
