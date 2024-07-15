using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    public int roadSegmentID;
    [SerializeField]
    public float MaxSpeedLimit = 60; //SerializeField can be deleted in the future
    [SerializeField]
    public float MinSpeedLimit = 0; //SerializeField can be deleted in the future

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getMaxSpeedLimit()
    {
        return MaxSpeedLimit;
    }

    public float getMinSpeedLimit()
    {
        return MinSpeedLimit;
    }
}
