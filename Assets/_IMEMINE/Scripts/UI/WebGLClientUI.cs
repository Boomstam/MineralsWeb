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
    #region Variables

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
    [SerializeField] private TextMeshProUGUI highText;
    [SerializeField] private TextMeshProUGUI lowText;
    [SerializeField] private Slider distortionSlider;
    [SerializeField] private TextMeshProUGUI hardText;
    [SerializeField] private TextMeshProUGUI softText;
    [SerializeField] private Slider waysOfWaterSlider;
    [SerializeField] private Slider voteProgressBar;
    [SerializeField] private GameObject tutorialSliders;
    [SerializeField] private Slider tutorialHighLowSlider;
    [SerializeField] private TextMeshProUGUI tutorialVoteHighText;
    [SerializeField] private TextMeshProUGUI tutorialVoteLowText;
    [SerializeField] private TextMeshProUGUI tutorialHighText;
    [SerializeField] private TextMeshProUGUI tutorialLowText;
    [SerializeField] private Slider tutorialDistortionSlider;
    [SerializeField] private TextMeshProUGUI tutorialHardText;
    [SerializeField] private TextMeshProUGUI tutorialSoftText;
    [SerializeField] private GameObject tutorialVoting;
    [SerializeField] private Slider tutorialVoteSlider;
    [SerializeField] private Slider tutorialAverageSlider;
    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TypeAnimation tutorialTextAnimator;
    [SerializeField] private float nextPartButtonAnimationSpeed = 0.3f;
    [SerializeField] private Button previousTutorialPartButton;
    [SerializeField] private Button nextTutorialPartButton;
    [SerializeField] private AuraText[] tutorialTexts;
    [SerializeField] private float highlightBlinkSpeed;
    [SerializeField] private Image seatButtonBackground;
    [SerializeField] private Image languageButtonBackground;
    [SerializeField] private TextMeshProUGUI tutorialPageText;
    [Header("Modes")]
    [SerializeField] private GameObject introductionCanvas;
    [Header("Other")]
    public AuraTextDisplay auraTextDisplay;
    [SerializeField] private float maxProgressBarRight = 527;
    [Header("Deprecated? Or for testing?")]
    [SerializeField] private TMP_InputField oscMessageInput;
    [SerializeField] private Button sendOSCButton;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI contentText;
    
    public bool PlayFadeClips { get; set; } = true;
    public ReactiveProperty<Language> currentLanguage;
    
    private bool SeatInputActive => seatInputHolder.activeSelf;

    private const string row10sKey = "Row 10s";
    private const string row1sKey = "Row 1s";
    private const string seat10sKey = "Seat 10s";
    private const string seat1sKey = "Seat 1s";

    private TutorialPartType currentTutorialPart;

    private float seatButtonBackgroundStartAlpha;
    private float languageButtonBackgroundStartAlpha;
    private float connectionImageStartAlpha;
    
    private Coroutine seatButtonBackgroundBlinkRoutine;
    private Coroutine languageButtonBackgroundBlinkRoutine;
    private Coroutine connectionImageBlinkRoutine;

    private float nextButtonOnPressAnimationStart;
    private Coroutine nextButtonOnPressAnimationRoutine;
    
    private string languagePlayerPrefsKey = "SavedLanguage";

    #endregion
    
    #region Setup
    
    public void Start()
    {
        seatButtonBackgroundStartAlpha = seatButtonBackground.color.a;
        languageButtonBackgroundStartAlpha = languageButtonBackground.color.a;
        connectionImageStartAlpha = connectionImage.color.a;
        
        voteSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal =>
            {
                // Instances.AudioManager.EnableLowPassFilter(300 + (7200 * sliderVal));
                Instances.AudioManager.SetFadeVal(1 - sliderVal);
                imageFader.SetFadeVal(1 - sliderVal);
                
                Instances.NetworkedVoting.SendVoteUpdate(sliderVal, Instances.SeatNumber);
            });
        tutorialVoteSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal =>
            {
                // Instances.AudioManager.EnableLowPassFilter(300 + (7200 * sliderVal));
                // Instances.AudioManager.SetFadeVal(1 - sliderVal);
                imageFader.SetFadeVal(1 - sliderVal);
                
                // Instances.NetworkedVoting.SendVoteUpdate(sliderVal, Instances.SeatNumber);
            });
        
        highLowSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal => { Instances.AudioManager.doubleFader.SetFadeValHighLow(sliderVal); });
        distortionSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal => { Instances.AudioManager.doubleFader.SetFadeValDistortion(sliderVal); });
        
        tutorialHighLowSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal => { Instances.AudioManager.tutorialDoubleFader.SetFadeValHighLow(sliderVal); });
        tutorialDistortionSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal => { Instances.AudioManager.tutorialDoubleFader.SetFadeValDistortion(sliderVal); });
        
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
        seatDisplayButton.onClick.AsObservable().Subscribe(_ => ToggleEnterSeatDialog());
        languageButton.onClick.AsObservable().Subscribe(_ => ToggleLanguage());
        
        rowNumberIncrease10sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(true, SeatElement.Row10s); });
        rowNumberDecrease10sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(false, SeatElement.Row10s); });
        rowNumberIncrease1sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(true, SeatElement.Row1s); });
        rowNumberDecrease1sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(false, SeatElement.Row1s); });
        
        seatNumberIncrease10sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(true, SeatElement.Seat10s); });
        seatNumberDecrease10sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(false, SeatElement.Seat10s); });
        seatNumberIncrease1sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(true, SeatElement.Seat1s); });
        seatNumberDecrease1sButton.onClick.AsObservable().Subscribe(_ => { UpdateSeatVal(false, SeatElement.Seat1s); });
        
        previousTutorialPartButton.onClick.AsObservable().Subscribe(_ => GoToNextTutorialPart(false));
        nextTutorialPartButton.onClick.AsObservable().Subscribe(_ => GoToNextTutorialPart(true));
        
        if(PlayerPrefs.HasKey(row10sKey))
        {
            int row10s = PlayerPrefs.GetInt(row10sKey);
            int row1s = PlayerPrefs.GetInt(row1sKey, 1);
            int seat10s = PlayerPrefs.GetInt(seat10sKey, 0);
            int seat1s = PlayerPrefs.GetInt(seat1sKey, 1);
            
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
        // else
        // {
        //     ShowEnterSeatDialog(true);
        // }
        if (PlayerPrefs.HasKey(languagePlayerPrefsKey))
        {
            if (PlayerPrefs.GetInt(languagePlayerPrefsKey) == 1)
            {
                // Giggity
                ToggleLanguage(true);
                ToggleLanguage(true);
            }
        }
        
        ToggleTutorial(true);
    }
    
    #endregion
    
    #region Seat Input

    private void OnConfirmSeatNumber()
    {
        DisplayCurrentSeatInSeatDisplayButton();
        
        ToggleEnterSeatDialog();
        // ShowEnterSeatDialog(false);
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
            newVal = 1;

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

        Image buttonImage = ButtonForSeatElement(increase, seatElement).GetComponent<Image>();

        if(nextButtonOnPressAnimationRoutine != null)
            StopCoroutine(nextButtonOnPressAnimationRoutine);
        
        nextButtonOnPressAnimationRoutine = StartCoroutine(DoOnImagePressAnimation(buttonImage));
    }

    private string KeyForSeatElement(SeatElement seatElement) => seatElement switch
    {
        SeatElement.Row10s => row10sKey,
        SeatElement.Row1s => row1sKey,
        SeatElement.Seat10s => seat10sKey,
        SeatElement.Seat1s => seat1sKey,
    };
    
    private Button ButtonForSeatElement(bool increase, SeatElement seatElement) => seatElement switch
    {
        SeatElement.Row10s => increase ? rowNumberIncrease10sButton : rowNumberDecrease10sButton,
        SeatElement.Row1s => increase ? rowNumberIncrease1sButton : rowNumberDecrease1sButton,
        SeatElement.Seat10s => increase ? seatNumberIncrease10sButton : seatNumberDecrease10sButton,
        SeatElement.Seat1s => increase ? seatNumberIncrease1sButton : seatNumberDecrease1sButton,
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

    #endregion

    #region Modes

    public void SetToAppState(AppState appState)
    {
        switch (appState)
        {
            case AppState.Tutorial: ToggleTutorial(true);
                break;
            case AppState.Introduction: EnableIntroductionMode();
                break;
            case AppState.MicroOrganisms: EnableMicroOrganismsMode();
                break;
        }
    }

    private void ToggleColorOverlayVisual(bool show)
    {
        colorOverlay.StartDelay = 0;
        colorOverlay.gameObject.SetActive(show);
    }
    
    [Button]
    public void ToggleVotingMode(bool votingModeOn)
    {
        DisableAllModes();
        
        imageFader.DisplayFadeImages(fadeSprites);
        
        Instances.AudioManager.StopAllPlayback();
        Instances.AudioManager.ResetAllFx();
        
        if (votingModeOn)
        {
            voteSlider.gameObject.SetActive(true);
            averageSlider.gameObject.SetActive(true);
            votingHolder.SetActive(true);
            
            ShowEnterSeatDialog(false);
            
            // if(PlayFadeClips)
            //     Instances.AudioManager.PlayFadeSamples(new [] {  });
            
            //, ClipType.MineralsC });
        }
    }

    [Button]
    public void EnableEffectSlidersMode()
    {
        DisableAllModes();
        
        effectsSliders.SetActive(true);
        colorOverlay.gameObject.SetActive(true);
        
        Instances.AudioManager.doubleFader.PlayFadeSamples();
    }
    
    [Button]
    public void EnableTutorialSlidersMode()
    {
        DisableAllModes();
        
        tutorialSliders.SetActive(true);
        colorOverlay.gameObject.SetActive(true);
        
        Instances.AudioManager.tutorialDoubleFader.PlayFadeSamples();
    }
    
    [Button]
    public void EnableIntroductionMode()
    {
        DisableAllModes();
        
        // backgroundVideo.gameObject.SetActive(true);
        
    }
    
    
    [Button]
    public void EnableMicroOrganismsMode()
    {
        DisableAllModes();
        
        // backgroundVideo.gameObject.SetActive(true);
        
    }
    
    [Button]
    public void EnableWaysOfWaterMode()
    {
        DisableAllModes();
        
        waysOfWater.SetActive(true);
    }
    
    [Button]
    public void EnableTutorialVotingMode()
    {
        DisableAllModes();
        
        imageFader.DisplayFadeImages(fadeSprites);
        
        Instances.AudioManager.StopAllPlayback();
        Instances.AudioManager.ResetAllFx();
        
        // tutorialVoteSlider.gameObject.SetActive(true);
        // tutorialAverageSlider.gameObject.SetActive(true);
        tutorialVoting.SetActive(true);
    }

    [Button]
    private void DisableAllModes()
    {
        ShowEnterSeatDialog(false);
        
        StopBlinkAnimations();
        
        colorOverlay.gameObject.SetActive(false);
        backgroundVideo.gameObject.SetActive(false);
        
        waysOfWater.SetActive(false);
        
        // voteSlider.gameObject.SetActive(false);
        // averageSlider.gameObject.SetActive(false);
        votingHolder.SetActive(false);
        tutorialVoting.SetActive(false);
        
        Instances.AudioManager.StopAllPlayback();
        Instances.AudioManager.ResetAllFx();
        
        effectsSliders.SetActive(false);
        tutorialSliders.SetActive(false);
    }
    
    #endregion
    
    #region Language
    
    private void ToggleLanguage(bool isInitVersion = false)
    {
        Language language = (Language)PlayerPrefs.GetInt(languagePlayerPrefsKey, 0);
        
        bool nl = (language == Language.NL);
        
        nl = !nl;
        
        currentLanguage.Value = nl ? Language.NL : Language.EN;
        
        SetLanguageInTextComponents(nl);
        
        int newLanguageVal = (nl ? 0 : 1);
        
        PlayerPrefs.SetInt(languagePlayerPrefsKey, newLanguageVal);
        //
        // if(isInitVersion)
        //     return;
        //
        // if (SeatInputActive)
        // {
        //     return;
        // }
        
        // if (InstanceFinder.IsOffline == false)
        // {
        //     // if(Instances.NetworkedAppState.tutorial)
        //     //     SetTutorialPart(currentTutorialPart);
        //     
        //     
        //     
        //     //TODO Enable current mode
        //     // else
        //     //     
        // }
        // else
        // {
        //     SetTutorialPart(currentTutorialPart);
        // }
    }
    
    private void SetLanguageInTextComponents(bool nl)
    {
        languageButton.GetComponentInChildren<TextMeshProUGUI>().text = (nl ? Language.NL : Language.EN).ToString();
        
        rowTitleText.text = nl ? "RIJ" : "ROW";
        seatTitleText.text = nl ? "STOEL" : "SEAT";
        
        seatConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = nl ? "KIES" : "CONFIRM";
        
        highText.text = nl ? "HOOG" : "HIGH";
        tutorialHighText.text = nl ? "HOOG" : "HIGH";
        tutorialVoteHighText.text = nl ? "HOOG" : "HIGH";
        lowText.text = nl ? "LAAG" : "LOW";
        tutorialLowText.text = nl ? "LAAG" : "LOW";
        tutorialVoteLowText.text = nl ? "LAAG" : "LOW";
        
        hardText.text = nl ? "HARD" : "HARSH";
        tutorialHardText.text = nl ? "HARD" : "HARSH";
        softText.text = nl ? "ZACHT" : "SOFT";
        tutorialSoftText.text = nl ? "ZACHT" : "SOFT";
    }
    
    #endregion
    
    #region Voting Control
    
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
    
    #endregion
    
    #region Tutorial
    
    [Button]
    public void ToggleTutorial(bool tutorial, bool disableAllModes = true)
    {
        Debug.Log($"<color=green>Toggle tutorial {tutorial}, disableAllModes {disableAllModes}</color>");
        
        tutorialCanvas.SetActive(tutorial);
        
        if(disableAllModes)
            DisableAllModes();
        
        if(tutorial)
        {
            ToggleColorOverlayVisual(true);
            
            SetTutorialPart(currentTutorialPart);
        }
    }

    private void GoToNextTutorialPart(bool next)
    {
        int currentTutorialPartIndex = (int)currentTutorialPart;

        int lastIndex = (int)TutorialPartType.Enjoy;
        int newIndex = currentTutorialPartIndex + (next ? 1 : -1);

        if(nextButtonOnPressAnimationRoutine != null)
            StopCoroutine(nextButtonOnPressAnimationRoutine);
            
        
        Image image = next ? 
            nextTutorialPartButton.GetComponent<Image>() : 
            previousTutorialPartButton.GetComponent<Image>();
        
        nextButtonOnPressAnimationRoutine = StartCoroutine(DoOnImagePressAnimation(image));

        SetTutorialPart((TutorialPartType)newIndex);
    }

    private IEnumerator DoOnImagePressAnimation(Image image)
    {
        nextButtonOnPressAnimationStart = Time.time;

        float alpha = 1;
        
        while (Time.time - nextButtonOnPressAnimationStart < nextPartButtonAnimationSpeed)
        {
            float percentage = (Time.time - nextButtonOnPressAnimationStart) / nextPartButtonAnimationSpeed;
            alpha = 0.5f + (0.5f * percentage);
            SetAlphaOfImage(image, alpha);
            yield return 0;
        }
        // while (Time.time - nextButtonOnPressAnimationStart < nextPartButtonAnimationSpeed * 2)
        // {
        //     float percentage = (Time.time - nextButtonOnPressAnimationStart - nextPartButtonAnimationSpeed) / nextPartButtonAnimationSpeed;
        //     alpha = 1 - (0.5f * percentage);
        //     SetNextTutorialPartButtonAlpha(next, alpha);
        //     yield return 0;
        // }
    }

    [Button]
    private void SetTutorialPart(TutorialPartType tutorialPartType)
    {
        DisableAllModes();
        
        currentTutorialPart = tutorialPartType;
        
        tutorialText.text = tutorialPartType.ToString();
        tutorialPageText.text = $"{(int)tutorialPartType + 1} / {(int)TutorialPartType.Enjoy + 1}";
        
        Debug.Log($"Set tutorial part: {tutorialPartType}");
        
        StopBlinkAnimations();

        previousTutorialPartButton.interactable = true;
        nextTutorialPartButton.interactable = true;
        
        if (tutorialPartType == TutorialPartType.Welcome)
        {
            previousTutorialPartButton.interactable = false;
        }
        if (tutorialPartType == TutorialPartType.Enjoy)
        {
            nextTutorialPartButton.interactable = false;
        }
        
        switch (currentTutorialPart)
        {
            case TutorialPartType.Welcome:
                ShowTutorialText(0);
                break;
            case TutorialPartType.Seat:
                seatButtonBackgroundBlinkRoutine = StartCoroutine(DoBlinkAnimation(seatButtonBackground));
                ShowTutorialText(1);
                break;
            case TutorialPartType.Language:
                languageButtonBackgroundBlinkRoutine = StartCoroutine(DoBlinkAnimation(languageButtonBackground));
                ShowTutorialText(2);
                break;
            case TutorialPartType.Connection:
                connectionImageBlinkRoutine = StartCoroutine(DoBlinkAnimation(connectionImage));
                ShowTutorialText(3);
                break;
            case TutorialPartType.SlidersExplanation:
                ShowTutorialText(4);
                break;
            case TutorialPartType.Slider:
                tutorialText.gameObject.SetActive(false);
                tutorialAverageSlider.transform.SetSiblingIndex(0);
                tutorialVoteSlider.interactable = true;
                tutorialAverageSlider.interactable = false;
                EnableTutorialVotingMode();
                break;
            case TutorialPartType.MajorityExplanation:
                ShowTutorialText(5);
                break;
            case TutorialPartType.Majority:
                tutorialText.gameObject.SetActive(false);
                tutorialVoteSlider.transform.SetSiblingIndex(0);
                tutorialVoteSlider.interactable = false;
                tutorialAverageSlider.interactable = false;
                EnableTutorialVotingMode();
                break;
            case TutorialPartType.AudioExplanation:
                ShowTutorialText(6);
                break;
            case TutorialPartType.Audio:
                tutorialText.gameObject.SetActive(false);
                EnableTutorialSlidersMode();
                break;
            case TutorialPartType.Enjoy:
                ShowTutorialText(7);
                break;
            default:
                Debug.LogError($"Couldn't handle TutorialPartType {tutorialPartType}");
                // DisableAllModes();
                break;
        }
    }

    private void ShowTutorialText(int textIndex)
    {
        tutorialText.gameObject.SetActive(true);
        tutorialTextAnimator.TypeAnimate(tutorialTexts[textIndex].GetText(currentLanguage.Value));
        
        // ToggleColorOverlay(true);
        ToggleColorOverlayVisual(true);
    }

    private IEnumerator DoBlinkAnimation(Image image)
    {
        Debug.Log($"DoBlinkAnimation image {image}");
        
        bool increasingAlpha = true;
        
        while (true)
        {
            float imageAlpha = image.color.a;

            imageAlpha += highlightBlinkSpeed * Time.deltaTime * (increasingAlpha ? 1 : -1);

            if (imageAlpha is <= 0 or >= 1)
                increasingAlpha = !increasingAlpha;
            
            SetAlphaOfImage(image, imageAlpha);
            
            yield return 0;
        }
    }

    [Button]
    private void StopBlinkAnimations()
    {
        Debug.Log($"Stop Blink animations");
        
        if(seatButtonBackgroundBlinkRoutine != null)
            StopCoroutine(seatButtonBackgroundBlinkRoutine);
        if(languageButtonBackgroundBlinkRoutine != null)
            StopCoroutine(languageButtonBackgroundBlinkRoutine);
        if(connectionImageBlinkRoutine != null)
            StopCoroutine(connectionImageBlinkRoutine);
        
        SetAlphaOfImage(seatButtonBackground, seatButtonBackgroundStartAlpha);
        SetAlphaOfImage(languageButtonBackground, languageButtonBackgroundStartAlpha);
        SetAlphaOfImage(connectionImage, connectionImageStartAlpha);
    }
    
    private void SetAlphaOfImage(Image image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }
    
    #endregion
    
    #region Other
    
    public void SetVoteAverage(float voteAverage)
    {
        averageSlider.value = voteAverage;
    }
    
    private void ToggleEnterSeatDialog()
    {
        bool show = (seatInputHolder.activeSelf == false);
        
        DisableAllModes();
        
        if(show)
        {
            ToggleTutorial(false, false);
            StopBlinkAnimations();
        }
        else
        {
            Debug.Log($"<color=green>Toggle Enter Seat Dialog, show false</color>");
            if (InstanceFinder.IsOffline == false)
            {
                if (Instances.NetworkedAppState.appState == AppState.Tutorial)
                {
                    ToggleTutorial(true);
                }
            }
            else
            {
                ToggleTutorial(true);
            }
        }
        
        ShowEnterSeatDialog(show);
    }
    
    private void ShowEnterSeatDialog(bool show)
    {
        seatInputHolder.SetActive(show);
    }

    #endregion
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

public enum TutorialPartType
{
    Welcome, // Also language
    Seat,
    Language,
    Connection,
    SlidersExplanation,
    Slider,
    MajorityExplanation,
    Majority,
    AudioExplanation,
    Audio,
    Enjoy,
}