using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WebGLVideoPlayer : MonoBehaviour
{
    [SerializeField] private string videoFileName;
    [SerializeField] private VideoPlayer videoPlayer;
    
    void Start()
    {
        PlayVideo();
    }
    
    private void PlayVideo()
    {
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        Debug.Log(videoPath);
        videoPlayer.url = videoPath;
        videoPlayer.Play();
    }
}
