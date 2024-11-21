using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPhysicController : MonoBehaviour
{
    [SerializeField] private List<WheelCollider> _frontTires;
    [SerializeField] private List<WheelCollider> _rearTires;

    [Header("Calculation Parameter")]
    [SerializeField] float _extremumSlip = 0.6f;
    [SerializeField] float _asymptoteSlip = 0.7f;
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
        var weatherState = (int)WeatherSystem.Instance.CurrentWeather;
        RoadPhysicScriptableObject roadPhysicSO;
        if (weatherState > 0 && weatherState < _listOfPhysics.Count)
        {
            roadPhysicSO = _listOfPhysics[(int)WeatherSystem.Instance.CurrentWeather];
        }
        else
        {
            roadPhysicSO = _listOfPhysics[0]; //setDefault
        }
        foreach (var tires in _frontTires)
        {
            var forwardFrontWfc = tires.forwardFriction;
            forwardFrontWfc.extremumValue = roadPhysicSO.ForwardExtremumValue;
            forwardFrontWfc.extremumSlip = _extremumSlip;
            forwardFrontWfc.asymptoteValue = roadPhysicSO.ForwardAsymptoteValue;
            forwardFrontWfc.asymptoteSlip = _asymptoteSlip;

            tires.forwardFriction = forwardFrontWfc;

            var sideFrontWfc = tires.sidewaysFriction;
            sideFrontWfc.extremumValue = roadPhysicSO.FrontSidewayExtremumValue;
            sideFrontWfc.extremumSlip = _extremumSlip;
            sideFrontWfc.asymptoteValue = roadPhysicSO.ForwardAsymptoteValue;
            sideFrontWfc.asymptoteSlip = _asymptoteSlip;

            tires.sidewaysFriction = sideFrontWfc;
        }

        foreach (var tires in _rearTires)
        {
            var forwardRearWfc = tires.forwardFriction;
            forwardRearWfc.extremumValue = roadPhysicSO.ForwardExtremumValue;
            forwardRearWfc.extremumSlip = _extremumSlip;
            forwardRearWfc.asymptoteValue = roadPhysicSO.ForwardAsymptoteValue;
            forwardRearWfc.asymptoteSlip = _asymptoteSlip;
            
            tires.forwardFriction = forwardRearWfc;
            
            var sideRearWfc = tires.sidewaysFriction;
            sideRearWfc.extremumValue = roadPhysicSO.RearSidewayExtremumValue;
            sideRearWfc.extremumSlip = _extremumSlip;
            sideRearWfc.asymptoteValue = roadPhysicSO.RearSidewayExtremumValue;
            sideRearWfc.asymptoteSlip = _asymptoteSlip;
            
            tires.sidewaysFriction = sideRearWfc;
        }

        Debug.Log("Road physics is set according to " + roadPhysicSO.name);
    }
}
