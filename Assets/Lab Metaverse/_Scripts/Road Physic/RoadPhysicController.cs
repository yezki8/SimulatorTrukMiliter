using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPhysicController : MonoBehaviour
{
    [SerializeField] private List<WheelCollider> _frontTires;
    [SerializeField] private List<WheelCollider> _rearTires;

    [Header("Calculation Parameter")]
    [SerializeField] private float _frontSidewayExtremumDivider = 3 / 0.4f;
    [SerializeField] private float _frontSidewayAsymptoteParameter = 1 / 0.8f;
    [SerializeField] private float _rearSidewayExtremumDivider = 5 / 0.4f;
    [SerializeField] private float _rearSidewayAsymptoteParameter = 2 / 0.8f;

    [SerializeField] private List<RoadPhysicScriptableObject> _listOfPhysics;       //0 = default

    public static RoadPhysicController Instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void UpdateWheelPhysic()
    {
        int weatherState = (int)WeatherSystem.Instance.CurrentWeather;
        RoadPhysicScriptableObject roadPhysicSO;
        if (weatherState > 0 && weatherState < _listOfPhysics.Count)
        {
            roadPhysicSO = _listOfPhysics[(int)WeatherSystem.Instance.CurrentWeather];
        }
        else
        {
            roadPhysicSO = _listOfPhysics[0]; //setDefault
        }
        foreach (WheelCollider tires in _frontTires)
        {
            WheelFrictionCurve forwardFrontWfc = tires.forwardFriction;
            forwardFrontWfc.extremumValue = roadPhysicSO.ForwardExtremumParameter;
            forwardFrontWfc.extremumSlip = roadPhysicSO.ForwardExtremumParameter / 10;
            forwardFrontWfc.asymptoteValue = roadPhysicSO.ForwardAsymptoteParameter;
            forwardFrontWfc.asymptoteSlip = roadPhysicSO.ForwardExtremumParameter / 2;

            tires.forwardFriction = forwardFrontWfc;

            WheelFrictionCurve sideFrontWfc = tires.sidewaysFriction;
            forwardFrontWfc.extremumValue = roadPhysicSO.ForwardExtremumParameter;
            forwardFrontWfc.extremumSlip = 
                roadPhysicSO.ForwardExtremumParameter / _frontSidewayExtremumDivider;
            forwardFrontWfc.asymptoteValue = roadPhysicSO.ForwardAsymptoteParameter;
            forwardFrontWfc.asymptoteSlip = 
                roadPhysicSO.ForwardExtremumParameter / _frontSidewayAsymptoteParameter;

            tires.sidewaysFriction = sideFrontWfc;
        }

        Debug.Log("Road physics is set according to " + roadPhysicSO.name);
    }
}
