using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    
    [SerializeField] private TimerCountdown _timerCountdown;
    [SerializeField] private TimerStopwatch _timerStopwatch;
    [SerializeField] private ScoreBoardManager _scoreBoardManager;
    [SerializeField] private TMP_InputField _nameInputField;

    public static ScoreManager Instance;

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

    void ControlTimerDirectly()
    {
        // space to start/pause/unpause, r to reset
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeCountdownState();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            // add timer to scoreboard
            RecordAndReset();
        }
        // show score board when tab is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PrintScore();
        }
    }

    public void ChangeCountdownState()
    {
        if (_timerCountdown.IsTimerRunning)
        {
            _timerCountdown.HandlePauseTimer();
        }
        else
        {
            _timerCountdown.StartTimer();
        }
    }

    public void RecordAndReset()
    {
        RecordScore();
        _timerCountdown.ResetTimer();
    }

    public void RecordScore()
    {
        int score = CalculateScore(_timerStopwatch.CurrentTime);
        string driverName = _nameInputField.text;
        if (driverName == string.Empty)
        {
            driverName = "Pengemudi";
        }
        _scoreBoardManager.AddScore(new Score(driverName, System.DateTime.Now, _timerStopwatch.CurrentTime, score));
    }

    public void ResetNameField()
    {
        _nameInputField.text = string.Empty;
    }

    int CalculateScore(float currTime)
    {
        float thresholdSecond = TimerCountdown.Instance.StartTime;
        float score = 100;

        float timePlusage = currTime - thresholdSecond;
        if (timePlusage > 0)
        {
            score = ((thresholdSecond - timePlusage) / thresholdSecond) * 100;
        }
        return (int)score;
    }

    public void PrintScore()
    {
        _scoreBoardManager.ShowScoreBoard();
    }
}
