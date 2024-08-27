using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LogitechForceFeedback : ForceFeedbackProvider
{
    private StringBuilder steeringWheelName = new StringBuilder(256);
    private int deviceIdx;
    public int SpringSaturation = 20;
    public int SpringCoefficient = 10;

    public override void InitProvider()
    {
        if (LogitechGSDK.LogiSteeringInitialize(false))
        {
            deviceIdx = 0;
        }
        if (LogitechGSDK.LogiIsDeviceConnected(deviceIdx, LogitechGSDK.LOGI_DEVICE_TYPE_WHEEL))
        {
            LogitechGSDK.LogiGetFriendlyProductName(deviceIdx, steeringWheelName, 256);
            Debug.Log($"Steering Wheel: {steeringWheelName}");
        }
        else
        {
            Debug.Log("No Logitech device found");
        }
    }
    // Main FFB from centering spring
    public override void ApplySpringForce(float force)
    {
        int magnitude = (int)(force * 100);
        LogitechGSDK.LogiPlaySpringForce(deviceIdx, magnitude, SpringSaturation, SpringCoefficient);
    }
}
