using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class TutorialDoubleFader : MonoBehaviour
{
    [SerializeField] private AudioFader[] audioFaders;
    [SerializeField] private AudioMixerGroup lowAudioMixerGroup;
    [SerializeField] private AudioMixerGroup highAudioMixerGroup;

    [SerializeField] private AudioClip[] clipTypesLow;
    [SerializeField] private AudioClip[] clipTypesHigh;
    
    private const float minVolume = 0.01f;
    
    private AudioSource[] sources;
    
    private float lastFadeVal = 0.5f;

    [Button]
    public void PlayFadeSamples()
    {
        int type = Random.Range(0, clipTypesLow.Length / 2);
        
        Instances.WebGLClientUI.SetTutorialSliderTexts(type);
        
        AudioClip[] lowFadeClips = { clipTypesLow[type * 2], clipTypesLow[type * 2 + 1] };
        AudioClip[] highFadeClips = { clipTypesHigh[type * 2], clipTypesHigh[type * 2 + 1] };
        
        audioFaders[0].PlayFadeSamples(lowFadeClips);
        audioFaders[1].PlayFadeSamples(highFadeClips);
        
        audioFaders[0].SetAudioMixerGroup(lowAudioMixerGroup);
        audioFaders[1].SetAudioMixerGroup(highAudioMixerGroup);
    }
    
    public void SetFadeValHighLow(float fadeVal)
    {
        AudioMixerGroup[] audioMixerGroups = { lowAudioMixerGroup, highAudioMixerGroup };
        
        // https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
        if (fadeVal <= minVolume)
            fadeVal = minVolume;
        
        int numAudioMixerGroups = audioMixerGroups.Length;
        float percentagePerSource = 1f / (float)(numAudioMixerGroups - 1);
        
        int startSample = Mathf.FloorToInt(fadeVal / percentagePerSource);
        
        float remainder = fadeVal - (percentagePerSource * startSample);
        
        float remainderPercentage = remainder / percentagePerSource;
        
        for (int i = 0; i < numAudioMixerGroups; i++)
        {
            AudioMixerGroup audioMixerGroup = audioMixerGroups[i];

            float volume = minVolume;
            
            if (i == startSample)
                volume = 1 - remainderPercentage;
            if (i == startSample + 1)
                volume = remainderPercentage;

            float scaledVolume = Mathf.Log(volume) * 20;
            
            audioMixerGroup.audioMixer.SetFloat(NameForIndex(i), scaledVolume);
        }
    }
    
    private string NameForIndex(int index)
    {
        if (index == 0)
            return $"Low";
        
        return $"High";
    }
    
    public void SetFadeValDistortion(float fadeVal)
    {
        audioFaders.ForEach(audioFader => audioFader.SetFadeValue(fadeVal));
    }
    
    public void StopAllPlayback()
    {
        audioFaders.ForEach(audioFader => audioFader.StopAllPlayback());
    }
}