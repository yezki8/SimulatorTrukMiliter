using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PG;

public class DashboardController : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    [SerializeField] private DashboardUIHandler _dashboardUIHandler;
    private float carSpeed;
    private float carRPM;
    private int carGear;
    
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
        if (GameStateController.Instance.GameState == StateOfGame.Match)
        {
            carSpeed = _carController.SpeedInHour;
            carRPM = _carController.EngineRPM;
            carGear = _carController.CurrentGear;
        }
        else
        {
            carSpeed = _zeroSpeedDegree;
            carRPM = _zeroRPMDegree;
            carGear = -2;
        }
        CalculateSpeedPin();
    }

    void CalculateSpeedPin()
    {
        float speedDegreeRange = (_maxSpeedDegree - _zeroSpeedDegree);
        float currentSpeedDegree = _zeroSpeedDegree + ((speedDegreeRange / _maxDashboardSpeed) * carSpeed);

        float speedRPMRange = (_maxRPMDegree - _zeroRPMDegree);
        float currentRPMDegree = _zeroRPMDegree + ((speedRPMRange / _maxDashboardRPM) * carRPM);

        _dashboardUIHandler.DisplayDashboard(currentSpeedDegree, currentRPMDegree, carGear);
    }
}
