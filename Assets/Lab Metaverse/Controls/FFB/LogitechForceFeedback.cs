using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LogitechForceFeedback : ForceFeedbackProvider
{
    private StringBuilder steeringWheelName = new StringBuilder(256);
    private int deviceIdx;

    public override void InitProvider()
    {
        if (LogitechGSDK.LogiSteeringInitialize(false))
        {
            Debug.Log("Searching for steering wheel...");
            // check which device is a steering wheel
            // arbitrary number picked = 10 (max number of devices)
            for (int i = 0; i < 10; i++)
            {
                if (LogitechGSDK.LogiIsDeviceConnected(i, LogitechGSDK.LOGI_DEVICE_TYPE_WHEEL))
                {
                    deviceIdx = i;
                    LogitechGSDK.LogiGetFriendlyProductName(deviceIdx, steeringWheelName, 256);
                    Debug.Log($"Steering Wheel: {steeringWheelName}");
                    break;
                }
                // not found
                Debug.Log("No Logitech Steering Wheel found");
            }
        }
        else
        {
            Debug.Log("Logitech Steering Wheel not initialized");
        }
    }
    public override int GetPosOffset()
    {
        return _springPosOffset;
    }
    // Main FFB from centering spring
    public override void ApplySpringForce()
    {
        // sanity check
        if (_springSaturation > 0)
        {
            LogitechGSDK.LogiPlaySpringForce(deviceIdx, _springPosOffset, _springSaturation, _springCoefficient);
        }
    }
    public override void ApplyDirtRoadEffect()
    {
        if (_dirtRoadFFBMagnitude > 2)
        {
            // Debug.Log($"Applying Dirt Road Effect with magnitude: {_dirtRoadFFBMagnitude}");
            LogitechGSDK.LogiPlayDirtRoadEffect(deviceIdx, _dirtRoadFFBMagnitude);
        } 
        else
        {
            LogitechGSDK.LogiStopDirtRoadEffect(deviceIdx);
        }
    }

    public override void SetSpringMultiplier(float multipler)
    {
        _springSaturation = (int)(multipler * SpringMultiplier);
        _springCoefficient = (int)Mathf.Ceil(multipler * SpringMultiplier / 2f);
    }

    public override void OnShutdown()
    {
        LogitechGSDK.LogiSteeringShutdown();
    }
}
