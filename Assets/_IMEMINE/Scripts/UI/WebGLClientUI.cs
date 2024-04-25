using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class WebGLClientUI : UIWithConnection
{
    #region Variables
    
    [Header("Backgrounds/Overlays")]
    [SerializeField] private ColorOverlay colorOverlay;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite[] fadeSprites;
    [SerializeField] private ImageFader imageFader;
    [SerializeField] private RawImage backgroundVideo;
    [SerializeField] private WebGLVideoPlayer votingClientVideoPlayer;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject microOrganisms;
    [SerializeField] private GameObject waysOfWater;
    [SerializeField] private GameObject voteWarning;
    [SerializeField] private TextMeshProUGUI voteWarningText;
    [SerializeField] private TextMeshProUGUI votingHighText;
    [SerializeField] private TextMeshProUGUI votingLowText;
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
    [SerializeField] private TextMeshProUGUI tutorialVoteMajorityText;
    [SerializeField] private TextMeshProUGUI tutorialVoteYourVoteText;
    [SerializeField] private TextMeshProUGUI tutorialHighText;
    [SerializeField] private TextMeshProUGUI tutorialLowText;
    [SerializeField] private TextMeshProUGUI tutorialVotingStatusTextNL;
    [SerializeField] private TextMeshProUGUI tutorialVotingStatusTextEN;
    [SerializeField] private Image tutorialVoteStatusBackground;
    [SerializeField] private Slider tutorialDistortionSlider;
    [SerializeField] private TextMeshProUGUI tutorialHardNLText;
    [SerializeField] private TextMeshProUGUI tutorialHardENText;
    [SerializeField] private TextMeshProUGUI tutorialSoftNLText;
    [SerializeField] private TextMeshProUGUI tutorialSoftENText;
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
    [SerializeField] private GameObject introductionModeHolder;
    [SerializeField] private TextMeshProUGUI introductionModeText;
    [SerializeField] private GameObject auraTextHolder;
    [SerializeField] private TextMeshProUGUI votingStatusTextNL;
    [SerializeField] private TextMeshProUGUI votingStatusTextEN;
    [SerializeField] private TextMeshProUGUI microOrganismsHighText;
    [SerializeField] private TextMeshProUGUI microOrganismsLowText;
    [SerializeField] private TextMeshProUGUI organism1Text;
    [SerializeField] private TextMeshProUGUI organism2Text;
    [SerializeField] private Slider microOrganismsHighLowSlider;
    [SerializeField] private Slider organismsSlider;
    [SerializeField] private Image votingStatusBackgroundPanel;
    [SerializeField] private Color votingEnabledColor;
    [SerializeField] private Color votingDisabledColor;
    [SerializeField] private GameObject theEnd;
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
                // Instances.AudioManager.SetFadeVal(1 - sliderVal);
                imageFader.SetFadeVal(1 - sliderVal);
                
                Instances.NetworkedVoting.SendVoteUpdate(sliderVal, Instances.SeatNumber, Instances.RowNumber);
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
                Debug.Log($"Set waysOfWaterSlider: {sliderVal}");
                Instances.AudioManager.circlePlayer.SetFadeValue(sliderVal);
                Instances.AudioManager.delayPlayer.SetFadeValue(sliderVal);
            });
        
        microOrganismsHighLowSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal =>
            {
                Instances.AudioManager.microOrganismsDoubleFader.SetFadeValHighLow(sliderVal);
            });
        organismsSlider.onValueChanged.AsObservable()
            .Subscribe(sliderVal =>
            {
                Instances.AudioManager.microOrganismsDoubleFader.SetFadeValDistortion(sliderVal);
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
        Debug.Log($"Set seat: {seat}");
        Instances.SeatNumber = seat;
        
        Tuple<int, int> seatDigits = GetRowOrSeatDigits(seat);
        
        PlayerPrefs.SetInt(KeyForSeatElement(SeatElement.Seat10s), seatDigits.Item1);
        PlayerPrefs.SetInt(KeyForSeatElement(SeatElement.Seat1s), seatDigits.Item2);
    }
    
    #endregion
    
    #region Modes
    
    public void SetToAppState(AppState appState)
    {
        ToggleTutorial(appState == AppState.Tutorial);
        Debug.Log($"Set to appstate {appState}");
        switch (appState)
        {
            case AppState.Introduction: EnableIntroductionMode();
                break;
            case AppState.MicroOrganisms: EnableMicroOrganismsMode();
                break;
            case AppState.WaysOfWater: EnableWaysOfWaterMode();
                break;
            case AppState.Magma: EnableMagmaMode();
                break;
            case AppState.AboutCrystals: EnableAboutCrystalsMode();
                break;
        }
    }

    private Coroutine colorOverlayDisableRoutine;

    public void ToggleColorOverlayOverride(bool show)
    {
        if (show)
        {
            ToggleColorOverlayVisual(true);
            
            return;
        }
        
        if(colorOverlayDisableRoutine != null)
            return;

        colorOverlayDisableRoutine = StartCoroutine(DisableColorOverlayAfterTime());
    }

    private IEnumerator DisableColorOverlayAfterTime()
    {
        float waitTime = Random.Range(0, 22);
        
        Debug.Log($"Will disable color overlay after {waitTime}");

        yield return new WaitForSeconds(waitTime);
        
        ToggleColorOverlayVisual(false);

        backgroundVideo.gameObject.SetActive(false);
        
        colorOverlayDisableRoutine = null;
    }
    
    private void ToggleColorOverlayVisual(bool show)
    {
        colorOverlay.StartDelay = 0;
        colorOverlay.gameObject.SetActive(show);
    }
    
    [Button]
    public void EnableEffectSlidersMode()
    {
        DisableAllModes();
        
        effectsSliders.SetActive(true);
        colorOverlay.gameObject.SetActive(true);
        
        Instances.AudioManager.doubleFader.PlayFadeSamples();
    }
    
    private void EnableTutorialSlidersMode()
    {
        DisableAllModes();
        
        tutorialSliders.SetActive(true);
        colorOverlay.gameObject.SetActive(true);
        
        Instances.AudioManager.tutorialDoubleFader.PlayFadeSamples();
    }
    
    private void EnableIntroductionMode()
    {
        DisableAllModes(true);
        
        backgroundVideo.gameObject.SetActive(true);

        votingClientVideoPlayer.PlayVideo(VideoType.Aura);
        videoPlayer.playbackSpeed = 0.6f;
    }
    
    public void ToggleEffectSlidersMode(bool effectSlidersOn)
    {
        Debug.Log($"ToggleEffectSlidersMode: {effectSlidersOn}");
        microOrganisms.SetActive(false);
        waysOfWater.SetActive(false);
        
        if (effectSlidersOn)
        {
            if(Instances.NetworkedAppState.appState == AppState.MicroOrganisms)
            {
                microOrganisms.SetActive(true);
            }
            if(Instances.NetworkedAppState.appState == AppState.WaysOfWater)
            {
                waysOfWater.SetActive(true);
            }
        }
    }
    
    private void EnableMicroOrganismsMode()
    {
        DisableAllModes();
        
        backgroundVideo.gameObject.SetActive(true);
        
        votingClientVideoPlayer.PlayVideo(VideoType.MicroOrganisms);
        videoPlayer.playbackSpeed = 0.5f;
        
        if (Instances.NetworkedVoting.votingModeEnabled)
        {
            voteSlider.gameObject.SetActive(true);
            averageSlider.gameObject.SetActive(true);
            votingHolder.SetActive(true);
        }

        if (Instances.NetworkedAppState.effectSlidersOn)
        {
            ToggleEffectSlidersMode(true);
        }
    }
    
    private void EnableMagmaMode()
    {
        DisableAllModes();
        
        backgroundVideo.gameObject.SetActive(true);
        
        votingClientVideoPlayer.PlayVideo(VideoType.Magma);
        videoPlayer.playbackSpeed = 0.69f;
    }
    
    private void EnableWaysOfWaterMode()
    {
        DisableAllModes();
        
        backgroundVideo.gameObject.SetActive(true);
        
        votingClientVideoPlayer.PlayVideo(VideoType.WaysOfWater);
        videoPlayer.playbackSpeed = 0.5f;
        
        if (Instances.NetworkedVoting.votingModeEnabled)
        {
            voteSlider.gameObject.SetActive(true);
            averageSlider.gameObject.SetActive(true);
            votingHolder.SetActive(true);
        }
        if (Instances.NetworkedAppState.effectSlidersOn)
        {
            ToggleEffectSlidersMode(true);
        }
    }
    
    private void EnableAboutCrystalsMode()
    {
        DisableAllModes();
        
        backgroundVideo.gameObject.SetActive(true);
        
        votingClientVideoPlayer.PlayVideo(VideoType.AboutCrystals);
        videoPlayer.playbackSpeed = 0.5f;
    }
    
    [Button]
    public void EnableTutorialVotingMode()
    {
        DisableAllModes();
        
        imageFader.DisplayFadeImages(fadeSprites.Take(3).ToArray());
        
        Instances.AudioManager.StopAllPlayback();
        Instances.AudioManager.ResetAllFx();
        
        tutorialVoting.SetActive(true);
    }

    [Button]
    private void DisableAllModes(bool disableTutorial = false)
    {
        if(disableTutorial)
            tutorialCanvas.gameObject.SetActive(false);
        
        ShowEnterSeatDialog(false);
        
        StopBlinkAnimations();
        
        colorOverlay.gameObject.SetActive(false);
        // backgroundVideo.gameObject.SetActive(false);

        waysOfWater.SetActive(false);
        microOrganisms.SetActive(false);
        
        // voteSlider.gameObject.SetActive(false);
        // averageSlider.gameObject.SetActive(false);
        votingHolder.SetActive(false);
        tutorialVoting.SetActive(false);
        imageFader.gameObject.SetActive(false);
        
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

        if (SeatInputActive)
        {
            return;
        }
        
        if (isInitVersion)
        {
            return;
        }
        
        if (InstanceFinder.IsOffline == false)
        {
            if(Instances.NetworkedAppState.appState == AppState.Tutorial)
                SetTutorialPart(currentTutorialPart);

            Debug.Log($"TOGGLE LANGUAGE when online");
            
            //TODO Enable current mode
            // else
            //     
        }
        else
        {
            SetTutorialPart(currentTutorialPart);
        }
    }
    
    private void SetLanguageInTextComponents(bool nl)
    {
        languageButton.GetComponentInChildren<TextMeshProUGUI>().text = (nl ? Language.NL : Language.EN).ToString();
        
        rowTitleText.text = nl ? "RIJ" : "ROW";
        seatTitleText.text = nl ? "STOEL" : "SEAT";
        
        seatConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = nl ? "KIES" : "CONFIRM";
        
        highText.text = nl ? "HOOG" : "HIGH";
        tutorialHighText.text = nl ? "HOOG" : "HIGH";
        microOrganismsHighText.text = nl ? "HOOG" : "HIGH";
        tutorialVoteHighText.text = nl ? "HOOG" : "HIGH";
        
        lowText.text = nl ? "LAAG" : "LOW";
        tutorialLowText.text = nl ? "LAAG" : "LOW";
        tutorialVoteLowText.text = nl ? "LAAG" : "LOW";
        microOrganismsLowText.text = nl ? "LAAG" : "LOW";
        
        tutorialVoteMajorityText.text = nl ? "Publieks-meerderheid" : "Majority of Votes";
        tutorialVoteYourVoteText.text = nl ? "Uw Stem" : "Your Vote";
        
        hardText.text = nl ? "HARD" : "HARSH";
        // tutorialHardText.text = nl ? "HARD" : "HARSH";
        softText.text = nl ? "ZACHT" : "SOFT";
        // tutorialSoftText.text = nl ? "ZACHT" : "SOFT";

        introductionModeText.text = nl ? 
                "Welkom bij de voorstelling Minerals. Geef alstublieft je stoelnummer in als je dit nog niet gedaan hebt."
                : "Welcome to the performance of minerals. Please enter your seat number via the button in the upper left corner if you haven't done this already.";
        
        votingHighText.text = nl ? EnglishToDutch(votingHighText.text) : DutchToEnglish(votingHighText.text);
        votingLowText.text = nl ? EnglishToDutch(votingLowText.text) : DutchToEnglish(votingLowText.text);
        
        votingStatusTextNL.gameObject.SetActive(nl);
        votingStatusTextEN.gameObject.SetActive(nl == false);
        
        tutorialVotingStatusTextNL.gameObject.SetActive(nl);
        tutorialVotingStatusTextEN.gameObject.SetActive(nl == false);
        
        tutorialHardNLText.gameObject.SetActive(nl);
        tutorialHardENText.gameObject.SetActive(nl == false);
        
        tutorialSoftNLText.gameObject.SetActive(nl);
        tutorialSoftENText.gameObject.SetActive(nl == false);
    }
    
    #endregion
    
    #region Voting Control
    
    [Button]
    public void ToggleVotingMode(bool votingModeOn)
    {
        DisableAllModes();
        
        imageFader.gameObject.SetActive(votingModeOn);
        imageFader.DisplayFadeImages(fadeSprites);
        
        Instances.AudioManager.StopAllPlayback();
        Instances.AudioManager.ResetAllFx();

        Sprite[] fadeSpritesForAppState = { fadeSprites[0], fadeSprites[1], fadeSprites[2] };
        
        switch (Instances.NetworkedAppState.appState)
        {
            case AppState.WaysOfWater:
                fadeSpritesForAppState = new[] { fadeSprites[3], fadeSprites[4], fadeSprites[5] };
                break;
            case AppState.Magma:
                fadeSpritesForAppState = new[] { fadeSprites[6], fadeSprites[7], fadeSprites[8] };
                break;
        }
        
        imageFader.DisplayFadeImages(fadeSpritesForAppState);
        
        if (votingModeOn)
        {
            voteSlider.gameObject.SetActive(true);
            averageSlider.gameObject.SetActive(true);
            votingHolder.SetActive(true);
            
            ShowEnterSeatDialog(false);
        }
    }
    
    public void SetBlockVoting(bool blockVoting)
    {
        voteSlider.interactable = (blockVoting == false);
        
        if(blockVoting)
            voteSlider.transform.SetSiblingIndex(0);
        else
            averageSlider.transform.SetSiblingIndex(0);
        
        votingStatusTextNL.text = blockVoting ? "Stem vergrendeld" : "Stemmen gestart!";
        votingStatusTextEN.text = blockVoting ? "Voting block" : "Voting enabled!";

        votingStatusBackgroundPanel.color = blockVoting ? votingDisabledColor : votingEnabledColor;
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
            case TutorialPartType.Language:
                languageButtonBackgroundBlinkRoutine = StartCoroutine(DoBlinkAnimation(languageButtonBackground));
                ShowTutorialText(1);
                break;
            case TutorialPartType.Seat:
                seatButtonBackgroundBlinkRoutine = StartCoroutine(DoBlinkAnimation(seatButtonBackground));
                ShowTutorialText(2);
                break;
            case TutorialPartType.Connection:
                connectionImageBlinkRoutine = StartCoroutine(DoBlinkAnimation(connectionImage));
                ShowTutorialText(3);
                break;
            case TutorialPartType.SlidersExplanation:
                ShowTutorialText(4);
                imageFader.gameObject.SetActive(false);
                break;
            case TutorialPartType.Slider:
                tutorialText.gameObject.SetActive(false);
                tutorialAverageSlider.transform.SetSiblingIndex(0);
                tutorialVoteSlider.interactable = true;
                tutorialAverageSlider.interactable = false;
                tutorialVotingStatusTextNL.text = "Stem nu!";
                tutorialVotingStatusTextEN.text = "Vote now!";
                // voteWarning.GetComponent<Image>().color = connectedColor;
                tutorialVoteStatusBackground.color = connectedColor;
                EnableTutorialVotingMode();
                imageFader.gameObject.SetActive(true);
                imageFader.DisplayFadeImages(new []{ fadeSprites[0], fadeSprites[1], fadeSprites[2] });
                break;
            case TutorialPartType.MajorityExplanation:
                ShowTutorialText(5);
                imageFader.gameObject.SetActive(false);
                break;
            case TutorialPartType.Majority:
                tutorialText.gameObject.SetActive(false);
                tutorialVoteSlider.transform.SetSiblingIndex(0);
                tutorialVoteSlider.interactable = false;
                tutorialAverageSlider.interactable = false;
                tutorialVotingStatusTextNL.text = "Stem vergrendeld";
                tutorialVotingStatusTextEN.text = "Voting blocked";
                // voteWarning.GetComponent<Image>().color = disconnectedColor;
                tutorialVoteStatusBackground.color = disconnectedColor;
                EnableTutorialVotingMode();
                imageFader.gameObject.SetActive(true);
                imageFader.DisplayFadeImages(new []{ fadeSprites[0], fadeSprites[1], fadeSprites[2] });
                break;
            case TutorialPartType.AudioExplanation1:
                ShowTutorialText(6);
                imageFader.gameObject.SetActive(false);
                break;
            case TutorialPartType.AudioExplanation2:
                ShowTutorialText(7);
                break;
            case TutorialPartType.Audio:
                tutorialText.gameObject.SetActive(false);
                EnableTutorialSlidersMode();
                break;
            case TutorialPartType.Enjoy:
                ShowTutorialText(8);
                break;
            default:
                Debug.LogError($"Couldn't handle TutorialPartType {tutorialPartType}");
                // DisableAllModes();
                break;
        }
    }

    public void SetTutorialSliderTexts(int soundIndex)
    {
        switch (soundIndex)
        {
            case 0: 
                tutorialSoftNLText.text = "STALAGMIETEN";
                tutorialHardNLText.text = "STALACTIETEN";
                
                tutorialSoftENText.text = "STALAGMITES";
                tutorialHardENText.text = "STALACTITES";
                break;
            case 1: 
                tutorialSoftNLText.text = "STALACTIETEN";
                tutorialHardNLText.text = "STALAGFLIETEN";
                
                tutorialSoftENText.text = "STALACTITES";
                tutorialHardENText.text = "STALAGFLITES";
                break;
            case 2: 
                tutorialSoftNLText.text = "STALAGFLIETEN";
                tutorialHardNLText.text = "STALAGMIETEN";
                
                tutorialSoftENText.text = "STALAGFLITES";
                tutorialHardENText.text = "STALAGMITES";
                break;
        }
    }
    
    public void SetMicroOrganismsSliderTexts(int soundIndex)
    {
        switch (soundIndex)
        {
            case 0:
                organism1Text.text = "HEMATODES";
                organism2Text.text = "METHANOGENS";
                break;
            case 1:
                organism1Text.text = "METHANOGENS";
                organism2Text.text = "BRADYRHIZOBIA";
                break;
            case 2:
                organism1Text.text = "BRADYRHIZOBIA";
                organism2Text.text = "ACTINOMYCETES";
                break;
            case 3:
                organism1Text.text = "ACTINOMYCETES";
                organism2Text.text = "HELMINTHS";
                break;
            case 4:
                organism1Text.text = "HELMINTHS";
                organism2Text.text = "ARCHAEA";
                break;
            case 5:
                organism1Text.text = "ARCHAEA";
                organism2Text.text = "FUNGI";
                break;
            case 6:
                organism1Text.text = "FUNGI";
                organism2Text.text = "RHIZOBIA";
                break;
            case 7:
                organism1Text.text = "RHIZOBIA";
                organism2Text.text = "AMOEBAS";
                break;
            case 8:
                organism1Text.text = "AMOEBAS";
                organism2Text.text = "HEMATODES";
                break;
        }
    }

    public void SetVotingTags(string highTag, string lowTag)
    {
        Debug.Log($"Set voting Tags: {highTag}, {lowTag}");
        bool nl = (currentLanguage.Value == Language.NL); 
        
        votingHighText.text = nl ? highTag : DutchToEnglish(highTag);
        votingLowText.text = nl ? lowTag : DutchToEnglish(lowTag);
    }

    private string DutchToEnglish(string nlString)
    {
        switch (nlString)
        {
            case "HOOG": return "HIGH";
            case "LAAG": return "LOW";
            
            case "INTENS": return "INTENSE";
            case "KALM": return "CALM";
            
            case "LUID": return "LOUD";
            case "HARD": return "LOUD";
            case "ZACHT": return "SOFT";
            
            case "SNEL": return "FAST";
            case "TRAAG": return "SLOW";
        }
        Debug.LogError($"Couldn't find English translation for {nlString}!");
        
        return nlString;
    }

    private string EnglishToDutch(string enString)
    {
        switch (enString)
        {
            case "HIGH": return "HOOG";
            case "LOW": return "LAAG";
            
            case "INTENSE": return "INTENS";
            case "CALM": return "KALM";
            
            case "LOUD": return "LUID";
            // case "HARD": return "LOUD";
            case "SOFT": return "ZACHT";
            
            case "FAST": return "SNEL";
            case "SLOW": return "TRAAG";
        }
        Debug.LogError($"Couldn't find Dutch translation for {enString}!");
        
        return enString;
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
            
            if (InstanceFinder.IsOffline == false)
            {
                if (Instances.NetworkedAppState.appState == AppState.Tutorial)
                {
                    ToggleColorOverlayVisual(true);
                }
            }
            else
            {
                ToggleColorOverlayVisual(true);
            }
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
    
    public void ShowEnterSeatDialog(bool show)
    {
        seatInputHolder.SetActive(show);
    }
    
    #endregion
    
    public void ToggleTheEnd(bool showTheEnd)
    {
        theEnd.SetActive(showTheEnd);
    }

    public void ToggleAuraText(bool showAuraText)
    {
        auraTextHolder.SetActive(showAuraText);
    }

    public void ToggleIntroductionMode(bool showIntroductionMode)
    {
        introductionModeHolder.SetActive(showIntroductionMode);
    }

    #region Static Video

    private Coroutine stopStaticVideoRoutine;
    
    public void ShowStaticVideo()
    {
        backgroundVideo.gameObject.SetActive(true);
        videoPlayer.playbackSpeed = 1;

        votingClientVideoPlayer.PlayVideo(VideoType.Noise);
        
        videoPlayer.time = Random.Range(0, 75);
    }

    public void StopStaticVideo()
    {
        if(stopStaticVideoRoutine != null)
            return;

        stopStaticVideoRoutine = StartCoroutine(StopStaticVideoAfterTime());
    }
    
    private IEnumerator StopStaticVideoAfterTime()
    {
        float waitTime = Random.Range(0, 22);
        Debug.Log($"Will turn of static video after {waitTime}");
        
        yield return new WaitForSeconds(waitTime);
        
        backgroundVideo.gameObject.SetActive(false);
        
        stopStaticVideoRoutine = null;
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
    Language,
    Seat,
    Connection,
    SlidersExplanation,
    Slider,
    MajorityExplanation,
    Majority,
    AudioExplanation1,
    AudioExplanation2,
    Audio,
    Enjoy,
}