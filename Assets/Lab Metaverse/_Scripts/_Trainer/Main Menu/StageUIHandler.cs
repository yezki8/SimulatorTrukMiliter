using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageUIHandler : MonoBehaviour
{
    [SerializeField] private GeneralMenuUIHandler _generalMenuUIHandler;
    [SerializeField] private List<Button> _stageButtons;       //Count need to be the same as Stage data list count

    [SerializeField] private TextMeshProUGUI _stageDescription;
    [SerializeField] private Image _stageImage;

    //[SerializeField] private CanvasGroup _confirmationPanel;

    public void SetUnlockedStageButton(int stageIndex, bool status)
    {
        _stageButtons[stageIndex].interactable = status;
    }

    public void SetStageDescription(string desc)
    {
        _stageDescription.SetText(desc);
    }

    //public void SpawnConfirmationPanel()
    //{
    //    _generalMenuUIHandler.CanvasGroupOn(_confirmationPanel);
    //}
}
