using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyRoads3Dv3;
using System;
using Unity.VisualScripting;
using PG;

public class PathFinding : MonoBehaviour
{
    Dictionary<string, Node> nodes = new Dictionary<string, Node>();
    // Name: (Center Point, (List Connection Point))
    Dictionary<ERConnection, (Vector3, List<(ERRoad, Vector3)>)> connectionObject = new();
    // Name: (List Control Point)
    Dictionary<ERRoad, (List<Vector3>, float)> roadObject = new();
    private ERRoadNetwork roadNetwork;
    private ERRoad[] allRoads;
    private ERConnection[] allconnections;
    [SerializeField] private Transform startPoint;
    private Vector3 endPoint;
    [SerializeField] private GameObject parentObjectName;
    [SerializeField] private GameObject groupObjectName;
    private bool findPath = false;
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

                Vector3[] markers = road.GetMarkerPositions();
                List<Vector3> roadPoints = new();
                foreach (Vector3 marker in markers)
                {
                    roadPoints.Add(marker);
                }
                roadObject[road] = (roadPoints, road.GetWidth());
            }
        }
    }

    void Update()
    {
        if (!findPath)
        {
            List<Vector3> paths = StartPathFinding(startPoint.position, new Vector3(187, 35, 95));
            // List<Vector3> paths = StartPathFinding(point.position, endPoint);
            DrawPath(paths);
        }
    }

    public List<Vector3> StartPathFinding(Vector3 startPoint, Vector3 endPoint)
    {
        (ERRoad, Vector3) roadStart = FindRoadStartEndPath(startPoint);
        List<(ERConnection, float)> connectionsStart = GetPointRoadDistanceToConnection(roadStart);
        (ERRoad, Vector3) roadEnd = FindRoadStartEndPath(endPoint);
        List<(ERConnection, float)> connectionsEnd = GetPointRoadDistanceToConnection(roadEnd);
        float distance = float.MaxValue;
        List<Node> directions = new();
        List<Vector3> paths = new();

        Debug.Log($"Start Point: {roadStart.Item2}");
        Debug.Log($"End Point: {roadEnd.Item2}");

        Debug.Log("Finding...");
        foreach ((ERConnection, float) connStart in connectionsStart)
        {
            foreach ((ERConnection, float) connEnd in connectionsEnd)
            {
                ResetNodes();
                Debug.Log($"Try start connection: {connStart.Item1.GetName()}");
                Debug.Log($"Try end connection: {connEnd.Item1.GetName()}");
                Node startNode = nodes[connStart.Item1.GetName()];
                Node targetNode = nodes[connEnd.Item1.GetName()];

                (List<Node>, float) path = AStar(startNode, targetNode);

                if (path.Item1 != null)
                {
                    Debug.Log($"Path found! Distance: {path.Item2}");
                    if (distance > connStart.Item2 + path.Item2 + connEnd.Item2)
                    {
                        distance = connStart.Item2 + path.Item2 + connEnd.Item2;
                        directions = path.Item1;
                    }
                }
                else
                {
                    Debug.Log("No path found.");
                }
            }
        }
        Debug.Log($"Chosen path distance: {distance}");
        
        paths.Add(startPoint);
        CreatePaths(paths, roadStart, directions, roadEnd);
        paths.Add(endPoint);
        foreach (Vector3 point in paths)
        {
            Debug.Log($"{point}");
        }
        return paths;
    }

    float Heuristic(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    private (List<Node>, float) AStar(Node startNode, Node targetNode)
    {
        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();
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
        List<Node> path = new List<Node>();
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

    public string GetBuildingNameContainingPoint(Vector3 point)
    {
        GameObject parentObject = GameObject.Find(parentObjectName.name);
        if (parentObject == null)
        {
            Debug.LogError($"Parent GameObject '{parentObjectName}' not found.");
            return null;
        }

        Transform groupTransform = parentObject.transform.Find(groupObjectName.name);
        if (groupTransform == null)
        {
            Debug.LogError($"Group GameObject '{groupObjectName}' not found under '{parentObjectName}'.");
            return null;
        }

        foreach (Transform building in groupTransform)
        {
            if (IsPointInsideBuildingUsingChildren(building, point))
            {
                return building.gameObject.name;
            }
        }
        return null;
    }

    private bool IsPointInsideBuildingUsingChildren(Transform buildingTransform, Vector3 point)
    {
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
        Quaternion rotation = Quaternion.Euler(0, child.rotation.eulerAngles.y, 0);
        Vector3 rotatedPoint = rotation * (point - child.position) + child.position;

        Vector3 halfScale = child.localScale / 2;
        float minX = child.position.x - halfScale.x;
        float maxX = child.position.x + halfScale.x;
        float minZ = child.position.z - halfScale.z;
        float maxZ = child.position.z + halfScale.z;
        
        bool insideX = rotatedPoint.x >= minX && rotatedPoint.x <= maxX;
        bool insideZ = rotatedPoint.z >= minZ && rotatedPoint.z <= maxZ;
        return insideX && insideZ;
    }

    private ERRoad CheckPointInTheRoad(Vector3 point)
    {
        foreach (ERRoad road in allRoads)
        {
            Vector3[] markers = road.GetMarkerPositions();
            for (int i = 0; i < markers.Length-1; i++)
            {
                (bool, Vector3) projection = ProjectPointOnLineSegment(point, markers[i], markers[i+1]);
                if (projection.Item1)
                {
                    if (Vector3.Distance(point, projection.Item2) <= road.GetWidth())
                    {
                        return road;
                    }
                }
            }
        }
        return null;
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

    private ERRoad FindRoadInBuilding(string buildingName)
    {
        foreach (ERRoad road in allRoads)
        {
            foreach (Vector3 marker in road.GetMarkerPositions())
            {
                if (GetBuildingNameContainingPoint(marker) == buildingName)
                {
                    return road;
                }
            }
        }
        return null;
    }

    private (ERRoad, Vector3) FindRoadStartEndPath(Vector3 point)
    {
        string findBuilding = GetBuildingNameContainingPoint(point);
        if (findBuilding != null)
        {
            ERRoad road = FindRoadInBuilding(findBuilding);
            if (road != null)
            {
                Vector3[] markers = road.GetMarkerPositions();
                Vector3 closestPoint = ProjectPointOnLineSegment(point, markers[0], markers[markers.Length-1]).Item2;
                return (road, closestPoint);
            }
            else
            {
                return FindClosestRoad(point);
            }
            
        }
        else
        {
            return FindClosestRoad(point);
        }
    }

    private void CreatePaths(List<Vector3> paths, (ERRoad, Vector3) roadStart, List<Node> directions, (ERRoad, Vector3) roadEnd)
    {  
        paths.Add(roadStart.Item2);
        Vector3[] markers1 = roadStart.Item1.GetMarkerPositions();
        if (roadStart.Item1.GetConnectionAtStart() != null)
        {
            if (roadStart.Item1.GetConnectionAtStart().GetName() == directions[0].Name)
            {
                paths.Add(markers1[0]);
            }
            else
            {
                paths.Add(markers1[markers1.Length-1]);
            }
        }
        else
        {
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

        Vector3[] markers2 = roadEnd.Item1.GetMarkerPositions();
        if (roadEnd.Item1.GetConnectionAtStart() != null)
        {
            if (roadEnd.Item1.GetConnectionAtStart().GetName() == directions[0].Name)
            {
                paths.Add(markers2[0]);
            }
            else
            {
                paths.Add(markers2[markers1.Length-1]);
            }
        }
        else
        {
            paths.Add(markers2[markers1.Length-1]);
        }
        paths.Add(roadEnd.Item2);
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

    public void setEndPoint(Vector3 point)
    {
        endPoint = point;
    }
}
