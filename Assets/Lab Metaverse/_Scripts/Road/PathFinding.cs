using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyRoads3Dv3;
using System;

public class PathFinding : MonoBehaviour
{
    // Store connection information as dictionary
    private Dictionary<string, (Vector3, List<(string, Vector3)>)> connectionInfo = new();

    // Store road start, end, and length information
    private Dictionary<string, List<object>> roadInfo = new Dictionary<string, List<object>>();

    // Store road control point positions
    private Dictionary<string, List<Vector3>> roadControlPoints = new Dictionary<string, List<Vector3>>();

    private ERRoadNetwork roadNetwork;
    [SerializeField] private Vector3 startPoint;

    // Node class for A* pathfinding
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

    void Start()
    {
        roadNetwork = new ERRoadNetwork();
        if (roadNetwork == null)
        {
            Debug.LogError("ERRoadNetwork reference is missing. Please assign it in the inspector.");
            return;
        }

        // Create node dictionary
        Dictionary<string, Node> nodes = new Dictionary<string, Node>();

        // 1. Get connection information and create nodes
        ERConnection[] connections = roadNetwork.GetConnections();
        foreach (ERConnection connection in connections)
        {
            string connectionName = connection.GetName();
            Vector3 centerConnection = connection.gameObject.GetComponent<Transform>().position;

            // Create a node for each connection
            Node node = new Node
            {
                Name = connectionName,
                Position = centerConnection
            };

            nodes[connectionName] = node;
        }

        // 2. Set neighbors based on connection info
        ERRoad[] allRoads = roadNetwork.GetRoadObjects();
        foreach (ERRoad road in allRoads)
        {
            string roadName = road.GetName();
            ERConnection startConnection = road.GetConnectionAtStart();
            ERConnection endConnection = road.GetConnectionAtEnd();

            if (startConnection != null && endConnection != null)
            {
                Node startConnectionNode = nodes[startConnection.GetName()];
                Node endConnectionNode = nodes[endConnection.GetName()];
                float roadLength = road.GetDistance();

                // Link both start and end nodes as neighbors
                startConnectionNode.Neighbors.Add((endConnectionNode, roadLength));
                endConnectionNode.Neighbors.Add((startConnectionNode, roadLength));
            }
        }

        // Test A* Pathfinding
        Vector3 endPoint = new Vector3 (-150, 6, 145);
        Debug.Log($"Closest Road to {endPoint}: {FindClosestRoad(endPoint).Item2}");
        Node startNode = nodes["Default T Crossing (3)"];  // Replace with actual start node name
        Node targetNode = nodes["Default T Crossing (9)"]; // Replace with actual target node name

        List<Node> path = AStar(startNode, targetNode);

        if (path != null)
        {
            Debug.Log("Path found!");
            foreach (Node node in path)
            {
                Debug.Log(node.Name);
            }
        }
        else
        {
            Debug.Log("No path found.");
        }
    }

    // Heuristic function for A* (Euclidean distance)
    float Heuristic(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    // A* Algorithm
    List<Node> AStar(Node startNode, Node targetNode)
    {
        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.GCost = 0;
        startNode.HCost = Heuristic(startNode.Position, targetNode.Position);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
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
                if (closedSet.Contains(neighbor)) continue;

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

        return null; // No path found
    }

    // Function to reconstruct the path from the start to the target node
    List<Node> ReconstructPath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Add(startNode);
        path.Reverse();

        return path;
    }

    // Debugging function to print road network info
    void PrintRoadNetworkInfo()
    {
        Debug.Log("=== Connection Info ===");
        foreach (var connection in connectionInfo)
        {
            Debug.Log($"Connection Name: {connection.Key}, Center Position: {connection.Value.Item1}");
            foreach (var conn in connection.Value.Item2)
            {
                Debug.Log($"Road Connected: {conn.Item1}, Connected Point: {conn.Item2}");
            }
        }

        Debug.Log("=== Road Info (Start, End, Length) ===");
        foreach (var road in roadInfo)
        {
            Debug.Log($"Road Name: {road.Key}, Start: {road.Value[0]}, End: {road.Value[1]}, Length: {road.Value[2]}");
        }

        Debug.Log("=== Road Control Points ===");
        foreach (var road in roadControlPoints)
        {
            Debug.Log($"Road Name: {road.Key}, Control Points: {string.Join(", ", road.Value)}");
        }
    }

    (string, Vector3) FindClosestRoad(Vector3 point)
    {
        ERRoad[] allRoads = roadNetwork.GetRoadObjects();

        string closestRoadName = null;
        float closestDistance = float.MaxValue;
        Vector3 closestPoint = Vector3.zero;

        // Loop through all road objects
        foreach (ERRoad road in allRoads)
        {
            int markerCount = road.GetMarkerCount();
            for (int i = 0; i < markerCount - 1; i++)
            {
                Vector3 p1 = road.GetMarkerPosition(i);
                Vector3 p2 = road.GetMarkerPosition(i + 1);

                Vector3 projection = ProjectPointOnLineSegment(point, p1, p2);
                float distance = Vector3.Distance(point, projection);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestRoadName = road.GetName();
                    closestPoint = projection;
                }
            }
        }

        return (closestRoadName, closestPoint);
    }

    // Project a point onto a line segment and return the closest point
    Vector3 ProjectPointOnLineSegment(Vector3 point, Vector3 p1, Vector3 p2)
    {
        Vector3 lineDir = p2 - p1;
        float lineLength = lineDir.magnitude;
        lineDir.Normalize();

        float projection = Vector3.Dot(point - p1, lineDir);
        if (projection < 0)
        {
            return p1;
        }
        else if (projection > lineLength)
        {
            return p2;
        }
        else
        {
            return p1 + lineDir * projection;
        }
    }
}
