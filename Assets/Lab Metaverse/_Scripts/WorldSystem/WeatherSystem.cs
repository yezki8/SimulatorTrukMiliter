using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

// heavy refactor needed
public class WeatherSystem : MonoBehaviour
{
    public enum WeatherType
    {
        Clear,
        LightCloud,
        Overcast,
        Rainy,
        ThunderStorm
    }
    public WeatherType CurrentWeather { get; private set; }
    // player to get the player's position
    [SerializeField] private GameObject _player;

    private VolumeProfile _volumeProfile;
    private Fog _fog;
    private ParticleSystem _rain;

    [Header("Weather Related Object")]
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private GameObject _rainSpawner;
    [SerializeField] private Light _sunLight;
    [SerializeField] private Light _moonLight;

    public UnityEvent OnWeatherChange;

    public static WeatherSystem Instance;

    private void Awake()
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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // manage rain spawner activation
    private void HandleRainSpawner()
    {
        if (CurrentWeather == WeatherType.Rainy || CurrentWeather == WeatherType.ThunderStorm)
        {
            _rainSpawner.SetActive(true);
            var newEmmision = _rain.emission;
            // set emmision rate
            switch (CurrentWeather)
            {
                case WeatherType.Rainy:
                    newEmmision.rateOverTime = 600;
                    break;
                case WeatherType.ThunderStorm:
                    newEmmision.rateOverTime = 1200;
                    break;
            }
        }
        else
        {
            _rainSpawner.SetActive(false);
        }
    }

    // manage lighting based on weather
    private void HandleSky()
    {
        // tryget fog
        _volumeProfile = _globalVolume.sharedProfile;

        if (!_volumeProfile.TryGet<Fog>(out var fog))
        {
            fog = _volumeProfile.Add<Fog>(false);
        }

        // tryget cloud
        if (!_volumeProfile.TryGet<VolumetricClouds>(out var cloud))
        {
            cloud = _volumeProfile.Add<VolumetricClouds>();
        }

        fog.enabled.overrideState = true;
        fog.meanFreePath.overrideState = true;

        // change directional light color and intensity
        switch (CurrentWeather)
        {
            case WeatherType.Clear:
                _sunLight.bounceIntensity = 1;
                fog.meanFreePath.value = 4000;
                cloud.cloudPreset = VolumetricClouds.CloudPresets.Sparse;
                break;
            case WeatherType.LightCloud:
                _sunLight.bounceIntensity = 0.95f;
                fog.meanFreePath.value = 4000;
                cloud.cloudPreset = VolumetricClouds.CloudPresets.Cloudy;
                break;
            case WeatherType.Overcast:
                _sunLight.bounceIntensity = 0.95f;
                fog.meanFreePath.value = 1000;
                cloud.cloudPreset = VolumetricClouds.CloudPresets.Overcast;
                break;
            case WeatherType.Rainy:
                _sunLight.bounceIntensity = 0.9f;
                fog.meanFreePath.value = 600;
                cloud.cloudPreset = VolumetricClouds.CloudPresets.Overcast;
                break;
            case WeatherType.ThunderStorm:
                _sunLight.bounceIntensity = 0.9f;
                fog.meanFreePath.value = 80;
                cloud.cloudPreset = VolumetricClouds.CloudPresets.Stormy;
                break;
            default:
                // default to clear
                _sunLight.bounceIntensity = 1;
                fog.meanFreePath.value = 4000;
                cloud.cloudPreset = VolumetricClouds.CloudPresets.Sparse;
                break;
        }
    }

    public WeatherType GetCurrentWeather()
    {
        return CurrentWeather;
    }

    private void setWeather(int weatherState)
    {
        switch (weatherState)
        {
            case 0:
                CurrentWeather = WeatherType.Clear;
                break;
            case 1:
                CurrentWeather = WeatherType.LightCloud;
                break;
            case 2:
                CurrentWeather = WeatherType.Overcast;
                break;
            case 3:
                CurrentWeather = WeatherType.Rainy;
                break;
            case 4:
                CurrentWeather = WeatherType.ThunderStorm;
                break;
            default:
                CurrentWeather = WeatherType.Clear;
                break;
        }
        HandleRainSpawner();
        HandleSky();
        Debug.Log("Weather set to " + CurrentWeather);

        OnWeatherChange?.Invoke();
    }

    // single button control to change weather
    public void ChangeWeather()
    {
        switch (CurrentWeather)
        {
            case WeatherType.Clear:
                setWeather(1);
                break;
            case WeatherType.LightCloud:
                setWeather(2);
                break;
            case WeatherType.Overcast:
                setWeather(3);
                break;
            case WeatherType.Rainy:
                setWeather(4);
                break;
            case WeatherType.ThunderStorm:
                setWeather(0);
                break;
            default:
                setWeather(0);
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // instantiate rain spawner prefab
        _rainSpawner = Instantiate(_rainSpawner);
        _rain = _rainSpawner.GetComponent<ParticleSystem>();
        setWeather(0);
    }

    // Update is called once per frame
    void Update()
    {
        // rain spawner follow the player, offset to the front and top of the player direction
        _rainSpawner.transform.position = _player.transform.position + _player.transform.forward * 15 + _player.transform.up * 15;
    }
}
