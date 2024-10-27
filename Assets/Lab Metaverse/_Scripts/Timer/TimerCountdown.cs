using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TimerCountdown : MonoBehaviour
{
    public TextMeshProUGUI timerText; // UI text object
    public float CurrentTime;  // current time in seconds
    public bool IsTimerRunning { get; private set; }
    public bool IsTimerPaused { get; private set; }

    [Header("Time Limit for Score 100 (in minutes)")] //in minutes
    [SerializeField] private float _level1TimeThreshold = 5;
    [SerializeField] private float _level2TimeThreshold = 10;
    public float StartTime = 0; // start time in seconds

    //for references in other scripts
    public static TimerCountdown Instance;
    public UnityEvent OnCountdownEnd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public string FloatToTimeString(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        // milliseconds below 30 seconds
        if (time < 30)
        {
            int milliseconds = Mathf.FloorToInt((time - Mathf.Floor(time)) * 100);
            return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
        }
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        IsTimerRunning = false;
    }

    public void HandlePauseTimer()
    {
        if (IsTimerPaused)
        {
            IsTimerPaused = false;
        } else
        {
            IsTimerPaused = true;
        }
    }

    public void ManuallyPauseTimer()        //pause timer no matter what
    {
        IsTimerPaused = true;
    }

    public void ManuallyUnpauseTimer()      //unpause timer no matter what
    {
        IsTimerPaused = false;
    }

    // reset to start time
    public void ResetTimer()
    {
        if (IsTimerRunning)
        {
            StopTimer();
        }
        ResetCurretTime();
        IsTimerPaused = false;
        Debug.Log("Time resetted to level " + CheckpointManager.Instance.ActiveLevel);
    }

    public void ResetCurretTime()
    {
        if (CheckpointManager.Instance.ActiveLevel == 1)
        {
            StartTime = _level1TimeThreshold * 60;
        }
        else if (CheckpointManager.Instance.ActiveLevel == 2)
        {
            StartTime = _level2TimeThreshold * 60;
        }

        CurrentTime = StartTime;
        timerText.text = (FloatToTimeString(StartTime));
    }

    public void StartTimer()
    {
        if (!IsTimerRunning)
        {
            IsTimerRunning = true;
        }
        
        //Feature removed due to player cannot replay at checkpoint anymore
        //CurrentTime = CheckpointManager.Instance.GetRecordedTimer();            //changed to Get Recorded Time
    }

    //Get and Set Data ===============================================================
    public float GetStartTime()
    {
        return StartTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsTimerRunning && !IsTimerPaused)
        {
            CurrentTime -= Time.deltaTime;
            timerText.text = (FloatToTimeString(CurrentTime));
            if (CurrentTime <= 0)
            {
                StopTimer();
                CurrentTime = 0;
                OnCountdownEnd?.Invoke();           // reconsider alternative to sync timer and stopwatch
                timerText.text = ("Target waktu lewat");
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
