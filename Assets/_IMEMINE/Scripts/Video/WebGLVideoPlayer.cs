using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WebGLVideoPlayer : MonoBehaviour
{
    [SerializeField] private string videoFileExtension;
    [SerializeField] private VideoPlayer videoPlayer;

    void Start()
    {
        // this.RunDelayed(1, () => PlayVideo(VideoType.Lava));
    }
    
    private void PlayVideo(VideoType videoType)
    {
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoType + videoFileExtension);    
        Debug.Log($"Play video {videoType}, path: {videoPath}");
        
        videoPlayer.url = videoPath;
        videoPlayer.Play();
    }
}

public enum VideoType
{
    Lava,
    Stars,
}