using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseController : MonoBehaviour
{
    public Transform ObjectToFollow;
    public Vector3 Offset;
    public float FollowSpeed = 10;
    public float LookSpeed = 10;

    [Header("Follow Restriction")]
    public bool MoveX = true;
    public bool MoveY = true;
    public bool MoveZ = true;

    [Header("Look Restriction")]
    public bool LookX = true;
    public bool LookY = true;
    public bool LookZ = true;

    private float _movementCacheX;
    private float _movementCacheY;
    private float _movementCacheZ;

    private float _lookCacheX;
    private float _lookCacheY;
    private float _lookCacheZ;

    public void LookAtTarger()
    {
        Vector3 lookDirection = ObjectToFollow.position - transform.position;
        Quaternion rotationDirection = Quaternion.LookRotation(lookDirection, Vector3.up);
        rotationDirection = CheckRotationRestriction(rotationDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationDirection, LookSpeed * Time.deltaTime);
    }

    public void MoveToTarget()
    {
        Vector3 targetPos = ObjectToFollow.position +
            ObjectToFollow.forward * Offset.z +
            ObjectToFollow.right * Offset.x +
            ObjectToFollow.up * Offset.y;

        targetPos = CheckMovementRestriction(targetPos);

        transform.position = Vector3.Lerp(transform.position, targetPos, FollowSpeed * Time.deltaTime);
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

    public void Update()
    {
        LookAtTarger();
        MoveToTarget();
        UpdateInitials();
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
