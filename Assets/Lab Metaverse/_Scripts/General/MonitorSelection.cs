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
    [SerializeField] private TextMeshProUGUI A;
    [SerializeField] private TextMeshProUGUI B;
    [SerializeField] private TextMeshProUGUI C;
    [SerializeField] private TextMeshProUGUI D;
    [SerializeField] private TextMeshProUGUI E;
    [SerializeField] private TextMeshProUGUI F;
    public TMP_Dropdown[] displayDropdowns = new TMP_Dropdown[6];

    void Start()
    {
        _setDisplayPanel.SetActive(false);
        PopulateMonitorDropdowns();

        Debug.Log($"Unity detected {Display.displays.Length} displays.");
        for (int i = 0; i < Mathf.Min(Display.displays.Length, 6); i++)
        {
            Display.displays[i].Activate();
            Debug.Log($"Display {i + 1} activated.");
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
        Debug.Log("Saved");
        for (int i = 0; i < displayDropdowns.Length; i++)
        {
            int savedValue = PlayerPrefs.GetInt("MonitorSetup_" + i, 0);
            print($"{savedValue}");
        }
    }

    public void LoadMonitorSetup()
    {
        for (int i = 0; i < displayDropdowns.Length; i++)
        {
            int savedValue = PlayerPrefs.GetInt("MonitorSetup_" + i, 0);
            displayDropdowns[i].value = savedValue;
            Debug.Log($"Loaded Monitor Setup for Dropdown {i}: {displayDropdowns[i].value}");
        }
        Debug.Log("Monitor setup loaded.");
    }

    public void SetFrontViewCamera(int monitorIndex)
    {
        FrontViewCamera.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
        B.text = $"{FrontViewCamera.targetDisplay}";
    }

    public void SetInstructorDisplay(int monitorIndex)
    {
        MainCamera.targetDisplay = monitorIndex;
        InstructorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
        A.text = $"{MainCamera.targetDisplay}";
    }

    public void SetLeftMirrorDisplay(int monitorIndex)
    {
        LeftMirrorCamera.targetDisplay = monitorIndex;
        LeftMirrorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
        C.text = $"{LeftMirrorDisplay.targetDisplay}";
    }

    public void SetRightMirrorDisplay(int monitorIndex)
    {
        RightMirrorCamera.targetDisplay = monitorIndex;
        RightMirrorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
        D.text = $"{RightMirrorDisplay.targetDisplay}";
    }

    public void SetRearMirrorDisplay(int monitorIndex)
    {
        RearMirrorCamera.targetDisplay = monitorIndex;
        RearMirrorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
        E.text = $"{RearMirrorDisplay.targetDisplay}";
    }

    public void SetTruckDashboardDisplay(int monitorIndex)
    {
        DashboardDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
        F.text = $"{DashboardDisplay.targetDisplay}";
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
        SaveMonitorSetup();
    }
}
