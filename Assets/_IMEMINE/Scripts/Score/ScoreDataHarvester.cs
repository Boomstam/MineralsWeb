using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScoreDataHarvester : MonoBehaviour
{
    [SerializeField] private KeyCode addButton;
    [SerializeField] private ChoiceType choiceType;
    [SerializeField] private int startMeasure;
    [SerializeField] private ScoreDataSO scoreDataSO;

    private void Update()
    {
        if(Input.GetKeyDown(addButton))
            HarvestScoreData();
    }

    [Button]
    private void HarvestScoreData()
    {
        ScoreDataEntry scoreDataEntry = new ScoreDataEntry(transform.position.y, choiceType, startMeasure);
        
        scoreDataSO.scoreDataEntries.Add(scoreDataEntry);
        
        CycleChoiceType();
        
        Debug.Log($"Harvested {scoreDataEntry}");
    }

    private void CycleChoiceType()
    {
        if (choiceType == ChoiceType.A)
            choiceType = ChoiceType.B;
        
        if (choiceType == ChoiceType.B)
            choiceType = ChoiceType.C;
        
        if (choiceType == ChoiceType.C)
            choiceType = ChoiceType.A;
    }
}
