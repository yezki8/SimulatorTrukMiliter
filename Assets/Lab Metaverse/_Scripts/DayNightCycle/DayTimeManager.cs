using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayTimeManager : MonoBehaviour
{
    //TODO: Convert this float to Scriptable object when needed in the future
    [Range(0, 24)]
    public float DefaultDayStart = 9;   //The hour of the game started

    public float CheckpointTimeOfDay; //The hour of the game started

    [Range(0, 24)]
    [SerializeField] private float _timeOfDay = 4;  // Current time of day (0-24)
    [SerializeField] private float _dayDuration = 120f;  // Duration of a full day in seconds
    private int _currentDay = 1;
    
    [SerializeField]
    private Image _moon; // Moon icon
    [SerializeField]
    private Image _sun; // Sun icon

    [SerializeField]
    private TextMeshProUGUI _timeText;  // Time Text UI

    public DayNightCycle dayNightCycle;

    private void Start()
    {
        InitializeDayTime();
    }

    private void OnValidate()
    {
        dayNightCycle.UpdateSunMoonPosition(_timeOfDay);
    }

    void Update()
    {
        //Make sure that day clock only run when game is active
        if (GameStateController.Instance.GameState == StateOfGame.Match)
        {
            CalculateDayTime();
            dayNightCycle.UpdateSunMoonPosition(_timeOfDay);
        }
    }

    public void InitializeDayTime()
    {
        _timeOfDay = DefaultDayStart;
        CheckpointTimeOfDay = _timeOfDay;
        
        // Deactivate sun and moon from start of the game
        DeactivateIcon(_sun);
        DeactivateIcon(_moon);

        CalculateDayTime();
    }

    public void StartByCheckpoint()
    {
        _timeOfDay = CheckpointTimeOfDay;
        CalculateDayTime();
    }

    void CalculateDayTime()
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

        // Update sun & moon icon
        UpdateHUDIcons(hour);

        // Set Morning or Night status
        string period = getPeriodOfDay(hour);

        // Format the time as a string
        string timeString = $"Day {_currentDay}, {period}\n{hour:00}:{minute:00}";

        // Update the TextMeshProUGUI component
        if (_timeText != null)
        {
            _timeText.text = timeString;
        }
    }

    void UpdateHUDIcons(float timeOfDay)
    {
        if (timeOfDay >= 18 || timeOfDay < 6) // Night phase
        {
            DeactivateIcon(_sun);
            ActivateIcon(_moon);
        }
        else if (timeOfDay >= 6 && timeOfDay < 18) // Day phase
        {
            DeactivateIcon(_moon);
            ActivateIcon(_sun);
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

    string getPeriodOfDay(float timeOfDay)
    {
        if (timeOfDay >= 6 && timeOfDay < 12)
        {
            return "Morning";
        }
        else if (timeOfDay >= 12 && timeOfDay < 18)
        {
            return "Afternoon";
        }
        else if (timeOfDay >= 18 && timeOfDay < 21)
        {
            return "Evening";
        }
        else
        {
            return "Night";
        }
    }

    public float getTimeOfDay()
    {
        return _timeOfDay;
    }
}


