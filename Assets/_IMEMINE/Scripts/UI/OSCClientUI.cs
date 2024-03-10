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
    [SerializeField] private TMP_InputField ipAddress;
    [SerializeField] private TMP_InputField port;
    [SerializeField] private Button sendMessageButton;

    private void Start()
    {
        sendMessageButton.onClick.AsObservable()
            .Subscribe(_ => Instances.OSCManager.SendOSCMessage(addressInputField.text, messageInputField.text));
        
        ipAddress.onValueChanged.AsObservable().Subscribe(val => OSCManager.IPAddress = val);
        
        port.onValueChanged.AsObservable().Subscribe(val =>
        {
            if (int.TryParse(val, out int parsedPort))
                OSCManager.Port = parsedPort;
        });
        
        if(PlayerPrefs.HasKey(OSCManager.IPPlayerPrefsKey))
            ipAddress.text = PlayerPrefs.GetString(OSCManager.IPPlayerPrefsKey);
        
        if(PlayerPrefs.HasKey(OSCManager.PortPlayerPrefsKey))
            port.text = PlayerPrefs.GetInt(OSCManager.PortPlayerPrefsKey).ToString();
    }

    public void SetMessage(string message)
    {
        textMeshProUGUI.text = message;
    }
}
