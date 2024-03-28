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

public class DoubleFader : MonoBehaviour
{
    public bool sound1;
    
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioFader[] audioFaders;
    [SerializeField] private AudioMixerGroup lowAudioMixerGroup;
    [SerializeField] private AudioMixerGroup midAudioMixerGroup;
    [SerializeField] private AudioMixerGroup highAudioMixerGroup;

    [SerializeField] private AudioClip[] clipTypesLow1;
    [SerializeField] private AudioClip[] clipTypesMid1;
    [SerializeField] private AudioClip[] clipTypesHigh1;
    
    [SerializeField] private AudioClip[] clipTypesLow2;
    [SerializeField] private AudioClip[] clipTypesMid2;
    [SerializeField] private AudioClip[] clipTypesHigh2;
    
    [SerializeField] private AudioClip[] clipTypesLow;
    [SerializeField] private AudioClip[] clipTypesMid;
    [SerializeField] private AudioClip[] clipTypesHigh;
    // [SerializeField] private AudioMixerGroup master;
    
    private const float minVolume = 0.01f;
    
    private AudioSource[] sources;

    private int CurrentNumSources => sources?.Length ?? 0;

    private float lastFadeVal = 0.5f;
    
    [Button]
    private void PlayTestSamples()
    {
        
    }
    
    [Button]
    public void PlayFadeSamples()
    {
        // if()

        int type = Random.Range(0, clipTypesLow.Length / 2);

        AudioClip[] lowFadeClips = { clipTypesLow[type * 2], clipTypesLow[type * 2 + 1] };
        AudioClip[] midFadeClips = { clipTypesMid[type * 2], clipTypesMid[type * 2 + 1] };
        AudioClip[] highFadeClips = { clipTypesHigh[type * 2], clipTypesHigh[type * 2 + 1] };
        
        audioFaders[0].PlayFadeSamples(lowFadeClips);
        audioFaders[1].PlayFadeSamples(midFadeClips);
        audioFaders[2].PlayFadeSamples(highFadeClips);
        
        audioFaders[0].SetAudioMixerGroup(lowAudioMixerGroup);
        audioFaders[1].SetAudioMixerGroup(midAudioMixerGroup);
        audioFaders[2].SetAudioMixerGroup(highAudioMixerGroup);
    }

    public void SetFadeValHighLow(float fadeVal)
    {
        AudioMixerGroup[] audioMixerGroups = { lowAudioMixerGroup, midAudioMixerGroup, highAudioMixerGroup };

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

            //float scaledVolume = (volume * 80) - 80;
            float scaledVolume = Mathf.Log(volume) * 20;
            
            audioMixerGroup.audioMixer.SetFloat(NameForIndex(i), scaledVolume);
        }
    }

    private string NameForIndex(int index)
    {
        if (index == 0)
            return $"Low";
        if (index == 1)
            return $"Mid";
        
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