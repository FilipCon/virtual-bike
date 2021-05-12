using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Video;

public class ExtractFrames : MonoBehaviour
{
    [DllImport("FrameInterpolation")]
    public static extern void interpolate(Color32[] image0, Color32[] image1,
                                          Color32[] interpolatedImage,
                                          int width, int height, double t);

    public Renderer renderer;
    public UnityEngine.Video.VideoPlayer videoPlayer;

    // public bool isSeeking = false;
    // public long seekFrame = 0;
    // private long prevFrameIndex = 0;
    private Texture previousTexture;
    bool isPreviousTextureSet = false;

    double deltaTime = 0.0f;

    void Start()
    {
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.Stop();
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.skipOnDrop = true;

        videoPlayer.errorReceived += OnErrorReceived;
        videoPlayer.prepareCompleted += OnPrepareCompleted;
        // videoPlayer.seekCompleted += OnSeekCompleted;
        videoPlayer.frameReady += OnFrameReady;

        videoPlayer.Prepare();
    }

    private void OnErrorReceived(UnityEngine.Video.VideoPlayer source, string message)
    {
        UnityEngine.Debug.Log("VideoPlayer Error: " + message);
    }

    private void OnFrameReady(UnityEngine.Video.VideoPlayer source, long frameIdx)
    {
        double t = 0.5;
        Interpolate(videoPlayer, t);
    }

    void Interpolate(UnityEngine.Video.VideoPlayer source, double t)
    {
        // prevFrameIndex = frameIdx;
        var prevTex2D = toTexture2D((RenderTexture)previousTexture);
        var currTex2D = toTexture2D((RenderTexture)source.texture);

        var interTex = InterpolateFrames(currTex2D, prevTex2D, t);

        previousTexture = source.texture;
        renderer.material.mainTexture = interTex;
    }

    private void OnPrepareCompleted(UnityEngine.Video.VideoPlayer source)
    {
        if (!isPreviousTextureSet)
        {
            previousTexture = source.texture;
            isPreviousTextureSet = true;
        }
        // StartCoroutine(WaitForRenderTexture());
    }

    Texture2D InterpolateFrames(Texture2D currentTex, Texture2D previousTex, double t)
    {
        // get width/height
        int w = currentTex.width;
        int h = currentTex.height;

        // create new texture2D
        var interpolatedTex = new Texture2D(w, h, TextureFormat.RGBA32, false);

        // get pixels from textures
        var prevPixels = previousTex.GetPixels32();
        var currPixels = currentTex.GetPixels32();
        var interPixels = interpolatedTex.GetPixels32();

        // interpolate with external plugin
        var stopwatch = Stopwatch.StartNew();
        interpolate(prevPixels, currPixels, interPixels, w, h, t);
        UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);

        // set interpolated pixels in the new texture and apply
        interpolatedTex.SetPixels32(interPixels);
        interpolatedTex.Apply();

        return interpolatedTex;
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);

        // Remember currently active render texture
        RenderTexture currentActiveRT = RenderTexture.active;

        // make rTex active
        RenderTexture.active = rTex;

        // read pixels to tex2D and apply
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();

        // reset active rt before return
        RenderTexture.active = currentActiveRT;
        return tex;
    }

    // IEnumerator WaitForRenderTexture()
    // {
    //     yield return new WaitUntil(() => videoPlayer.isPrepared);
    // }

    // private void OnSeekCompleted(UnityEngine.Video.VideoPlayer source)
    // {
    //     isSeeking = false;
    // }

    void Update()
    {
        deltaTime += Time.deltaTime;
        if (deltaTime >= (double)(1 / videoPlayer.frameRate))
        {
            videoPlayer.StepForward();
            deltaTime = 0.0f;

        }
    }
}
