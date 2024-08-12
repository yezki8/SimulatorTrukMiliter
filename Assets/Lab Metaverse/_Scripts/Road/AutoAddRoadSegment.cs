using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad]
public class AutoAddRoadSegment
{
    static AutoAddRoadSegment()
    {
        // Register a callback for when objects are created or the hierarchy changes
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        // Call this method every frame to ensure new objects are processed
        ConfigureRoadObject();
        ConfigureConnectionObject();
    }

    private static void OnHierarchyChanged()
    {
        // Call this method when hierarchy changes
        ConfigureRoadObject();
        ConfigureConnectionObject();
    }

    private static void ConfigureRoadObject()
    {
        // Define the layer name and get its index
        int roadLayer = LayerMask.NameToLayer("Road");

        // Ensure the layer exists
        if (roadLayer == -1)
        {
            Debug.LogError($"Layer 'Road' does not exist. Please create it in the Tags and Layers settings.");
            return;
        }

        GameObject roadObjects = GameObject.Find("Road Objects");
        if (roadObjects == null)
        {
            return;
        }

        foreach (Transform child in roadObjects.transform)
        {
            // Add the RoadSegment script if it does not already have it
            if (child.GetComponent<RoadSegment>() == null)
            {
                child.gameObject.AddComponent<RoadSegment>();
            }

            // Set the layer to "Road"
            child.gameObject.layer = roadLayer;
        }
    }

    private static void ConfigureConnectionObject()
    {
        // Define the layer name and get its index
        int roadLayer = LayerMask.NameToLayer("Road");

        // Ensure the layer exists
        if (roadLayer == -1)
        {
            Debug.LogError($"Layer 'Road' does not exist. Please create it in the Tags and Layers settings.");
            return;
        }

        GameObject connectionObjects = GameObject.Find("Connection Objects");
        if (connectionObjects == null)
        {
            return;
        }

        foreach (Transform child in connectionObjects.transform)
        {
            // Add the RoadSegment script if it does not already have it
            if (child.GetComponent<RoadSegment>() == null)
            {
                child.gameObject.AddComponent<RoadSegment>();
            }

            // Set the layer to "Road"
            child.gameObject.layer = roadLayer;
        }
    }
}

#endif