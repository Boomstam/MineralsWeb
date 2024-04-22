using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup master;
    public AudioFader audioFader;
    // public AudioFader audioFader;
    public DoubleFader doubleFader;
    public TutorialDoubleFader tutorialDoubleFader;
    public CirclePlayer circlePlayer;
    public DelayPlayer delayPlayer;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioDistortionFilter audioDistortionFilter;
    [SerializeField] private AudioChorusFilter audioChorusFilter;
    [SerializeField] private AudioEchoFilter audioEchoFilter;
    [SerializeField] private AudioReverbFilter audioReverbFilter;
    [SerializeField] private AudioLowPassFilter audioLowPassFilter;
    [SerializeField] private AudioHighPassFilter audioHighPassFilter;
    [SerializeField] private AudioClip[] microOrganismsClips;
    [SerializeField] private AudioClip[] waysOfWaterClips;

    [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;

    private Coroutine fadeRoutine;

    public void OnQuadrantsModeEnabled(Vector2 seatMinMax, Vector2 rowMinMax)
    {
        if(fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        
        bool shouldSound = Instances.SeatNumber >= seatMinMax.x && 
                           Instances.SeatNumber <= seatMinMax.y &&
                           Instances.RowNumber >= rowMinMax.x &&
                           Instances.RowNumber <= rowMinMax.y;
        
        // SetUnscaledVolume(shouldSound ? 1 : 0);
        fadeRoutine = StartCoroutine(DoFade(shouldSound));
    }
    
    public void OnQuadrantsModeDisabled()
    {
        if(fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(DoFade(false));
    }
    
    private IEnumerator DoFade(bool fadeIn)
    {
        float startTime = Time.time;
        
        float fadeTime = fadeIn ? fadeInTime : fadeOutTime;
        
        float timeElapsed = 0;
        while (timeElapsed < fadeTime)
        {
            float currentPercentage = timeElapsed / fadeTime;
            
            SetUnscaledVolume(currentPercentage);
            
            timeElapsed = Time.time - startTime;
            yield return 0;
        }
        SetUnscaledVolume(fadeIn ? 1 : 0);
    }
    
    private void SetUnscaledVolume(float volume)
    {
        // Debug.Log($"SetUnscaledVolume from fade: {volume}");
        
        float scaledVolume = Mathf.Log(volume) * 20;
        
        master.audioMixer.SetFloat($"Master", scaledVolume);
    }
    
    public void PlayFadeSamples(AudioClip fadeClips)
    {
        // AudioClip[] fadeClips = GetClips(clipTypes);
        
        // audioFader.PlayFadeSamples(fadeClips);
    }
    
    public void SetFadeVal(float fadeVal)
    {
        audioFader.SetFadeValue(fadeVal);
    }
    
    public void StopAllPlayback()
    {
        audioSource.Stop();
        
        audioFader.StopAllPlayback();
        doubleFader.StopAllPlayback();
        tutorialDoubleFader.StopAllPlayback();
    }
    
    public void ResetAllFx()
    {
        audioDistortionFilter.enabled = false;
        audioChorusFilter.enabled = false;
        audioEchoFilter.enabled = false;
        audioReverbFilter.enabled = false;
        audioLowPassFilter.enabled = false;
        audioHighPassFilter.enabled = false;
    }
}