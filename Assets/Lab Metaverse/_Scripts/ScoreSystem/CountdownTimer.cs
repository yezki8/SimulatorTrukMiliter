using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // UI text object
    [SerializeField] private float startTime; // start time in seconds
    public float CurrentTime { get; private set; } // current time in seconds
    public bool IsTimerRunning { get; private set; }
    public bool IsTimerPaused { get; private set; }

    //for references in other scripts
    public static CountdownTimer Instance;
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
        // possible extra: add milliseconds below 30 seconds
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        IsTimerRunning = false;
        Debug.Log("Timer stopped");
    }

    public void HandlePauseTimer()
    {
        if (IsTimerPaused)
        {
            IsTimerPaused = false;
            Debug.Log("Timer unpaused");
        } else
        {
            IsTimerPaused = true;
            Debug.Log("Timer paused");
        }
    }

    // reset to start time
    public void ResetTimer()
    {
        if (IsTimerRunning)
        {
            StopTimer();
        }
        CurrentTime = startTime;
        IsTimerPaused = false;
        timerText.text = (FloatToTimeString(startTime));
        Debug.Log("Timer reset");
    }

    public void StartTimer()
    {
        if (!IsTimerRunning)
        {
            IsTimerRunning = true;
        }
        CurrentTime = startTime;
        Debug.Log("Timer started");
    }

    //Get and Set Data ===============================================================
    public float GetStartTime()
    {
        return startTime;
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
                timerText.text = (FloatToTimeString(CurrentTime));
                Debug.Log("Timer ended");

                OnCountdownEnd?.Invoke();
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
