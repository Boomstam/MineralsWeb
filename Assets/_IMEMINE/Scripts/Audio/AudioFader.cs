using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

public class AudioFader : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    // [SerializeField] private AudioMixerGroup master;
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
        
        if (CurrentNumSources == 3)
        {
            // Hardcode
            // sources[0].pitch = 2f;
            // sources[1].pitch = 1f;
            // sources[2].pitch = 0.5f;
            //
            // sources[0].volume *= 0.8f;
        }
    }

    [Button]
    private void PlayTestSamples()
    {
        AudioClip[] testFadeClips = new[]
        {
            audioManager.GetClip(ClipType.MineralsA),
            audioManager.GetClip(ClipType.MineralsB),
            audioManager.GetClip(ClipType.MineralsC),
        };
        
        PlayFadeSamples(testFadeClips);
    }
    
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
            sources[clipIndex] = Instantiate(fadeSamplePrefab);
            sources[clipIndex].name += $" {clipIndex}";
        }
    }

    public void StopAllPlayback()
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