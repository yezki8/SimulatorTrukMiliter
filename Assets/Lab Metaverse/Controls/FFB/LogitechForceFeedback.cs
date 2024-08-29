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
            Debug.Log($"Applying Dirt Road Effect with magnitude: {_dirtRoadFFBMagnitude}");
            LogitechGSDK.LogiPlayDirtRoadEffect(deviceIdx, _dirtRoadFFBMagnitude);
        }
    }

    public override void SetSpringMultiplier(float multipler)
    {
        _springSaturation = (int)(multipler * SpringMultiplier);
        _springCoefficient = (int)Mathf.Ceil(multipler * SpringMultiplier / 2f);
    }
}
