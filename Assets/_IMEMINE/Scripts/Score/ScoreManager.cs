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
    [SerializeField] private Color choiceAColor;
    [SerializeField] private Color choiceBColor;
    [SerializeField] private Color choiceCColor;
    
    private int HighlightWarningTime => Instances.NetworkedVoting.warningTime;

    private int currentPage;
    
    private Coroutine choiceRoutine;

    public void SetChoice(ChoiceType choiceType)
    {
        choiceLetter.text = choiceType.ToString();
    }
    
    [Button]
    public void StartHighlightChoiceRoutine(ChoiceType choiceType)
    {
        Debug.Log($"Highlight choice: {choiceType}");
        
        if(choiceRoutine != null)
            StopCoroutine(choiceRoutine);
        
        choiceRoutine = StartCoroutine(DoHighlightCountdown(choiceType));
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

        // Done by server
        // DoHighlight(choiceType);

        yield return new WaitForSeconds(1f);

        warningMessageBackground.gameObject.SetActive(false);
    }
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