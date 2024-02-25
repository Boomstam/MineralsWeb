using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreDataSO", menuName = "Minerals/ScoreDataSO", order = 1)]
public class ScoreDataSO : ScriptableObject
{
    public List<ScoreDataEntry> scoreDataEntries = new List<ScoreDataEntry>();
}

[Serializable]
public class ScoreDataEntry
{
    public float yPos;
    public ChoiceType choiceType;
    public int startMeasure;

    public ScoreDataEntry(float yPos, ChoiceType choiceType, int startMeasure)
    {
        this.yPos = yPos;
        this.choiceType = choiceType;
        this.startMeasure = startMeasure;
    }

    public override string ToString()
    {
        return $"{nameof(yPos)}: {yPos}, {nameof(choiceType)}: {choiceType}, {nameof(startMeasure)}: {startMeasure}";
    }
}

public enum ChoiceType
{
    None,
    A,
    B,
    C
}