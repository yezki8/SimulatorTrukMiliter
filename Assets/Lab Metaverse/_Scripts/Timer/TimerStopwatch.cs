using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimerStopwatch : MonoBehaviour
{
    public TextMeshProUGUI timerText; // UI text object
    [SerializeField] private float endTime; //
    public float CurrentTime;  // current time in seconds
    public bool IsStopwatchRunning { get; private set; }
    public bool IsStopwatchPaused { get; private set; }

    //for references in other scripts
    public static TimerStopwatch Instance;
    public UnityEvent OnStopwatchReset;

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
        int milliseconds = Mathf.FloorToInt((time - Mathf.Floor(time)) * 100);
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public void StopStopwatch()
    {
        IsStopwatchRunning = false;
        Debug.Log("Stopwatch stopped");
    }

    public void HandlePauseStopwatch()
    {
        if (IsStopwatchPaused)
        {
            IsStopwatchPaused = false;
            Debug.Log("Stopwatch unpaused");
        }
        else
        {
            IsStopwatchPaused = true;
            Debug.Log("Stopwatch paused");
        }
    }

    public void ManuallyPauseStopwatch()        // pause stopwatch no matter what
    {
        IsStopwatchPaused = true;
    }

    public void ManuallyUnpauseStopwatch()      //unpause stopwatch no matter what
    {
        IsStopwatchPaused = false;
    }

    // reset to start time
    public void ResetStopwatch()
    {
        if (IsStopwatchRunning)
        {
            StopStopwatch();
        }
        CurrentTime = 0.0f;
        IsStopwatchPaused = false;
        timerText.text = (FloatToTimeString(CurrentTime));
        Debug.Log("Stopwatch reset");
    }

    public void StartStopwatch()
    {
        if (!IsStopwatchRunning)
        {
            IsStopwatchRunning = true;
        }
        CurrentTime = CheckpointManager.Instance.GetRecordedStopwatch();            //changed to Get Recorded Time
        Debug.Log("Stopwatch started");
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetStopwatch();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsStopwatchRunning && !IsStopwatchPaused)
        {
            CurrentTime += Time.deltaTime;
            timerText.text = (FloatToTimeString(CurrentTime));
        }
    }

    // On Application pause
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            StopStopwatch();
        }
    }
}
