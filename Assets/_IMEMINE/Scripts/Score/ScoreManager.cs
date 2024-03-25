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
    [SerializeField] private TextMeshProUGUI choiceSwitchWarning;
    [SerializeField] private TextMeshProUGUI choiceLetter;
    [SerializeField] private GameObject letterModePanel;
    [SerializeField] private Button switchLetterModeButton;
    [SerializeField] private Button leftSideButton;
    [SerializeField] private Button rightSideButton;
    [SerializeField] private Color choiceAColor;
    [SerializeField] private Color choiceBColor;
    [SerializeField] private Color choiceCColor;
    [SerializeField] private Vector2[] commonPageRangesAllInclusive;
    
    public int HighlightWarningTime { get; set; }
    public PartType PartType { get; set; }
    
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
        choiceSwitchWarning.transform.parent.gameObject.SetActive(true);

        for (int i = 0; i < HighlightWarningTime; i++)
        {
            choiceSwitchWarning.text = $"{choiceType} in {HighlightWarningTime - i}...";

            yield return new WaitForSeconds(1f);
        }

        choiceSwitchWarning.text = $"Choice {choiceType}!";

        DoHighlight(choiceType);

        yield return new WaitForSeconds(1f);

        choiceSwitchWarning.transform.parent.gameObject.SetActive(false);
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
        
        ShowCurrentPage();
    }

    private void ShowCurrentPage()
    {
        Debug.Log($"ShowCurrentPage: {Instances.NetworkedVoting.currentChoice}, currentPage: {currentPage}");
        
        Sprite pageSprite = GetCurrentChoicePages(Instances.NetworkedVoting.currentChoice)[currentPage];

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
    SoloCello,
    Violin1,
    Violin2,
    Alto,
    Cello,
}

[Serializable]
public class PartsPerChoice
{
    public Sprite[] choiceA;
    public Sprite[] choiceB;
    public Sprite[] choiceC;

    public int NumPages => choiceA.Length;
}