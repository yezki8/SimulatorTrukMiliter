using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PG;

public class InstructorObstacleController : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

        //add function for sound and UI here
    }

    public void ChangeWheelockStatus(int wheelIndex)
    {
        Wheel targetWheel = _carController.Wheels[wheelIndex];
        targetWheel.IsLocked = !targetWheel.IsLocked;

        //add function for sound and UI here
    }

    public void ChangeHeadlampStatus(bool isRightLamp)
    {

    }
}
