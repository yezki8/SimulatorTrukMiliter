using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStateUIHandler : MonoBehaviour
{
    [System.Serializable]
    public class PanelGroup
    {
        public CanvasGroup[] GameStatePanels;
    }
    [Header("Game State Panels")]
    [SerializeField] private List<PanelGroup> _panelGroupList;

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
        for (int i = 0; i < _panelGroupList.Count; i++)
        {
            CanvasGroup[] targetGroup = _panelGroupList[i].GameStatePanels;
            if (i == targetPanel)
            {
                for (int j = 0; j < targetGroup.Length; j++)
                {
                    targetGroup[j].alpha = 1;
                    targetGroup[j].blocksRaycasts = true;
                    targetGroup[j].interactable = true;
                    targetGroup[j].GetComponentInChildren<Button>().Select();
                }
            }
            else
            {
                for (int j = 0; j < targetGroup.Length; j++)
                {
                    targetGroup[j].alpha = 0;
                    targetGroup[j].blocksRaycasts = false;
                    targetGroup[j].interactable = false;
                    targetGroup[j].GetComponentInChildren<Button>().Select();
                }
            }
        }
    }
}
