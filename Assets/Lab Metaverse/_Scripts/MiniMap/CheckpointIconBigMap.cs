using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class CheckpointIconBigMap
{
    static CheckpointIconBigMap()
    {
    #if UNITY_EDITOR
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    #endif

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
                    GameObject childPrefab;
#if UNITY_EDITOR
                    childPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lab Metaverse/Prefabs/Checkpoint Icon - bigmap.prefab");
#else
                    childPrefab = Resources.Load<GameObject>("Checkpoint Icon - bigmap");
#endif
                    if (childPrefab != null)
                    {
#if UNITY_EDITOR
                        PrefabUtility.InstantiatePrefab(childPrefab, child);
#else
                        GameObject checkpointIcon = GameObject.Instantiate(childPrefab, child);
#endif
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
            GameObject childPrefab;
#if UNITY_EDITOR
            childPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lab Metaverse/Prefabs/Finish Icon - bigmap.prefab");
#else
            childPrefab = Resources.Load<GameObject>("Checkpoint Icon - bigmap");
#endif
            if (childPrefab != null)
            {
#if UNITY_EDITOR
                PrefabUtility.InstantiatePrefab(childPrefab, finishObject.transform);
#else
                GameObject finishIcon = GameObject.Instantiate(childPrefab, finishObject.transform);
#endif
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
