using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWalkerDestinationPoint : MonoBehaviour
{
    public Transform NextWalkingTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("WalkingNPC"))
        {
            other.GetComponent<NPCWalkerNavigator>().AssignDestination(NextWalkingTarget);
        }
    }
}
