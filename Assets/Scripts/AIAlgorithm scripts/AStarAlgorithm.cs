using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm : IAIAlgorithm
{
    private GridManager gridManager;
    private BallPathRenderer pathRenderer;

    public AStarAlgorithm(GridManager gridManager, BallPathRenderer pathRenderer)
    {
        this.gridManager = gridManager;
        this.pathRenderer = pathRenderer;
    }

    public Node GetBestMove(Node currentNode, Node goalNode)
    {
        List<Node> path = CalculatePathAStar(currentNode, goalNode);
        if (path != null && path.Count > 1)
        {
            return path[1]; // Zwracamy pierwszy ruch po currentNode
        }
        return null;
    }

    private List<Node> CalculatePathAStar(Node startNode, Node goalNode)
    {
        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();

        foreach (var node in gridManager.Nodes)
        {
            gScore[node] = float.MaxValue;
            fScore[node] = float.MaxValue;
        }

        gScore[startNode] = 0;
        fScore[startNode] = Vector2.Distance(startNode.Position, goalNode.Position);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            foreach (var node in openSet)
            {
                if (fScore[node] < fScore[currentNode])
                {
                    currentNode = node;
                }
            }

            if (currentNode == goalNode)
            {
                return ReconstructPath(cameFrom, currentNode);
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (Node neighbor in currentNode.GetNeighbors())
            {
                if (closedSet.Contains(neighbor) || !pathRenderer.IsMoveLegal(currentNode, neighbor))
                {
                    continue;
                }

                float tentativeGScore = gScore[currentNode] + Vector2.Distance(currentNode.Position, neighbor.Position);
                if (tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = currentNode;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Vector2.Distance(neighbor.Position, goalNode.Position);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node currentNode)
    {
        List<Node> totalPath = new List<Node> { currentNode };
        while (cameFrom.ContainsKey(currentNode))
        {
            currentNode = cameFrom[currentNode];
            totalPath.Add(currentNode);
        }
        totalPath.Reverse();
        return totalPath;
    }
}
