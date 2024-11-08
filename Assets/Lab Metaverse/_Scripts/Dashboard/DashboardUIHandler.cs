using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PG;

public class DashboardUIHandler : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    [SerializeField] private TextMeshProUGUI _gearText;
    [Header("Parameters for speedometer")]
    [SerializeField] private RectTransform _speedPin;

    [Header("Parameters for RPMometer")]
    [SerializeField] private RectTransform _RPMPin;

    [Header("Parameters for Indicator")]
    [SerializeField] private Image _mainLampIndicator;
    [SerializeField] private Image _farLampIndicator;
    [SerializeField] private Image _rightTurnIndicator;
    [SerializeField] private Image _leftTurnIndicator;
    [SerializeField] private Image _handBreakIndicator;


    public void Start()
    {
        CallTurnSignal(0, false);

        ChangeMainLampStatus(false);
        ChangeFarLampStatus(false);
    }

    private void Update()
    {
        UpdateHandBreakStatus();
    }

    public void DisplayDashboard(float speedZ, float rpmZ, int gear)
    {
        _speedPin.rotation = Quaternion.Euler(
            _speedPin.rotation.x,
            _speedPin.rotation.y, 
            speedZ);

        _RPMPin.rotation = Quaternion.Euler(
            _RPMPin.rotation.x,
            _RPMPin.rotation.y,
            rpmZ);

        string gearText = gear.ToString();
        if (gear == -1)
        {
            gearText = "R";
        }
        else if (gear == 0)
        {
            gearText = "N";
        }
        else if (gear == -2)
        {
            gearText = "P";
        }
        _gearText.SetText(gearText);
    }

    public void CallTurnSignal(int index, bool status)
    {
        if (index == 0)
        {
            _leftTurnIndicator.enabled = false;
            _rightTurnIndicator.enabled = false;
        }
        if (index == 1)
        {
            _leftTurnIndicator.enabled = status;
            _rightTurnIndicator.enabled = false;
        }
        else if (index == 2)
        {
            _leftTurnIndicator.enabled = false;
            _rightTurnIndicator.enabled = status;
        }
    }

    public void ChangeMainLampStatus(bool status)
    {
        _mainLampIndicator.enabled = status;
    }

    public void ChangeFarLampStatus(bool status)
    {
        _farLampIndicator.enabled = status;
    }

    public void UpdateHandBreakStatus()
    {
        Color color;
        color = _handBreakIndicator.color;
        float offSpeed = 10 * Time.deltaTime;
        if (_carController.InHandBrake)
        {
            color.a = 1;
        }
        else
        {
            color.a = Mathf.MoveTowards(color.a, 0, offSpeed);
        }
        _handBreakIndicator.color = color;
    }
}
