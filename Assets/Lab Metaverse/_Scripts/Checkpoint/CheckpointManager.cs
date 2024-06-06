using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckpointManager : MonoBehaviour
{
    public CheckpointDataContainer[] Checkpoints;        //0 is the initial spawn place

    public GameObject Player;

    public static CheckpointManager Instance;

    public UnityEvent OnActivateCheckpoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    //Spawn Methods =====================================================================================
    public void ResetSpawnPlace()
    {
        foreach(var spawnPlace in Checkpoints)
        {
            spawnPlace.IsTimerActive = false;
            spawnPlace.IsStopwatchActive = false;
        }
        // both countdown and stopwatch must work
        if (TimerCountdown.Instance != null && TimerStopwatch.Instance != null)
        {
            Checkpoints[0].RecordedTimerThreshold = TimerCountdown.Instance.GetStartTime();
            Checkpoints[0].RecordedTimer = TimerCountdown.Instance.GetStartTime();
        }
        Checkpoints[0].IsTimerActive = true;
        Checkpoints[0].IsStopwatchActive = true;
    }

    public void RespawnAtCheckpoint()
    {
        ResetPlayerState();
        CheckpointDataContainer targetSpawnPlace = GetActiveCheckpoint();
        Debug.Log("Ceckpoint = " + targetSpawnPlace.gameObject.name);
        Vector3 spawnPos = targetSpawnPlace.SpawnArea.transform.position;
        Quaternion spawnRot = targetSpawnPlace.SpawnArea.transform.rotation;

        Player.transform.SetPositionAndRotation(spawnPos, spawnRot);
        Player.SetActive(true);
    }

    public void ActivateCheckpoint(GameObject targetCheckpoint)
    {
        if (GameStateController.Instance.GameState == StateOfGame.Match)
        {
            foreach (var spawnPlace in Checkpoints)
            {
                if (spawnPlace.gameObject == targetCheckpoint)
                {
                    spawnPlace.IsTimerActive = true;

                    //Record Collectibles here

                    //Record Timer & Stopwatch
                    if (TimerCountdown.Instance != null && TimerStopwatch.Instance != null)
                    {
                        spawnPlace.RecordedTimer = TimerCountdown.Instance.CurrentTime;
                        spawnPlace.RecordedStopwatch = TimerStopwatch.Instance.CurrentTime;

                        if (spawnPlace.RecordedTimer < spawnPlace.RecordedTimerThreshold)
                        {
                            spawnPlace.RecordedTimer = spawnPlace.RecordedTimerThreshold;
                        }
                    }

                    //Record Level Progression here
                    OnActivateCheckpoint?.Invoke();
                }
                else
                {
                    spawnPlace.IsTimerActive = false;
                }
            }
        }
    }

    public void ResetPlayerState()
    {
        if (Player != null)
        {
            //add reset stats or animations here
            Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public float GetRecordedTimer()          //invoked by 
    {
        CheckpointDataContainer spawnPlace = GetActiveCheckpoint();

        //Get Recorded Timer
        float recordedTime = spawnPlace.RecordedTimer;

        //to make sure there's a minimum value of timer recalled 
        if (spawnPlace.RecordedTimer < spawnPlace.RecordedTimerThreshold)
        {
            recordedTime = spawnPlace.RecordedTimerThreshold;
        }

        return recordedTime;
    }

    public float GetRecordedStopwatch()     // same as above but for stopwatch
    {
        CheckpointDataContainer spawnPlace = GetActiveCheckpoint();

        //Get Recorded Stopwatch
        float recordedStopwatch = spawnPlace.RecordedStopwatch;

        return recordedStopwatch;
    }

    public CheckpointDataContainer GetActiveCheckpoint()
    {
        CheckpointDataContainer targetPoint = null;
        foreach (var point in Checkpoints)
        {
            if (point.IsTimerActive)
            {
                targetPoint = point;
                break;
            }
        }
        if (targetPoint == null)
        {
            targetPoint = Checkpoints[0];
        }

        return targetPoint;
    }
}
