using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light sun;  // Reference to the sun light
    [SerializeField] private Light moon;  // Reference to the moon light
    private float _sunAngle;
    private float _moonAngle;
    public float SunIntensity = 3000f;
    [SerializeField] AnimationCurve SunIntensityMultiplier;

    public void UpdateSunMoonPosition(float TimeOfDay)
    {         
        // Calculate the sun's rotation based on the time of day
        _sunAngle = (TimeOfDay / 24f) * 360f;
        sun.transform.rotation = Quaternion.Euler(new Vector3(_sunAngle - 90, 170, 0));

        // moon rotation opposite to sun
        _moonAngle = (TimeOfDay / 24f) * 360f;
        moon.transform.rotation = Quaternion.Euler(new Vector3(_moonAngle + 90, 170, 0));

        // only one light can cast shadows at a time
        // if (_sunAngle > 180)
        // {
        //     sun.shadows = LightShadows.Soft;
        //     moon.shadows = LightShadows.None;
        // }
        // else
        // {
        //     sun.shadows = LightShadows.None;
        //     moon.shadows = LightShadows.Soft;
        // }

        // set the intensity of the sun
        HDAdditionalLightData sunData = sun.GetComponent<HDAdditionalLightData>();
        if (sunData != null)
        {
            sunData.intensity = SunIntensityMultiplier.Evaluate(TimeOfDay / 24f) * SunIntensity;
        }
    }
}

