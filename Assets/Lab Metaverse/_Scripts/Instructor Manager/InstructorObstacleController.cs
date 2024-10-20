using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PG;

public class InstructorObstacleController : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    [SerializeField] private InstructorObstacleUIHandler _instructorObstacleUIHandler;
    public void TurnOffTruck()
    {
        //TO DO: if there's time, need to add more "mogok" feel to it
        _carController.StopEngine();
    }

    public void ChangeBrakeStatus()
    {
        bool canBrakeNow = !_carController.CanBrake;
        _carController.CanBrake = !canBrakeNow;
    }

    public void ChangeBlowTireStatus(int wheelIndex)
    {
        Wheel targetWheel = _carController.Wheels[wheelIndex];
        targetWheel.IsBlownOut = !targetWheel.IsBlownOut;

        _instructorObstacleUIHandler.ChangeBlowOutText(wheelIndex, targetWheel.IsBlownOut);
    }

    public void ChangeWheelockStatus(int wheelIndex)
    {
        Wheel targetWheel = _carController.Wheels[wheelIndex];
        targetWheel.IsLocked = !targetWheel.IsLocked;

        _instructorObstacleUIHandler.ChangeLockText(wheelIndex, targetWheel.IsLocked);
    }

}
