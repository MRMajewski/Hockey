using System.Collections.Generic;
using UnityEngine;

public class MinimaxAlgorithm : IAIAlgorithm
{
    private const int Depth = 3; // G³êbokoœæ przeszukiwania
    private BallPathRenderer pathRenderer;

    public MinimaxAlgorithm(BallPathRenderer pathRenderer)
    {
        this.pathRenderer = pathRenderer;
    }

    //public  List<Node> FindPath(Node startNode, Node goalNode)
    //{
    //    return Minimax(startNode, goalNode, Depth, true).path;
    //}

    public Node GetBestMove(Node currentNode, Node goalNode)
    {
        var bestMove = Minimax(currentNode, goalNode, Depth, true);
        return bestMove.path.Count > 1 ? bestMove.path[1] : null; // Zwraca kolejny ruch
    }

    private (float score, List<Node> path) Minimax(Node currentNode, Node goalNode, int depth, bool isMaximizingPlayer)
    {
        if (depth == 0 || currentNode == goalNode)
        {
            return (EvaluateNode(currentNode, goalNode), new List<Node> { currentNode });
        }

        if (isMaximizingPlayer)
        {
            float maxEval = float.NegativeInfinity;
            List<Node> bestPath = null;

            foreach (Node neighbor in currentNode.GetNeighbors())
            {
                if (pathRenderer.IsMoveLegal(currentNode, neighbor)) // Sprawdzenie legalnoœci ruchu
                {
                    var result = Minimax(neighbor, goalNode, depth - 1, false);
                    float eval = result.score;
                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        bestPath = new List<Node>(result.path);
                    }
                }
            }
            bestPath.Insert(0, currentNode);
            return (maxEval, bestPath);
        }
        else
        {
            float minEval = float.PositiveInfinity;
            List<Node> bestPath = null;

            foreach (Node neighbor in currentNode.GetNeighbors())
            {
                if (pathRenderer.IsMoveLegal(currentNode, neighbor)) // Sprawdzenie legalnoœci ruchu
                {
                    var result = Minimax(neighbor, goalNode, depth - 1, true);
                    float eval = result.score;
                    if (eval < minEval)
                    {
                        minEval = eval;
                        bestPath = new List<Node>(result.path);
                    }
                }
            }
            bestPath.Insert(0, currentNode);
            return (minEval, bestPath);
        }
    }

    private float EvaluateNode(Node node, Node goalNode)
    {
        return Vector2.Distance(node.Position, goalNode.Position);
    }
}
