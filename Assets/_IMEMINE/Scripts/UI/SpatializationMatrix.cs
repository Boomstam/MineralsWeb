using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SpatializationMatrix : MonoBehaviour
{
    [SerializeField] private Toggle[] toggles;
    [SerializeField] private QuadrantRange[] ranges;

    private void Start()
    {
        if(toggles.Length != ranges.Length)
            Debug.LogError($"The amount of toggles: {toggles} doesn't equal the number of ranges: {ranges.Length}");
        
        for (int i = 0; i < toggles.Length; i++)
        {
            int quadrant = i;
            
            toggles[i].OnValueChangedAsObservable().Skip(1).Subscribe(toggleOn =>
            {
                if (toggleOn)
                    OnQuadrantEnabled(quadrant);
                else
                    OnAllQuadrantsDisabled();
            });
        }
    }
    
    private void OnQuadrantEnabled(int quadrant)
    {
        Debug.Log($"OnQuadrantEnabled: {quadrant}");
        DisableAllToggles();
        
        toggles[quadrant].SetIsOnWithoutNotify(true);

        Instances.NetworkedAppState.EnableQuadrantRanges(ranges[quadrant].seatMinMax, ranges[quadrant].rowMinMax);
    }
    
    private void OnAllQuadrantsDisabled()
    {
        Debug.Log($"OnAllQuadrantsDisabled");
        
        Instances.NetworkedAppState.DisableQuadrantsMode();
    }
    
    private void DisableAllToggles()
    {
        toggles.ForEach(toggle => toggle.SetIsOnWithoutNotify(false));
    }
}

[System.Serializable]
public class QuadrantRange
{
    public Vector2 seatMinMax;
    public Vector2 rowMinMax;
}
