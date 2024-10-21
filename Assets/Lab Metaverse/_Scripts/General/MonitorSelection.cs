using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PG;
using System.Linq;
using Unity.VisualScripting;

public class MonitorSelection : MonoBehaviour
{
    [SerializeField] private GameObject _setDisplayPanel;
    [SerializeField] private Camera BigMapCamera;
    [SerializeField] private Canvas InstructorDisplay;
    [SerializeField] private Canvas FrontViewDisplay;
    [SerializeField] private Canvas LeftMirrorDisplay;
    [SerializeField] private Canvas DashboardDisplay;
    [SerializeField] private Canvas RearMirrorDisplay;
    [SerializeField] private Canvas RightMirrorDisplay;
    private Dictionary<int, List<Canvas>> monitorStack = new();
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
        
        for (int i = 0; i < 6; i++)
        {
            monitorStack[i] = new();
        }

        monitorStack[FrontViewDisplay.targetDisplay].Add(FrontViewDisplay);
        monitorStack[LeftMirrorDisplay.targetDisplay].Add(LeftMirrorDisplay);
        monitorStack[DashboardDisplay.targetDisplay].Add(DashboardDisplay);
        monitorStack[RearMirrorDisplay.targetDisplay].Add(RearMirrorDisplay);
        monitorStack[RightMirrorDisplay.targetDisplay].Add(RightMirrorDisplay);

        InstructorDisplay.sortingOrder = 10;

        UpdateCanvasOrder();
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

    public void SetInstructorDisplay(int monitorIndex)
    {
        InstructorDisplay.targetDisplay = monitorIndex;
        BigMapCamera.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }

    public void SetFrontViewCamera(int monitorIndex)
    {
        RemoveDisplayFromList(FrontViewDisplay);
        monitorStack[monitorIndex].Add(FrontViewDisplay);
        UpdateCanvasOrder();
        FrontViewDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }

    public void SetLeftMirrorDisplay(int monitorIndex)
    {
        RemoveDisplayFromList(LeftMirrorDisplay);
        monitorStack[monitorIndex].Add(LeftMirrorDisplay);
        UpdateCanvasOrder();
        LeftMirrorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }
    public void SetTruckDashboardDisplay(int monitorIndex)
    {
        RemoveDisplayFromList(DashboardDisplay);
        monitorStack[monitorIndex].Add(DashboardDisplay);
        UpdateCanvasOrder();
        DashboardDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }
    public void SetRearMirrorDisplay(int monitorIndex)
    {
        RemoveDisplayFromList(RearMirrorDisplay);
        monitorStack[monitorIndex].Add(RearMirrorDisplay);
        UpdateCanvasOrder();
        RearMirrorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }
    public void SetRightMirrorDisplay(int monitorIndex)
    {
        RemoveDisplayFromList(RightMirrorDisplay);
        monitorStack[monitorIndex].Add(RightMirrorDisplay);
        UpdateCanvasOrder();
        RightMirrorDisplay.targetDisplay = monitorIndex;
        Display.displays[monitorIndex].Activate();
    }

    void UpdateCanvasOrder()
    {
        for (int i = 0; i < 6; i++)
        {
            if (monitorStack[i].Count > 1)
            {
                for (int j = 1; j < monitorStack[i].Count; j++)
                {
                    monitorStack[i][j].sortingOrder = monitorStack[i][j-1].sortingOrder + 1;
                }
            }
        }
    }

    void RemoveDisplayFromList(Canvas display)
    {
        for (int i = 0; i < 6; i++)
        {
            monitorStack[i].Remove(display);
        }
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

    public void CancelDisplaySettingPanel()
    {
        _setDisplayPanel.SetActive(false);
        LoadMonitorSetup();
    }
}
