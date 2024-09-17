using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Node
{
    public Vector2 Position;
    public bool IsOccupied;
    public List<int> Neighbors; // Lista s�siad�w

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

        // Generowanie w�z��w dla siatki (+1 w szeroko�ci i wysoko�ci)
        for (int x = 0; x <= gridWidth; x++)
        {
            for (int y = 0; y <= gridHeight; y++)
            {
                Vector2 position = new Vector2(x * nodeSpacing + offset.x, y * nodeSpacing + offset.y);
                Node newNode = new Node(position);
                nodes.Add(newNode);
                // Opcjonalnie rysowanie w�z��w
                // Debug.DrawLine(position - Vector2.right * 0.5f, position + Vector2.right * 0.5f, Color.red, 100f);
                // Debug.DrawLine(position - Vector2.up * 0.5f, position + Vector2.up * 0.5f, Color.red, 100f);
            }
        }

        // Generowanie w�z��w bramkowych
        int halfGridWidth = gridWidth / 2;
        int halfGoalWidth = goalWidth / 2;
        Vector2 topGoalCenter = new Vector2(halfGridWidth * nodeSpacing + offset.x, nodeSpacing - offset.y);
        Vector2 bottomGoalCenter = new Vector2(halfGridWidth * nodeSpacing + offset.x, -nodeSpacing + offset.y);

        goalNodes.Add(new Node(topGoalCenter));
        goalNodes.Add(new Node(bottomGoalCenter));

        // Przypisywanie s�siad�w
        AssignNeighbors(gridWidth, gridHeight);
    }

    private void AssignNeighbors(int gridWidth, int gridHeight)
    {
        // Najpierw przypisujemy s�siad�w
        for (int i = 0; i < nodes.Count; i++)
        {
            Node currentNode = nodes[i];
            Vector2 currentPosition = currentNode.Position;

            for (int j = 0; j < nodes.Count; j++)
            {
                if (i == j) continue; // Nie por�wnuj w�z�a z samym sob�

                Node neighborNode = nodes[j];
                Vector2 neighborPosition = neighborNode.Position;

                // Oblicz odleg�o�ci
                float distanceX = Mathf.Abs(currentPosition.x - neighborPosition.x);
                float distanceY = Mathf.Abs(currentPosition.y - neighborPosition.y);

                // Odleg�o�� po skosie
                float distanceDiagonal = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

                // Sprawdzamy, czy s�siad znajduje si� w odleg�o�ci r�wnej lub mniejszej ni� `nodeSpacing`
                bool isNeighbor = (distanceX <= nodeSpacing + 0.1f && distanceY <= nodeSpacing + 0.1f) ||
                                  (distanceDiagonal <= nodeSpacing * Mathf.Sqrt(2) + 0.1f);

                if (isNeighbor)
                {
                    currentNode.AddNeighbor(j);
                }
            }
        }

        // Modyfikujemy s�siedztwo w�z��w na kraw�dziach
        ModifyAdjacencyForEdgeNodes(gridWidth, gridHeight);

        // Nast�pnie rysujemy linie mi�dzy w�z�ami na podstawie listy s�siad�w
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

                // Rysowanie linii mi�dzy w�z�ami
                Debug.DrawLine(currentPosition, neighborPosition, Color.red, 100f);
            }
        }
    }

    private void ModifyAdjacencyForEdgeNodes(int gridWidth, int gridHeight)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Node currentNode = nodes[i];
            Vector2 currentPosition = currentNode.Position;

            // Sprawdzamy, czy w�ze� jest na kraw�dzi siatki
            if (currentPosition.x == 0 || currentPosition.x == gridWidth * nodeSpacing + offset.x)
            {
                // Usu� powi�zania z s�siadami w osi Y
                for (int j = 0; j < nodes.Count; j++)
                {
                    Node neighborNode = nodes[j];
                    if (Mathf.Abs(neighborNode.Position.y - currentPosition.y) <= nodeSpacing &&
                        Mathf.Abs(neighborNode.Position.x - currentPosition.x) <= 0.1f)
                    {
                        // Usuni�cie po��czenia
                        currentNode.Neighbors.Remove(j);
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

            // Rysowanie w�z��w bramkowych
            if (goalNodes != null)
            {
                Gizmos.color = Color.green;
                foreach (var node in goalNodes)
                {
                    Gizmos.DrawSphere(new Vector3(node.Position.x, node.Position.y, 0), 0.1f);
                }
            }
        }

        // Opcjonalne rysowanie po��cze� w edytorze
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
