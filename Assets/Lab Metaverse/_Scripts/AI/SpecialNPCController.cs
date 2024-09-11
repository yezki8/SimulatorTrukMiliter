using PG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialNPCController : MonoBehaviour
{
    [SerializeField] private List<CarController> _carControllers = new();
    private List<Transform> initialPos = new();

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
            initialPos.Add(car.transform);
            Debug.Log("Initial position: " + car.transform.position);
        }
    }

    public void OnFinish(CarController finishedVehicle)
    {
        Debug.Log("Vehicle finished: " + finishedVehicle.name);
        // reset vehicle and move it to the initial position
        finishedVehicle.ResetVehicle();
        finishedVehicle.transform.position = initialPos[_carControllers.IndexOf(finishedVehicle)].position;
        finishedVehicle.transform.rotation = initialPos[_carControllers.IndexOf(finishedVehicle)].rotation;
    }
}
