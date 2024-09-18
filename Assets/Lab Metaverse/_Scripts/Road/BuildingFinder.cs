using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFinder : MonoBehaviour
{
    public string parentObjectName = "Object Environment";
    public string groupObjectName = "Place 1 Group";

    void Start()
    {
        Vector3 testPoint = new Vector3(220, 35, 59);
        string buildingName = GetBuildingNameContainingPoint(testPoint);
        if (buildingName != null)
        {
            Debug.Log($"Point is inside the building: {buildingName}");
        }
        else
        {
            Debug.Log("No building contains the point.");
        }
    }

    public string GetBuildingNameContainingPoint(Vector3 point)
    {
        // Find the parent GameObject
        GameObject parentObject = GameObject.Find(parentObjectName);
        if (parentObject == null)
        {
            Debug.LogError($"Parent GameObject '{parentObjectName}' not found.");
            return null;
        }

        // Find the group GameObject within the parent
        Transform groupTransform = parentObject.transform.Find(groupObjectName);
        if (groupTransform == null)
        {
            Debug.LogError($"Group GameObject '{groupObjectName}' not found under '{parentObjectName}'.");
            return null;
        }

        // Iterate through the buildings in the group
        foreach (Transform building in groupTransform)
        {
            if (IsPointInsideBuildingUsingChildren(building, point))
            {
                return building.gameObject.name;
            }
        }
        return null; // Return null if no building contains the point
    }

    private bool IsPointInsideBuildingUsingChildren(Transform buildingTransform, Vector3 point)
    {
        // Iterate through the children of the building and check each child individually
        foreach (Transform child in buildingTransform)
        {
            if (IsPointInsideChildBounds(child, point))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPointInsideChildBounds(Transform child, Vector3 point)
    {
        // Get the child's local scale (size) and position
        Vector3 localPoint = child.InverseTransformPoint(point);
        Vector3 halfScale = child.localScale / 2;

        // Check if the point is inside the XZ bounds (ignoring Y for vertical space)
        bool insideX = Mathf.Abs(localPoint.x) <= halfScale.x;
        bool insideZ = Mathf.Abs(localPoint.z) <= halfScale.z;

        return insideX && insideZ;
    }
}
