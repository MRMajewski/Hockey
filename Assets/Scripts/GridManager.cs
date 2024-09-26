using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Node
{
    private Vector2 position;
    private List<Node> neighbors;
    private bool isGoalNode;

    public Vector2 Position { get => position; set => position = value; }
    public bool IsOccupied;
    public List<Node> Neighbors { get => neighbors; set => neighbors = value; }
    public bool IsGoalNode { get => isGoalNode; set => isGoalNode = value; }

    public Node(Vector2 position, bool isGoal = false)
    {
        Position = position;
        IsOccupied = false;
        IsGoalNode = isGoal;
        Neighbors = new List<Node>();
    }

    public bool IsNeighbor(Node node)
    {
        return Neighbors.Contains(node);
    }

    public void AddNeighbor(Node neighbor)
    {
        if (!Neighbors.Contains(neighbor))
        {
            Neighbors.Add(neighbor);
        }
    }

    public List<Node> GetNeighbors()
    {
        return Neighbors;
    }
}

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private float nodeSpacing = 1f;
    private Vector2 offset;
    [SerializeField]
    private List<Node> nodes = new List<Node>();
    [SerializeField]
    private List<Node> goalNodes = new List<Node>();
    public List<Node> Nodes => nodes;
    public List<Node> GoalNodes => goalNodes;

    private float edgeX;
    private float edgeXMax;
    private float edgeY;
    private float edgeYMax;

    public void GenerateNodes(int gridWidth, int gridHeight, int goalWidth = 2)
    {
        offset = new Vector2(gridWidth / -2f, gridHeight / -2f);
        nodes.Clear();
        goalNodes.Clear();

        edgeX = offset.x;
        edgeXMax = gridWidth * nodeSpacing + offset.x;
        edgeY = offset.y;
        edgeYMax = gridHeight * nodeSpacing + offset.y;

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
                 currentNode.AddNeighbor(neighborNode);
                }
            }
        }

        ModifyAdjacencyForSpecificNodes();

        foreach (var goalNode in goalNodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                Node currentNode = nodes[i];
                Vector2 distance = goalNode.Position - currentNode.Position;

                if (distance.magnitude <= nodeSpacing * Mathf.Sqrt(2) + 0.1f)
                {
                    goalNode.AddNeighbor(nodes[i]);
                    currentNode.AddNeighbor(goalNode);
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

            foreach (Node neighborIndex in currentNode.Neighbors)
            {
                Node neighborNode = neighborIndex;
                Debug.DrawLine(currentNode.Position, neighborNode.Position, Color.red, 100f);
            }
        }
    }
    private void ModifyAdjacencyForSpecificNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Node currentNode = nodes[i];

            bool isBottomLeftCorner = currentNode.Position.x == edgeX && currentNode.Position.y == edgeY;
            bool isBottomRightCorner = currentNode.Position.x == edgeXMax && currentNode.Position.y == edgeY;
            bool isTopLeftCorner = currentNode.Position.x == edgeX && currentNode.Position.y == edgeYMax;
            bool isTopRightCorner = currentNode.Position.x == edgeXMax && currentNode.Position.y == edgeYMax;

            if (currentNode.Position.y == edgeY && !isBottomLeftCorner && !isBottomRightCorner)
            {
                RemoveNeighbors(currentNode, n =>
                 (n.Position.x == currentNode.Position.x - 1 && n.Position.y == edgeYMax - 1) || 
                 (n.Position.x == currentNode.Position.x + 1 && n.Position.y == edgeYMax - 1));  
            }

            else if (currentNode.Position.y == edgeYMax && !isTopLeftCorner && !isTopRightCorner)
            {
                RemoveNeighbors(currentNode, n =>
                 (n.Position.x == currentNode.Position.x - 1 && n.Position.y == edgeY + 1) || 
                 (n.Position.x == currentNode.Position.x + 1 && n.Position.y == edgeY + 1));     
            }

            else if (currentNode.Position.x == edgeX || currentNode.Position.x == edgeXMax)
            {
                RemoveNeighbors(currentNode, n => n.Position.x == currentNode.Position.x);
            }

            else if (currentNode.Position.y == edgeY || currentNode.Position.y == edgeYMax)
            {
                RemoveNeighbors(currentNode, n => n.Position.y == currentNode.Position.y);
            }

            // Corners
            else if (isBottomLeftCorner)
            {
                RemoveNeighbors(currentNode, n => n.Position.x == edgeX && n.Position.y > edgeY);
                RemoveNeighbors(currentNode, n => n.Position.x < edgeX && n.Position.y == edgeY);
            }
            else if (isBottomRightCorner)
            {
                RemoveNeighbors(currentNode, n => n.Position.x == edgeXMax && n.Position.y > edgeY);
                RemoveNeighbors(currentNode, n => n.Position.x > edgeXMax && n.Position.y == edgeY);
            }
            else if (isTopLeftCorner)
            {
                RemoveNeighbors(currentNode, n => n.Position.x == edgeX && n.Position.y < edgeYMax);
                RemoveNeighbors(currentNode, n => n.Position.x < edgeX && n.Position.y == edgeYMax);
            }
            else if (isTopRightCorner)
            {
                RemoveNeighbors(currentNode, n => n.Position.x == edgeXMax && n.Position.y < edgeYMax);
                RemoveNeighbors(currentNode, n => n.Position.x > edgeXMax && n.Position.y == edgeYMax);
            }
        }
    }
    internal Node GetNodeAtPosition(Vector2 position)
    {
        foreach (var node in nodes)
        {
            if (Vector2.Distance(position, new Vector2(node.Position.x, node.Position.y)) < nodeSpacing * 0.5f)
            {
                return node;
            }
        }
        return null;
    }

    private void RemoveNeighbors(Node node, Predicate<Node> condition)
    {
        for (int j = node.Neighbors.Count - 1; j >= 0; j--)
        {
            Node neighborNode = node.Neighbors[j];

            if (condition(neighborNode))
            {
                node.Neighbors.RemoveAt(j);
            }
        }
    }
}
