using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public BallMovement ballMovement;
    public TurnManager turnManager;

    [SerializeField]
    private BallPathRenderer pathRenderer;

    [SerializeField]
    private Node targetNode;  // Docelowy węzeł AI
    [SerializeField]
    private Node currentNode;  // Obecny węzeł AI
    [SerializeField]
    private Node goalNode;     // Cel AI, węzeł do którego AI zmierza

    private const float DiagonalMoveDistance = 1.414f;  // √2 dla ruchu po skosie
    private const float StraightMoveDistance = 1f;  // Ruch po osi x/y

    public void PerformAITurn()
    {
        currentNode = ballMovement.GetCurrentNode();


        if (currentNode == null || goalNode == null)
        {
            Debug.LogWarning("AI nie ma ustawionych węzłów.");
            return;
        }

        // Znajdź sąsiadów bieżącego węzła
        List<Node> neighbors = new List<Node>(currentNode.GetNeighbors());

        // Filtruj tylko te, które są dozwolone w kierunku celu
        Node bestNode = null;
        float shortestDistance = float.MaxValue;
        bool foundLegalMove = false;  // Flaga do sprawdzenia, czy AI znalazło legalny ruch

        foreach (Node neighbor in neighbors)
        {
            // Sprawdź, czy przejście do sąsiada nie jest blokowane
            if (pathRenderer.IsMoveLegal(currentNode, neighbor))
            {
                foundLegalMove = true;  // Zaznacz, że AI znalazło przynajmniej jeden legalny ruch

                // Oblicz odległość od sąsiada do celu (goalNode)
                float distance = Vector2.Distance(neighbor.Position, goalNode.Position);
                Debug.Log($"Sprawdzanie węzła sąsiada: {neighbor.Position}, odległość: {distance}");

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

            pathRenderer.AddPosition(currentNode, bestNode, Color.red);
            bool moveSuccessful = ballMovement.TryMoveToNode(newPosition - ballMovement.GetCurrentNode().Position);

            if (moveSuccessful)
            {
                ballMovement.MoveBall();
                Debug.Log($"AI Move: {ballMovement.ball.transform.position} to {newPosition}");

                // Zaktualizuj bieżący węzeł
                ballMovement.SetCurrentNode(bestNode);
            }
            else
            {
                Debug.LogWarning("AI nie mogło wykonać ruchu.");
            }
        }
        else if (!foundLegalMove)
        {
            // Jeśli nie znaleziono żadnych legalnych ruchów
            Debug.LogError("Brak dostępnych legalnych ruchów dla AI. AI czeka na następną turę.");
        }

        // Zakończ turę AI i ustaw, że jest tura gracza
        turnManager.isPlayerTurn = true;
    }

    // Ustawia cel AI, czyli węzeł bramki przeciwnika
    public void SetGoalNodeForAI()
    {
        float arenaBottom = -ballMovement.arenaSize.y / 2f * ballMovement.gridSize;
        goalNode = ballMovement.GetNodeAtPosition(new Vector2(0, arenaBottom));

        Debug.Log($"AI GoalNode set to: {goalNode.Position}");
    }
}
