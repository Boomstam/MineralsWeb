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
    [Header("Backgrounds/Overlays")]
    [SerializeField] private ColorOverlay colorOverlay;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite[] fadeSprites;
    [SerializeField] private ImageFader imageFader;
    [SerializeField] private RawImage backgroundVideo;
    [Header("Row Number Input")]
    [SerializeField] private Button rowNumberIncrease10sButton;
    [SerializeField] private Button rowNumberDecrease10sButton;
    [SerializeField] private Button rowNumberIncrease1sButton;
    [SerializeField] private Button rowNumberDecrease1sButton;
    [SerializeField] private TextMeshProUGUI row10sNumberText;
    [SerializeField] private TextMeshProUGUI row1sNumberText;
    [SerializeField] private float maxRow10s;
    [Header("Seat Number Input")]
    [SerializeField] private Button seatNumberIncrease10sButton;
    [SerializeField] private Button seatNumberDecrease10sButton;
    [SerializeField] private Button seatNumberIncrease1sButton;
    [SerializeField] private Button seatNumberDecrease1sButton;
    [SerializeField] private TextMeshProUGUI seat10sNumberText;
    [SerializeField] private TextMeshProUGUI seat1sNumberText;
    [SerializeField] private float maxSeat10s;
    [SerializeField] private Button seatConfirmButton;
    [SerializeField] private Button seatDisplayButton;
    [Header("Sliders")]
    [SerializeField] private Slider voteSlider;
    [SerializeField] private Slider averageSlider;
    [SerializeField] private GameObject effectsSliders;
    [SerializeField] private Slider highLowSlider;
    [SerializeField] private Slider distortionSlider;
    [Header("Other")]
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private float maxProgressBarRight = 527;
    [SerializeField] private TextMeshProUGUI statusText;
    [Header("Deprecated?")]
    [SerializeField] private TMP_InputField oscMessageInput;
    [SerializeField] private Button sendOSCButton;
    [SerializeField] private Button[] choiceButtons;
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
        
        highLowSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal => { Instances.AudioManager.doubleFader.SetFadeValHighLow(sliderVal); });
        distortionSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal => { Instances.AudioManager.doubleFader.SetFadeValDistortion(sliderVal); });
        
        seatConfirmButton.onClick.AsObservable().Subscribe(_ =>
        {
            Instances.SeatNumber = int.Parse(seat10sNumberText.text);

            OnConfirmSeatNumber();
        });
        seatDisplayButton.onClick.AsObservable().Subscribe(_ => ToggleEnterSeatDialog(true));
        
        seatNumberDecrease10sButton.onClick.AsObservable().Subscribe(_ =>
        {
            int newSeatVal = Mathf.Max(1, int.Parse(seat10sNumberText.text) - 1);
            UpdateSeatVal(newSeatVal);
        });
        seatNumberIncrease10sButton.onClick.AsObservable().Subscribe(_ =>
        {
            int newSeatVal = int.Parse(seat10sNumberText.text) + 1;
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

        seatDisplayButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Seat: {Instances.SeatNumber}";

        ToggleEnterSeatDialog(false);
    }

    private void UpdateSeatVal(int newVal)
    {
        PlayerPrefs.SetInt(seatPlayerPrefsKey, newVal);
        
        seat10sNumberText.text = newVal.ToString();
    }

    public void ToggleColorOverlay(bool show)
    {
        DisableAllModes();
            
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
        DisableAllModes();
        
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
                Instances.AudioManager.PlayFadeClips(new [] { ClipType.MineralsA, ClipType.MineralsB});
            //, ClipType.MineralsC });
        }
        else
        {
            EnableIntroductionMode();
        }
    }

    public void EnableEffectSlidersMode()
    {
        DisableAllModes();
        
        effectsSliders.SetActive(true);
        colorOverlay.gameObject.SetActive(true);
        
        Instances.AudioManager.doubleFader.PlayFadeSamples();
    }
    
    public void EnableIntroductionMode()
    {
        DisableAllModes();
        
        backgroundVideo.gameObject.SetActive(true);
    }

    private void DisableAllModes()
    {
        ToggleEnterSeatDialog(false);
        
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
        
        seatConfirmButton.gameObject.SetActive(show);
        
        seatNumberDecrease10sButton.gameObject.SetActive(show);
        seatNumberIncrease10sButton.gameObject.SetActive(show);
        seat10sNumberText.gameObject.SetActive(show);
        
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
