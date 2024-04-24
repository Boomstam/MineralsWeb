using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalVoicePlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioSource source;
    
    public void PlayRandomSample()
    {
        if (source.isPlaying)
            return;

        int nextIndex = Random.Range(0, clips.Length);

        source.clip = clips[nextIndex];
        source.Play();
    }
    
    public void StopAllPlayback()
    {
        if (source.isPlaying == false)
            return;
        
        source.Stop();
    }
}
