using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField]
    private Image _moon; // Moon icon
    [SerializeField]
    private Image _sun; // Sun icon
    [SerializeField]
    private Image _sunrise; // Sunset & sunrise icon

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

    void UpdateHUDIcons(float timeOfDay)
    {
        if (timeOfDay >= 18.5 || timeOfDay < 4.5) // Night phase
        {
            DeactivateIcon(_sunrise);
            ActivateIcon(_moon);
        }
        else if (timeOfDay >= 4.5 && timeOfDay < 6) // Sunrise transition
        {
            DeactivateIcon(_moon);
            ActivateIcon(_sunrise);
        }
        else if (timeOfDay >= 7 && timeOfDay < 17.5) // Day phase
        {
            DeactivateIcon(_sunrise);
            ActivateIcon(_sun);
        }
        else if (timeOfDay >= 17.5 && timeOfDay < 18.5) // Sunset transition
        {
            DeactivateIcon(_sun);
            ActivateIcon(_sunrise);
        }
    }

    void ActivateIcon(Image icon)
    {
        if (icon != null)
        {
            icon.gameObject.SetActive(true);
        }
    }

    void DeactivateIcon(Image icon)
    {
        if (icon != null)
        {
            icon.gameObject.SetActive(false);
        }
    }

    public float getTimeOfDay()
    {
        return _timeOfDay;
    }
}


