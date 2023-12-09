using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class WebGLClientUI : UIWithConnection
{
    [SerializeField] private TMP_InputField oscMessageInput;
    [SerializeField] private Button sendOSCButton;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private float maxProgressBarRight = 527;
    
    public ReactiveProperty<string> oscMessage = new ReactiveProperty<string>();

    public void Start()
    {
        sendOSCButton.onClick.AsObservable().Subscribe(_ => oscMessage.SetValueAndForceNotify(oscMessageInput.text));
        
        choiceButtons.ForEach((button, i) =>
        {
            button.onClick.AsObservable().Subscribe(_ => OnChoiceClick(i));
        });
    }

    private void OnChoiceClick(int choice)
    {
        
    }

    [Button]
    private void SetProgress(float percentage)
    {
        float right = (1 - percentage) * maxProgressBarRight;
        
        progressBar.SetRight(right);
    }
}
