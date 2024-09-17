using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Node
{
    public Vector2 Position;
    public bool IsOccupied;
    public int[,] adjacencyMatrix;

    public Node(Vector2 position, int totalNodes)
    {
        Position = position;
        IsOccupied = false;
        adjacencyMatrix = new int[totalNodes, totalNodes];
    }

    public void AddNeighbor(int neighborId)
    {
        adjacencyMatrix[neighborId, neighborId] = 1;
    }
}

public class GridManager : MonoBehaviour
{
    public float nodeSpacing = 1f;
    public Vector2 offset = new Vector2(0.5f, 0.5f);
    [SerializeField]
    private List<Node> nodes = new List<Node>();
    [SerializeField]
    private List<Node> goalNodes = new List<Node>();

    public void GenerateNodes(int gridWidth, int gridHeight, int goalWidth = 2)
    {
        nodes.Clear();
        goalNodes.Clear();
        int totalNodes = (gridWidth + 1) * (gridHeight + 1);

        // Generowanie wêz³ów dla siatki (+1 w szerokoœci i wysokoœci)
        for (int x = 0; x <= gridWidth; x++)
        {
            for (int y = 0; y <= gridHeight; y++)
            {
                Vector2 position = new Vector2(x * nodeSpacing + offset.x, y * nodeSpacing + offset.y);
                Node newNode = new Node(position, totalNodes);
                nodes.Add(newNode);
                Debug.DrawLine(position - Vector2.right * 0.5f, position + Vector2.right * 0.5f, Color.red, 100f);
                Debug.DrawLine(position - Vector2.up * 0.5f, position + Vector2.up * 0.5f, Color.red, 100f);
            }
        }

        // Generowanie wêz³ów bramkowych
        int halfGridWidth = gridWidth / 2;
        int halfGoalWidth = goalWidth / 2;
        Vector2 topGoalCenter = new Vector2(halfGridWidth * nodeSpacing + offset.x, nodeSpacing - offset.y);
        Vector2 bottomGoalCenter = new Vector2(halfGridWidth * nodeSpacing + offset.x, -nodeSpacing + offset.y);

        goalNodes.Add(new Node(topGoalCenter, totalNodes));
        goalNodes.Add(new Node(bottomGoalCenter, totalNodes));

        Debug.DrawLine(topGoalCenter - Vector2.right * 1f, topGoalCenter + Vector2.right * 1f, Color.red, 100f);
        Debug.DrawLine(topGoalCenter - Vector2.up * 1f, topGoalCenter + Vector2.up * 1f, Color.red, 100f);
        Debug.DrawLine(bottomGoalCenter - Vector2.right * 1f, bottomGoalCenter + Vector2.right * 1f, Color.red, 100f);
        Debug.DrawLine(bottomGoalCenter - Vector2.up * 1f, bottomGoalCenter + Vector2.up * 1f, Color.red, 100f);

        // Przypisywanie s¹siadów
        AssignNeighbors(gridWidth, gridHeight);
    }

    private void AssignNeighbors(int gridWidth, int gridHeight)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Node currentNode = nodes[i];
            Vector2 currentPosition = currentNode.Position;

            foreach (Node node in nodes)
            {
                if (Vector2.Distance(currentNode.Position, node.Position) <= nodeSpacing + 0.1f)
                {
                    currentNode.AddNeighbor(nodes.IndexOf(node));
                }
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

            Gizmos.color = Color.blue;
            int gridWidth = (int)Mathf.Sqrt(nodes.Count) - 1;
            int gridHeight = gridWidth;

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector2 position = new Vector2(x * nodeSpacing + offset.x, y * nodeSpacing + offset.y);
                    Node currentNode = nodes.Find(node => node.Position == position);

                    if (currentNode != null)
                    {
                        if (x < gridWidth && y < gridHeight)
                        {
                            Vector2 diagonal1 = new Vector2((x + 1) * nodeSpacing + offset.x, (y + 1) * nodeSpacing + offset.y);
                            Gizmos.DrawLine(new Vector3(position.x, position.y, 0), new Vector3(diagonal1.x, diagonal1.y, 0));
                        }
                        if (x < gridWidth && y > 0)
                        {
                            Vector2 diagonal2 = new Vector2((x + 1) * nodeSpacing + offset.x, (y - 1) * nodeSpacing + offset.y);
                            Gizmos.DrawLine(new Vector3(position.x, position.y, 0), new Vector3(diagonal2.x, diagonal2.y, 0));
                        }
                    }
                }
            }
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

    private Vector2 GetClosestNodePosition(Vector2 position)
    {
        Node closestNode = null;
        float closestDistance = Mathf.Infinity;

        foreach (var node in GetAllNodes())
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

    public List<Node> GetAllNodes()
    {
        return nodes;
    }

    public List<Node> GetGoalNodes()
    {
        return goalNodes;
    }
}
