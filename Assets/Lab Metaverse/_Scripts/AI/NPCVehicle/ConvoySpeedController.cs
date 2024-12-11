using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvoySpeedController : MonoBehaviour
{
    [SerializeField] private Transform _onFrontVehicle;
    [SerializeField] private float _distanceThreshold;
    public float CurrentOnFrontDistance;
    public float SpeedLimitMultiplierThreshold = 2;

    [SerializeField] private ConvoySpeedController _lastCarSpeedController;

    [Header("For Car Leader")]
    [SerializeField] private bool _isLeader = false;
    [SerializeField] private Transform _lastVehicle;
    [SerializeField] private Transform _onBehindVehicle;
    [SerializeField] private float _behindDistanceThreshold;
    [SerializeField] private float _wholeDistanceThreshold;
    public float CurrentBehindDistance;
    public float CurrentWholeDistance;

    [SerializeField] [Range(1, 9)] private float _speedLimitLevel = 5;

    public float CurrentSpeedLimit = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDistance();
    }

    void CalculateDistance()
    {
        if (_isLeader && _lastVehicle != null)
        {
            CurrentWholeDistance = Vector3.Distance(this.transform.position, _lastVehicle.position);
            CurrentBehindDistance = Vector3.Distance(this.transform.position, _onBehindVehicle.position);
        }
        else
        {
            if (_onFrontVehicle != null)
            {
                CurrentOnFrontDistance = Vector3.Distance(this.transform.position, _onFrontVehicle.position);
            }
        }
    }

    public float GetDynamicSpeedLimit(float speedLimit)
    {
        float dynamicLimit = speedLimit;
        if (_lastCarSpeedController != null)
        {
            dynamicLimit = _lastCarSpeedController.CurrentSpeedLimit;
        }
        if (_isLeader)
        {
            if (CurrentWholeDistance > _wholeDistanceThreshold)
            {
                dynamicLimit = (speedLimit / 10) * _speedLimitLevel;
            }
            else
            {
                dynamicLimit = speedLimit * (CurrentBehindDistance / _behindDistanceThreshold);
            }
        }
        else
        {
            dynamicLimit = speedLimit * (CurrentOnFrontDistance /_distanceThreshold);
        }

        if (dynamicLimit > speedLimit * SpeedLimitMultiplierThreshold)
        {
            dynamicLimit = speedLimit * SpeedLimitMultiplierThreshold;
        }

        CurrentSpeedLimit = dynamicLimit;

        return dynamicLimit;
    }
}
