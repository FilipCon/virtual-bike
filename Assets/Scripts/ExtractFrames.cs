using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ExtractFrames : MonoBehaviour
{

    public Renderer renderer;
    public UnityEngine.Video.VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.Stop();
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        // videoPlayer.prepareCompleted += Prepared;
        // videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += OnFrameReady;
        videoPlayer.Prepare();
        videoPlayer.Play();
        videoPlayer.Pause();
    }

    void Prepared(UnityEngine.Video.VideoPlayer vp) => vp.Play();

    void FrameReady(UnityEngine.Video.VideoPlayer vp, long frameIndex)
    {
        renderer.material.mainTexture = vp.texture;
    }

    void OnFrameReady(UnityEngine.Video.VideoPlayer vp, long frameIndex)
    {

        Debug.Log("FrameReady: " + frameIndex);
        vp.StepForward();
    }

}
