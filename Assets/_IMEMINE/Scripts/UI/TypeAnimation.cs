using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class TypeAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private string cursorCharacter;
    [SerializeField] private float characterTypeSpeed;
    [SerializeField] private float spaceWaitTime;
    [SerializeField] private float periodWaitTime;
    [SerializeField] private float percentageOfTypingSounds;
    [SerializeField] private AudioClip[] typingClips;
    [SerializeField] private AudioSource typingSoundSource;

    private Coroutine typingRoutine;

    [Button]
    public void TypeAnimate(string text)
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);
        
        typingRoutine = StartCoroutine(DoTyping(text));
    }

    private IEnumerator DoTyping(string text)
    {
        Debug.Log($"Do typing: {text}");
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
            else
            {
                PlayNextRandomTypingSound();
            }

            if (i == text.Length - 1)
                isShowingCursor = false;

            textComponent.text = $"{partToShow}{(isShowingCursor ? cursorCharacter : "")}";

            yield return new WaitForSeconds(secondsToWait);
        }
    }

    private void PlayNextRandomTypingSound()
    {
        if(Random.value > percentageOfTypingSounds)
            return;
        
        int randIndex = Random.Range(0, typingClips.Length);
        AudioClip clip = typingClips[randIndex];

        typingSoundSource.Stop();
        
        typingSoundSource.clip = clip;
        float randPitch = 0.5f + Random.value;
        typingSoundSource.pitch = randPitch;
        
        typingSoundSource.Play();
    }
}
