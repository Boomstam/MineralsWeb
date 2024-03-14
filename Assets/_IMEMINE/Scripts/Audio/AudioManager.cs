using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioFader audioFader;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioDistortionFilter audioDistortionFilter;
    [SerializeField] private AudioChorusFilter audioChorusFilter;
    [SerializeField] private AudioEchoFilter audioEchoFilter;
    [SerializeField] private AudioReverbFilter audioReverbFilter;
    [SerializeField] private AudioLowPassFilter audioLowPassFilter;
    [SerializeField] private AudioHighPassFilter audioHighPassFilter;

    public AudioClip GetClip(ClipType clipType) => clips[(int)clipType];

    [Button]
    public void PlayClip(ClipType clipType)
    {
        audioSource.Stop();

        audioSource.clip = GetClip(clipType);
        
        audioSource.Play();
    }

    public void PlayFadeClips(ClipType[] clipTypes)
    {
        AudioClip[] fadeClips = clipTypes.Select(GetClip).ToArray();
        
        audioFader.PlayFadeSamples(fadeClips);
    }

    public void SetFadeVal(float fadeVal)
    {
        audioFader.SetFadeValue(fadeVal);
    }
    
    public void StopAllPlayback()
    {
        audioSource.Stop();

        audioFader.StopAllPlayback();
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

    public void EnableDistortion(float distortionLevel)
    {
        ResetAllFx();

        audioDistortionFilter.distortionLevel = distortionLevel;
        audioDistortionFilter.enabled = true;
    }

    public void EnableChorus()
    {
        ResetAllFx();

        audioChorusFilter.enabled = true;
    }
    
    public void EnableReverb(AudioReverbPreset reverbPreset)
    {
        ResetAllFx();

        audioReverbFilter.reverbPreset = reverbPreset;
        audioReverbFilter.enabled = true;
    }
    
    [Button]
    public void EnableLowPassFilter(float cutoffFrequency)
    {
        ResetAllFx();

        audioLowPassFilter.cutoffFrequency = cutoffFrequency;
        audioLowPassFilter.enabled = true;
    }
}

public enum ClipType
{
    Chapter1,
    Chapter2,
    Chapter3,
    Chapter4,
    Chapter5,
    MineralsA,
    MineralsB,
    MineralsC,
}