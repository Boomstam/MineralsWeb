using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup master;
    public AudioFader audioFader;
    public DoubleFader doubleFader;
    public TutorialDoubleFader microOrganismsDoubleFader;
    public TutorialDoubleFader tutorialDoubleFader;
    public CirclePlayer circlePlayer;
    public DelayPlayer delayPlayer;
    public AudioFader microOrganismsAudioFader;
    public CrystalVoicePlayer crystalVoicePlayer;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioDistortionFilter audioDistortionFilter;
    [SerializeField] private AudioChorusFilter audioChorusFilter;
    [SerializeField] private AudioEchoFilter audioEchoFilter;
    [SerializeField] private AudioReverbFilter audioReverbFilter;
    [SerializeField] private AudioLowPassFilter audioLowPassFilter;
    [SerializeField] private AudioHighPassFilter audioHighPassFilter;
    [SerializeField] private AudioClip[] microOrganismsClips;
    [SerializeField] private AudioClip[] waysOfWaterClips;
    
    [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;
    
    private Coroutine masterFadeRoutine;
    private Coroutine circlesFadeRoutine;
    
    // FUCK THISSSSSSS
    private const float minVolume = 0.01f;
    
    private bool hasFadedInCircles;
    private bool hasFadedOutCircles;
    
    [Button]
    private void TestMasterAttenuation(float val)
    {
        master.audioMixer.SetFloat($"Master", val);
    }
    
    public void OnQuadrantsModeEnabled(Vector2 seatMinMax, Vector2 rowMinMax)
    {
        if(masterFadeRoutine != null)
            return;
        
        // StopCoroutine(masterFadeRoutine);
        
        bool shouldSound = Instances.SeatNumber >= seatMinMax.x && 
                           Instances.SeatNumber <= seatMinMax.y &&
                           Instances.RowNumber >= rowMinMax.x &&
                           Instances.RowNumber <= rowMinMax.y;
        
        // SetUnscaledVolume(shouldSound ? 1 : 0);
        masterFadeRoutine = StartCoroutine(DoMasterFade(shouldSound));
    }
    
    public void OnQuadrantsModeDisabled()
    {
        if(masterFadeRoutine != null)
            return;
        
        // StopCoroutine(masterFadeRoutine);
        
        masterFadeRoutine = StartCoroutine(DoMasterFade(false));
    }
    
    private IEnumerator DoMasterFade(bool fadeIn)
    {
        float startTime = Time.time;
        
        float fadeTime = fadeIn ? fadeInTime : fadeOutTime;
        
        float timeElapsed = 0;
        while (timeElapsed < fadeTime)
        {
            float currentPercentage = timeElapsed / fadeTime;
            
            if (fadeIn == false)
                currentPercentage = 1 - currentPercentage;
            
            SetUnscaledVolume(currentPercentage, $"Master");
            
            timeElapsed = Time.time - startTime;
            yield return 0;
        }
        SetUnscaledVolume(fadeIn ? 1 : minVolume, $"Master");
        masterFadeRoutine = null;
    }
    
    private void SetUnscaledVolume(float volume, string exposedParam)
    {
        volume = (volume <= 0) ? minVolume : volume;
        
        float scaledVolume = Mathf.Log(volume) * 20;
        
        // Debug.Log($"SetUnscaledVolume: {volume}, scaledVolume{scaledVolume} exposedParam: {exposedParam}");
        
        master.audioMixer.SetFloat(exposedParam, scaledVolume);
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
    
    public void ResetCirclesVolume()
    {
        // master.audioMixer.SetFloat($"Circles", 0);
    }
    
    public void MuteCircles(bool mute)
    {
        Debug.Log($"Mute circles: {mute}");
        SetUnscaledVolume(mute ? 0 : 1, $"Circles");
    }
    
    public void PlayMicroOrganisms()
    {
        Debug.Log($"PlayMicroOrganisms");

        microOrganismsDoubleFader.PlayFadeSamples();
    }

    public void StopMicroOrganisms()
    {
        Debug.Log($"StopMicroOrganisms");
        
        microOrganismsDoubleFader.StopAllPlayback();
        // this.RunDelayed(5, () => { microOrganismsDoubleFader.StopAllPlayback(); });
    }

    public void StartRandomStaticVoice()
    {
        crystalVoicePlayer.PlayRandomSample();
    }

    public void StopRandomStaticVoice()
    {
        crystalVoicePlayer.StopAllPlayback();
    }
    
    // public void FadeCircles(bool fadeIn)
    // {
    //     if (circlesFadeRoutine != null)
    //         StopCoroutine(circlesFadeRoutine);
    //     
    //     circlesFadeRoutine = StartCoroutine(DoCirclesFade(fadeIn));
    // }
    
    // private IEnumerator DoCirclesFade(bool fadeIn)
    // {
    //     if(hasFadedInCircles == false)
    //     {
    //         float startTime = Time.time;
    //
    //         float fadeTime = 0.69f;
    //
    //         float timeElapsed = 0;
    //         while (timeElapsed < fadeTime)
    //         {
    //             float currentPercentage = timeElapsed / fadeTime;
    //
    //             if (fadeIn == false)
    //                 currentPercentage = 1 - currentPercentage;
    //
    //             SetUnscaledVolume(currentPercentage, $"Circles");
    //
    //             timeElapsed = Time.time - startTime;
    //             yield return 0;
    //         }
    //         SetUnscaledVolume(fadeIn ? 1 : 0, $"Circles");
    //     }
    //
    //     hasFadedInCircles = true;
    // }
    
    public void StopAllPlayback()
    {
        audioSource.Stop();
        
        audioFader.StopAllPlayback();
        doubleFader.StopAllPlayback();
        tutorialDoubleFader.StopAllPlayback();
        circlePlayer.StopPlayback();
        delayPlayer.StopAllPlaybackAndRemoveSources();
        microOrganismsAudioFader.StopAllPlayback();
        microOrganismsDoubleFader.StopAllPlayback();
        crystalVoicePlayer.StopAllPlayback();
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