using System;

[Serializable]
public class Score
{
    // data class for score
    public string PlayerName { get; private set;  }
    public DateTime DateTime { get; private set; }
    public float TimerValue { get; private set; }
    public float StopwatchValue { get; private set; }

    public Score(string name, DateTime dateTime, float timerValue, float stopwatchValue)
    {
        this.PlayerName = name;
        this.DateTime = dateTime;
        this.TimerValue = timerValue;
        this.StopwatchValue = stopwatchValue;
    }
}
