using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoardManager : MonoBehaviour
{
    // create dynamic list of TextMeshProUGUI objects
    public List<ScoreBoardEntry> ScoreBoardEntries;

    private List<Score> ScoreBoard;

    // consider where players set their name
    public void AddScore(Score score)
    {
        ScoreBoard.Add(score);
        // sort based on stopwatch value
        ScoreBoard.Sort((a, b) => a.StopwatchValue.CompareTo(b.StopwatchValue));
    }

    public void DeleteScore(Score score)
    {
        ScoreBoard.Remove(score);
    }

    // update the scoreboard
    public void UpdateScoreBoard()
    {
        for (int i = 0; i < ScoreBoardEntries.Count; i++)
        {
            if (i < ScoreBoard.Count)
            {
                ScoreBoardEntries[i].NameText.text = ScoreBoard[i].Name;
                ScoreBoardEntries[i].DateTimeText.text = ScoreBoard[i].DateTime.ToString("dd-MM-yyyy HH:mm:ss");
                ScoreBoardEntries[i].TimerValueText.text = ScoreBoard[i].TimerValue.ToString();
                ScoreBoardEntries[i].StopwatchValueText.text = ScoreBoard[i].StopwatchValue.ToString();
            }
            else
            {
                ScoreBoardEntries[i].NameText.text = "";
                ScoreBoardEntries[i].DateTimeText.text = "";
                ScoreBoardEntries[i].TimerValueText.text = "";
                ScoreBoardEntries[i].StopwatchValueText.text = "";
            }
        }
    }

    public void ShowScoreBoard()
    {
        Debug.Log("Scoreboard ======================================================");
        Debug.Log("Name         DateTime        Countdown       Stopwatch");
        foreach (Score score in ScoreBoard)
        {
            Debug.Log(score.Name + " " + score.DateTime + " " + score.TimerValue + " " + score.StopwatchValue);
        }
        Debug.Log("End Scoreboard ======================================================");
    }

    // Start is called before the first frame update
    void Start()
    {
        // get the scoreboard entries
        ScoreBoardEntries = new List<ScoreBoardEntry>();
        foreach (Transform child in transform)
        {
            ScoreBoardEntries.Add(child.GetComponent<ScoreBoardEntry>());
        }

        // get the scoreboard
        ScoreBoard = new List<Score>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
