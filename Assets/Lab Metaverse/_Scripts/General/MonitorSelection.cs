using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonitorSelection : MonoBehaviour
{
    [SerializeField] private GameObject _setDisplayPanel;
    public TMP_Dropdown[] displayDropdowns = new TMP_Dropdown[6];

    void Start()
    {
        Debug.Log($"Unity detected {Display.displays.Length} displays.");
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
            Debug.Log($"Display {i + 1} activated.");
        }
        CloseDisplaySettingPanel();
        PopulateMonitorDropdowns();

        for (int i = 1; i < Mathf.Min(Display.displays.Length, 6); i++)
        {
            Display.displays[i].Activate();
        }
    }

    // Populate the monitor dropdowns with monitor indices
    void PopulateMonitorDropdowns()
    {
        for (int i = 0; i < displayDropdowns.Length; i++)
        {
            TMP_Dropdown dropdown = displayDropdowns[i];
            dropdown.ClearOptions();

            // Add "Monitor X" to the dropdown options (no actual monitor name)
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
        else
        {
            Debug.LogError($"Monitor {monitorIndex + 1} not available.");
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
