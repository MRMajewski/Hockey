using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Node
{
    public Vector2 Position;
    public bool IsOccupied;
    public List<int> Neighbors;
    public bool IsGoalNode;

    public Node(Vector2 position, bool isGoal = false)
    {
        Position = position;
        IsOccupied = false;
        IsGoalNode = isGoal;
        Neighbors = new List<int>();
    }

    public void AddNeighbor(int neighborId)
    {
        if (!Neighbors.Contains(neighborId))
        {
            Neighbors.Add(neighborId);
        }
    }
}

public class GridManager : MonoBehaviour
{  [SerializeField]
    private float nodeSpacing = 1f;
    private Vector2 offset;
    [SerializeField]
    private List<Node> nodes = new List<Node>();
    [SerializeField]
    private List<Node> goalNodes = new List<Node>();

    public void GenerateNodes(int gridWidth, int gridHeight, int goalWidth = 2)
    {
        offset = new Vector2(gridWidth / -2f, gridHeight / -2f);
        nodes.Clear();
        goalNodes.Clear();

        int totalNodes = (gridWidth + 1) * (gridHeight + 1);

        for (int x = 0; x <= gridWidth; x++)
        {
            for (int y = 0; y <= gridHeight; y++)
            {
                Vector2 position = new Vector2(x * nodeSpacing + offset.x, y * nodeSpacing + offset.y);
                Node newNode = new Node(position);
                nodes.Add(newNode);
            }
        }

        Vector2 topGoalCenter = new Vector2(gridWidth / 2 * nodeSpacing + offset.x, nodeSpacing - offset.y);
        Vector2 bottomGoalCenter = new Vector2(gridWidth / 2 * nodeSpacing + offset.x, -nodeSpacing + offset.y);

        Node topGoal = new Node(topGoalCenter, true);
        Node bottomGoal = new Node(bottomGoalCenter, true);

        goalNodes.Add(topGoal);
        goalNodes.Add(bottomGoal);

        nodes.Add(topGoal);
        nodes.Add(bottomGoal);

        AssignNeighbors(gridWidth, gridHeight);
    }

    private void AssignNeighbors(int gridWidth, int gridHeight)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Node currentNode = nodes[i];

            for (int j = 0; j < nodes.Count; j++)
            {
                if (i == j) continue;

                Node neighborNode = nodes[j];
                Vector2 distance = currentNode.Position - neighborNode.Position;

                if (distance.magnitude <= nodeSpacing * Mathf.Sqrt(2) + 0.1f)
                {
                    currentNode.AddNeighbor(j);
                }
            }
        }

        ModifyAdjacencyForEdgeNodes(gridWidth, gridHeight);

        foreach (var goalNode in goalNodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                Node currentNode = nodes[i];
                Vector2 distance = goalNode.Position - currentNode.Position;

                if (distance.magnitude <= nodeSpacing * Mathf.Sqrt(2) + 0.1f)
                {
                    goalNode.AddNeighbor(i);
                    currentNode.AddNeighbor(nodes.IndexOf(goalNode));
                }
            }
        }

        DrawConnections();
    }

    private void DrawConnections()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Node currentNode = nodes[i];

            foreach (int neighborIndex in currentNode.Neighbors)
            {
                Node neighborNode = nodes[neighborIndex];
                Debug.DrawLine(currentNode.Position, neighborNode.Position, Color.red, 100f);
            }
        }
    }

    private void ModifyAdjacencyForEdgeNodes(int gridWidth, int gridHeight)
    {
        float edgeX = offset.x;
        float edgeXMax = gridWidth * nodeSpacing + offset.x;
        float edgeY = offset.y;
        float edgeYMax = gridHeight * nodeSpacing + offset.y;

        for (int i = 0; i < nodes.Count; i++)
        {
            Node currentNode = nodes[i];

            if (Mathf.Approximately(currentNode.Position.x, edgeX) || Mathf.Approximately(currentNode.Position.x, edgeXMax))
            {
                RemoveEdgeNeighbors(currentNode, n => Mathf.Approximately(n.Position.x, currentNode.Position.x));
            }

            if (Mathf.Approximately(currentNode.Position.y, edgeY) || Mathf.Approximately(currentNode.Position.y, edgeYMax))
            {
                RemoveEdgeNeighbors(currentNode, n => Mathf.Approximately(n.Position.y, currentNode.Position.y));
            }
        }
    }

    private void RemoveEdgeNeighbors(Node node, Predicate<Node> condition)
    {
        for (int j = node.Neighbors.Count - 1; j >= 0; j--)
        {
            int neighborIndex = node.Neighbors[j];
            Node neighborNode = nodes[neighborIndex];

            if (condition(neighborNode))
            {
                node.Neighbors.RemoveAt(j);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (nodes != null)
        {
            Gizmos.color = Color.red;
            foreach (var node in nodes)
            {
                Gizmos.DrawSphere(new Vector3(node.Position.x, node.Position.y, 0), 0.1f);
            }

            if (goalNodes != null)
            {
                Gizmos.color = Color.green;
                foreach (var node in goalNodes)
                {
                    Gizmos.DrawSphere(new Vector3(node.Position.x, node.Position.y, 0), 0.1f);
                }
            }
        }
    }

    private Vector2 GetClosestNodePosition(Vector2 position)
    {
        Node closestNode = null;
        float closestDistance = Mathf.Infinity;

        foreach (var node in nodes)
        {
            float distance = Vector2.Distance(position, node.Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode != null ? closestNode.Position : position;
    }

    public List<Node> GetAllNodes() => nodes;
    public List<Node> GetGoalNodes() => goalNodes;
}
