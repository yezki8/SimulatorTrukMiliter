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
    public override int GetPosOffset()
    {
        return _springPosOffset;
    }
    // Main FFB from centering spring
    public override void ApplySpringForce()
    {
        int magnitude = _springPosOffset;
        // sanity check
        if (_springSaturation > 0)
        {
            LogitechGSDK.LogiPlaySpringForce(deviceIdx, magnitude, _springSaturation, _springCoefficient);
        }
    }
    public override void ApplyDirtRoadEffect()
    {
        int magnitude = _dirtRoadFFBMagnitude;
        Debug.Log(magnitude);
        if (magnitude > 1)
        {
            LogitechGSDK.LogiPlayDirtRoadEffect(deviceIdx, magnitude);
        }
    }

    public override void SetSpringMultiplier(float multipler)
    {
        _springSaturation = (int)(multipler * SpringMultiplier);
        _springCoefficient = (int)Mathf.Ceil(multipler * SpringMultiplier / 2f);
    }
    public override void SetSpringPosOffset(float offset)
    {
        _springPosOffset = (int)(offset * 90);
    }
    public override void SetDirtRoadEffect(int magnitude)
    {
        // max wheel count + adjustments
       _dirtRoadFFBMagnitude = (6 - magnitude) * 5;
    }
}
