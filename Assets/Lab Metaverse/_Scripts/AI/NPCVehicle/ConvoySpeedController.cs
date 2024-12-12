using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PG;

public class ConvoySpeedController : MonoBehaviour
{
    [SerializeField] private Transform _onFrontVehicle;
    [SerializeField] private float _distanceThreshold;
    public float CurrentOnFrontDistance;
    private float _prevFrontDistance;

    [SerializeField] private ConvoySpeedController _lastCarSpeedController;
    [SerializeField] private Transform _onBehindVehicle;
    public float CurrentBehindDistance;
    private float _prevBehindDistance;

    [SerializeField] [Range(1, 9)] private float _speedLimitLevel = 5;

    public float CurrentSpeedLimit = 0;

    [SerializeField] private bool _isWaitingForVehicleBehind = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _prevFrontDistance = CurrentOnFrontDistance;
        _prevBehindDistance = CurrentBehindDistance;
        CalculateDistance();
    }

    void CalculateDistance()
    {
        if (_onFrontVehicle != null)
        {
            CurrentOnFrontDistance = Vector3.Distance(this.transform.position, _onFrontVehicle.position);
        }
        if (_onBehindVehicle != null)
        {
            CurrentBehindDistance = Vector3.Distance(this.transform.position, _onBehindVehicle.position);
        }
    }

    public float GetDynamicSpeedLimit(float speedLimit)
    {
        float targetLimit = speedLimit;
        if (_lastCarSpeedController != null)
        {
            targetLimit = _lastCarSpeedController.CurrentSpeedLimit;
        }
        float dynamicLimit = targetLimit;

        if (_isWaitingForVehicleBehind)
        {
            // checks: vehicle behind exist, distance threshold, currentSpeed - prevSpeed > 0f
            if (_onBehindVehicle != null && CurrentBehindDistance > _distanceThreshold && _prevBehindDistance - CurrentBehindDistance < 1f)
            {
                float tempLimit = targetLimit - 
                    ((targetLimit * ((CurrentBehindDistance - _distanceThreshold) / _distanceThreshold)) * 
                    (_speedLimitLevel/10));


                if (tempLimit < targetLimit / 4)
                {
                    dynamicLimit = targetLimit / 4;
                }
                else
                {
                    dynamicLimit = tempLimit;
                }
                
            }
            else
            {
                dynamicLimit = targetLimit;
            }
        }        

        CurrentSpeedLimit = dynamicLimit;

        return dynamicLimit;
    }
}
