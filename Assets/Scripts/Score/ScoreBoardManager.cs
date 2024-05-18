using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoardManager : MonoBehaviour
{
    // create dynamic list of TextMeshProUGUI objects
    public List<ScoreBoardEntry> ScoreBoardTexts;

    public List<Score> ScoreBoard;

    // consider where players set their name
    public void AddScore(Score score)
    {
        ScoreBoard.Add(score);
        ScoreBoard.Sort((a, b) => a.ScoreValue.CompareTo(b.ScoreValue));
    }

    public void DeleteScore(Score score)
    {
        ScoreBoard.Remove(score);
    }

    // update the scoreboard
    public void UpdateScoreBoard()
    {
        for (int i = 0; i < ScoreBoardTexts.Count; i++)
        {
            if (i < ScoreBoard.Count)
            {
                ScoreBoardTexts[i].NameText.text = ScoreBoard[i].Name;
                ScoreBoardTexts[i].DateTimeText.text = ScoreBoard[i].DateTime.ToString("dd-MM-yyyy HH:mm:ss");
                ScoreBoardTexts[i].ScoreValueText.text = ScoreBoard[i].ScoreValue.ToString();
            }
            else
            {
                ScoreBoardTexts[i].NameText.text = "";
                ScoreBoardTexts[i].DateTimeText.text = "";
                ScoreBoardTexts[i].ScoreValueText.text = "";
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
