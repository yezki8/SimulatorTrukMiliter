using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CheckpointIconBigMap
{
    static CheckpointIconBigMap()
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
            return;
        }

        foreach (Transform child in parentObject.transform)
        {
            if (child != null)
            {
                if (!HasCheckpoint(child.gameObject))
                {
                    GameObject childPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lab Metaverse/Prefabs/Checkpoint Icon - bigmap.prefab");
                    if (childPrefab != null)
                    {
                        PrefabUtility.InstantiatePrefab(childPrefab, child);
                    }
                }
                foreach (Transform go in child)
                {
                    if (go.name == "Checkpoint Icon - bigmap")
                    {
                        go.position = child.position;
                    }
                }
            }
        }
        GameObject finishObject = GameObject.Find("Finish");
        if (!HasCheckpointFinish(finishObject))
        {
            GameObject childPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lab Metaverse/Prefabs/Finish Icon - bigmap.prefab");
            if (childPrefab != null)
            {
                PrefabUtility.InstantiatePrefab(childPrefab, finishObject.transform);
            }
        }
        foreach (Transform go in finishObject.transform)
        {
            if (go.name == "Checkpoint Icon - bigmap")
            {
                go.position = finishObject.transform.position;
            }
        }
    }

    public static bool HasCheckpoint(GameObject parentObject)
    {
        foreach (Transform child in parentObject.transform)
        {
            if (child.name == "Checkpoint Icon - bigmap")
            {
                return true;
            }
        }
        return false;
    }

    public static bool HasCheckpointFinish(GameObject parentObject)
    {
        foreach (Transform child in parentObject.transform)
        {
            if (child.name == "Finish Icon - bigmap")
            {
                return true;
            }
        }
        return false;
    }
}
