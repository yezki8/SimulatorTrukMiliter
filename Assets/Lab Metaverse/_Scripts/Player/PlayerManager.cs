using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PG;
using TMPro;
using System;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    [SerializeField] private ControllerInput _carControllerInput;

    // UI handling here
    [SerializeField]
    private TextMeshProUGUI _truckSpeed;                               // Truck Speedometer
    public LayerMask roadLayerMask;
    private RoadSegment _currentRoadSegment;                            // Road class, can be change in the future
    private GameObject _detectedRoadObject;                             // Terdeteksi sebelumnya
    private GameObject _currentRoadObject;                              // Baru terdeteksi
    [SerializeField]
    private Image _warning;

    public void EnableTruck()
    {
        _carControllerInput.EnableControls();
    }

    public void DisableTruck()
    {
        _carControllerInput.DisableControls();
        _carController.StopEngine();
        _carControllerInput.ResetCar();
    }

    // Function to detect the road segment
    public void DetectCurrentRoadSegment()
    {
        RaycastHit hit;
        // float raycastDistance = 10f;
        Vector3 raycastStartPosition = transform.position + Vector3.up * 1.0f; // Offset the starting position
                                                                               // Debug.DrawRay(raycastStartPosition, Vector3.down * raycastDistance, Color.red);

        if (Physics.Raycast(raycastStartPosition, Vector3.down, out hit, Mathf.Infinity, roadLayerMask))
        {
            _currentRoadObject = hit.collider.gameObject;
            _currentRoadSegment = hit.collider.GetComponent<RoadSegment>();
            if (_currentRoadObject != _detectedRoadObject)
            {
                if (_currentRoadSegment != null)
                {
                    Debug.Log("Detected road segment: " + _currentRoadSegment.roadSegmentID + "\nMax speed: " + _currentRoadSegment.getMaxSpeedLimit() + ", Min speed: " + _currentRoadSegment.getMinSpeedLimit());
                }
                else
                {
                    Debug.LogWarning("Hit object does not have a RoadSegment component.");
                }
            }
        }
        else
        {
            _currentRoadObject = null;
            _currentRoadSegment = null;                                                     // No road segment detected
            if (_currentRoadObject != _detectedRoadObject)
            {
                Debug.LogWarning("Raycast did not hit any object on the road layer.");
            }
        }
        _detectedRoadObject = _currentRoadObject;
    }

    public void ActivateIcon(Image icon)
    {
        if (icon != null)
        {
            icon.gameObject.SetActive(true);
        }
    }

    public void DeactivateIcon(Image icon)
    {
        if (icon != null)
        {
            icon.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        DeactivateIcon(_warning);

        //Calculating body speed and angle
        string truckSpeed = $"{(int)Math.Truncate(_carController.SpeedInHour)} Km/h";
        if (_truckSpeed != null)
        {
            _truckSpeed.text = truckSpeed;
            if (_currentRoadSegment != null)
            {
                float maxSpeed = _currentRoadSegment.getMaxSpeedLimit();
                float minSpeed = _currentRoadSegment.getMinSpeedLimit();
                if (maxSpeed == -1)
                {
                    _truckSpeed.color = Color.white;
                    DeactivateIcon(_warning);
                }
                else
                {
                    if (_carController.SpeedInHour < minSpeed)
                    {
                        _truckSpeed.color = Color.blue;
                        DeactivateIcon(_warning);

                    }
                    else if (_carController.SpeedInHour > maxSpeed)
                    {
                        _truckSpeed.color = Color.red;
                        ActivateIcon(_warning);
                    }
                    else
                    {
                        _truckSpeed.color = Color.white;
                        DeactivateIcon(_warning);
                    }
                }
            }
            else
            {
                _truckSpeed.color = Color.white;
                DeactivateIcon(_warning);
            }
        }

        // Only player vehicle detects the road segment
        DetectCurrentRoadSegment();
    }

    private void OnTriggerEnter(Collider other)
    {
        //To Ensure these trigger calls only happen during gameplay
        if (GameStateController.Instance.GameState == StateOfGame.Match)
        {
            if (other.tag == "Checkpoint")
            {
                CheckpointManager.Instance.ActivateCheckpoint(other.gameObject);
            }
            else if (other.tag == "Finish")
            {
                GameStateController.Instance.ChangeGameState((int)StateOfGame.End);
                other.GetComponent<CheckpointController>().SetCheckpointVisual(false);
            }
        }
    }
}
