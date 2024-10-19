using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PG;

public class CheckpointManager : MonoBehaviour
{
    public CheckpointController[] Checkpoints;        //0 is the initial spawn place

    public GameObject Player;

    public static CheckpointManager Instance;

    public UnityEvent OnActivateCheckpoint;
    [SerializeField] private GameObject Level1Parent;
    [SerializeField] private GameObject Level2Parent;

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
        //Reset the states of checkpoints
        foreach(var spawnPlace in Checkpoints)
        {
            spawnPlace.ResetCheckpointStates();
            spawnPlace.CheckQuestReuqirement();
        }

        //Reset the place player spawns to checkpoint 0
        if (TimerCountdown.Instance != null)
        {
            Checkpoints[0].RecordedTimerThreshold = TimerCountdown.Instance.GetStartTime();
            Checkpoints[0].RecordedTimer = TimerCountdown.Instance.GetStartTime();
        }
        Checkpoints[0].SetActiveCheckpoint(true);

        Level2Parent.SetActive(false);
    }

    public void RespawnAtCheckpoint()
    {
        ResetPlayerState();
        CheckpointController targetSpawnPlace = GetActiveCheckpoint();
        Vector3 spawnPos = targetSpawnPlace.SpawnArea.transform.position;

        Quaternion spawnRot = targetSpawnPlace.SpawnArea.transform.rotation;
        Debug.Log("Reset vehicle position from Checkpoint manager");


        Player.GetComponent<CarController>().ResetVehicle();
        Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        Player.GetComponent<Rigidbody>().isKinematic = true;
        Player.GetComponent<Rigidbody>().position = spawnPos + Vector3.up * 1f;
        Player.GetComponent<Rigidbody>().rotation = spawnRot;
        Player.GetComponent<Rigidbody>().isKinematic = false;

        //Player.transform.SetPositionAndRotation(spawnPos, spawnRot);
    }

    public void ActivateCheckpoint(GameObject targetCheckpoint)
    {
        if (GameStateController.Instance.GameState == StateOfGame.Match)
        {
            //First loop for checkpoint activation
            foreach (var spawnPlace in Checkpoints)
            {
                if (spawnPlace.gameObject == targetCheckpoint)
                {
                    spawnPlace.SetActiveCheckpoint(true);

                    //Record Collectibles here

                    //Record Timer
                    if (TimerCountdown.Instance != null)
                    {
                        spawnPlace.RecordedTimer = TimerCountdown.Instance.CurrentTime;
                        if (spawnPlace.RecordedTimer < spawnPlace.RecordedTimerThreshold)
                        {
                            spawnPlace.RecordedTimer = spawnPlace.RecordedTimerThreshold;
                        }
                    }

                    //Record Level Progression here
                    OnActivateCheckpoint?.Invoke();
                }
            }

            //Second loop for quest checking
            foreach (var spawnPlace in Checkpoints)
            {
                if (spawnPlace.gameObject != targetCheckpoint)
                {
                    spawnPlace.IsActive = false;
                    spawnPlace.CheckQuestReuqirement();
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

    #region Get Recorded Data
    public float GetRecordedTimer()          //Called By TimerCountdown
    {
        CheckpointController spawnPlace = GetActiveCheckpoint();

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
        CheckpointController spawnPlace = GetActiveCheckpoint();

        //Get Recorded Stopwatch
        float recordedStopwatch = spawnPlace.RecordedStopwatch;

        return recordedStopwatch;
    }

    #endregion

    public CheckpointController GetActiveCheckpoint()
    {
        CheckpointController targetPoint = null;
        foreach (var point in Checkpoints)
        {
            if (point.IsActive)
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
