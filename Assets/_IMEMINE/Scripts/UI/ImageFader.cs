using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFader : MonoBehaviour
{
    [SerializeField] private Image fadeImageTemplate;
    
    private int CurrentNumImages => images?.Length ?? 0;

    private Image[] images;

    public void SetFadeVal(float fadeVal)
    {
        if(CurrentNumImages == 0)
        {
            Debug.LogWarning($"No images, can't fade");
            return;
        }
        if(CurrentNumImages == 1)
        {
            Debug.LogWarning($"Only 1 image, can't fade");
            return;
        }
        float percentagePerSource = 1f / (float)(CurrentNumImages - 1);

        int startSample = Mathf.FloorToInt(fadeVal / percentagePerSource);

        float remainder = fadeVal - (percentagePerSource * startSample);
        
        float remainderPercentage = remainder / percentagePerSource;
        
        for (int i = 0; i < CurrentNumImages; i++)
        {
            Image image = images[i];
            
            float alpha = 0;
            
            if (i == startSample)
                alpha = 1 - remainderPercentage;
            if (i == startSample + 1)
                alpha = remainderPercentage;

            Color currentColor = image.color;
            image.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
        }
    }
    
    public void DisplayFadeImages(Sprite[] sprites)
    {
        DeleteExistingImages();
        
        images = new Image[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            Image image = Instantiate(fadeImageTemplate, transform);
            image.sprite = sprites[i];
            image.gameObject.SetActive(true);
            images[i] = image;
        }
    }

    public void DeleteExistingImages()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        images = Array.Empty<Image>();
    }
}
