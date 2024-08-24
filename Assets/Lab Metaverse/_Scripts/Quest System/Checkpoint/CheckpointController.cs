using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public GameObject SpawnArea;

    [Header("Visual Parameter")]
    public MeshRenderer VisualRenderer;
    public CapsuleCollider CheckpointCollider;

    public enum CheckpointType
    {
        RecordStats,
        HealFully
    }

    public CheckpointType TypeOfCheckpoint = CheckpointType.HealFully;

    [Header("Player Stats")]
    public float RecordedHealth;              //maybe changed to float
    public float RecordedArmor;
    public float HealthThreshold;
    public float ArmorThreshold;

    [Header("Timer Variables")]
    public float RecordedTimer;               //records the time player has left when touching this checkpoint
    public float RecordedTimerThreshold;     //the minimum of how much player has time left when it respawns
    public bool IsActive;

    [Header("Stopwatch Variables")]
    public float RecordedStopwatch;
    public float RecordedStopwatchThreshold;    // zero
    public bool IsStopwatchActive;

    [Header("Distance Variables")]
    public float LastDistance;

    [Header("Quest Variables")]
    [SerializeField] private bool _deactivateOnceActive = true;
    public bool HasActivatedOnce = false;
    [SerializeField] private List<CheckpointController> _checkpointQuestList;
    private IconHandler iconHandler;

    void Awake()
    {
        iconHandler = FindObjectOfType<IconHandler>();
        if (iconHandler == null) 
        {
            Debug.Log("iconHandler tidak ditemukan");
        }
    }

    public void SetActiveCheckpoint(bool status)
    {
        IsActive = status;
        
        if (status == true)
        {
            HasActivatedOnce = true;        //once it's true, it will never be false
        }
        SetCheckpointVisual(!status);
    }

    public void SetCheckpointVisual(bool status)
    {
        // TODO: add animation of checkpoint activation here
        if (status == true)
        {
            if ((_deactivateOnceActive && !HasActivatedOnce) ||
                !_deactivateOnceActive)
            {
                VisualRenderer.enabled = status;
                CheckpointCollider.enabled = status;
            }
        }
        else
        {
            VisualRenderer.enabled = status;
            CheckpointCollider.enabled = status;
        }

        // Update the checkpoint icon status
        UpdateCheckpointIcon(VisualRenderer.enabled);
    }


    public void ResetCheckpointStates()
    {
        IsActive = false;
        HasActivatedOnce = false;
    }

    public void CheckQuestReuqirement()
    {
        bool checkpointQuestFlow = CheckCheckpointQuest();

        if (checkpointQuestFlow)
        {
            SetCheckpointVisual(true);
        }
        else
        {
            SetCheckpointVisual(false);
        }
    }

    public bool CheckCheckpointQuest()
    {
        bool isCompleted = true;
        if (_checkpointQuestList.Count > 0)
        {
            for (int i = 0; i < _checkpointQuestList.Count; i++)
            {
                if (!_checkpointQuestList[i].HasActivatedOnce)
                {
                    isCompleted = false;
                    break;
                }
            }
        }
        return isCompleted;
    }

    private void UpdateCheckpointIcon(bool status)
    {
        Transform checkpointIcon = transform.Find("Checkpoint Icon - bigmap");
        Transform finishIcon = transform.Find("Finish Icon - bigmap");
        if (status == true)
        {
            if (gameObject.name == "Finish")
            {
                iconHandler.AddFinishIcon(gameObject);
                finishIcon.gameObject.SetActive(status);
            }
            else
            {
                iconHandler.AddCheckpointIcon(gameObject);
                checkpointIcon.gameObject.SetActive(status);
            }

        }
        else 
        {
            if (gameObject.name == "Finish")
            {
                finishIcon.gameObject.SetActive(status);
            }
            else
            {
                checkpointIcon.gameObject.SetActive(status);
            }
            iconHandler.RemoveIcon(gameObject);
        }
    }
}
