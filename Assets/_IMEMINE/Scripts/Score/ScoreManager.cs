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
    [SerializeField] private Sprite[] pages;
    [SerializeField] private Image page;
    [SerializeField] private TextMeshProUGUI choiceSwitchWarning;
    [SerializeField] private TextMeshProUGUI choiceLetter;
    [SerializeField] private GameObject letterModePanel;
    [SerializeField] private Button switchLetterModeButton;
    [SerializeField] private Color choiceAColor;
    [SerializeField] private Color choiceBColor;
    [SerializeField] private Color choiceCColor;
    [SerializeField] private Vector2[] commonPageRangesAllInclusive;
    
    public int highlightWarningTime { get; set; }
    
    private ChoiceType currentChoice;
    
    private void Start()
    {
        switchLetterModeButton.onClick.AsObservable().Subscribe(_ => SwitchLetterMode());
    }
    
    [Button]
    public void HighlightChoice(ChoiceType choiceType)
    {
        if (choiceType == currentChoice || choiceType == ChoiceType.None)
            return;
        
        Debug.Log($"Highlight choice: {choiceType}");

        currentChoice = choiceType;
        StartCoroutine(DoHighlightCountdown(choiceType));
    }
    
    // TODO: There is still a bug here where the warning will disappear if a second routine is started while another is running.
    private IEnumerator DoHighlightCountdown(ChoiceType choiceType)
    {
        Debug.Log($"DoHighlightCountdown to {choiceType}");
        choiceSwitchWarning.transform.parent.gameObject.SetActive(true);

        for (int i = 0; i < highlightWarningTime; i++)
        {
            choiceSwitchWarning.text = $"{choiceType} in {highlightWarningTime - i}...";

            yield return new WaitForSeconds(1f);
        }

        choiceSwitchWarning.text = $"Choice {choiceType}!";

        DoHighlight(choiceType);

        yield return new WaitForSeconds(1f);

        choiceSwitchWarning.transform.parent.gameObject.SetActive(false);
    }
    
    private void DoHighlight(ChoiceType choiceType)
    {
        
    }
    
    private void SwitchLetterMode()
    {
        bool inLetterMode = letterModePanel.activeSelf;
        
        letterModePanel.SetActive(inLetterMode == false);
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