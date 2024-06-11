using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayTimeManager : MonoBehaviour
{
    //TODO: Convert this float to Scriptable object when needed in the future
    [Range(0, 24)]
    public float DefaultDayStart = 9;   //The hour of the game started
    
    [Range(0, 24)]
    public float timeOfDay;  // Current time of day (0-24)
    public float dayDuration = 120f;  // Duration of a full day in seconds
    public int currentDay = 1;
    public TextMeshProUGUI timeText;  // Time Text UI

    private void Start()
    {
        InitializeDayTime();
    }

    void Update()
    {
        //Make sure that day clock only run when game is active
        if (GameStateController.Instance.GameState == StateOfGame.Match)
        {
            CalculateDayTime();
        }
    }

    public void InitializeDayTime()
    {
        timeOfDay = DefaultDayStart;
        CalculateDayTime();
    }

    void CalculateDayTime()
    {
        // Calculate how much time to add to timeOfDay this frame
        timeOfDay += (24 / dayDuration) * Time.deltaTime;

        // Ensure timeOfDay stays within 0-24 range
        if (timeOfDay >= 24)
        {
            timeOfDay = 0;
            currentDay += 1;
        }

        // Convert timeOfDay to hour:minute format
        int hour = Mathf.FloorToInt(timeOfDay) % 24;
        int minute = Mathf.FloorToInt((timeOfDay - Mathf.Floor(timeOfDay)) * 60);

        // Format the time as a string
        string timeString = $"Day {currentDay}, {hour:00}:{minute:00}";

        // Update the TextMeshProUGUI component
        if (timeText != null)
        {
            timeText.text = timeString;
        }
    }
}

