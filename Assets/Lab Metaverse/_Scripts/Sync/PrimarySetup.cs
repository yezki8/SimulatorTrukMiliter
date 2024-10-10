using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimarySetup : MonoBehaviour
{
    public Camera[] camerasToStream;

    void Start()
    {
        var streamer = gameObject.AddComponent<CameraStreamer>();

        for (int i = 0; i < camerasToStream.Length; i++)
        {
            streamer.streamingCameras.Add(new CameraStreamer.StreamingCamera
            {
                camera = camerasToStream[i],
                targetDisplayOnSecondaryPC = i
            });
        }
    }
}
