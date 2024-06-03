using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField]
    private Light _sun;  // Reference to the sun light
    [SerializeField]
    private TimeManager _timeManager;  // Reference to the TimeManager

    void Update()
    {
        if (_timeManager != null && _sun != null)
        {
            // Calculate the sun's rotation based on the time of day
            float sunAngle = (_timeManager.getTimeOfDay() / 24f) * 360f;
            _sun.transform.rotation = Quaternion.Euler(new Vector3(sunAngle - 90, 170, 0));
        }
    }
}

