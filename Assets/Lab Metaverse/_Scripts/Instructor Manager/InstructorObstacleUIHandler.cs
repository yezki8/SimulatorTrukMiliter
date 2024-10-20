using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructorObstacleUIHandler : MonoBehaviour
{
    [SerializeField] private List<Button> _blowTireButtons;
    [SerializeField] private List<Button> _lockButtons;
    [SerializeField] private CanvasGroup CrackWindowPanels;

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

    public void DisplayCrackPanel()
    {
        CrackWindowPanels.alpha = 1;
    }

    public void HideCrackPanel()
    {
        CrackWindowPanels.alpha = 0;
    }
}
