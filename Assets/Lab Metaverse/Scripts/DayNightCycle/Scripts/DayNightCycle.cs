using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sun;  // Reference to the sun light
    public TimeManager timeManager;  // Reference to the TimeManager

    void Update()
    {
        if (timeManager != null && sun != null)
        {
            // Calculate the sun's rotation based on the time of day
            float sunAngle = (timeManager.timeOfDay / 24f) * 360f;
            sun.transform.rotation = Quaternion.Euler(new Vector3(sunAngle - 90, 170, 0));
        }
    }
}

