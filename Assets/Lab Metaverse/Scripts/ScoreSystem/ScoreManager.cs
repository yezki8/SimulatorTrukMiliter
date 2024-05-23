using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // for now it's intermediate for
    // countdown timer and scoreboard manager game objects
    public GameObject countdownTimerObject;
    public GameObject scoreBoardManagerObject;
    private CountdownTimer countdownTimer;
    private ScoreBoardManager scoreBoardManager;

    // Start is called before the first frame update
    void Start()
    {
        countdownTimer = countdownTimerObject.GetComponent<CountdownTimer>();
        scoreBoardManager = scoreBoardManagerObject.GetComponent<ScoreBoardManager>();

        // test if the objects are found
        if (countdownTimer == null)
        {
            Debug.LogError("Countdown timer object not found");
        }
        if (scoreBoardManager == null)
        {
            Debug.LogError("Scoreboard manager object not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // temporary control handling
        // space to start/pause/unpause, r to reset
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (countdownTimer.IsTimerRunning)
            {
                countdownTimer.HandlePauseTimer();
            }
            else
            {
                countdownTimer.StartTimer();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            // add timer to scoreboard
            scoreBoardManager.AddScore(new Score("Player", System.DateTime.Now, countdownTimer.CurrentTime));
            countdownTimer.ResetTimer();
        }
        // show score board when tab is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoardManager.UpdateScoreBoard();
            scoreBoardManager.ShowScoreBoard();
        }
    }
}
