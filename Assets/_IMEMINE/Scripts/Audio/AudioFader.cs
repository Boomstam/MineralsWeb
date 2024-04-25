using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioFader : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioSource fadeSamplePrefab;
        
    private AudioSource[] sources;
    
    private int CurrentNumSources => sources?.Length ?? 0;
    
    private float lastFadeVal = 0.5f;

    [Button]
    public void SetFadeValue(float fadeVal)
    {
        if(CurrentNumSources == 0)
        {
            Debug.LogWarning($"No fade samples playing, can't fade");
            return;
        }
        if(CurrentNumSources == 1)
        {
            Debug.LogWarning($"Only 1 fade sample playing, can't fade");
            return;
        }
        float percentagePerSource = 1f / (float)(CurrentNumSources - 1);
        
        int startSample = Mathf.FloorToInt(fadeVal / percentagePerSource);
        
        float remainder = fadeVal - (percentagePerSource * startSample);
        
        float remainderPercentage = remainder / percentagePerSource;
        
        for (int i = 0; i < CurrentNumSources; i++)
        {
            AudioSource audioSource = sources[i];
            
            float volume = 0;
            
            if (i == startSample)
                volume = 1 - remainderPercentage;
            if (i == startSample + 1)
                volume = remainderPercentage;
            
            audioSource.volume = volume;
        }
    }
    
    /*
     * Modes???
     * effects = micro organisms
     * ways of water: delays + circles => new mode!!!
     * voting, with progress bar
     * 
     * 1 create AudioFaders for delay => use multiplication for volume? Or map to new audio mixers?
     * check if effects mode still works
     * 2 implement circles on row number => slider on monitor
     * 3 program routine
     * 4 slider labels
     * vote timer!
     * optional: block routine - switch frontness of sliders
     * 
     */
    public void PlayFadeSamples(AudioClip[] fadeClips)
    {
        int numClips = fadeClips.Length;
        
        CreateAudioSources(numClips);
        
        for (int clipIndex = 0; clipIndex < numClips; clipIndex++)
        {
            sources[clipIndex].clip = fadeClips[clipIndex];
            sources[clipIndex].Play();
        }
        SetFadeValue(lastFadeVal);
    }
    
    private void CreateAudioSources(int numClips)
    {
        sources = new AudioSource[numClips];
        
        for (int clipIndex = 0; clipIndex < numClips; clipIndex++)
        {
            AudioSource newSource = Instantiate(fadeSamplePrefab);
            sources[clipIndex] = newSource;
            sources[clipIndex].name += $" {clipIndex}";
        }
    }
    
    public void StopAllPlayback(bool forceAll = false)
    {
        for (int sourceIndex = 0; sourceIndex < CurrentNumSources; sourceIndex++)
        {
            Destroy(sources[sourceIndex].gameObject);
        }

        sources = Array.Empty<AudioSource>();
    }
    
    public void SetAudioMixerGroup(AudioMixerGroup audioMixerGroup)
    {
        sources.ForEach(source => source.outputAudioMixerGroup = audioMixerGroup);
    }
}