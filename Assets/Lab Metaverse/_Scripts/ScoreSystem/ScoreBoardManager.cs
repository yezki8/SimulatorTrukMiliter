using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoardManager : MonoBehaviour
{
    // print score using ScoreEntryContainer prefab
    [SerializeField] private GameObject ScoreEntryPrefab;
    public GameObject ScoreTable;
    // create dynamic list of objects
    public List<GameObject> ScoreBoardEntries;

    private List<Score> _scoreBoard;

    // entry height 40
    [SerializeField] private float _entryHeight = 40f;

    public static ScoreBoardManager Instance;
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

    // consider where players set their name
    public void AddScore(Score score)
    {
        _scoreBoard.Add(score);
        // sort based on stopwatch value
        _scoreBoard.Sort((a, b) => a.StopwatchValue.CompareTo(b.StopwatchValue));
    }

    public void DeleteScore(Score score)
    {
        _scoreBoard.Remove(score);
    }

    // duplicate of timer
    public string FloatToTimeString(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        // milliseconds below 30 seconds
        if (time < 30)
        {
            int milliseconds = Mathf.FloorToInt((time - Mathf.Floor(time)) * 100);
            return string.Format("{0:00}:{1:00}:{2:0000}", minutes, seconds, milliseconds);
        }
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // called by game state manager
    public void ShowScoreBoard()
    {
        // disable when adding entries
        ScoreTable.SetActive(false);
        Debug.Log("Showing Score Board with count:");
        Debug.Log(_scoreBoard.Count);
        for (int i = 0; i < _scoreBoard.Count; i++)
        {
            GameObject entry = Instantiate(ScoreEntryPrefab, ScoreTable.transform);
            entry.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i * _entryHeight);
            // get every tmp text component
            entry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            entry.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _scoreBoard[i].PlayerName;
            entry.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _scoreBoard[i].DateTime.ToString("dd-MM-yyyy HH:mm:ss");
            entry.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = FloatToTimeString(_scoreBoard[i].StopwatchValue);
            entry.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = FloatToTimeString(_scoreBoard[i].TimerValue);
            ScoreBoardEntries.Add(entry);
        }
        ScoreTable.SetActive(true);
    }
    
    // Debug
    public void RemoveScoreBoard() {
        if (ScoreBoardEntries.Count > 0)
        {
            foreach (GameObject entry in ScoreBoardEntries)
            {
                Destroy(entry);
            }
            ScoreBoardEntries.Clear();
            Debug.Log("Score Board Cleared");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _scoreBoard = new List<Score>();
        ScoreBoardEntries = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
