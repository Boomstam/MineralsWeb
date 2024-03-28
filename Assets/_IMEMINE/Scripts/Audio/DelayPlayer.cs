using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class DelayPlayer : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioFader faderTemplate;
    [SerializeField] private AudioClip[] delayClips;
    [SerializeField] private AudioSource delaySamplePrefab;
    [SerializeField] private AudioClip testDelaySample;
    
    [SerializeField] private AudioMixerGroup delaysLowMixer;
    [SerializeField] private AudioMixerGroup delaysMidMixer;
    [SerializeField] private AudioMixerGroup delaysHighMixer;

    private const int numSources = 3;
    private int numDifferentSounds => delayClips.Length / numSources;
    
    private float lastPlayTime = -1f;
    private float currentRandomWaitTime = 0.5f;

    private float timeSinceLastPlay => Time.time - lastPlayTime;
    
    private void Update()
    {
        try
        {
            bool playDelays = Instances.NetworkedMonitor.shouldPlayDelays;
        }
        catch (Exception e)
        {
            return;
        }
        
        if (Instances.NetworkedMonitor.shouldPlayDelays)
        {
            if (timeSinceLastPlay + currentRandomWaitTime > Instances.NetworkedMonitor.delayIntervalLength)
            {
                PlayRandomDelay();
                
                currentRandomWaitTime = Random.Range(0, Instances.NetworkedMonitor.delayIntervalLength);
                lastPlayTime = Time.time;
            }
        }
    }
    
    // [Button]
    // public void PlayTestDelay(float delayTime)
    // {
    //     PlayWithDelay(testDelaySample, delayTime, 0.85f);
    // }

    private void PlayRandomDelay()
    {
        int nextIndex = Random.Range(0, numDifferentSounds);

        AudioClip[] clips = new[]
        {
            delayClips[(nextIndex * numSources)],
            delayClips[(nextIndex * numSources) + 1],
            delayClips[(nextIndex * numSources) + 2],
        };

        float delayTime =
            Random.Range(Instances.NetworkedMonitor.minDelayTime, Instances.NetworkedMonitor.maxDelayTime);
        
        PlayWithDelay(clips, delayTime, 0.85f);
    }
    
    private void PlayWithDelay(AudioClip[] audioClips, float delayTime, float feedback)
    {
        StartCoroutine(PlayWithDelayRoutine(audioClips, delayTime, feedback));
    }

    // createa audio mixer groups for audio fader
    // dont break double fader tough, make dupliacte or child
    // pass in clips here
    // random generate before?
    private IEnumerator PlayWithDelayRoutine(AudioClip[] audioClips, float delayTime, float feedback)
    {
        if(feedback is < 0 or > 1)
            Debug.LogError($"Feedback {feedback} not between 0 and 1");
        
        List<AudioSource> createdSources = new List<AudioSource>();

        float clipLength = audioClips.First().length;
        
        float volume = 1;

        createdSources.AddRange(NewAudioSourcesWithClips(audioClips, volume));

        float fallOff = 1 - feedback;
        
        while (volume > 0)
        {
            yield return new WaitForSeconds(delayTime);
        
            volume = volume - fallOff;
            
            createdSources.AddRange(NewAudioSourcesWithClips(audioClips, volume));
        }
        
        yield return new WaitForSeconds(clipLength);
        
        for (int sourceIndex = 0; sourceIndex < createdSources.Count; sourceIndex++)
        {
            AudioSource source = createdSources[sourceIndex];
            
            source.Stop();
            Destroy(source.gameObject);
        }
    }

    // which clips though?
    private AudioSource[] NewAudioSourcesWithClips(AudioClip[] audioClips, float volume)
    {
        AudioSource lowSource = Instantiate(delaySamplePrefab);
        AudioSource midSource = Instantiate(delaySamplePrefab);
        AudioSource highSource = Instantiate(delaySamplePrefab);

        lowSource.outputAudioMixerGroup = delaysLowMixer;
        midSource.outputAudioMixerGroup = delaysMidMixer;
        highSource.outputAudioMixerGroup = delaysHighMixer;

        lowSource.clip = audioClips[0];
        midSource.clip = audioClips[1];
        highSource.clip = audioClips[2];
        
        lowSource.volume = volume;
        midSource.volume = volume;
        highSource.volume = volume;
        
        lowSource.Play();
        midSource.Play();
        highSource.Play();

        return new []{ lowSource, midSource, highSource };
    }
}
/*
 * private IEnumerator PlayWithDelayRoutine(AudioClip audioClip, float delayTime, float feedback)
    {
        if(feedback is < 0 or > 1)
            Debug.LogError($"Feedback {feedback} not between 0 and 1");
        
        List<AudioSource> createdSources = new List<AudioSource>();
        
        float volume = 1;

        createdSources.Add(NewAudioSourceWithClip(audioClip, volume));

        float fallOff = 1 - feedback;
        
        while (volume > 0)
        {
            yield return new WaitForSeconds(delayTime);
        
            volume = volume - fallOff;
            
            createdSources.Add(NewAudioSourceWithClip(audioClip, volume));
        }
        
        yield return new WaitForSeconds(audioClip.length);
        
        for (int sourceIndex = 0; sourceIndex < createdSources.Count; sourceIndex++)
        {
            AudioSource source = createdSources[sourceIndex];
            
            Destroy(source.gameObject);
        }
    }
    
    private AudioSource NewAudioSourceWithClip(AudioClip audioClip, float volume)
    {
        AudioSource audioSource = Instantiate(delaySamplePrefab);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        return audioSource;
    }
    
     */