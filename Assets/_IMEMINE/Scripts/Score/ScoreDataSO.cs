using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreDataSO", menuName = "Minerals/ScoreDataSO", order = 1)]
public class ScoreDataSO : ScriptableObject
{
    public List<ScoreDataEntry> scoreDataEntries = new List<ScoreDataEntry>();

    [Button]
    public ScoreDataEntry GetClosestEntryForMeasure(int measure)
    {
        for (int i = 0; i < scoreDataEntries.Count; i++)
        {
            ScoreDataEntry scoreDataEntry = scoreDataEntries[i];

            if (scoreDataEntry.startMeasure > measure)
            {
                if (i == 0)
                {
                    Debug.Log($"Closest entry for {measure}: {scoreDataEntry}");
                    return scoreDataEntry;
                }
                else
                {
                    Debug.Log($"Closest entry for {measure}: {scoreDataEntries[i - 1]}");
                    return scoreDataEntries[i - 1];
                }
            }
        }

        return scoreDataEntries.Last();
    }
    
    [Button]
    private void CheckForDoubles()
    {
        foreach (ScoreDataEntry scoreDataEntry in scoreDataEntries)
        {
            if (scoreDataEntries.Any(entry => entry != scoreDataEntry &&
                                                  entry.choiceType == scoreDataEntry.choiceType
                                                  && entry.startMeasure == scoreDataEntry.startMeasure))
            {
                Debug.LogError($"Found double for {scoreDataEntry}!");
            }
        }
        Debug.Log($"Checked for doubles");
    }
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