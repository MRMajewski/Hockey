using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Node
{
    public Vector2 Position;
    public bool IsOccupied;
    public List<int> Neighbors; // Lista s¹siadów

    public Node(Vector2 position)
    {
        Position = position;
        IsOccupied = false;
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
{
    public float nodeSpacing = 1f;
    public Vector2 offset;
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

        // Generowanie wêz³ów dla siatki (+1 w szerokoœci i wysokoœci)
        for (int x = 0; x <= gridWidth; x++)
        {
            for (int y = 0; y <= gridHeight; y++)
            {
                Vector2 position = new Vector2(x * nodeSpacing + offset.x, y * nodeSpacing + offset.y);
                Node newNode = new Node(position);
                nodes.Add(newNode);
                // Opcjonalnie rysowanie wêz³ów
                // Debug.DrawLine(position - Vector2.right * 0.5f, position + Vector2.right * 0.5f, Color.red, 100f);
                // Debug.DrawLine(position - Vector2.up * 0.5f, position + Vector2.up * 0.5f, Color.red, 100f);
            }
        }

        // Generowanie wêz³ów bramkowych
        int halfGridWidth = gridWidth / 2;
        int halfGoalWidth = goalWidth / 2;
        Vector2 topGoalCenter = new Vector2(halfGridWidth * nodeSpacing + offset.x, nodeSpacing - offset.y);
        Vector2 bottomGoalCenter = new Vector2(halfGridWidth * nodeSpacing + offset.x, -nodeSpacing + offset.y);

        goalNodes.Add(new Node(topGoalCenter));
        goalNodes.Add(new Node(bottomGoalCenter));

        // Przypisywanie s¹siadów
        AssignNeighbors(gridWidth, gridHeight);
    }

    private void AssignNeighbors(int gridWidth, int gridHeight)
    {
        // Najpierw przypisujemy s¹siadów
        for (int i = 0; i < nodes.Count; i++)
        {
            Node currentNode = nodes[i];
            Vector2 currentPosition = currentNode.Position;

            for (int j = 0; j < nodes.Count; j++)
            {
                if (i == j) continue; // Nie porównuj wêz³a z samym sob¹

                Node neighborNode = nodes[j];
                Vector2 neighborPosition = neighborNode.Position;

                // Oblicz odleg³oœci
                float distanceX = Mathf.Abs(currentPosition.x - neighborPosition.x);
                float distanceY = Mathf.Abs(currentPosition.y - neighborPosition.y);

                // Odleg³oœæ po skosie
                float distanceDiagonal = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

                // Sprawdzamy, czy s¹siad znajduje siê w odleg³oœci równej lub mniejszej ni¿ `nodeSpacing`
                bool isNeighbor = (distanceX <= nodeSpacing + 0.1f && distanceY <= nodeSpacing + 0.1f) ||
                                  (distanceDiagonal <= nodeSpacing * Mathf.Sqrt(2) + 0.1f);

                if (isNeighbor)
                {
                    currentNode.AddNeighbor(j);
                }
            }
        }

        // Modyfikujemy s¹siedztwo wêz³ów na krawêdziach
        ModifyAdjacencyForEdgeNodes(gridWidth, gridHeight);

        // Nastêpnie rysujemy linie miêdzy wêz³ami na podstawie listy s¹siadów
        DrawConnections();
    }

    private void DrawConnections()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Node currentNode = nodes[i];
            Vector2 currentPosition = currentNode.Position;

            foreach (int neighborIndex in currentNode.Neighbors)
            {
                Node neighborNode = nodes[neighborIndex];
                Vector2 neighborPosition = neighborNode.Position;

                // Rysowanie linii miêdzy wêz³ami
                Debug.DrawLine(currentPosition, neighborPosition, Color.red, 100f);
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
            Vector2 currentPosition = currentNode.Position;

            // Sprawdzamy, czy wêze³ jest na lewej krawêdzi siatki
            if (Mathf.Approximately(currentPosition.x, edgeX))
            {
                // Usuñ powi¹zania z s¹siadami w osi X
                for (int j = currentNode.Neighbors.Count - 1; j >= 0; j--)
                {
                    int neighborIndex = currentNode.Neighbors[j];
                    Node neighborNode = nodes[neighborIndex];
                    if (Mathf.Approximately(neighborNode.Position.x, edgeX))
                    {
                        // Usuñ po³¹czenie, jeœli s¹siad jest równie¿ na lewej krawêdzi
                        currentNode.Neighbors.RemoveAt(j);
                    }
                }
            }
            // Sprawdzamy, czy wêze³ jest na prawej krawêdzi siatki
            else if (Mathf.Approximately(currentPosition.x, edgeXMax))
            {
                // Usuñ powi¹zania z s¹siadami w osi X
                for (int j = currentNode.Neighbors.Count - 1; j >= 0; j--)
                {
                    int neighborIndex = currentNode.Neighbors[j];
                    Node neighborNode = nodes[neighborIndex];
                    if (Mathf.Approximately(neighborNode.Position.x, edgeXMax))
                    {
                        // Usuñ po³¹czenie, jeœli s¹siad jest równie¿ na prawej krawêdzi
                        currentNode.Neighbors.RemoveAt(j);
                    }
                }
            }

            // Sprawdzamy, czy wêze³ jest na górnej krawêdzi siatki
            if (Mathf.Approximately(currentPosition.y, edgeY))
            {
                // Usuñ powi¹zania z s¹siadami w osi Y
                for (int j = currentNode.Neighbors.Count - 1; j >= 0; j--)
                {
                    int neighborIndex = currentNode.Neighbors[j];
                    Node neighborNode = nodes[neighborIndex];
                    if (Mathf.Approximately(neighborNode.Position.y, edgeY))
                    {
                        // Usuñ po³¹czenie, jeœli s¹siad jest równie¿ na górnej krawêdzi
                        currentNode.Neighbors.RemoveAt(j);
                    }
                }
            }
            // Sprawdzamy, czy wêze³ jest na dolnej krawêdzi siatki
            else if (Mathf.Approximately(currentPosition.y, edgeYMax))
            {
                // Usuñ powi¹zania z s¹siadami w osi Y
                for (int j = currentNode.Neighbors.Count - 1; j >= 0; j--)
                {
                    int neighborIndex = currentNode.Neighbors[j];
                    Node neighborNode = nodes[neighborIndex];
                    if (Mathf.Approximately(neighborNode.Position.y, edgeYMax))
                    {
                        // Usuñ po³¹czenie, jeœli s¹siad jest równie¿ na dolnej krawêdzi
                        currentNode.Neighbors.RemoveAt(j);
                    }
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

            // Rysowanie wêz³ów bramkowych
            if (goalNodes != null)
            {
                Gizmos.color = Color.green;
                foreach (var node in goalNodes)
                {
                    Gizmos.DrawSphere(new Vector3(node.Position.x, node.Position.y, 0), 0.1f);
                }
            }
        }

        // Opcjonalne rysowanie po³¹czeñ w edytorze
        // DrawConnections();
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
