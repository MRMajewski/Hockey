using UnityEngine;
using System.Collections.Generic;
using System;

public class GreedyAlgorithm : IAIAlgorithm
{

    private BallPathRenderer pathRenderer;
    public GreedyAlgorithm(BallPathRenderer pathRenderer)
    {
        this.pathRenderer = pathRenderer;
    }

    public Node GetBestMove(Node currentNode, Node goalNode)
    {
        List<Node> neighbors = new List<Node>(currentNode.GetNeighbors());
        Node bestNode = null;
        float shortestDistance = float.MaxValue;

        foreach (Node neighbor in neighbors)
        {
            if (pathRenderer.IsMoveLegal(currentNode, neighbor))
            {
                float distance = Vector2.Distance(neighbor.Position, goalNode.Position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    bestNode = neighbor;
                }
            }
        }

        return bestNode;
    }
}
