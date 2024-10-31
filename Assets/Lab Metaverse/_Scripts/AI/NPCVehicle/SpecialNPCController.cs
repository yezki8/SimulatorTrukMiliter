using PG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialNPCController : MonoBehaviour
{
    [SerializeField] private List<SimAIControl> _carControllers = new();
    private List<Vector3> initialPos = new();
    private List<Quaternion> initialRot = new();

    public static SpecialNPCController Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void GetInitialPos()
    {
        // get every car initial position
        foreach (var car in _carControllers)
        {
            initialPos.Add(car.transform.position);
            initialRot.Add(car.transform.rotation);
        }
    }

    public void OnFinish(SimAIControl finishedVehicle)
    {
        // reset vehicle and move it to the initial position
        finishedVehicle.Car.ResetVehicle();
        // restore if damaged
        finishedVehicle.Car.RestoreVehicle();
        int index = _carControllers.IndexOf(finishedVehicle);
        finishedVehicle.transform.position = initialPos[index];
        finishedVehicle.transform.rotation = initialRot[index];
        finishedVehicle.ResetProgress();
    }
}
