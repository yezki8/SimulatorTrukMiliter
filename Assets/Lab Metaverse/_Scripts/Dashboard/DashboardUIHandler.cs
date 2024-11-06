using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DashboardUIHandler : MonoBehaviour
{
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
    private int turnIndicatorValue = 0;     //0 = off, 1 = left, 2 = right;
    private IEnumerator turnSignalLeft;
    private IEnumerator turnSignalRight;


    public void Start()
    {
        turnIndicatorValue = 0;
        turnSignalLeft = TurnOnTurnSignal(1);
        turnSignalRight = TurnOnTurnSignal(2);
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

    public void CallTurnSignal(int index)
    {
        if (index == 0)
        {
            StopCoroutine(turnSignalLeft); 
            StopCoroutine(turnSignalRight);
        }
        else if (index == 1)
        {
            StartCoroutine(turnSignalLeft);
            StopCoroutine(turnSignalRight);
        }
        else if (index == 2)
        {
            StopCoroutine(turnSignalLeft);
            StartCoroutine(turnSignalRight);
        }
    }

    IEnumerator TurnOnTurnSignal(int index)
    {
        Image targetInd = null;
        if (index == 1)
        {
            targetInd = _leftTurnIndicator;
        }
        else if (index == 2)
        {
            targetInd = _rightTurnIndicator;
        }

        targetInd.enabled = true;
        yield return new WaitForSeconds(0.5f);
        targetInd.enabled = false;
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(TurnOnTurnSignal(index));
    }

    public void ChangeMainLampStatus(bool status)
    {
        _mainLampIndicator.enabled = status;
    }

    public void ChangeFarLampStatus(bool status)
    {
        _farLampIndicator.enabled = status;
    }
}
