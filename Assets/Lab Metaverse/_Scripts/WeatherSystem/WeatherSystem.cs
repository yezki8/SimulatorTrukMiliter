using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeatherSystem : MonoBehaviour
{
    public enum WeatherType
    {
        Sunny,
        Cloudy,
        Rainy,
        // Snowy,
    }
    public WeatherType CurrentWeather { get; private set; }
    // player to get the player's position
    [SerializeField] private GameObject _player;

    [Header("Weather Related Object")]
    [SerializeField] private GameObject _rainSpawner;
    [SerializeField] private GameObject _sunLight;

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
        if (CurrentWeather == WeatherType.Rainy)
        {
            _rainSpawner.SetActive(true);
        }
        else
        {
            _rainSpawner.SetActive(false);
        }
    }

    // manage lighting based on weather
    private void HandleSkyLighting()
    {
        // change directional light color and intensity
        switch (CurrentWeather)
        {
            case WeatherType.Sunny:
                _sunLight.GetComponent<Light>().intensity = 1.0f;
                _sunLight.GetComponent<Light>().color = new Color(1.0f, 0.9f, 0.8f);
                break;
            case WeatherType.Cloudy:
                _sunLight.GetComponent<Light>().intensity = 0.8f;
                _sunLight.GetComponent<Light>().color = new Color(0.9f, 0.9f, 0.8f);
                break;
            case WeatherType.Rainy:
                _sunLight.GetComponent<Light>().intensity = 0.8f;
                _sunLight.GetComponent<Light>().color = new Color(0.8f, 0.8f, 0.8f);
                break;
            // case WeatherType.Snowy:
            //     _sunLight.SetActive(false);
            //     break;
            default:
                // default to sunny
                _sunLight.GetComponent<Light>().intensity = 1.0f;
                _sunLight.GetComponent<Light>().color = new Color(1.0f, 0.9f, 0.8f);
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
                CurrentWeather = WeatherType.Sunny;
                break;
            case 1:
                CurrentWeather = WeatherType.Cloudy;
                break;
            case 2:
                CurrentWeather = WeatherType.Rainy;
                break;
            // case 3:
            //     _currentWeather = WeatherType.Snowy;
            //     break;
            default:
                CurrentWeather = WeatherType.Sunny;
                break;
        }
        HandleRainSpawner();
        HandleSkyLighting();
        Debug.Log("Weather set to " + CurrentWeather);
    }

    // single button control to change weather
    public void ChangeWeather()
    {
        switch (CurrentWeather)
        {
            case WeatherType.Sunny:
                setWeather(1);
                break;
            case WeatherType.Cloudy:
                setWeather(2);
                break;
            case WeatherType.Rainy:
                setWeather(0);
                break;
            // case WeatherType.Snowy:
            //     setWeather(0);
            //     break;
            default:
                setWeather(0);
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        setWeather(0);
    }

    // Update is called once per frame
    void Update()
    {
        // rain spawner follow the player, offset to the front and top of the player direction
        _rainSpawner.transform.position = _player.transform.position + _player.transform.forward * 10 + _player.transform.up * 10;
    }
}
