using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // UI text object
    [SerializeField] private float startTime; // start time in seconds
    private float currentTime = 0;
    private bool isRunning = false;


    private string floatToTimeString(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        // possible extra: add milliseconds below 30 seconds
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void StopTimer()
    {
        isRunning = false;
        Debug.Log("Timer stopped");
    }

    // reset to start time
    public void ResetTimer()
    {
        if (isRunning)
        {
            StopTimer();
        }
        timerText.text = (floatToTimeString(startTime));
        Debug.Log("Timer reset");
    }

    public void StartTimer()
    {
        if (!isRunning)
        {
            isRunning = true;
            currentTime = startTime;
        }
        Debug.Log("Timer started");
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime;
            timerText.text = (floatToTimeString(currentTime));
            if (currentTime <= 0)
            {
                StopTimer();
                currentTime = 0;
                Debug.Log("Timer ended");
            }
        }

    }

    // On Application pause
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            StopTimer();
        }
    }
}
