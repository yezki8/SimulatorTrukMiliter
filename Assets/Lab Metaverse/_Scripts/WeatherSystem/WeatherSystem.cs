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
    // rain spawner gameobject
    [Header("Rain Manager")]
    [SerializeField] private GameObject _rainSpawner;

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
        // rain spawner follow the player
        _rainSpawner.transform.position = new Vector3(_player.transform.position.x, _rainSpawner.transform.position.y, _player.transform.position.z);
    }
}
