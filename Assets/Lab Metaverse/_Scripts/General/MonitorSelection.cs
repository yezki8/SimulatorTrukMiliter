using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonitorSelection : MonoBehaviour
{
    [SerializeField] private GameObject _setDisplayPanel;
    [SerializeField] private Camera FrontViewCamera;
    [SerializeField] private Camera LeftMirrorCamera;
    [SerializeField] private Camera RightMirrorCamera;
    [SerializeField] private Camera RearMirrorCamera;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Canvas InstructorDisplay;
    [SerializeField] private Canvas LeftMirrorDisplay;
    [SerializeField] private Canvas RightMirrorDisplay;
    [SerializeField] private Canvas RearMirrorDisplay;
    [SerializeField] private Canvas DashboardDisplay;
    public TMP_Dropdown[] displayDropdowns = new TMP_Dropdown[6];

    void Start()
    {
        _setDisplayPanel.SetActive(false);
        PopulateMonitorDropdowns();

        for (int i = 0; i < Mathf.Min(Display.displays.Length, 6); i++)
        {
            Display.displays[i].Activate();
        }

        displayDropdowns[0].onValueChanged.AddListener((value) => SetInstructorDisplay(value));
        displayDropdowns[1].onValueChanged.AddListener((value) => SetFrontViewCamera(value));
        displayDropdowns[2].onValueChanged.AddListener((value) => SetLeftMirrorDisplay(value));
        displayDropdowns[3].onValueChanged.AddListener((value) => SetRightMirrorDisplay(value));
        displayDropdowns[4].onValueChanged.AddListener((value) => SetRearMirrorDisplay(value));
        displayDropdowns[5].onValueChanged.AddListener((value) => SetTruckDashboardDisplay(value));

        LoadMonitorSetup();
    }

    void PopulateMonitorDropdowns()
    {
        int availableDisplays = Display.displays.Length;

        for (int i = 0; i < displayDropdowns.Length; i++)
        {
            TMP_Dropdown dropdown = displayDropdowns[i];
            dropdown.ClearOptions();

            for (int j = 0; j < availableDisplays; j++)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData("Monitor " + (j + 1)));
            }

            dropdown.RefreshShownValue();
        }
    }

    public void SaveMonitorSetup()
    {
        for (int i = 0; i < displayDropdowns.Length; i++)
        {
            PlayerPrefs.SetInt("MonitorSetup_" + i, displayDropdowns[i].value);
        }
        PlayerPrefs.Save();
    }

    public void LoadMonitorSetup()
    {
        for (int i = 0; i < displayDropdowns.Length; i++)
        {
            int savedValue = PlayerPrefs.GetInt("MonitorSetup_" + i, 0);
            displayDropdowns[i].value = savedValue;
        }
    }

    public void SetFrontViewCamera(int monitorIndex)
    {
        FrontViewCamera.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }

    public void SetInstructorDisplay(int monitorIndex)
    {
        MainCamera.targetDisplay = monitorIndex;
        InstructorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }

    public void SetLeftMirrorDisplay(int monitorIndex)
    {
        LeftMirrorCamera.targetDisplay = monitorIndex;
        LeftMirrorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }

    public void SetRightMirrorDisplay(int monitorIndex)
    {
        RightMirrorCamera.targetDisplay = monitorIndex;
        RightMirrorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }

    public void SetRearMirrorDisplay(int monitorIndex)
    {
        RearMirrorCamera.targetDisplay = monitorIndex;
        RearMirrorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }

    public void SetTruckDashboardDisplay(int monitorIndex)
    {
        DashboardDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }

    public void OpenDisplaySettingPanel()
    {
        _setDisplayPanel.SetActive(true);
    }

    public void CloseDisplaySettingPanel()
    {
        _setDisplayPanel.SetActive(false);
        SaveMonitorSetup();
    }
}
