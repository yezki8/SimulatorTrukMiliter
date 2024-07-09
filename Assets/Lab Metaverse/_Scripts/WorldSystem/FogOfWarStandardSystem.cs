using UnityEngine;
using System.Collections.Generic;

// Using 3D Raycast

public class FogOfWar : MonoBehaviour
{
    public Transform player;
    public LayerMask obstaclesMask;
    public float viewRadius = 10f;
    public float meshResolution = 1f;
    public float fogYOffset = 0.1f;

    public MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    private void Start()
    {
        // Create a new mesh for the view mesh
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        // move mesh so it's visible in the scene
        viewMeshFilter.transform.position = new Vector3(0, fogYOffset, 0);
    }

    private void LateUpdate()
    {
        // move view mesh to player position
        transform.position = player.position;
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(360 * meshResolution);
        float stepAngleSize = 360f / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = player.eulerAngles.y - 180 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(player.position, dir, out hit, viewRadius, obstaclesMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, player.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += player.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
}