using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    
    [SerializeField] private TimerCountdown _timerCountdown;
    [SerializeField] private TimerStopwatch _timerStopwatch;
    [SerializeField] private ScoreBoardManager _scoreBoardManager;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // temporary control handling
        //ControlTimerDirectly();
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
        _scoreBoardManager.AddScore(new Score("Player", System.DateTime.Now, _timerCountdown.CurrentTime, _timerStopwatch.CurrentTime));
    }

    public void PrintScore()
    {
        _scoreBoardManager.UpdateScoreBoard();
        _scoreBoardManager.ShowScoreBoard();
    }
}
