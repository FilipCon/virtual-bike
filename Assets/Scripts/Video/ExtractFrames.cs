using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Video;

/// Class that is used to extract frames from a video one-by-one and use them in
/// combination with an external plugin to interpolate frames in real-time and
/// increase the video FPS in play mode. WARNING It is still under development.
/// Currently it does not support frame interpolation.
public class ExtractFrames : MonoBehaviour
{
    // load external plugin that interpolates frames
    [DllImport("FrameInterpolation")]
    public static extern void interpolate(Color32[] image0, Color32[] image1,
                                          Color32[] interpolatedImage,
                                          int width, int height, double t);

    [SerializeField] private float targetFrameRate; // target video FPS

    public UnityEngine.Video.VideoPlayer videoPlayer; // unity video player module
    public Renderer renderer;

    private Texture _previousTexture; // previous video frame
    private bool _isPreviousTextureSet = false;
    private float _deltaTime = 0.0f;

    void Start()
    {
        // initialize video player component
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.Stop();
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.skipOnDrop = true;

        // subscribe functions that are invoked on video events
        videoPlayer.errorReceived += OnErrorReceived;
        videoPlayer.prepareCompleted += OnPrepareCompleted;
        // videoPlayer.seekCompleted += OnSeekCompleted;
        videoPlayer.frameReady += OnFrameReady;

        // prepare the video
        videoPlayer.Prepare();
    }

    // Log error during video playback. Invoked on video error
    private void OnErrorReceived(UnityEngine.Video.VideoPlayer source, string message)
    {
        UnityEngine.Debug.Log("VideoPlayer Error: " + message);
    }

    // Render video frame. Invoked on video frame ready.
    private void OnFrameReady(UnityEngine.Video.VideoPlayer source, long frameIdx)
    {
        double t = 0.5; // TODO

        var prevTex2D = ConvertRenderTexToTexture2D((RenderTexture)_previousTexture);
        var currTex2D = ConvertRenderTexToTexture2D((RenderTexture)source.texture);

        // TODO it does not work here. move it elsewhere
        // var interTex = InterpolateFrames(currTex2D, prevTex2D, t);

        _previousTexture = source.texture;
        // renderer.material.mainTexture = interTex; // TODO
        renderer.material.mainTexture = source.texture;
    }

    // Convert RenderTexture to Texture2D
    Texture2D ConvertRenderTexToTexture2D(RenderTexture rTex)
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

    // Frame interpolation between previous and current video frame.
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

    // Invoked on video preparation.
    private void OnPrepareCompleted(UnityEngine.Video.VideoPlayer source)
    {
        if (!_isPreviousTextureSet)
        {
            _previousTexture = source.texture;
            _isPreviousTextureSet = true;
        }
    }

    // Update video frame based on target FPS (without interpolation)
    void UpdateVideoFrame(double targetFPS)
    {
        if (_deltaTime >= (float)(1 / targetFPS))
        {
            videoPlayer.StepForward();
            _deltaTime = 0.0f;
        }
    }

    void Update()
    {
        // Update video frames only when spacebar is pressed. TODO set analogue
        // (pedal) controller to determine video playback speed and determine if
        // video frame interpolation is used to increase fps
        _deltaTime += Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            UpdateVideoFrame(targetFrameRate);
        }
    }
}
