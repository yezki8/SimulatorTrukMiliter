using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    [Range(0, 24)]
    private float _timeOfDay;  // Current time of day (0-24)
    [SerializeField]
    private float _dayDuration = 120f;  // Duration of a full day in seconds
    private int _currentDay = 1;
    [SerializeField]
    private TextMeshProUGUI _timeText;  // Time Text UI

    void Update()
    {
        // Calculate how much time to add to timeOfDay this frame
        _timeOfDay += (24 / _dayDuration) * Time.deltaTime;

        // Ensure timeOfDay stays within 0-24 range
        if (_timeOfDay >= 24)
        {
            _timeOfDay = 0;
            _currentDay += 1;
        }

        // Convert timeOfDay to hour:minute format
        int hour = Mathf.FloorToInt(_timeOfDay) % 24;
        int minute = Mathf.FloorToInt((_timeOfDay - Mathf.Floor(_timeOfDay)) * 60);

        // Format the time as a string
        string timeString = $"Day {_currentDay}, {hour:00}:{minute:00}";

        // Update the TextMeshProUGUI component
        if (_timeText != null)
        {
            _timeText.text = timeString;
        }
    }

    public float getTimeOfDay()
    {
        return _timeOfDay;
    }
}


