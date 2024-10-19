using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// Probably should've been implemented in separate script,
// if separation of concerns is deemed necessary

public enum CameraMode
{
    FirstPerson,
    Chase
}

public class ChaseController : MonoBehaviour
{
    // alternative: Strategy Pattern
    [SerializeField] private CameraMode _cameraState;

    public Transform ObjectToFollow;
    public Vector3 ChaseOffset;
    public Vector3 FirstPersonCameraAnchor;
    public Vector3 ChaseCameraAnchor;
    public Vector3 FirstPersonCameraPosition;
    public float FollowSpeed = 10;
    public float LookSpeed = 10;
    public float MaxAngle = 80;

    [Header("Follow Restriction")]
    public bool MoveX = true;
    public bool MoveY = true;
    public bool MoveZ = true;

    [Header("Look Restriction")]
    public bool LookX = true;
    public bool LookY = true;
    public bool LookZ = true;

    [Header("Changing Values")]
    [SerializeField] private bool _isLooking = false;
    [SerializeField] private float _turnDegree;
    [SerializeField] private Vector3 _offset;

    private float _movementCacheX;
    private float _movementCacheY;
    private float _movementCacheZ;

    private float _lookCacheX;
    private float _lookCacheY;
    private float _lookCacheZ;

    // invoked by controller
    public void ChangeCameraState()
    {
        _cameraState = _cameraState == CameraMode.Chase ? CameraMode.FirstPerson : CameraMode.Chase;
        SetAnchorAndOffsetAfterChange();
    }

    public void SetCameraState(CameraMode state)
    {
        _cameraState = state;
        SetAnchorAndOffsetAfterChange();
    }

    public void SetAnchorAndOffsetAfterChange()
    {
        if (_cameraState == CameraMode.FirstPerson)
        {
            _offset = FirstPersonCameraPosition;
            ObjectToFollow.localPosition = FirstPersonCameraAnchor;
        }
        else if (_cameraState == CameraMode.Chase)
        {
            _offset = ChaseOffset;
            ObjectToFollow.localPosition = ChaseCameraAnchor;
        }
    }

    public void LookAtTarget()
    {
        Vector3 lookDirection = ObjectToFollow.position - transform.position;
        Quaternion rotationDirection = Quaternion.LookRotation(lookDirection, Vector3.up);
        rotationDirection = CheckRotationRestriction(rotationDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationDirection, LookSpeed * Time.deltaTime);
        
        if (_isLooking)
        {
            // add additional rotation
            transform.Rotate(Vector3.up, _turnDegree * LookSpeed * Time.deltaTime);
            // slight rotation downward
            transform.Rotate(Vector3.forward, -0.1f * _turnDegree * LookSpeed * Time.deltaTime);
        }
    }

    public void MoveToTarget()
    {
        Vector3 targetPos = ObjectToFollow.position +
            ObjectToFollow.forward * _offset.z +
            ObjectToFollow.right * _offset.x +
            ObjectToFollow.up * _offset.y;

        targetPos = CheckMovementRestriction(targetPos);

        if (_cameraState == CameraMode.Chase)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, FollowSpeed * Time.deltaTime);
        }
        else if (_cameraState == CameraMode.FirstPerson)
        {
            transform.position = targetPos;
        }
    }

    Vector3 CheckMovementRestriction(Vector3 targetPos)
    {
        if (!MoveX)
        {
            targetPos.x = _movementCacheX;
        }
        if (!MoveY)
        {
            targetPos.y = _movementCacheY;
        }
        if (!MoveZ)
        {
            targetPos.z = _movementCacheZ;
        }
        return targetPos;
    }

    Quaternion CheckRotationRestriction(Quaternion rotationDirection)
    {
        if (!LookX)
        {
            rotationDirection.x = _lookCacheX;
        }
        if (!LookY)
        {
            rotationDirection.y = _lookCacheY;
        }
        if (!LookZ)
        {
            rotationDirection.z = _lookCacheZ;
        }
        return rotationDirection;
    }

    public void Start()
    {
        SetCameraState(_cameraState);
    }

    public void Update()
    {
        LookAtTarget();
        MoveToTarget();
        UpdateInitials();
    }

    // updated by controller
    // note to dev: refactor to using interfaces if this feature ever needed later
    public void UpdateOffset(float value)
    {
        if (_cameraState == CameraMode.Chase)
        {
            // _offset.y = value.y > 0 ? _offset.z + value.y : InitialOffset.y;
            // circle camera around player, max 90 degree
            // use _offset.z as radius
            _offset.x = Mathf.Sin(value) * ChaseOffset.z; // left right
            _offset.z = Mathf.Cos(value) * ChaseOffset.z;
        } else if (_cameraState == CameraMode.FirstPerson)
        {
            if (value != 0)
            {
                _isLooking = true;
                _turnDegree = value * MaxAngle;
            }
            else
            {
                _isLooking = false;
            }
        }
    }

    void UpdateInitials()
    {
        //Update Movement 
        if (MoveX)
        {
            _movementCacheX = transform.position.x;
        }
        if (MoveY)
        {
            _movementCacheY = transform.position.y;
        }
        if (MoveZ)
        {
            _movementCacheZ = transform.position.z;
        }

        //Update Look Rotation
        if (LookX)
        {
            _lookCacheX = transform.rotation.x;
        }
        if (LookY)
        {
            _lookCacheY = transform.rotation.y;
        }
        if (LookZ)
        {
            _lookCacheZ = transform.rotation.z;
        }
    }

}
