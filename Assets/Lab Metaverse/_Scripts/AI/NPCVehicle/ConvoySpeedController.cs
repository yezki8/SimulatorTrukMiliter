using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PG;

public class ConvoySpeedController : MonoBehaviour
{
    [SerializeField] private Transform _onFrontVehicle;
    [SerializeField] private float _distanceThreshold;
    public float CurrentOnFrontDistance;

    [SerializeField] private ConvoySpeedController _lastCarSpeedController;
    [SerializeField] private Transform _onBehindVehicle;
    public float CurrentBehindDistance;

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

        if (CurrentBehindDistance > _distanceThreshold && _onBehindVehicle !=null)
        {
            dynamicLimit = targetLimit * (_distanceThreshold / CurrentBehindDistance);
        }
        else
        {
            dynamicLimit = targetLimit;
        }

        CurrentSpeedLimit = dynamicLimit;

        return dynamicLimit;
    }
}
