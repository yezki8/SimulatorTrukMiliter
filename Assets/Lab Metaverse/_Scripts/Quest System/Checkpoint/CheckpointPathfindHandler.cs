using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointPathfindHandler : MonoBehaviour
{
    [SerializeField] private GameObject TargetCrossingRoad;
    [SerializeField] private ChaseController ArrowChaseController;
    [SerializeField] private Transform NextCheckpoint;
    
    // Start is called before the first frame update
    void Start()
    {
        RelocateToCrossing();
        AssignArrowTarget(NextCheckpoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RelocateToCrossing()
    {
        if (TargetCrossingRoad != null)
        {
            Transform targetTransform = TargetCrossingRoad.GetComponent<Transform>(); ;
            this.transform.SetPositionAndRotation(targetTransform.position + Vector3.up, targetTransform.rotation);
        }
    }

    public void AssignArrowTarget(Transform targetToLook)
    {
        if (targetToLook != null)
        {
            ArrowChaseController.gameObject.SetActive(true);
            ArrowChaseController.ObjectToFollow = targetToLook;
        }
        else
        {
            ArrowChaseController.gameObject.SetActive(false);
        }
    }

    public GameObject GetRoadObject() {
        return TargetCrossingRoad;
    }

    public GameObject GetNextCheckpoint() {
        return NextCheckpoint.gameObject;
    }
}
