using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sun;  // Reference to the sun light
    public DayTimeManager dayTimeManager;  // Reference to the TimeManager

    void Update()
    {
        //Make sure that day clock only run when game is active
        if (GameStateController.Instance.GameState == StateOfGame.Match)
        {
            if (dayTimeManager != null && sun != null)
            {
                // Calculate the sun's rotation based on the time of day
                float sunAngle = (dayTimeManager.timeOfDay / 24f) * 360f;
                sun.transform.rotation = Quaternion.Euler(new Vector3(sunAngle - 90, 170, 0));
            }
        }
    }
}

