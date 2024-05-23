using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateUIHandler : MonoBehaviour
{
    [Header("Game State Panels")]
    [SerializeField] private CanvasGroup [] _gameStatePanels;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivatePanel(int targetPanel)
    {
        for (int i = 0; i < _gameStatePanels.Length; i++)
        {
            if (i == targetPanel)
            {
                //TO DO: replace this with animation
                _gameStatePanels[i].alpha = 1;
                _gameStatePanels[i].blocksRaycasts = true;
                _gameStatePanels[i].interactable = true;
            }
            else
            {
                //TO DO: replace this with animation
                _gameStatePanels[i].alpha = 0;
                _gameStatePanels[i].blocksRaycasts = false;
                _gameStatePanels[i].interactable = false;
            }
        }
    }
}
