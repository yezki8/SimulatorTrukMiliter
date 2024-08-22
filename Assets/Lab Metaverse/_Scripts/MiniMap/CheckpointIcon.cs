using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CheckpointIcon
{
    static CheckpointIcon()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private static void OnHierarchyChanged()
    {
        ConfigureChildObjects();
    }

    private static void ConfigureChildObjects()
    {
        GameObject parentObject = GameObject.Find("CP");
        if (parentObject == null)
        {
            // Debug.LogWarning("Parent object 'CP' tidak ditemukan.");
            return;
        }

        foreach (Transform child in parentObject.transform)
        {
            if (child != null)
            {
                if (!HasCheckpoint(child.gameObject))
                {
                    GameObject childPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lab Metaverse/Prefabs/CheckpointIcon.prefab");
                    if (childPrefab != null)
                    {
                        PrefabUtility.InstantiatePrefab(childPrefab, child);
                        // Debug.Log($"Checkpoint Icon ditambahkan ke {child.name}");
                    }
                }
                foreach (Transform go in child.transform)
                {
                    if (go.name == "CheckpointIcon")
                    {
                        go.position = child.position;
                    }
                }
            }
        }
    }

    public static bool HasCheckpoint(GameObject parentObject)
    {
        foreach (Transform child in parentObject.transform)
        {
            if (child.name == "CheckpointIcon")
            {
                // Debug.Log($"{parentObject.name} sudah memiliki icon checkpoint");
                return true;
            }
        }
        // Debug.Log($"{parentObject.name} belum memiliki icon checkpoint");
        return false;
    }
}
