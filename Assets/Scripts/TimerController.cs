using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class TimerController : MonoBehaviour {
    private float startTime;
    private bool timerOn = false;
    public float StartTime
    {
        get { return startTime; }
    }

    [HideInInspector]
    public Text timerText; 
	// Use this for initialization
	void Start () {
        timerText = GetComponent<Text>(); 
	}
	
	// Update is called once per frame
	void Update () {
        if (timerOn){
            UpdateTimer();
        }

	}
    public void StartTimer()
    {
        if (!timerOn)
        {   
            timerOn = true;
            startTime = Time.time;
        }   
    }   

    void UpdateTimer(){
        float currentTime = Time.time - startTime;
        timerText.text = "Time : " + FormatTime(currentTime);
    }   

    string FormatTime(float currentTime) {
        float minute = Mathf.Floor(currentTime / 60);
        float second = Mathf.Floor(currentTime - minute * 60);
        float centi = Mathf.Floor(100 * (currentTime - minute * 60 - second));

        return minute.ToString("00")+ ":" + second.ToString("00")+ ":" + centi.ToString("00"); 


    }
}
