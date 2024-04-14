using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;

public class AuraTextDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI auraTextComponent;
    [SerializeField] private AuraText[] auraTexts;
    
    private void Start()
    {
        Instances.WebGLClientUI.currentLanguage.Subscribe(SetLanguage);
    }
    
    // [Button]
    // private void GoToNextText(bool next)
    // {
    //     int lastIndex = auraTexts.Length - 1;
    //     int newIndex = currentTextIndex + (next ? 1 : -1);
    //     
    //     if (newIndex < 0)
    //         newIndex = lastIndex;
    //     if (newIndex > lastIndex)
    //         newIndex = 0;
    //     
    //     currentTextIndex = newIndex;
    //     
    // }

    public void GoToText(int index)
    {
        if (index >= auraTexts.Length)
        {
            Debug.LogError($"Index {index} was too big for the auraText array of size {auraTexts.Length} in GoToText");
            return;
        }
        
        auraTextComponent.text = auraTexts[index].GetText(Instances.WebGLClientUI.currentLanguage.Value);
    }
    
    private void SetLanguage(Language language)
    {
        int index = Instances.NetworkedAppState.currentAuraTextIndex;

        if (index >= auraTexts.Length)
        {
            Debug.LogError($"Index {index} was too big for the auraText array of size {auraTexts.Length} in SetLanguage");
            return;
        }
        
        auraTextComponent.text = auraTexts[index].GetText(language);
    }
}

[Serializable]
public class AuraText
{
    [SerializeField] private string nlText;
    [SerializeField] private string enText;
    
    public string GetText(Language language)
    {
        return language == Language.NL ? nlText : enText;
    }
}