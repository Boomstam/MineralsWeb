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

    private int numDifferentSounds => circleClips.Length / 3;
    private AudioSource[] sources => new[] { lowAudioSource, midAudioSource, highAudioSource };
    private const int CurrentNumSources = 3;

    // TODO implement max playing time to prevent endless playing
    private float lastVolume = -1f;

    private void Update()
    {
        try
        {
            bool playCircles = Instances.NetworkedMonitor.playCircles;
        }
        catch (Exception e)
        {
            return;
        }
        
        if (Instances.NetworkedMonitor.playCircles)
        {
            if (lowAudioSource.isPlaying == false)
            {
                StartPlayback();
            } 
        }
        else
        {
            if (lowAudioSource.isPlaying)
            {
                StopPlayback();
            }
        }

        if (lastVolume != Instances.NetworkedMonitor.volume)
        {
            SetVolume(Instances.NetworkedMonitor.volume);
            
            lastVolume = Instances.NetworkedMonitor.volume;
        }
    }

    private void StartPlayback()
    {
        int soundIndex = Instances.SeatNumber % numDifferentSounds;
        
        Debug.Log($"StartPlayback soundIndex: {soundIndex}, numDifferentSounds: {numDifferentSounds}, CurrentNumSources: {CurrentNumSources}");

        lowAudioSource.clip = circleClips[(soundIndex * CurrentNumSources)];
        midAudioSource.clip = circleClips[(soundIndex * CurrentNumSources) + 1];
        highAudioSource.clip = circleClips[(soundIndex * CurrentNumSources) + 2];
        
        lowAudioSource.Play();
        midAudioSource.Play();
        highAudioSource.Play();
    }
    
    private void StopPlayback()
    {
        Debug.Log($"StopPlayback");
        
        lowAudioSource.Stop();
        midAudioSource.Stop();
        highAudioSource.Stop();
    }
    
    private void SetVolume(float volume)
    {
        circlesMixer.audioMixer.SetFloat("Circles", volume);
    }
    
    [Button]
    public void SetFadeValue(float fadeVal)
    {
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
}
