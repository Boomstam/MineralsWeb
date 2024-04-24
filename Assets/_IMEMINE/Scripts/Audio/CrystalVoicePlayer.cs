using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalVoicePlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioSource source;
    
    private Coroutine playAfterDelayRoutine;
    
    public void PlayRandomSample()
    {
        if(playAfterDelayRoutine != null)
            return;
        
        playAfterDelayRoutine = StartCoroutine(PlaySampleAfterDelay());
    }
    
    private IEnumerator PlaySampleAfterDelay()
    {
        float randomOffset = Random.Range(0, 22);
        Debug.Log($"PlaySampleAfterDelay: {randomOffset}");
        
        yield return new WaitForSeconds(randomOffset);
        
        PlaySample();
        
        playAfterDelayRoutine = null;
    }
    
    private void PlaySample()
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
