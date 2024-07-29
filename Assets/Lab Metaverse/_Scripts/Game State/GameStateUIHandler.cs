using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStateUIHandler : MonoBehaviour
{
    [Header("Game State Panels")]
    [SerializeField] private CanvasGroup [] _gameStatePanels;

    // reconsider how to handle this
    [Header("Button Text to Change")]
    public Button SetWeatherButton;

    [Header("Weather System Reference")]
    [SerializeField] private WeatherSystem _weatherSystem;


    // update text
    public void updateButtonText(Button button, string text)
    {
        if (button == SetWeatherButton)
        {
            SetWeatherButton.GetComponentInChildren<TMP_Text>().text = text;
            Debug.Log("Button text updated");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        updateButtonText(SetWeatherButton, "Current Weather: " + _weatherSystem.GetCurrentWeather());
        // listener to change button text
        SetWeatherButton.onClick.AddListener(() => updateButtonText(SetWeatherButton, "Current Weather: " + _weatherSystem.GetCurrentWeather()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        
    public void ActivatePanel(int targetPanel)      //Called directly by GameStateController
    {
        for (int i = 0; i < _gameStatePanels.Length; i++)
        {
            if (i == targetPanel)
            {
                //TO DO: replace this with animation
                _gameStatePanels[i].alpha = 1;
                _gameStatePanels[i].blocksRaycasts = true;
                _gameStatePanels[i].interactable = true;
                _gameStatePanels[i].GetComponentInChildren<Button>().Select();
            }
            else
            {
                //TO DO: replace this with animation
                _gameStatePanels[i].alpha = 0;
                _gameStatePanels[i].blocksRaycasts = false;
                _gameStatePanels[i].interactable = false;
                _gameStatePanels[i].GetComponentInChildren<Button>().Select();
            }
        }
    }
}
