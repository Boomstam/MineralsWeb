using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] clips;
    [SerializeField] private AudioSource audioSource;
    
    public void PlayClip(ClipType clipType)
    {
        audioSource.Stop();
        
        int clipIndex = (int)clipType;
        AudioClip clip = clips[clipIndex];

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopPlayback()
    {
        audioSource.Stop();
    }
}

public enum ClipType
{
    Chapter1,
    Chapter2,
    Chapter3,
    Chapter4,
    Chapter5,
}