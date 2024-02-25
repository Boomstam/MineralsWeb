using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private float distanceBetweenPages;
    [SerializeField] private Sprite[] pages;
    [SerializeField] private SpriteRenderer pagePrefab;
    
    private Vector3 firstPagePos => pagePrefab.transform.position;

    private void Start()
    {
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
}
