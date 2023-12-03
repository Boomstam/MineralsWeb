using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class OSCClientUI : MonoBehaviour
{
    [SerializeField] private Image connectionImage;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private Color connectedColor;
    [SerializeField] private Color disconnectedColor;

    public void SetConnection(bool connected)
    {
        connectionImage.color = connected ? connectedColor : disconnectedColor;
    }

    public void SetMessage(string message)
    {
        textMeshProUGUI.text = message;
    }
}
