using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class WebGLClientUI : MonoBehaviour
{
    [SerializeField] private Image connectionImage;
    [SerializeField] private TMP_InputField oscMessageInput;
    [SerializeField] private Button sendOSCButton;
    [SerializeField] private Color connectedColor;
    [SerializeField] private Color disconnectedColor;

    public ReactiveProperty<string> oscMessage = new ReactiveProperty<string>();

    public void SetConnection(bool connected)
    {
        connectionImage.color = connected ? connectedColor : disconnectedColor;
    }

    public void Start()
    {
        sendOSCButton.onClick.AsObservable().Subscribe(_ => oscMessage.SetValueAndForceNotify(oscMessageInput.text));
    }
}
