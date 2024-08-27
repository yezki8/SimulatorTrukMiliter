using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the interface for a force feedback provider.
/// Using Constant feedback
/// </summary>

public abstract class ForceFeedbackProvider : MonoBehaviour
{
    public abstract void InitProvider();
    public abstract void ApplySpringForce(float force);
}
