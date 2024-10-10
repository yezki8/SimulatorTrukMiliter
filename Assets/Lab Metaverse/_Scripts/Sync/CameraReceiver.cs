using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

// Secondary PC: Camera Receiver Component
public class CameraReceiver : MonoBehaviour
{
    [System.Serializable]
    public class ReceiverDisplay
    {
        public int displayIndex;
        public Camera displayCamera;
        public Material displayMaterial;
    }

    [Header("Receiver Setup")]
    public List<ReceiverDisplay> displays = new List<ReceiverDisplay>();
    public int basePort = 8900;

    private List<UdpClient> udpClients = new List<UdpClient>();
    private List<Texture2D> receiveTextures = new List<Texture2D>();

    void Start()
    {
        InitializeReceivers();
    }

    void InitializeReceivers()
    {
        for (int i = 0; i < displays.Count; i++)
        {
            var display = displays[i];

            // Create UDP client for each display
            var udpClient = new UdpClient(basePort + i);
            udpClients.Add(udpClient);

            // Create texture for receiving
            var tex = new Texture2D(1, 1);
            receiveTextures.Add(tex);

            // Start receiving for this display
            StartCoroutine(ReceiveFrames(i));
        }
    }

    IEnumerator ReceiveFrames(int displayIndex)
    {
        var display = displays[displayIndex];
        var udpClient = udpClients[displayIndex];
        var ipEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            try
            {
                // Receive data
                byte[] receivedBytes = udpClient.Receive(ref ipEndPoint);

                // Check if it's camera info or frame data
                if (receivedBytes.Length < 1000) // Assume small packets are camera info
                {
                    string jsonInfo = System.Text.Encoding.UTF8.GetString(receivedBytes);
                    CameraStreamInfo cameraInfo = JsonUtility.FromJson<CameraStreamInfo>(jsonInfo);
                    UpdateDisplaySetup(displayIndex, cameraInfo);
                }
                else // Frame data
                {
                    receiveTextures[displayIndex].LoadImage(receivedBytes);
                    if (display.displayMaterial != null)
                    {
                        display.displayMaterial.mainTexture = receiveTextures[displayIndex];
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error receiving frame for display {displayIndex}: {e.Message}");
            }

            yield return null;
        }
    }

    void UpdateDisplaySetup(int displayIndex, CameraStreamInfo cameraInfo)
    {
        var display = displays[displayIndex];

        // Update texture size if needed
        if (receiveTextures[displayIndex].width != cameraInfo.width ||
            receiveTextures[displayIndex].height != cameraInfo.height)
        {
            receiveTextures[displayIndex] = new Texture2D(cameraInfo.width, cameraInfo.height);
        }

        // Update camera settings if needed
        if (display.displayCamera != null)
        {
            display.displayCamera.targetDisplay = cameraInfo.targetDisplay;
        }
    }

    void OnDestroy()
    {
        foreach (var udpClient in udpClients)
        {
            if (udpClient != null)
                udpClient.Close();
        }
    }
}
