using System;

[Serializable]
public class Score
{
    // data class for score
    public string Name { get; private set;  }
    public DateTime DateTime { get; private set; }
    public float TimerValue { get; private set; }
    public float StopwatchValue { get; private set; }

    public Score(string name, DateTime dateTime, float timerValue, float stopwatchValue)
    {
        this.Name = name;
        this.DateTime = dateTime;
        this.TimerValue = timerValue;
        this.StopwatchValue = stopwatchValue;
    }
}
