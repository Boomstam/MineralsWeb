using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
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
    
    [SerializeField] private AudioMixerGroup delaysLowMixer;
    [SerializeField] private AudioMixerGroup delaysMidMixer;
    [SerializeField] private AudioMixerGroup delaysHighMixer;
    [SerializeField] private float chanceOfPlaying;

    private const int numSources = 3;
    private int numDifferentSounds => delayClips.Length / numSources;
    
    private float lastPlayTime = -1f;
    private float currentRandomWaitTime = 0.5f;

    private float timeSinceLastPlay => Time.time - lastPlayTime;
    private List<AudioSource> createdSources = new List<AudioSource>();
    
    private int randomIndex;
    
    private void Start()
    {
        randomIndex = Random.Range(0, numDifferentSounds);
    }
    
    private void Update()
    {
        if(InstanceFinder.IsOffline)
            return;
        
        try
        {
            bool playDelays = Instances.NetworkedAppState.shouldPlayDelays;
        }
        catch (Exception e)
        {
            return;
        }
        
        if (Instances.NetworkedAppState.shouldPlayDelays)
        {
            if (timeSinceLastPlay + currentRandomWaitTime > Instances.NetworkedMonitor.delayIntervalLength)
            {
                PlayRandomDelay();
                
                currentRandomWaitTime = Random.Range(0, Instances.NetworkedMonitor.delayIntervalLength);
                lastPlayTime = Time.time;
            }
        }
    }
    
    [Button]
    private void PlayRandomDelay()
    {
        Debug.Log($"Play random delay, nextIndex: {randomIndex}");
        
        if(Random.value > chanceOfPlaying)
        {
            Debug.Log($"Didn't play, didn't pass chance");
            return;
        }
        
        AudioClip[] clips = new[]
        {
            delayClips[(randomIndex * numSources)],
            delayClips[(randomIndex * numSources) + 1],
            delayClips[(randomIndex * numSources) + 2],
        };
        
        float delayTime =
            Random.Range(Instances.NetworkedMonitor.minDelayTime, Instances.NetworkedMonitor.maxDelayTime);
        
        PlayWithDelay(clips, delayTime, 0.85f);
    }
    
    private void PlayWithDelay(AudioClip[] audioClips, float delayTime, float feedback)
    {
        StartCoroutine(PlayWithDelayRoutine(audioClips, delayTime, feedback));
    }
    
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
        
        StopAllPlaybackAndRemoveSources();
    }

    private AudioSource[] NewAudioSourcesWithClips(AudioClip[] audioClips, float volume)
    {
        Debug.Log($"NewAudioSourcesWithClips");
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
        
        this.RunAfterFrames(3, () =>
        {
            lowSource.Play();
            midSource.Play();
            highSource.Play();
            
            Debug.Log($"low: {lowSource.clip}, mid: {midSource.clip}, high: {highSource.clip}");
            
            // lowSource.PlayOneShot(audioClips[0]);
            // midSource.PlayOneShot(audioClips[1]);
            // highSource.PlayOneShot(audioClips[2]);
        });

        return new []{ lowSource, midSource, highSource };
    }

    public void StopAllPlaybackAndRemoveSources()
    {
        for (int sourceIndex = 0; sourceIndex < createdSources.Count; sourceIndex++)
        {
            AudioSource source = createdSources[sourceIndex];
            
            this.RunAfterFrames(1, () => { source.Stop(); });
            this.RunAfterFrames(2, () => { Destroy(source.gameObject); });
            
            // source.Stop();
            // Destroy(source.gameObject);
        }
    }
    
    // [SerializeField] private bool setFadeVal;
    [Button]
    public void SetFadeValue(float fadeVal)
    {
        // if(setFadeVal == false)
        //     return;
        Debug.Log($"Set fade val: {fadeVal}");
        float percentagePerSource = 1f / (float)(numSources - 1);

        int startSample = Mathf.FloorToInt(fadeVal / percentagePerSource);

        float remainder = fadeVal - (percentagePerSource * startSample);
        
        float remainderPercentage = remainder / percentagePerSource;
        
        for (int i = 0; i < numSources; i++)
        {
            AudioMixerGroup audioMixerGroup = MixerGroupForIndex(i);
            
            float volume = 0;
            
            if (i == startSample)
                volume = 1 - remainderPercentage;
            if (i == startSample + 1)
                volume = remainderPercentage;

            float scaledVolume = Mathf.Log(volume) * 20;

            Debug.Log($"audioMixerGroup: {audioMixerGroup}, parameter: {ParameterNameForIndex(i)}, volume {scaledVolume}");
            audioMixerGroup.audioMixer.SetFloat(ParameterNameForIndex(i), scaledVolume);
        }
    }

    private AudioMixerGroup MixerGroupForIndex(int index)
    {
        return index switch
        {
            0 => delaysLowMixer,
            1 => delaysMidMixer,
            2 => delaysHighMixer,
        };
    }
    
    private string ParameterNameForIndex(int index)
    {
        return index switch
        {
            0 => "DelayLow",
            1 => "DelayMid",
            2 => "DelayHigh",
        };
    }
}
