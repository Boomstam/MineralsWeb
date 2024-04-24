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
    public AuraText[] auraTexts;
    [SerializeField] private GameObject backgroundPanel;

    private string lastNLText; 
    private string lastENText; 
    
    private void Start()
    {
        if(Instances.BuildType != BuildType.Voting)
            return;
        
        Instances.WebGLClientUI.currentLanguage.Subscribe(SetLanguage);
    }
    
    public void Toggle(bool show)
    {
        auraTextComponent.gameObject.SetActive(show);
        backgroundPanel.SetActive(show);
    }
    
    public void SetText(string nlText, string enText)
    {
        // Debug.Log($"SetText nl: {nlText}; enText: {enText} on {name}");

        lastNLText = nlText;
        lastENText = enText;
        
        auraTextComponent.text = (Instances.WebGLClientUI.currentLanguage.Value == Language.NL) ? nlText : enText;
    }
    
    public void GoToText(int index)
    {
        Debug.Log($"Go to text {index} on {name}");
        
        if (index >= auraTexts.Length)
        {
            Debug.LogError($"Index {index} was too big for the auraText array of size {auraTexts.Length} in GoToText");
            return;
        }

        auraTextComponent.text = auraTexts[index].GetText(Instances.WebGLClientUI.currentLanguage.Value);
    }
    
    private void SetLanguage(Language language)
    {
        auraTextComponent.text = (language == Language.NL) ? lastNLText : lastENText;
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