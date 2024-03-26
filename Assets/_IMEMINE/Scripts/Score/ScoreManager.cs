using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private PartsPerChoice[] pages;
    [SerializeField] private Image page;
    [SerializeField] private Image warningMessageBackground;
    [SerializeField] private TextMeshProUGUI choiceSwitchWarning;
    [SerializeField] private float minWarningFadeAlpha;
    [SerializeField] private float maxScaleOffset;
    [SerializeField] private float warningFadeTime;
    [SerializeField] private int warningFadeSteps;
    [SerializeField] private TextMeshProUGUI choiceLetter;
    [SerializeField] private GameObject letterModePanel;
    [SerializeField] private Button switchLetterModeButton;
    [SerializeField] private Button leftSideButton;
    [SerializeField] private Button rightSideButton;
    [SerializeField] private Color choiceAColor;
    [SerializeField] private Color choiceBColor;
    [SerializeField] private Color choiceCColor;
    
    public int HighlightWarningTime => Instances.NetworkedMonitor.warningTime;
    public PartType PartType { get; set; } // Leave undefined if deleting other parts' pages
    
    private PartsPerChoice CurrentPartChoices => pages[(int)PartType];
    private int NumPages => CurrentPartChoices.NumPages;

    private ChoiceType CurrentChoice => Instances.NetworkedVoting.currentChoice;
    
    private int currentPage;
    
    private void Start()
    {
        switchLetterModeButton.onClick.AsObservable().Subscribe(_ => SwitchLetterMode());
        
        leftSideButton.onClick.AsObservable().Subscribe(_ => TurnPage(false));
        rightSideButton.onClick.AsObservable().Subscribe(_ => TurnPage(true));
    }
    
    [Button]
    public void HighlightChoice(ChoiceType choiceType)
    {
        Debug.Log($"Highlight choice: {choiceType}");
        
        StartCoroutine(DoHighlightCountdown(choiceType));
    }
    
    // TODO: There is still a bug here where the warning will disappear if a second routine is started while another is running.
    private IEnumerator DoHighlightCountdown(ChoiceType choiceType)
    {
        Debug.Log($"DoHighlightCountdown to {choiceType}");
        warningMessageBackground.gameObject.SetActive(true);

        for (int secondsPassed = 0; secondsPassed < HighlightWarningTime; secondsPassed++)
        {
            choiceSwitchWarning.text = $"{choiceType} in {HighlightWarningTime - secondsPassed}...";

            bool fadeIn = secondsPassed % 2 == 0;

            for (int currentStep = 0; currentStep < warningFadeSteps; currentStep++)
            {
                float offsetPercentage = (float)currentStep / (float)warningFadeSteps;
                
                // Color animation
                float scaledAlphaOffset = offsetPercentage * (1f - minWarningFadeAlpha);
            
                float alpha = minWarningFadeAlpha + (fadeIn ? scaledAlphaOffset : (1f - minWarningFadeAlpha) - scaledAlphaOffset);

                Color bgColor = warningMessageBackground.color;
                warningMessageBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, alpha);
                
                // Size animation
                float scaledSizeOffset = offsetPercentage * maxScaleOffset;
                float size = 1f + (fadeIn ? scaledSizeOffset : maxScaleOffset - scaledSizeOffset);

                warningMessageBackground.transform.localScale = new Vector3(size, size, 1f);
                
                yield return new WaitForSeconds(warningFadeTime / (float)warningFadeSteps);
            }

            // yield return new WaitForSeconds(1f);
        }

        choiceSwitchWarning.text = $"Choice {choiceType}!";

        DoHighlight(choiceType);

        yield return new WaitForSeconds(1f);

        warningMessageBackground.gameObject.SetActive(false);
    }

    private void DoHighlight(ChoiceType choiceType)
    {
        if(choiceType != CurrentChoice)
            Debug.LogError($"the CurrentChoice {CurrentChoice} should be equal to choiceType {choiceType}");
        
        ShowCurrentPage();
    }

    private void TurnPage(bool next)
    {
        int offset = next ? 1 : -1;

        int pageToGoTo = currentPage + offset;

        if (pageToGoTo < 0)
            pageToGoTo = NumPages - 1;
        if (pageToGoTo >= NumPages)
            pageToGoTo = 0;

        currentPage = pageToGoTo;
        Debug.Log($"Turn page to {currentPage}");
        ShowCurrentPage();
    }

    private void ShowCurrentPage()
    {
        Debug.Log($"ShowCurrentPage: {Instances.NetworkedVoting.currentChoice}, currentPage: {currentPage}");

        Sprite pageSprite = GetCurrentChoicePages(Instances.NetworkedVoting.currentChoice)[currentPage];

        if (pageSprite == null)
            pageSprite = CurrentPartChoices.choiceA[currentPage];
        
        page.sprite = pageSprite;
    }
    
    private void SwitchLetterMode()
    {
        bool inLetterMode = letterModePanel.activeSelf;
        
        letterModePanel.SetActive(inLetterMode == false);
    }
    
    private Sprite[] GetCurrentChoicePages(ChoiceType choice)
    {
        if (choice == ChoiceType.A)
            return CurrentPartChoices.choiceA;
        if (choice == ChoiceType.B)
            return CurrentPartChoices.choiceB;
        if (choice == ChoiceType.C)
            return CurrentPartChoices.choiceC;

        throw new Exception($"Couldn't get choice pages for choice {choice}");
    }
}

public enum PartType
{
    Violin1,
    Violin2,
    Alto,
    Cello,
    Contrabass,
}

[Serializable]
public class PartsPerChoice
{
    public Sprite[] choiceA;
    public Sprite[] choiceB;
    public Sprite[] choiceC;
    
    public Vector2[] choicePageRangesAllInclusive;

    public int NumPages => choiceA.Length;
}