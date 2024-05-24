using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private ChaseController _mainChaseController;

    //for cutscene and trainer's control of camera
    public void CameraFollowObject(Transform targetTransform)
    {
        _mainChaseController.ObjectToFollow = targetTransform;
    }
}
