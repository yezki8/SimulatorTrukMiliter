using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PrimarySetup : MonoBehaviour
{
    public Camera[] camerasToStream;

    void Start()
    {
        Debug.Log("Setting up primary PC...");
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

    public void StartStreaming()
    {
        Debug.Log("Starting streaming...");
        var streamer = GetComponent<CameraStreamer>();
        streamer.StreamCameras();
    }
}
