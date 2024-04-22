using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup master;
    public AudioFader audioFader;
    public DoubleFader doubleFader;
    public TutorialDoubleFader tutorialDoubleFader;
    public CirclePlayer circlePlayer;
    public DelayPlayer delayPlayer;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioDistortionFilter audioDistortionFilter;
    [SerializeField] private AudioChorusFilter audioChorusFilter;
    [SerializeField] private AudioEchoFilter audioEchoFilter;
    [SerializeField] private AudioReverbFilter audioReverbFilter;
    [SerializeField] private AudioLowPassFilter audioLowPassFilter;
    [SerializeField] private AudioHighPassFilter audioHighPassFilter;

    public void OnQuadrantsModeEnabled(Vector2 seatMinMax, Vector2 rowMinMax)
    {
        bool shouldSound = Instances.SeatNumber >= seatMinMax.x && 
                           Instances.SeatNumber <= seatMinMax.y &&
                           Instances.RowNumber >= rowMinMax.x &&
                           Instances.RowNumber <= rowMinMax.y;
        
        SetUnscaledVolume(shouldSound ? 1 : 0);
    }
    
    public void OnQuadrantsModeDisabled()
    {
        SetUnscaledVolume(1);
    }

    private void SetUnscaledVolume(float volume)
    {
        float scaledVolume = Mathf.Log(volume) * 20;
     
        master.audioMixer.SetFloat($"Master", scaledVolume);
    }

    public void PlayFadeSamples(AudioClip fadeClips)
    {
        // AudioClip[] fadeClips = GetClips(clipTypes);
        
        // audioFader.PlayFadeSamples(fadeClips);
    }

    public void SetFadeVal(float fadeVal)
    {
        audioFader.SetFadeValue(fadeVal);
    }
    
    public void StopAllPlayback()
    {
        audioSource.Stop();

        audioFader.StopAllPlayback();
        doubleFader.StopAllPlayback();
        tutorialDoubleFader.StopAllPlayback();
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
}