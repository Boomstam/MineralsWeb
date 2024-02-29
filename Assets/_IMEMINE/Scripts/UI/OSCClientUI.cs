using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class OSCClientUI : UIWithConnection
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private TMP_InputField addressInputField;
    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private Button sendMessageButton;

    private void Start()
    {
        sendMessageButton.onClick.AsObservable()
            .Subscribe(_ => Instances.OSCManager.SendOSCMessage(addressInputField.text, messageInputField.text));
    }

    public void SetMessage(string message)
    {
        textMeshProUGUI.text = message;
    }
}
