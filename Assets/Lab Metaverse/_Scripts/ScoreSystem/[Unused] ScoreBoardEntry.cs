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
    public TextMeshProUGUI TimerValueText { get; private set; }
    public TextMeshProUGUI StopwatchValueText { get; private set; }

    public ScoreBoardEntry(string name, DateTime dateTime, float stopwatchValue, float timerValue)
    {
        this.NameText.text = name;
        // format date to DD/MM/YYYY
        this.DateTimeText.text = dateTime.ToString("dd-MM-yyyy HH:mm:ss");
        this.StopwatchValueText.text = stopwatchValue.ToString();
        this.TimerValueText.text = timerValue.ToString();
    }
}
