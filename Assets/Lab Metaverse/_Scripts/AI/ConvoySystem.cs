using PG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvoySystem : MonoBehaviour
{
    // List of all vehicles in the convoy
    [SerializeField] private List<ConvoyAIControl> _vehicleControls = new();

    // start & end point
    [Header("Convoy Points")]
    public GameObject startPoint;
    public GameObject endPoint;

    public static ConvoySystem Instance;

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
            // await a second before enabling the convoy
            Debug.Log("Awaiting convoy vehicles...");
            StartCoroutine(EnableConvoyVehiclesCoroutine());
            foreach (ConvoyAIControl vehicle in _vehicleControls)
            {
                vehicle.ConvoyEnabled = true;
            }
            Debug.Log("Convoy started");
            DisableTrigger(startPoint);
            EnableTrigger(endPoint);
        } 
        else
        {
            Debug.LogWarning("No vehicles in convoy");
        }
        
    }

    private IEnumerator EnableConvoyVehiclesCoroutine()
    {
        yield return new WaitForSeconds(5);
    }

    public void StopConvoyVehicles()
    {
        foreach (ConvoyAIControl vehicle in _vehicleControls)
        {
            vehicle.ConvoyEnabled = false;
        }
        DisableTrigger(endPoint);
        Debug.Log("Convoy finished");
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
