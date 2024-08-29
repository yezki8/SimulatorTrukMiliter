using System.Collections;
using System.Collections.Generic;
using PG;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private CarController player;
    [SerializeField] private RectTransform speed_arrow;
    private float minSpeedAngle = 17f;
    private float maxSpeedAngle = -61.5f;

    void Update()
    {
        float currentSpeed = player.SpeedInHour;
        float maxSpeed = player.Engine.SpeedLimit;
        Debug.Log($"Current: {currentSpeed}, Max: {maxSpeed}");
        if (speed_arrow != null)
            speed_arrow.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minSpeedAngle, maxSpeedAngle, currentSpeed / maxSpeed));
    }
}
