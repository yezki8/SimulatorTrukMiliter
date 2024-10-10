using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Shared data structure for camera info
[Serializable]
public class CameraStreamInfo
{
    public int displayIndex;
    public int width;
    public int height;
    public int targetDisplay;
}

// Primary PC: Camera Streaming Component
public class CameraStreamer : MonoBehaviour
{

    [System.Serializable]
    public class StreamingCamera
    {
        public Camera camera;
        public int targetDisplayOnSecondaryPC;
    }

    [Header("Streaming Setup")]
    public List<StreamingCamera> streamingCameras = new List<StreamingCamera>();
    public string secondaryPCIP = "192.168.100.1";
    public int basePort = 8900;

    [Header("Streaming Quality")]
    [Range(1, 100)]
    public int jpegQuality = 75;
    [Range(1, 60)]
    public int streamFPS = 30;

    private List<RenderTexture> renderTextures = new List<RenderTexture>();
    private List<Texture2D> texture2Ds = new List<Texture2D>();
    private List<UdpClient> udpClients = new List<UdpClient>();

    void Start()
    {
        InitializeStreaming();
    }

    void InitializeStreaming()
    {
        for (int i = 0; i < streamingCameras.Count; i++)
        {
            var streamCam = streamingCameras[i];
            if (streamCam.camera == null) continue;

            // Create render texture for each camera
            var rt = new RenderTexture(streamCam.camera.pixelWidth, streamCam.camera.pixelHeight, 24);
            renderTextures.Add(rt);
            streamCam.camera.targetTexture = rt;

            // Create Texture2D for reading pixels
            var tex2D = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            texture2Ds.Add(tex2D);

            // Create UDP client for each camera
            var udpClient = new UdpClient();
            udpClients.Add(udpClient);

            // Send initial camera info
            SendCameraInfo(i);
        }

        // Start streaming coroutine
        StartCoroutine(StreamCameras());
    }

    void SendCameraInfo(int cameraIndex)
    {
        var streamCam = streamingCameras[cameraIndex];
        var cameraInfo = new CameraStreamInfo
        {
            displayIndex = cameraIndex,
            width = streamCam.camera.pixelWidth,
            height = streamCam.camera.pixelHeight,
            targetDisplay = streamCam.targetDisplayOnSecondaryPC
        };

        string jsonInfo = JsonUtility.ToJson(cameraInfo);
        byte[] infoBytes = System.Text.Encoding.UTF8.GetBytes(jsonInfo);
        udpClients[cameraIndex].Send(infoBytes, infoBytes.Length, secondaryPCIP, basePort + cameraIndex);
    }

    IEnumerator StreamCameras()
    {
        var wait = new WaitForSeconds(1f / streamFPS);

        while (true)
        {
            for (int i = 0; i < streamingCameras.Count; i++)
            {
                if (streamingCameras[i].camera == null) continue;

                // Render camera to texture
                streamingCameras[i].camera.Render();

                // Read pixels and encode
                RenderTexture.active = renderTextures[i];
                texture2Ds[i].ReadPixels(new Rect(0, 0, renderTextures[i].width, renderTextures[i].height), 0, 0);
                texture2Ds[i].Apply();
                byte[] bytes = texture2Ds[i].EncodeToJPG(jpegQuality);

                // Send frame
                try
                {
                    udpClients[i].Send(bytes, bytes.Length, secondaryPCIP, basePort + i);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to send frame for camera {i}: {e.Message}");
                }
            }

            yield return wait;
        }
    }

    void OnDestroy()
    {
        foreach (var rt in renderTextures)
        {
            if (rt != null)
                rt.Release();
        }

        foreach (var udpClient in udpClients)
        {
            if (udpClient != null)
                udpClient.Close();
        }
    }
}