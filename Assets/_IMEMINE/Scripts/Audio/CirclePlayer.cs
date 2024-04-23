using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class CirclePlayer : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioClip[] circleClips;
    [SerializeField] private AudioSource lowAudioSource;
    [SerializeField] private AudioSource midAudioSource;
    [SerializeField] private AudioSource highAudioSource;
    [SerializeField] private AudioMixerGroup circlesMixer;
    [SerializeField] private AudioMixerGroup circlesLowMixer;
    [SerializeField] private AudioMixerGroup circlesMidMixer;
    [SerializeField] private AudioMixerGroup circlesHighMixer;

    private const int numSources = 3;
    private int numDifferentSounds => circleClips.Length / numSources;
    private AudioSource[] sources => new[] { lowAudioSource, midAudioSource, highAudioSource };

    private bool lastShouldSpatialize;
    private float lastVolume = -1f;
    private float lastLeftRightBalance = -1f;

    // TODO implement max playing time to prevent endless playing
    // private void Update()
    // {
    //     try
    //     {
    //         bool playCircles = Instances.NetworkedMonitor.playCircles;
    //     }
    //     catch (Exception e)
    //     {
    //         return;
    //     }
    //     
    //     if (Instances.NetworkedMonitor.playCircles)
    //     {
    //         if (lowAudioSource.isPlaying == false)
    //         {
    //             StartPlayback();
    //         } 
    //     }
    //     else
    //     {
    //         if (lowAudioSource.isPlaying)
    //         {
    //             StopPlayback();
    //         }
    //     }
    //
    //     if (lastVolume != Instances.NetworkedMonitor.volume || 
    //         lastLeftRightBalance != Instances.NetworkedMonitor.leftRightBalance ||
    //         lastShouldSpatialize != Instances.NetworkedMonitor.shouldSpatialize)
    //     {
    //         SetVolume();
    //         
    //         lastVolume = Instances.NetworkedMonitor.volume;
    //         lastLeftRightBalance = Instances.NetworkedMonitor.leftRightBalance;
    //         lastShouldSpatialize = Instances.NetworkedMonitor.shouldSpatialize;
    //     }
    // }

    public void StartPlayback()
    {
        if(lowAudioSource.isPlaying)
            return;
        
        int soundIndex = Instances.SeatNumber % numDifferentSounds;
        
        Debug.Log($"StartPlayback soundIndex: {soundIndex}, numDifferentSounds: {numDifferentSounds}, CurrentNumSources: {numSources}");
        
        lowAudioSource.clip = circleClips[(soundIndex * numSources)];
        midAudioSource.clip = circleClips[(soundIndex * numSources) + 1];
        highAudioSource.clip = circleClips[(soundIndex * numSources) + 2];
        
        lowAudioSource.Play();
        midAudioSource.Play();
        highAudioSource.Play();
    }
    
    public void StopPlayback()
    {
        if(lowAudioSource.isPlaying == false)
            return;
        
        Debug.Log($"StopPlayback");
        
        lowAudioSource.Stop();
        midAudioSource.Stop();
        highAudioSource.Stop();
    }
    
    private void SetVolume()
    {
        float volume = Instances.NetworkedMonitor.volume;
        
        float spatializeMod = 1;
        
        if (Instances.NetworkedMonitor.shouldSpatialize)
        {
            float leftness = (float)(Instances.SeatNumber % Instances.NetworkedMonitor.seatsPerRow) / (float)Instances.NetworkedMonitor.seatsPerRow;
            float deltaPos = Mathf.Abs(leftness - Instances.NetworkedMonitor.leftRightBalance);

            spatializeMod = Mathf.Clamp01(1 - deltaPos);
            
            Debug.Log($"leftness: {leftness}, deltaPos {deltaPos}, spatializeMod {spatializeMod}");
        }

        float resultVolume = volume * spatializeMod;
        float scaledVolume = Mathf.Log(resultVolume) * 20;

        bool couldSetFloat = circlesMixer.audioMixer.SetFloat("Circles", scaledVolume);
        
        Debug.Log($"SetVolume, volume {volume}, spatializeMod {spatializeMod}, result {resultVolume}, scaledVolume {scaledVolume}, couldSetFloat {couldSetFloat}");
    }
    
    [Button]
    public void SetFadeValue(float fadeVal)
    {
        float percentagePerSource = 1f / (float)(numSources - 1);

        int startSample = Mathf.FloorToInt(fadeVal / percentagePerSource);

        float remainder = fadeVal - (percentagePerSource * startSample);
        
        float remainderPercentage = remainder / percentagePerSource;
        
        for (int i = 0; i < numSources; i++)
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
}
