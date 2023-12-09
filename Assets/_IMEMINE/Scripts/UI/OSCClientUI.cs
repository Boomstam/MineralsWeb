using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class OSCClientUI : UIWithConnection
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    public void SetMessage(string message)
    {
        textMeshProUGUI.text = message;
    }
}
