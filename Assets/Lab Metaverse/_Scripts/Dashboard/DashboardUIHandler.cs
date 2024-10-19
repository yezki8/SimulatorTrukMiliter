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
}
