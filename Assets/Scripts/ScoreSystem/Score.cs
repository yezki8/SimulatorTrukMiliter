using System;

public class Score
{
    // data class for score
    public string Name { get; private set;  }
    public DateTime DateTime { get; private set; }
    public float ScoreValue { get; private set; } // for now it's time, but it can be anything

    public Score(string name, DateTime dateTime, float scoreValue)
    {
        this.Name = name;
        this.DateTime = dateTime;
        this.ScoreValue = scoreValue;
    }
}
