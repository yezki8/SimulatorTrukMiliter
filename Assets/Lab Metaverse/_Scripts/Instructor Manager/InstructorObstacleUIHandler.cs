using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PG;

public class InstructorObstacleUIHandler : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    [SerializeField] private List<Button> _blowTireButtons;
    [SerializeField] private List<Button> _lockButtons;
    [SerializeField] private CanvasGroup CrackWindowPanels;
    [SerializeField] private Button _brakeButton;
    [SerializeField] private Button _handBrakeButton;

    public void ChangeBlowOutText(int index, bool isBlownOut)
    {
        _blowTireButtons[index].transform.GetChild(0).gameObject.SetActive(!isBlownOut);
        _blowTireButtons[index].transform.GetChild(1).gameObject.SetActive(isBlownOut);
    }

    public void ChangeLockText(int index, bool isLocked)
    {
        _lockButtons[index].transform.GetChild(0).gameObject.SetActive(!isLocked);
        _lockButtons[index].transform.GetChild(1).gameObject.SetActive(isLocked);
    }

    public void ChangeBrakeButtonStatus(bool canBrake)
    {
        _brakeButton.transform.GetChild(0).gameObject.SetActive(canBrake);
        _brakeButton.transform.GetChild(1).gameObject.SetActive(!canBrake);
    }

    public void ChangeHandBrakeButtonStatus(bool canHandBrake)
    {
        _handBrakeButton.transform.GetChild(0).gameObject.SetActive(canHandBrake);
        _handBrakeButton.transform.GetChild(1).gameObject.SetActive(!canHandBrake);
    }

    public void DisplayCrackPanel()
    {
        CrackWindowPanels.alpha = 1;
    }

    public void HideCrackPanel()
    {
        CrackWindowPanels.alpha = 0;
    }
}
