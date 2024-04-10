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
    [SerializeField] private GameObject waysOfWater;
    [SerializeField] private GameObject voteWarning;
    [SerializeField] private TextMeshProUGUI voteWarningText;
    [SerializeField] private GameObject votingHolder;
    [Header("Row Number Input")]
    [SerializeField] private TextMeshProUGUI rowTitleText;
    [SerializeField] private Button rowNumberIncrease10sButton;
    [SerializeField] private Button rowNumberDecrease10sButton;
    [SerializeField] private Button rowNumberIncrease1sButton;
    [SerializeField] private Button rowNumberDecrease1sButton;
    [SerializeField] private TextMeshProUGUI row10sNumberText;
    [SerializeField] private TextMeshProUGUI row1sNumberText;
    [SerializeField] private int maxRow;
    [Header("Seat Number Input")]
    [SerializeField] private TextMeshProUGUI seatTitleText;
    [SerializeField] private Button seatNumberIncrease10sButton;
    [SerializeField] private Button seatNumberDecrease10sButton;
    [SerializeField] private Button seatNumberIncrease1sButton;
    [SerializeField] private Button seatNumberDecrease1sButton;
    [SerializeField] private TextMeshProUGUI seat10sNumberText;
    [SerializeField] private TextMeshProUGUI seat1sNumberText;
    [SerializeField] private int maxSeat;
    [SerializeField] private GameObject seatInputHolder;
    [SerializeField] private Button seatConfirmButton;
    [SerializeField] private Button seatDisplayButton;
    [SerializeField] private Button languageButton;
    [Header("Sliders")]
    [SerializeField] private Slider voteSlider;
    [SerializeField] private Slider averageSlider;
    [SerializeField] private GameObject effectsSliders;
    [SerializeField] private Slider highLowSlider;
    [SerializeField] private Slider distortionSlider;
    [SerializeField] private Slider waysOfWaterSlider;
    [SerializeField] private Slider voteProgressBar;
    [Header("Other")]
    [SerializeField] private float maxProgressBarRight = 527;
    [SerializeField] private TextMeshProUGUI statusText;
    [Header("Deprecated?")]
    [SerializeField] private TMP_InputField oscMessageInput;
    [SerializeField] private Button sendOSCButton;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI contentText;

    public bool PlayFadeClips { get; set; } = true;

    private const string row10sKey = "Row 10s";
    private const string row1sKey = "Row 1s";
    private const string seat10sKey = "Seat 10s";
    private const string seat1sKey = "Seat 1s";

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
        waysOfWaterSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal =>
            {
                Debug.Log($"Set microOrganismsSlider: {sliderVal}");
                Instances.AudioManager.circlePlayer.SetFadeValue(sliderVal);
                Instances.AudioManager.delayPlayer.SetFadeValue(1 - sliderVal);
            });
        
        // Initial fade values
        Instances.AudioManager.doubleFader.SetFadeValHighLow(0.5f);
        Instances.AudioManager.doubleFader.SetFadeValDistortion(0.5f);
        Instances.AudioManager.circlePlayer.SetFadeValue(0.5f);
        Instances.AudioManager.delayPlayer.SetFadeValue(0.5f);
        
        seatConfirmButton.onClick.AsObservable().Subscribe(_ =>
        {
            OnConfirmSeatNumber();
        });
        seatDisplayButton.onClick.AsObservable().Subscribe(_ => ToggleEnterSeatDialog(true));
        languageButton.onClick.AsObservable().Subscribe(_ => ToggleLanguage());
        
        rowNumberIncrease10sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(true, SeatElement.Row10s); });
        rowNumberDecrease10sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(false, SeatElement.Row10s); });
        rowNumberIncrease1sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(true, SeatElement.Row1s); });
        rowNumberDecrease1sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(false, SeatElement.Row1s); });
        
        seatNumberIncrease10sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(true, SeatElement.Seat10s); });
        seatNumberDecrease10sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(false, SeatElement.Seat10s); });
        seatNumberIncrease1sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(true, SeatElement.Seat1s); });
        seatNumberDecrease1sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(false, SeatElement.Seat1s); });
        
        if(PlayerPrefs.HasKey(row10sKey))
        {
            int row10s = PlayerPrefs.GetInt(row10sKey);
            int row1s = PlayerPrefs.GetInt(row1sKey, 0);
            int seat10s = PlayerPrefs.GetInt(seat10sKey, 0);
            int seat1s = PlayerPrefs.GetInt(seat1sKey, 0);

            int row = (row10s * 10) + row1s;
            int seat = (seat10s * 10) + seat1s;
            
            SetRow(row);
            SetSeat(seat);
            
            row10sNumberText.text = row10s.ToString();
            row1sNumberText.text = row1s.ToString();
            
            seat10sNumberText.text = seat10s.ToString();
            seat1sNumberText.text = seat1s.ToString();

            DisplayCurrentSeatInSeatDisplayButton();
        }
        else
        {
            ToggleEnterSeatDialog(true);
        }
    }

    private void OnConfirmSeatNumber()
    {
        DisplayCurrentSeatInSeatDisplayButton();
        
        ToggleEnterSeatDialog(false);
    }

    private void DisplayCurrentSeatInSeatDisplayButton()
    {
        Instances.RowNumber = GetRow();
        Instances.SeatNumber = GetSeat();
        
        Debug.Log($"Seat now: {Instances.RowNumber} - {Instances.SeatNumber}");

        seatDisplayButton.GetComponentInChildren<TextMeshProUGUI>().text = $"R:{Instances.RowNumber} - S:{Instances.SeatNumber}";
    }
    
    private void UpdateSeatVal(bool increase, SeatElement seatElement)
    {
        bool isRow = seatElement is SeatElement.Row10s or SeatElement.Row1s;
        bool is10s = seatElement is SeatElement.Row10s or SeatElement.Seat10s;
        
        int currentVal = isRow ? GetRow() : GetSeat();

        int offset = increase ? 1 : -1;

        int newVal = currentVal + (offset * (is10s ? 10 : 1));

        int max = isRow ? maxRow : maxSeat;

        if (newVal <= 0)
            newVal = max;
        else if(newVal > max)
            newVal = 0;
        
        Debug.Log($"UpdateSeatVal, increase: {increase}, seatElement: {seatElement}, currentVal: {currentVal}, max: {max} newVal: {newVal}");
        
        if(isRow)
            SetRow(newVal);
        else
            SetSeat(newVal);

        Tuple<int, int> newValDigits = GetRowOrSeatDigits(newVal);

        if (isRow)
        {
            row10sNumberText.text = newValDigits.Item1.ToString();
            row1sNumberText.text = newValDigits.Item2.ToString();
        }
        else
        {
            seat10sNumberText.text = newValDigits.Item1.ToString();
            seat1sNumberText.text = newValDigits.Item2.ToString();
        }

        DisplayCurrentSeatInSeatDisplayButton();
    }

    private string KeyForSeatElement(SeatElement seatElement) => seatElement switch
    {
        SeatElement.Row10s => row10sKey,
        SeatElement.Row1s => row1sKey,
        SeatElement.Seat10s => seat10sKey,
        SeatElement.Seat1s => seat1sKey,
    };

    private int GetRow()
    {
        int row = (PlayerPrefs.GetInt(KeyForSeatElement(SeatElement.Row10s), 0) * 10)
               + PlayerPrefs.GetInt(KeyForSeatElement(SeatElement.Row1s), 0);

        Debug.Log($"Get row: {row}");
        
        return row;
    }

    private int GetSeat()
    {
        int seat =  (PlayerPrefs.GetInt(KeyForSeatElement(SeatElement.Seat10s), 0) * 10)
                   + PlayerPrefs.GetInt(KeyForSeatElement(SeatElement.Seat1s), 0);

        Debug.Log($"Get seat: {seat}");
        
        return seat;
    }

    private Tuple<int, int> GetRowOrSeatDigits(int rowOrSeat)
    {
        int rowOrSeat10s = Mathf.FloorToInt((float)rowOrSeat / 10f);
        int rowOrSeat1s = rowOrSeat % 10;
        
        Debug.Log($"GetRowOrSeatDigits: {rowOrSeat10s}, {rowOrSeat1s}");

        return new Tuple<int, int>(rowOrSeat10s, rowOrSeat1s);
    }

    private void SetRow(int row)
    {
        Debug.Log($"Set row: {row}");
        Instances.RowNumber = row;
        
        Tuple<int, int> rowDigits = GetRowOrSeatDigits(row);
        
        PlayerPrefs.SetInt(KeyForSeatElement(SeatElement.Row10s), rowDigits.Item1);
        PlayerPrefs.SetInt(KeyForSeatElement(SeatElement.Row1s), rowDigits.Item2);
    }

    private void SetSeat(int seat)
    {
        Debug.Log($"Set set: {seat}");
        Instances.SeatNumber = seat;
        
        Tuple<int, int> seatDigits = GetRowOrSeatDigits(seat);
        
        PlayerPrefs.SetInt(KeyForSeatElement(SeatElement.Seat10s), seatDigits.Item1);
        PlayerPrefs.SetInt(KeyForSeatElement(SeatElement.Seat1s), seatDigits.Item2);
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
            votingHolder.SetActive(true);
            
            ToggleEnterSeatDialog(false);
            
            // if(PlayFadeClips)
            //     Instances.AudioManager.PlayFadeSamples(new [] {  });
            
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

        if (PlayerPrefs.HasKey(row10sKey) == false)
            ToggleEnterSeatDialog(true);
    }
    
    public void EnableWaysOfWaterMode()
    {
        DisableAllModes();
        
        waysOfWater.SetActive(true);
    }

    private void DisableAllModes()
    {
        ToggleEnterSeatDialog(false);
        
        colorOverlay.gameObject.SetActive(false);
        backgroundVideo.gameObject.SetActive(false);
        
        waysOfWater.SetActive(false);
        
        // voteSlider.gameObject.SetActive(false);
        // averageSlider.gameObject.SetActive(false);
        votingHolder.SetActive(false);
        
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
        seatInputHolder.SetActive(show);
        
        SetStatusText(show ? $"Enter Your Seat Number" : "");
    }
    
    private void ToggleLanguage()
    {
        string languagePlayerPrefsKey = "SavedLanguage";
        Language language = (Language)PlayerPrefs.GetInt(languagePlayerPrefsKey, 0);

        languageButton.GetComponentInChildren<TextMeshProUGUI>().text = language.ToString();

        bool nl = (language == Language.NL);

        nl = !nl;
        
        SetLanguageInTextComponents(nl);

        int newLanguageVal = (nl ? 0 : 1);

        PlayerPrefs.SetInt(languagePlayerPrefsKey, newLanguageVal);
    }

    private void SetLanguageInTextComponents(bool nl)
    {
        rowTitleText.text = nl ? "Rij" : "Row";
        seatTitleText.text = nl ? "Stoel" : "Seat";

        seatConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = nl ? "Kies" : "Confirm";
    }
    
    private void SetStatusText(string text)
    {
        statusText.text = text;
    }
    
    public void SetBlockVoting(bool blockVoting)
    {
        voteSlider.interactable = blockVoting == false;
    }

    public void SetVotingProgress(float progress)
    {
        bool inProgress = (progress > 0);
        
        SetBlockVoting(inProgress == false);
        
        voteProgressBar.gameObject.SetActive(inProgress);

        if (inProgress)
        {
            voteProgressBar.value = progress;
        }
    }

    public void ShowStartVotingWarning()
    {
        voteWarning.SetActive(true);
        voteWarningText.text = "Vote!";
        
        this.RunDelayed(1.5f, () => voteWarning.SetActive(false));
    }

    public void ShowStopVotingWarning()
    {
        voteWarning.SetActive(true);
        voteWarningText.text = "Stop!";
        
        this.RunDelayed(1.5f, () => voteWarning.SetActive(false));
    }
}

public enum SeatElement
{
    Row10s,
    Row1s,
    Seat10s,
    Seat1s,
}

public enum Language
{
    NL,
    EN,
}