using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    public int roadSegmentID;
    private int roadSegmentClass;
    [SerializeField]
    private float MaxSpeedLimit = 60; //SerializeField can be deleted in the future
    [SerializeField]
    private float MinSpeedLimit = 0; //SerializeField can be deleted in the future

    // Update is called once per frame
    void Update()
    {
        // autosetSpeedLimit();
    }

    void autosetSpeedLimit()
    {
        if (roadSegmentClass == 0)                  // Jalan umum
        {
            MaxSpeedLimit = 60;
            MinSpeedLimit = 0;
        }
        else if (roadSegmentClass == 1)             // Jalan perumahan atau pabrik
        {
            MaxSpeedLimit = 25;
            MinSpeedLimit = 0;
        }
        else if (roadSegmentClass == 2)             // Jalan Tol
        {
            MaxSpeedLimit = 100;
            MinSpeedLimit = 60;
        }
        else
        {
            MaxSpeedLimit = -1;                     // Tidak ada batas max speed limit (infinite)
            MinSpeedLimit = 0;
        }
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
