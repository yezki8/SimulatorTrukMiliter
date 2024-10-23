using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PG;

public class InstructorObstacleController : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    [SerializeField] private InstructorObstacleUIHandler _instructorObstacleUIHandler;
    [SerializeField] private float windowCrackTreshold = 4;

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

    public void CheckWindowCrack(float damageForceX)
    {
        damageForceX = Mathf.Abs(damageForceX);
        Debug.Log("Damage = " + damageForceX);
        if (damageForceX > windowCrackTreshold)
        {
            _instructorObstacleUIHandler.DisplayCrackPanel();
        }
    }

    public void FixAll()
    {
        for (int i = 0; i < _carController.Wheels.Length; i++)
        {
            Wheel targetWheel = _carController.Wheels[i];
            if (targetWheel.IsBlownOut)
            {
                ChangeBlowTireStatus(i);
            }
            if (targetWheel.IsLocked)
            {
                ChangeWheelockStatus(i);
            }
        }
        _instructorObstacleUIHandler.HideCrackPanel();
    }

}
