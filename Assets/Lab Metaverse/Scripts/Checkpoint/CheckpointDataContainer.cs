using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointDataContainer : MonoBehaviour
{
    public GameObject SpawnArea;
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
    public float RecordedTimerTreshold;     //the minimum of how much player has time left when it respawns
    public bool IsActive;

    [Header("Distance Variables")]
    public float LastDistance;
}
