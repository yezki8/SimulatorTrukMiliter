using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PG;

public class DashboardController : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    private float CarSpeed;
    private float CarRPM;
    private float CarGear;
    
    [Header("Parameters for speedometer")]
    [SerializeField] private float _zeroSpeedDegree;
    [SerializeField] private float _maxSpeedDegree;
    [SerializeField] private float _maxDashboardSpeed;

    [Header("Parameters for RPMometer")]
    [SerializeField] private float _zeroRPMDegree;
    [SerializeField] private float _maxRPMDegree;
    [SerializeField] private float _maxDashboardRPM;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CarSpeed = _carController.SpeedInHour;
        CarRPM = _carController.EngineRPM;
        CarGear = _carController.CurrentGear;
    }
}
