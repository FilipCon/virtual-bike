using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Video;

public class ExtractFrames : MonoBehaviour
{

    [DllImport("FrameInterpolation"]
    public static extern void interpolate(Color32[] image0, Color32[] image1,
                                         int width, int height, double t);

    [DllImport("FrameInterpolation")]
    private static extern void GetRawImageBytes(IntPtr data, int width, int height);

    private Texture2D tex;
    private Color32[] pixel32;

    private GCHandle pixelHandle;
    private IntPtr pixelPtr;

    // [DllImport("FrameInterpolation", EntryPoint = "freeMem")]
    // public static extern void freeMem(IntPtr ptr);

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
        Debug.Log("VideoPlayer Error: " + message);
    }

    private void OnFrameReady(UnityEngine.Video.VideoPlayer source, long frameIdx)
    {
        // prevFrameIndex = frameIdx;
    }

    private void OnPrepareCompleted(UnityEngine.Video.VideoPlayer source)
    {

        if (!isPreviousTextureSet)
        {
            previousTexture = source.texture;
            isPreviousTextureSet = true;
        }

        var firstTex2D = toTexture2D((RenderTexture)previousTexture);
        var secondTex2D = toTexture2D((RenderTexture)source.texture);

        int w = firstTex2D.width;
        int h = firstTex2D.height;

        double t = 0.5;
        var image0 = secondTex2D.GetPixels32();
        var image1 = firstTex2D.GetPixels32();


        interpolate(image0, image1, w, h, t);
        // byte[] returnedResult = new byte[firstTex2D.GetRawTextureData().Length];
        // Marshal.Copy(returnedPtr, returnedResult, 0, firstTex2D.GetRawTextureData().Length);
        // freeMem(returnedPtr);

        // Texture2D temp = new Texture2D(w, h);
        // temp.LoadImage(returnedResult);
        // temp.Apply();
        // renderer.material.mainTexture = (Texture)(temp);

        tex = secondTex2D;
        //Pin pixel32 array
        pixelHandle = GCHandle.Alloc(image0, GCHandleType.Pinned);
        //Get the pinned address
        pixelPtr = pixelHandle.AddrOfPinnedObject();

        previousTexture = source.texture;

        renderer.material.mainTexture = source.texture;


        // StartCoroutine(WaitForRenderTexture());
    }

    void MatToTexture2D()
    {
        //Convert Mat to Texture2D
        GetRawImageBytes(pixelPtr, tex.width, tex.height);
        //Update the Texture2D with array updated in C++
        tex.SetPixels32(pixel32);
        tex.Apply();
    }

    public Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D dest = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);

        Graphics.CopyTexture(rTex, dest);

        return dest;
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



    void OnApplicationQuit()
    {
        //Free handle
        pixelHandle.Free();
    }

}
