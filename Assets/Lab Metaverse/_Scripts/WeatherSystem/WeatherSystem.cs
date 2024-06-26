using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeatherSystem : MonoBehaviour
{
    private enum WeatherType
    {
        Sunny,
        Cloudy,
        Rainy,
        // Snowy,
    }
    [SerializeField] private WeatherType _currentWeather = WeatherType.Sunny;
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

    private void setWeather(int weatherState)
    {
        switch (weatherState)
        {
            case 0:
                _currentWeather = WeatherType.Sunny;
                _rainSpawner.SetActive(false);
                break;
            case 1:
                _currentWeather = WeatherType.Cloudy;
                _rainSpawner.SetActive(false);
                break;
            case 2:
                _currentWeather = WeatherType.Rainy;
                _rainSpawner.SetActive(true);
                break;
            // case 3:
            //     _currentWeather = WeatherType.Snowy;
            //     break;
            default:
                _currentWeather = WeatherType.Sunny;
                _rainSpawner.SetActive(false);
                break;
        }
        Debug.Log("Weather set to " + _currentWeather);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // rain spawner follow the player
        _rainSpawner.transform.position = new Vector3(_player.transform.position.x, _rainSpawner.transform.position.y, _player.transform.position.z);
    }
}
