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

    public void ResetAllGroups()
    {
        foreach (var npcGroup in GroupOfNPCs)
        {
            npcGroup.ResetGroup();
        }
    }

    public void ResetGroups(string groupName)
    {
        foreach(var npcGroup in GroupOfNPCs)
        {
            if (npcGroup.GroupName.Contains(groupName))
            {
                npcGroup.ResetGroup();
                break;
            }
        }
    }

    public void ActivateGroup(string groupName)
    {
        foreach (var npcGroup in GroupOfNPCs)
        {
            if (npcGroup.GroupName.Contains(groupName))
            {
                npcGroup.ActivateGroup();
                break;
            }
        }
    }
}
