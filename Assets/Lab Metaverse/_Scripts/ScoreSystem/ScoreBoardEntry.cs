using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public class ScoreBoardEntry : MonoBehaviour
{
    // data class for scoreboard entry
    public TextMeshProUGUI NameText { get; private set; }
    public TextMeshProUGUI DateTimeText { get; private set; }
    public TextMeshProUGUI ScoreValueText { get; private set; } // for now it's time, but it can be anything

    public ScoreBoardEntry(string name, DateTime dateTime, float scoreValue)
    {
        this.NameText.text = name;
        // format date to DD/MM/YYYY
        this.DateTimeText.text = dateTime.ToString("dd-MM-yyyy HH:mm:ss");
        this.ScoreValueText.text = scoreValue.ToString();
    }
}
