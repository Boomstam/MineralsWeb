using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;

public class TypeAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private string cursorCharacter;
    [SerializeField] private float characterTypeSpeed;
    [SerializeField] private float spaceWaitTime;
    [SerializeField] private float periodWaitTime;

    private Coroutine typingRoutine;

    [Button]
    public void TypeAnimate(string text)
    {
        if(typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(DoTyping(text));
    }
    
    private IEnumerator DoTyping(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            string partToShow = text.Substring(0, i + 1);

            bool isShowingCursor = true;
            float secondsToWait = characterTypeSpeed;

            if (partToShow.Last() == '.')
            {
                isShowingCursor = false;
                secondsToWait = periodWaitTime;
            }
            else if (partToShow.Last() == ' ')
            {
                secondsToWait = spaceWaitTime;
            }
            
            if (i == text.Length - 1)
                isShowingCursor = false;
            
            textComponent.text = $"{partToShow}{(isShowingCursor ? cursorCharacter : "")}";
            
            yield return new WaitForSeconds(secondsToWait);
        }
    }
}
