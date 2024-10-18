using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyRoads3Dv3;
using System;
using Unity.VisualScripting;
using PG;
using Unity.PlasticSCM.Editor.WebApi;

public class PathFinding : MonoBehaviour
{
    Dictionary<string, Node> nodes = new Dictionary<string, Node>();
    Dictionary<ERConnection, (Vector3, List<(ERRoad, Vector3)>)> connectionObject = new();
    private ERRoadNetwork roadNetwork;
    private ERRoad[] allRoads;
    private ERConnection[] allconnections;
    private List<GameObject> checkpoints;
    [SerializeField] private Transform startPoint;
    private LineRenderer lineRenderer;
    [SerializeField] private Material lineMaterial;

    public class Node
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public List<(Node, float)> Neighbors { get; set; } = new List<(Node, float)>();
        public float GCost { get; set; } = float.MaxValue; // Cost from start
        public float HCost { get; set; } = 0; // Heuristic cost to target
        public Node Parent { get; set; } = null;

        public float FCost => GCost + HCost; // Total cost
    }

    private void ResetNodes()
    {
        foreach (var node in nodes.Values)
        {
            node.GCost = float.MaxValue;
            node.HCost = 0;
            node.Parent = null;
        }
    }

    void Start()
    {
        roadNetwork = new ERRoadNetwork();
        if (roadNetwork == null)
        {
            Debug.LogError("ERRoadNetwork reference is missing. Please assign it in the inspector.");
            return;
        }
        
        allconnections = roadNetwork.GetConnections();
        foreach (ERConnection connection in allconnections)
        {
            string connectionName = connection.GetName();
            Vector3 centerConnection = connection.gameObject.GetComponent<Transform>().position;

            Node node = new Node
            {
                Name = connectionName,
                Position = centerConnection
            };

            nodes[connectionName] = node;

            ERConnectionData[] connectionsData = connection.GetConnectionData();
            List<(ERRoad, Vector3)> connectPoints = new ();
            foreach (ERConnectionData connectData in connectionsData)
            {
                connectPoints.Add((connectData.road, connectData.road.GetMarkerPosition(connectData.marker)));
            }
            connectionObject[connection] = (centerConnection, connectPoints);
        }

        allRoads = roadNetwork.GetRoadObjects();
        foreach (ERRoad road in allRoads)
        {
            ERConnection startConnection = road.GetConnectionAtStart();
            ERConnection endConnection = road.GetConnectionAtEnd();

            if (startConnection != null && endConnection != null)
            {
                Node startConnectionNode = nodes[startConnection.GetName()];
                Node endConnectionNode = nodes[endConnection.GetName()];
                float roadLength = road.GetDistance();

                startConnectionNode.Neighbors.Add((endConnectionNode, roadLength));
                endConnectionNode.Neighbors.Add((startConnectionNode, roadLength));
            }
        }
    }

    void Update()
    {
        checkpoints = FindRemainingCheckpoint();
        List<Vector3> paths = StartPathFinding(startPoint.position, checkpoints);
        DrawPath(paths);
    }

    public List<Vector3> StartPathFinding(Vector3 startPoint, List<GameObject> checkpoint)
    {
        float distance = float.MaxValue;
        List<Node> directions = new();
        List<Vector3> paths = new();

        (ERRoad, Vector3) roadStart = FindClosestRoad(startPoint);
        Vector3 endPoint = checkpoint[^1].transform.position;
        (ERRoad, Vector3) roadEnd = FindClosestRoad(endPoint);

        float startToEndDistance = Vector2.Distance(new Vector2(startPoint.x, startPoint.z), new Vector2(endPoint.x, endPoint.y));
        float endPointToRoadDistance = Vector2.Distance(new Vector2(endPoint.x, endPoint.z), new Vector2(roadEnd.Item2.x, roadEnd.Item2.y));

        if ((startToEndDistance < endPointToRoadDistance) && (checkpoint.Count == 1))
        {
            paths.Add(startPoint);
            paths.Add(endPoint);
        }
        else
        {
            if ((roadStart.Item2 == roadEnd.Item2) && (checkpoint.Count == 1))
            {
                paths.Add(startPoint);
                paths.Add(roadStart.Item2);
                paths.Add(roadEnd.Item2);
                paths.Add(endPoint);
            }
            else
            {
                List<(ERConnection, float)> connectionsStart = GetPointRoadDistanceToConnection(roadStart);
                List<(ERConnection, float)> connectionsEnd = GetPointRoadDistanceToConnection(roadEnd);

                paths.Add(startPoint);
                if (checkpoint.Count == 1 )
                {
                    foreach ((ERConnection, float) connStart in connectionsStart)
                    {
                        foreach ((ERConnection, float) connEnd in connectionsEnd)
                        {
                            ResetNodes();
                            Node startNode = nodes[connStart.Item1.GetName()];
                            Node targetNode = nodes[connEnd.Item1.GetName()];

                            (List<Node>, float) path = AStar(startNode, targetNode);

                            if (path.Item1 != null)
                            {
                                if (distance > connStart.Item2 + path.Item2 + connEnd.Item2)
                                {
                                    distance = connStart.Item2 + path.Item2 + connEnd.Item2;
                                    directions = path.Item1;
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    CreatePaths(paths, roadStart, directions, roadEnd);
                    paths.Add(endPoint);
                }
                else
                {
                    foreach ((ERConnection, float) connStart in connectionsStart)
                    {
                        ResetNodes();
                        Node startNode = nodes[connStart.Item1.GetName()];
                        Node targetNode = nodes[checkpoint[0].name];

                        (List<Node>, float) path = AStar(startNode, targetNode);

                        if (path.Item1 != null)
                        {
                            if (distance > connStart.Item2 + path.Item2)
                            {
                                distance = connStart.Item2 + path.Item2;
                                directions = path.Item1;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    CreatePaths(paths, roadStart, directions, roadEnd);
                }
            }
            
        }        
        return paths;
    }

    float Heuristic(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    private (List<Node>, float) AStar(Node startNode, Node targetNode)
    {
        List<Node> openSet = new() { startNode };
        HashSet<Node> closedSet = new();
        (List<Node>, float) nullResult = new();

        startNode.GCost = 0;
        startNode.HCost = Heuristic(startNode.Position, targetNode.Position);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || 
                    (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = openSet[i];
                }
            }

            if (currentNode == targetNode)
            {
                return ReconstructPath(startNode, targetNode);
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (var (neighbor, cost) in currentNode.Neighbors)
            {
                float tentativeGCost = currentNode.GCost + cost;
                if (tentativeGCost < neighbor.GCost)
                {
                    neighbor.Parent = currentNode;
                    neighbor.GCost = tentativeGCost;
                    neighbor.HCost = Heuristic(neighbor.Position, targetNode.Position);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return nullResult;
    }

    private (List<Node>, float) ReconstructPath(Node startNode, Node endNode)
    {
        List<Node> path = new();
        Node currentNode = endNode;
        float distance = 0;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            foreach ((Node, float) node in currentNode.Neighbors)
            {
                if (node.Item1 == currentNode.Parent)
                {
                    distance += node.Item2;
                }
            }
            currentNode = currentNode.Parent;
        }

        path.Add(startNode);
        path.Reverse();

        return (path, distance);
    }

    (ERRoad, Vector3) FindClosestRoad(Vector3 point)
    {
        ERRoad closestRoad = null;
        float closestDistance = float.MaxValue;
        Vector3 closestPoint = Vector3.zero;

        foreach (ERRoad road in allRoads)
        {
            int markerCount = road.GetMarkerCount();
            for (int i = 0; i < markerCount - 1; i++)
            {
                Vector3 p1 = road.GetMarkerPosition(i);
                Vector3 p2 = road.GetMarkerPosition(i + 1);

                Vector3 projection = ProjectPointOnLineSegment(point, p1, p2).Item2;
                float distance = Vector3.Distance(point, projection);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestRoad = road;
                    closestPoint = projection;
                }
            }
        }

        return (closestRoad, closestPoint);
    }

    (bool, Vector3) ProjectPointOnLineSegment(Vector3 point, Vector3 p1, Vector3 p2)
    {
        Vector3 lineDir = p2 - p1;
        float lineLength = lineDir.magnitude;
        lineDir.Normalize();

        float projection = Vector3.Dot(point - p1, lineDir);
        if (projection < 0)
        {
            return (false, p1);
        }
        else if (projection > lineLength)
        {
            return (false, p2);
        }
        else
        {
            return (true, p1 + lineDir * projection);
        }
    }

    private List<(ERConnection, float)> GetPointRoadDistanceToConnection((ERRoad, Vector3) roadPoint)
    {
        Vector3 point = roadPoint.Item2;
        List<(ERConnection, float)> connectionDistances = new();
        float distance = 0;
        Vector3[] markers = roadPoint.Item1.GetMarkerPositions();
        Vector3 startMarker = markers[0];
        for (int i = 1; i < markers.Length; i++)
        {
            if (ProjectPointOnLineSegment(point, startMarker, markers[i]).Item1)
            {
                distance += Vector3.Distance(startMarker, point);
                if (roadPoint.Item1.GetConnectionAtStart() != null)
                {
                    connectionDistances.Add((roadPoint.Item1.GetConnectionAtStart(), distance));
                }
                distance = Vector3.Distance(point, markers[i]);
            }
            else {
                distance += Vector3.Distance(startMarker, markers[i]);
            }
            startMarker = markers[i];
        }
        if (roadPoint.Item1.GetConnectionAtEnd() != null)
        {
            connectionDistances.Add((roadPoint.Item1.GetConnectionAtEnd(), distance));
        }
        return connectionDistances;
    }

    private void CreatePaths(List<Vector3> paths, (ERRoad, Vector3) roadStart, List<Node> directions, (ERRoad, Vector3)? roadEnd = null)
    {  
        bool passRoadPoint = false;
        paths.Add(roadStart.Item2);

        Vector3[] markers1 = roadStart.Item1.GetMarkerPositions();
        if (roadStart.Item1.GetConnectionAtStart() != null)
        {
            if (roadStart.Item1.GetConnectionAtStart().GetName() == directions[0].Name)
            {
                Vector3 startMarker = markers1[markers1.Length-1];
                for (int i = markers1.Length - 2; i >= 0; i--)
                {
                    if (!passRoadPoint)
                    {
                        if (ProjectPointOnLineSegment(roadStart.Item2, startMarker, markers1[i]).Item1)
                        {
                            paths.Add(markers1[i]);
                            passRoadPoint = true;
                        }
                    }
                }
                paths.Add(markers1[0]);
            }
            else
            {
                Vector3 startMarker = markers1[0];
                for (int i = 1; i <= markers1.Length-1; i++)
                {
                    if (!passRoadPoint)
                    {
                        if (ProjectPointOnLineSegment(roadStart.Item2, startMarker, markers1[i]).Item1)
                        {
                            paths.Add(markers1[i]);
                            passRoadPoint = true;
                        }
                    }
                }
                paths.Add(markers1[markers1.Length-1]);
            }
        }
        else
        {
            Vector3 startMarker = markers1[0];
            for (int i = 1; i <= markers1.Length-1; i++)
            {
                if (!passRoadPoint)
                {
                    if (ProjectPointOnLineSegment(roadStart.Item2, startMarker, markers1[i]).Item1)
                    {
                        paths.Add(markers1[i]);
                        passRoadPoint = true;
                    }
                }
            }
            paths.Add(markers1[markers1.Length-1]);
        }

        Node start = directions[0];
        paths.Add(start.Position);

        for (int i = 1; i < directions.Count; i++)
        {
            ERRoad road = FindRoadBetweenConnections(start, directions[i]);
            if (road != null)
            {
                Vector3[] markers = road.GetMarkerPositions();
                if (road.GetConnectionAtStart().GetName() == start.Name)
                {
                    for (int j = 1; j <= markers.Length - 2; j++)
                    {
                        paths.Add(markers[j]);
                    }
                }
                else
                {
                    for (int j = markers.Length - 2; j >= 0; j--)
                    {
                        paths.Add(markers[j]);
                    }
                }
                paths.Add(directions[i].Position);
            }
            start = directions[i];
        }

        Vector3[] markers2 = roadEnd.Value.Item1.GetMarkerPositions();
        if (roadEnd.Value.Item1.GetConnectionAtStart() != null)
        {
            if (roadEnd.Value.Item1.GetConnectionAtStart().GetName() == directions[0].Name)
            {
                Vector3 startMarker = markers2[markers2.Length-1];
                for (int i = markers2.Length - 2; i >= 0; i--)
                {
                    if (!passRoadPoint)
                    {
                        if (ProjectPointOnLineSegment(roadEnd.Value.Item2, startMarker, markers2[i]).Item1)
                        {
                            paths.Add(markers2[i]);
                            passRoadPoint = true;
                        }
                    }
                }
                paths.Add(markers2[0]);
            }
            else
            {
                Vector3 startMarker = markers2[0];
                for (int i = 1; i <= markers2.Length-1; i++)
                {
                    if (!passRoadPoint)
                    {
                        if (ProjectPointOnLineSegment(roadEnd.Value.Item2, startMarker, markers2[i]).Item1)
                        {
                            paths.Add(markers2[i]);
                            passRoadPoint = true;
                        }
                    }
                }
                paths.Add(markers2[markers2.Length-1]);
            }
        }
        else
        {
            Vector3 startMarker = markers2[0];
            for (int i = 1; i <= markers2.Length-1; i++)
            {
                if (!passRoadPoint)
                {
                    if (ProjectPointOnLineSegment(roadEnd.Value.Item2, startMarker, markers2[i]).Item1)
                    {
                        paths.Add(markers2[i]);
                        passRoadPoint = true;
                    }
                }
            }
            paths.Add(markers2[markers2.Length-1]);
        }

        paths.Add(roadEnd.Value.Item2);
    }

    private ERRoad FindRoadBetweenConnections(Node start, Node end)
    {
        foreach (ERConnection connection1 in connectionObject.Keys)
        {
            foreach (ERConnection connection2 in connectionObject.Keys)
            {
                if ((connection1.name == start.Name) && (connection2.name == end.Name))
                {
                    foreach ((ERRoad, Vector3) road1 in connectionObject[connection1].Item2)
                    {
                        foreach ((ERRoad, Vector3) road2 in connectionObject[connection2].Item2)
                        {
                            if (road1.Item1 == road2.Item1)
                            {
                                return road1.Item1;
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    private (Vector3, Vector3) ShiftLeft(Vector3 pointA, Vector3 pointB)
    { 
        Vector3 direction = (pointB - pointA).normalized;

        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        Vector3 shiftedPointA = pointA + perpendicular * 5.0f;
        Vector3 shiftedPointB = pointB + perpendicular * 5.0f;
        return (shiftedPointA, shiftedPointB);
    }

    private void DrawPath(List<Vector3> paths)
    {
        GameObject pathParent = GameObject.Find("Path Finding Line");
        if (pathParent == null)
        {
            pathParent = new GameObject("Path Finding Line");
        }

        lineRenderer = pathParent.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = pathParent.AddComponent<LineRenderer>();
        }

        lineRenderer.positionCount = 0;
        lineRenderer.positionCount = paths.Count;

        lineRenderer.SetPosition(0, new Vector3(paths[0].x, 39, paths[0].z));
        for (int i = 1; i < paths.Count - 1; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(paths[i].x, 39, paths[i].z));
        }
        lineRenderer.SetPosition(paths.Count - 1, new Vector3(paths[paths.Count - 1].x, 39, paths[paths.Count - 1].z));

        lineRenderer.numCornerVertices = 100;
        lineRenderer.numCapVertices = 100;
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 5f;
        lineRenderer.endWidth = 5f;
    }

    private void DeletePath()
    {
        lineRenderer.positionCount = 0;
    }

    private List<GameObject> FindRemainingCheckpoint()
    {
        List<GameObject> activeCheckpoints = new();
        GameObject parentObject = GameObject.Find("CP");
        foreach (Transform child in parentObject.transform)
        {
            if ((!child.gameObject.GetComponent<CheckpointController>().HasActivatedOnce) && child.gameObject.activeSelf)
            {
                activeCheckpoints.Add(child.gameObject);
                break;
            }
        }

        if (activeCheckpoints != null)
        {
            GameObject nextCheckpoint = activeCheckpoints[^1].GetComponent<CheckpointPathfindHandler>().GetNextCheckpoint();
            while (nextCheckpoint.name != "Finish")
            {
                activeCheckpoints.Add(nextCheckpoint);
                nextCheckpoint = activeCheckpoints[^1].GetComponent<CheckpointPathfindHandler>().GetNextCheckpoint();
            }
            activeCheckpoints.Add(nextCheckpoint);
        }

        return activeCheckpoints;
    }
}
