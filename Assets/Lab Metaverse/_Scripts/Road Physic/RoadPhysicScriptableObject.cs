using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoadPhysicSO", menuName = "ScriptableObjects/Physic", order = 1)]
public class RoadPhysicScriptableObject : ScriptableObject
{
    [Header("Forward Friction")]
    public float ForwardExtremumValue = 6;
    public float ForwardAsymptoteValue = 4f;

    [Header("Front Sideways Friction")]
    public float FrontSidewayExtremumValue = 4;
    public float FrontSidewayAsymptoteValue = 3.2f;

    [Header("Rear Sideways Friction")]
    public float RearSidewayExtremumValue = 5;
    public float RearSidewayAsymptoteValue = 4;
}
