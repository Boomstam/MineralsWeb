using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorOverlay : MonoBehaviour
{
    [SerializeField] private Image overlayImage;
    [SerializeField] private float fadeTime;
    [SerializeField] private Color[] colors;

    private Color startColor;

    private float startTime;
    private Color previousColor;
    private Color targetColor;
    private int currentColorIndex;
    private bool isFading;

    private void Awake()
    {
        startColor = overlayImage.color;
    }

    private void OnEnable()
    {
        overlayImage.color = startColor;

        isFading = false;
        currentColorIndex = 0;
    }

    private void Update()
    {
        if(isFading)
            DoFade();
        else
            SetNewTargetColor();
    }

    private void DoFade()
    {
        float deltaTime = Time.time - startTime;

        float percentage = deltaTime / fadeTime;
        
        overlayImage.color = Color.Lerp(previousColor, targetColor, percentage);

        if(percentage >= 1)
            isFading = false;
    }

    private void SetNewTargetColor()
    {
        previousColor = overlayImage.color;
        
        startTime = Time.time;
        currentColorIndex++;

        if (currentColorIndex >= colors.Length)
            currentColorIndex = 0;

        targetColor = colors[currentColorIndex];
        
        isFading = true;
    }
}
