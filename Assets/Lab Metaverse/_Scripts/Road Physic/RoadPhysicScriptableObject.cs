using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Road Physic SO", menuName = "ScriptableObjects/", order = 1)]
public class RoadPhysicScriptableObject : ScriptableObject
{
    [Header("Forward Friction")]
    public float ForwardExtremumParameter = 4;
    public float ForwardAsymptoteParameter = 1.5f;

    [Header("Front Sideway Friction")]
    public float FrontSidewayExtremumParameter = 3;
    public float FrontSidewayAsymptoteParameter = 1;

    [Header("Rear Sideway Friction")]
    public float RearSidewayExtremumParameter = 5;
    public float RearSidewayAsymptoteParameter = 2;

    public float RainDivider = 2;
}
