using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField]
    private BallMovement ballMovement;
    [SerializeField]
    private TurnManager turnManager;
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private BallPathRenderer pathRenderer;

    [Header("Node references")]
    [SerializeField]
    private Node targetNode; 
    [SerializeField]
    private Node currentNode; 
    [SerializeField]
    private Node goalNode;  

    public Node GoalNode { get => goalNode; }

    private const float DiagonalMoveDistance = 1.414f;  // √2 dla ruchu po skosie
    private const float StraightMoveDistance = 1f;  // Ruch po osi x/y

    public void PerformAITurn()
    {
        currentNode = ballMovement.GetConfirmedNode();

        if (currentNode == null || goalNode == null)
        {
            Debug.LogWarning("AI nie ma ustawionych węzłów.");
            return;
        }

        List<Node> neighbors = new List<Node>(currentNode.GetNeighbors());

        Node bestNode = null;
        float shortestDistance = float.MaxValue;

        foreach (Node neighbor in neighbors)
        {
            // Sprawdź, czy przejście do sąsiada nie jest blokowane
            if (pathRenderer.IsMoveLegal(currentNode, neighbor))
            {

                float distance = Vector2.Distance(neighbor.Position, goalNode.Position);

                // Znajdź najlepszego sąsiada (o najkrótszej odległości do celu)
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    bestNode = neighbor;
                }
            }
        }

        // Wykonaj ruch do najlepszego węzła
        if (bestNode != null)
        {
            Vector2 direction = bestNode.Position - currentNode.Position;

            // Sprawdź, czy ruch jest po skosie (zmiana na osi x i y jednocześnie)
            float moveDistance = (Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0) ? DiagonalMoveDistance : StraightMoveDistance;

            // Przeskaluj wektor ruchu na odpowiednią odległość
            Vector2 newPosition = currentNode.Position + direction.normalized * moveDistance;

            pathRenderer.AddPosition(ref currentNode, ref bestNode, Color.red);
            bool moveSuccessful = ballMovement.TryMoveToNode(newPosition - ballMovement.GetTargetNode().Position);

            if (moveSuccessful)
            {
                Debug.Log($"AI Move: {currentNode.Position} to {newPosition}");
                ballMovement.SetConfirmedNode(ref bestNode);
            }
            else
            {
                gameController.PlayerWinsDueToNoAiMoves();
                Debug.LogWarning("AI nie mogło wykonać ruchu.");
                return;
            }
        }
        else
        {
            gameController.PlayerWinsDueToNoAiMoves();
            // Jeśli nie znaleziono żadnych legalnych ruchów
            Debug.LogError("Brak dostępnych legalnych ruchów dla AI. AI czeka na następną turę.");
            return;
        }
        turnManager.IsPlayerTurn = false;
        gameController.CheckIfGameEnded(ref bestNode);
    }

    // Ustawia cel AI, czyli węzeł bramki przeciwnika
    public void SetGoalNodeForAI()
    {
        float arenaBottom = -ballMovement.arenaSize.y / 2f * ballMovement.gridSize;
        goalNode = ballMovement.GetNodeAtPosition(new Vector2(0, arenaBottom));

        Debug.Log($"AI GoalNode set to: {goalNode.Position}");
    }
}
