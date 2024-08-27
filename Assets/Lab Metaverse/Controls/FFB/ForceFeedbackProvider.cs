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
    [SerializeField] protected int _springSaturation = 20;
    [SerializeField] protected int _springCoefficient = 10;
    public int SpringMultiplier = 50;
    public abstract void InitProvider();
    public abstract void ApplySpringForce(float force);
    public abstract void SetSpringMultiplier(float saturation);
}
