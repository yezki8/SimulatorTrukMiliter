using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the interface for a force feedback provider.
/// Using Constant feedback
/// </summary>

public abstract class ForceFeedbackProvider : MonoBehaviour
{
    [Header("Centering Spring")]
    [Range(0,100)]
    [SerializeField] protected int _springSaturation = 0;
    [Range(-100,100)]
    [SerializeField] protected int _springCoefficient = 0;
    [SerializeField] protected int _springPosOffset = 0;
    [SerializeField] protected int _dirtRoadFFBMagnitude = 0;
    public int SpringMultiplier = 50;

    public abstract void InitProvider();
    public abstract int GetPosOffset();
    public abstract void ApplySpringForce();
    public abstract void ApplyDirtRoadEffect();
    public abstract void SetSpringMultiplier(float saturation);
    public abstract void SetSpringPosOffset(float offset);
    public abstract void SetDirtRoadEffect(int magnitude);
}
