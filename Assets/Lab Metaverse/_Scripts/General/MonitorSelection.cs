using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonitorSelection : MonoBehaviour
{
    [SerializeField] private GameObject _setDisplayPanel;
    public TMP_Dropdown[] displayDropdowns = new TMP_Dropdown[8]; // 8 dropdowns, one for each display

    void Start()
    {
        CloseDisplaySettingPanel();
        PopulateMonitorDropdowns();

        for (int i = 1; i < Mathf.Min(Display.displays.Length, 8); i++)
        {
            Display.displays[i].Activate();
        }
    }

    void PopulateMonitorDropdowns()
    {
        for (int i = 0; i < displayDropdowns.Length; i++)
        {
            TMP_Dropdown dropdown = displayDropdowns[i];
            dropdown.ClearOptions();

            for (int j = 0; j < Display.displays.Length; j++)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData("Monitor " + (j + 1)));
            }

            dropdown.RefreshShownValue();
        }
    }

    public void OnMonitorSelected(int displayIndex)
    {
        int monitorIndex = displayDropdowns[displayIndex].value;

        if (monitorIndex < Display.displays.Length)
        {
            Display.displays[monitorIndex].Activate();
            Debug.Log($"Display {displayIndex + 1} is now using Monitor {monitorIndex + 1}.");
        }
    }

    public void OpenDisplaySettingPanel()
    {
        _setDisplayPanel.SetActive(true);
    }

    public void CloseDisplaySettingPanel()
    {
        _setDisplayPanel.SetActive(false);
    }
}
