using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Progress bar of the recorded track/video.
public class TrackProgress : MonoBehaviour
{
    public Slider slider;  // progress slider
    [SerializeField] ExtractFrames video;

    float value = 0.0f;
    double time;
    double duration;

    void Start()
    {
        duration = video.videoPlayer.length;
        slider.value = 0.0f;
        slider.maxValue = (float)duration;
        slider.minValue = 0;
    }

    void Update()
    {
        // Update the progress bar only when spacebar is pressed.
        // TODO combine with an analogue (pedal) controller to control the
        // progress bar/video speed
        time = video.videoPlayer.time;
        if (value <= duration)
        {
            slider.value = value;
            value = (float)time;
        }
    }
}
