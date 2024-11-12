using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonitorSelection : MonoBehaviour
{
    [SerializeField] private GameObject _setDisplayPanel;
    [SerializeField] private Camera FrontViewCamera;
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

        for (int i = 0; i < Mathf.Min(Display.displays.Length, 6); i++)
        {
            Display.displays[i].Activate();
        }

        PopulateMonitorDropdowns();

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
        List<TMP_Dropdown.OptionData> monitorOptions = new ();
        for (int i = 0; i < Display.displays.Length; i++)
        {
            monitorOptions.Add(new TMP_Dropdown.OptionData("Monitor " + (i + 1)));
        }

        for (int i = 0; i < displayDropdowns.Length; i++)
        {
            TMP_Dropdown dropdown = displayDropdowns[i];
            dropdown.ClearOptions();
            dropdown.AddOptions(monitorOptions);
            dropdown.value = i < monitorOptions.Count ? i : 0;
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

    public void SetInstructorDisplay(int monitorIndex)
    {
        InstructorDisplay.targetDisplay = monitorIndex;
        MainCamera.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }
    public void SetFrontViewCamera(int monitorIndex)
    {
        FrontViewCamera.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }

    public void SetLeftMirrorDisplay(int monitorIndex)
    {
        LeftMirrorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }

    public void SetRightMirrorDisplay(int monitorIndex)
    {
        RightMirrorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }

    public void SetRearMirrorDisplay(int monitorIndex)
    {
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