using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWithConnection : MonoBehaviour
{
    [SerializeField] protected Image connectionImage;
    [SerializeField] protected Color connectedColor = Color.green;
    [SerializeField] protected Color disconnectedColor = Color.red;
    
    public void SetConnection(bool connected)
    {
        connectionImage.color = connected ? connectedColor : disconnectedColor;
    }
}
