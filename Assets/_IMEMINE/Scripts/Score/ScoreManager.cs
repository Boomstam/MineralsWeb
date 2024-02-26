using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private float distanceBetweenPages;
    [SerializeField] private float scrollTime;
    [SerializeField] private Sprite[] pages;
    [SerializeField] private SpriteRenderer pagePrefab;
    [SerializeField] private ScoreHighlighter highlighterPrefab;
    [SerializeField] private ScoreDataSO scoreDataSO;
    
    private Vector3 firstPagePos => pagePrefab.transform.position;
    private RectTransform rectTransform;

    private IDisposable scrollRoutine;
    private float scrollStartTime;
    private float scrollStart;
    private float scrollEnd;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        DestroyAllChildren();
        
        CreatePages();
    }

    private void DestroyAllChildren()
    {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    private void CreatePages()
    {
        float currentYPos = firstPagePos.y;
        
        foreach (Sprite pageSprite in pages)
        {
            SpriteRenderer page = Instantiate(pagePrefab, transform, false);
            page.transform.position = new Vector3(firstPagePos.x, currentYPos, firstPagePos.z);

            page.sprite = pageSprite;
            
            currentYPos -= distanceBetweenPages;
        }
    }

    [Button]
    public void GoToMeasure(int measure)
    {
        ScoreDataEntry scoreDataEntry = scoreDataSO.GetClosestEntryForMeasure(measure);

        float targetY = scoreDataEntry.yPos * -1f;
        
        if (scrollRoutine != null)
        {
            if (scrollEnd == targetY)
            {
                Debug.Log($"WAS EQUAL");
                return;
            }
        }

        scrollEnd = scoreDataEntry.yPos * -1f;
        
        scrollStart = rectTransform.anchoredPosition.y;

        scrollStartTime = Time.time;
        
        scrollRoutine?.Dispose();
        
        scrollRoutine = Observable.FromCoroutine(DoScroll).Subscribe();
    }

    private IEnumerator DoScroll()
    {
        while (Time.time - scrollStartTime < scrollTime)
        {
            float timeMod = (Time.time - scrollStartTime) / scrollTime;
            
            float newY = Mathf.Lerp(scrollStart, scrollEnd, timeMod);
            
            rectTransform.anchoredPosition = new Vector2(0, newY);
            
            yield return 0;
        }
        rectTransform.anchoredPosition = new Vector2(0, scrollEnd);
        scrollRoutine = null;
    }

    [Button]
    public void HighlightChoice(ChoiceType choiceType)
    {
        if(choiceType == ChoiceType.None)
            return;
        
        ScoreHighlighter[] currentHighlighters = GetComponentsInChildren<ScoreHighlighter>();

        for (int i = 0; i < currentHighlighters.Length; i++)
        {
            Destroy(currentHighlighters[i].gameObject);
        }

        ScoreDataEntry[] entriesToHighlight = scoreDataSO.scoreDataEntries.Where(entry => entry.choiceType == choiceType).ToArray();

        foreach (ScoreDataEntry entryToHighlight in entriesToHighlight)
        {
            ScoreHighlighter scoreHighlighter = Instantiate(highlighterPrefab, transform, true);

            scoreHighlighter.transform.position = new Vector3(
                scoreHighlighter.transform.position.x,
                entryToHighlight.yPos, 
                scoreHighlighter.transform.position.z);
        }
    }
}
