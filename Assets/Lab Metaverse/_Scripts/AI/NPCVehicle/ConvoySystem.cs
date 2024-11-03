using PG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvoySystem : MonoBehaviour
{

    // List of all vehicles in the convoy
    [SerializeField] private List<ConvoyAIControl> _vehicleControls = new();
    private int _vehicleFinishedCount = 0;

    // start & end point
    [Header("Convoy Points")]
    public GameObject startPoint;
    public GameObject endPoint;
    public GameObject FinishLocation;

    public static ConvoySystem Instance;

    private bool _isRunning = false;
    private bool _isPlayerFinished = false;
    private bool _isOtherVehiclesFinished = false;

    private Coroutine _convoyCoroutine;

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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        EnableTrigger(startPoint);
        DisableTrigger(endPoint);
    }

    public void StartConvoyVehicles()
    {
        if (_vehicleControls.Count > 0)
        {
            DisableTrigger(startPoint);
            // await a second before enabling the convoy
            if (!_isRunning)
            {
                _isRunning = true;
                _isPlayerFinished = false;
                _isOtherVehiclesFinished = false;
                Debug.Log("Starting convoy...");
                _convoyCoroutine = StartCoroutine(EnableConvoyVehiclesCoroutine());
            }
        }
        else
        {
            Debug.LogWarning("No vehicles in convoy");
        }   
    }

    private IEnumerator EnableConvoyVehiclesCoroutine()
    {
        yield return new WaitForSeconds(5);
        foreach (ConvoyAIControl vehicle in _vehicleControls)
        {
            vehicle.ConvoyEnabled = true;
        }
        Debug.Log("Convoy started");
        EnableTrigger(endPoint);
    }

    public void StopConvoyVehicles()
    {
        // check if coroutine is running
        if (_convoyCoroutine != null)
        {
            StopCoroutine(_convoyCoroutine);
        }
        foreach (ConvoyAIControl vehicle in _vehicleControls)
        {
            vehicle.ConvoyEnabled = false;
        }
        DisableTrigger(endPoint);
        _isRunning = false;
        Debug.Log("Convoy finished");
    }

    public void ResetConvoyVehicles()
    {
        // safeguard
        if (_isRunning)
        {
            StopConvoyVehicles();
        }
        foreach (ConvoyAIControl vehicle in _vehicleControls)
        {
            vehicle.ConvoyEnabled = false;
            vehicle.Car.ResetVehicle();
            vehicle.ResetPosRotProgress();
        }
        DisableTrigger(endPoint);
        EnableTrigger(startPoint);
        EnableTrigger(FinishLocation);
    }

    public void FinishConvoy()
    {
        _isPlayerFinished = true;
    }

    public void VehicleFinished()
    {
        _vehicleFinishedCount++;
        if (_vehicleFinishedCount == _vehicleControls.Count)
        {
            Debug.Log("All convoy vehicles finished");
            _vehicleFinishedCount = 0;
            _isOtherVehiclesFinished = true;
            DisableTrigger(FinishLocation);
        }
    }

    private void Update()
    {
        if (_isPlayerFinished && _isOtherVehiclesFinished)
        {
            StopConvoyVehicles();
            _isPlayerFinished = false;
            _isOtherVehiclesFinished = false;
        }
    }

    private void EnableTrigger(GameObject Trigger)
    {
        Trigger.SetActive(true);
    }
    private void DisableTrigger(GameObject Trigger)
    {
        Trigger.SetActive(false);
    }
}
