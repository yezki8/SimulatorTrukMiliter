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
            spawnPlace.IsActive = false;
        }
        if (CountdownTimer.Instance != null)
        {
            Checkpoints[0].RecordedTimerThreshold = CountdownTimer.Instance.GetStartTime();
            Checkpoints[0].RecordedTimer = CountdownTimer.Instance.GetStartTime();
        }
        Checkpoints[0].IsActive = true;
    }

    public void RespawnAtCheckpoint()
    {
        CheckpointDataContainer targetSpawnPlace = GetActiveCheckpoint();
        Player.transform.SetPositionAndRotation(targetSpawnPlace.SpawnArea.transform.position, 
            targetSpawnPlace.SpawnArea.transform.rotation);
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
                    spawnPlace.IsActive = true;

                    //Record Collectibles here

                    //Record Timer
                    if (CountdownTimer.Instance != null)
                    {
                        spawnPlace.RecordedTimer = CountdownTimer.Instance.CurrentTime;
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
                    spawnPlace.IsActive = false;
                }
            }
        }
    }

    public void DisablePlayer()
    {
        if (Player != null)
        {
            //add reset stats or animations here
            Player.SetActive(false);
        }
    }

    public float GetRecordedData()          //invoked by 
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

    public CheckpointDataContainer GetActiveCheckpoint()
    {
        CheckpointDataContainer targetPoint = null;
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
