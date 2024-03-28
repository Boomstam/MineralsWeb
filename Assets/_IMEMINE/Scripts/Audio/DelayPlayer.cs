using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DelayPlayer : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioFader faderTemplate;
    [SerializeField] private AudioClip[] delayClips;
    [SerializeField] private AudioSource delaySamplePrefab;
    [SerializeField] private AudioClip testDelaySample;

    [Button]
    public void PlayTestDelay(float delayTime)
    {
        PlayWithDelay(testDelaySample, delayTime, 0.75f);
    }
    
    public void PlayWithDelay(AudioClip audioClip, float delayTime, float feedback)
    {
        StartCoroutine(PlayWithDelayRoutine(audioClip, delayTime, feedback));
    }

    createa audio mixer groups for audio fader
    pass in clips here
    random generate before?
    private IEnumerator PlayWithDelayRoutine(AudioClip audioClip, float delayTime, float feedback)
    {
        if(feedback is < 0 or > 1)
            Debug.LogError($"Feedback {feedback} not between 0 and 1");
        
        List<AudioFader> createdFaders = new List<AudioFader>();
        
        float volume = 1;

        createdFaders.Add(NewAudioFaderWithClip(audioClip, volume));

        float fallOff = 1 - feedback;
        
        while (volume > 0)
        {
            yield return new WaitForSeconds(delayTime);
        
            volume = volume - fallOff;
            
            createdFaders.Add(NewAudioFaderWithClip(audioClip, volume));
        }
        
        yield return new WaitForSeconds(audioClip.length);
        
        for (int sourceIndex = 0; sourceIndex < createdFaders.Count; sourceIndex++)
        {
            AudioFader fader = createdFaders[sourceIndex];
            
            fader.StopAllPlayback();
            Destroy(fader.gameObject);
        }
    }

    which clips though?
    private AudioSource NewAudioFaderWithClip(AudioClip audioClip, float volume)
    {
        AudioSource audioSource = Instantiate(delaySamplePrefab);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        return audioSource;
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