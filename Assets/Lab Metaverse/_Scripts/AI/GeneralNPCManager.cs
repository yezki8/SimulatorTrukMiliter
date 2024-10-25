using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralNPCManager : MonoBehaviour
{
    [System.Serializable]
    public class NPCGroup
    {
        public string GroupName;
        public List<NPCWalkerNavigator> ListOfPedestrian;
        public List<NPCVehicleController> ListOfVehicles;

        public void ResetGroup()
        {
            foreach (var pedestrian in ListOfPedestrian)
            {
                pedestrian.ResetPedestrian();
            }
            foreach (var vehicle in ListOfVehicles)
            {
                vehicle.ResetNPCVehicle();
            }
        }

        public void ActivateGroup()
        {
            foreach (var pedestrian in ListOfPedestrian)
            {
                pedestrian.ActivatePedestrian();
            }
            foreach (var vehicle in ListOfVehicles)
            {
                vehicle.EnableNPCVehicles();
            }
        }
    }
    public List<NPCGroup> GroupOfNPCs;

    public void ResetGroups()
    {
        foreach(var npcGroup in GroupOfNPCs)
        {
            npcGroup.ResetGroup();
        }
    }

    public void ActivateGroup()
    {
        foreach (var npcGroup in GroupOfNPCs)
        {
            npcGroup.ActivateGroup();
        }
    }
}
