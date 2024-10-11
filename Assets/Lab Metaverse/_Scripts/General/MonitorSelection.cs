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

    // Assign cameras and canvases for each display

    void Start()
    {
        CloseDisplaySettingPanel();
        PopulateMonitorDropdowns();

        Debug.Log($"Unity detected {Display.displays.Length} displays.");
        // Activate as many displays as Unity detects (up to 6 for your setup)
        for (int i = 0; i < Mathf.Min(Display.displays.Length, 6); i++)
        {
            Display.displays[i].Activate();
            Debug.Log($"Display {i + 1} activated.");
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

    public void SetFrontViewCamera(int monitorIndex)
    {
        FrontViewCamera.targetDisplay = monitorIndex + 1;
        Display.displays[monitorIndex].Activate();
    }

    public void SetInstructorDisplay(int monitorIndex)
    {
        MainCamera.targetDisplay = monitorIndex + 1;
        InstructorDisplay.targetDisplay = monitorIndex + 1;
        Display.displays[monitorIndex].Activate();
    }

    public void SetLeftMirrorDisplay(int monitorIndex)
    {
        LeftMirrorCamera.targetDisplay = monitorIndex + 1;
        LeftMirrorDisplay.targetDisplay = monitorIndex + 1;
        Display.displays[monitorIndex].Activate();
    }

    public void SetRightMirrorDisplay(int monitorIndex)
    {
        RightMirrorCamera.targetDisplay = monitorIndex + 1;
        RightMirrorDisplay.targetDisplay = monitorIndex + 1;
        Display.displays[monitorIndex].Activate();
    }

    public void SetRearMirrorDisplay(int monitorIndex)
    {
        RearMirrorCamera.targetDisplay = monitorIndex + 1;
        RearMirrorDisplay.targetDisplay = monitorIndex + 1;
        Display.displays[monitorIndex].Activate();
    }

    public void SetTruckDashboardDisplay(int monitorIndex)
    {
        DashboardDisplay.targetDisplay = monitorIndex + 1;
        Display.displays[monitorIndex].Activate();
    }

    // Open the display setting panel
    public void OpenDisplaySettingPanel()
    {
        _setDisplayPanel.SetActive(true);
    }

    // Close the display setting panel
    public void CloseDisplaySettingPanel()
    {
        _setDisplayPanel.SetActive(false);
    }
}
