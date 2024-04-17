using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionManager : MonoBehaviour
{
    public AuraTextDisplay auraTextDisplay;

    public void GoToAuraText(int index)
    {
        auraTextDisplay.GoToText(index);
    }
}
