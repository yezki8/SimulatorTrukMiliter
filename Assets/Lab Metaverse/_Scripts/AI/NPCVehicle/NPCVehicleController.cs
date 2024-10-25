using PG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCVehicleController : MonoBehaviour
{
    // all npc vehicle
    [SerializeField] private List<GameObject> _npcVehicles = new();
    // [SerializeField] private List<Vector3> _npcVehiclesInitialPosition = new();

    public void ResetNPCVehiclePosition()
    {
        for (int i = 0; i < _npcVehicles.Count; i++)
        {
            // reset car
            _npcVehicles[i].GetComponent<CarController>().ResetVehicle();
            // _npcVehicles[i].transform.position = _npcVehiclesInitialPosition[i];
            _npcVehicles[i].SetActive(false);
        }
    }

    public void EnableNPCVehicles()
    {
        foreach (GameObject vehicle in _npcVehicles)
        {
            vehicle.SetActive(true);
        }
        // for special npc
        SpecialNPCController.Instance.GetInitialPos();
    }
    public void DisableNPCVehicles()
    {
        foreach (GameObject vehicle in _npcVehicles)
        {
            vehicle.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("SNEED");
        // get all vehicle with tag NPCVehicle
        GameObject[] vehicles = GameObject.FindGameObjectsWithTag("NPCVehicle");
        // get initial position of the first vehicle
        foreach (GameObject vehicle in vehicles)
        {
            // vehicle.SetActive(false);
            _npcVehicles.Add(vehicle);
            // _npcVehiclesInitialPosition.Add(vehicle.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
