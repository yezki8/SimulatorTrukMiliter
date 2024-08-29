using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the interface for a force feedback provider.
/// Using Constant feedback
/// </summary>

public abstract class ForceFeedbackProvider : MonoBehaviour
{
    private bool _enableFFB = false;
    [SerializeField] private int _springMaxPosOffset = 90;
    [SerializeField] private int _dirtRoadEffectMaxSpeedInfluence = 15;
    [Header("Centering Spring")]
    [Range(0,100)]
    [SerializeField] protected int _springSaturation = 0;
    [Range(-100,100)]
    [SerializeField] protected int _springCoefficient = 0;
    [SerializeField] protected int _springPosOffset = 0;
    [SerializeField] protected int _dirtRoadFFBMagnitude = 0;
    public int SpringMultiplier = 50;

    // Applied to all FFB providers
    public virtual void EnableFFB(bool enable) 
    {
        _enableFFB = enable;
    }
    public virtual bool IsFFBEnabled()
    {
        return _enableFFB;
    }
    public virtual int getDirtRoadEffectMaxSpeedInfluence()
    {
        return _dirtRoadEffectMaxSpeedInfluence;
    }
    public virtual void SetSpringPosOffset(float offset)
    {
        _springPosOffset = (int)(offset * _springMaxPosOffset);
    }
    public virtual void SetDirtRoadEffect(int magnitude)
    {
        _dirtRoadFFBMagnitude = magnitude * _dirtRoadEffectMaxSpeedInfluence;
    }

    public abstract void InitProvider();
    public abstract int GetPosOffset();
    public abstract void ApplySpringForce();
    public abstract void ApplyDirtRoadEffect();
    public abstract void SetSpringMultiplier(float saturation);
    
}
