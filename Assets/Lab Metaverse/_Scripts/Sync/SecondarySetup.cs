using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example setup script for Secondary PC
public class SecondarySetup : MonoBehaviour
{
    public int numberOfDisplays = 4;

    void Start()
    {
        var receiver = gameObject.AddComponent<CameraReceiver>();

        for (int i = 0; i < numberOfDisplays; i++)
        {
            // Create a new camera for each display
            var go = new GameObject($"Display{i}Camera");
            var camera = go.AddComponent<Camera>();
            camera.targetDisplay = i;

            // Create material for displaying received texture
            var material = new Material(Shader.Find("Unlit/Texture"));

            // Add stream display component
            var streamDisplay = go.AddComponent<StreamDisplay>();
            streamDisplay.displayMaterial = material;

            // Add to receiver
            receiver.displays.Add(new CameraReceiver.ReceiverDisplay
            {
                displayIndex = i,
                displayCamera = camera,
                displayMaterial = material
            });
        }
    }
}
