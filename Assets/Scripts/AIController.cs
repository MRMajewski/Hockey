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
    [SerializeField]
    private GridManager gridManager;


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

    private IAIAlgorithm aiAlgorithm;

    private AIAlgorithmType algorithmType;
    public enum AIAlgorithmType
    {
        Greedy,
        AStar
    }

    public void SetAIAlgorithmFromButton(int index)
    {
        algorithmType = (AIAlgorithmType)index;

        if(algorithmType== AIAlgorithmType.Greedy)
        {
            SetAIAlgorithm(new GreedyAlgorithm(pathRenderer));
        }
        else if(algorithmType == AIAlgorithmType.AStar)
        {
            SetAIAlgorithm(new AStarAlgorithm(gridManager, pathRenderer));
        }
    }

    public void SetAIAlgorithm(IAIAlgorithm algorithm)
    {
        this.aiAlgorithm = algorithm;
    }

    public void Start()
    {
      SetAIAlgorithm(new AStarAlgorithm(gridManager, pathRenderer));
      //  SetAIAlgorithm(new GreedyAlgorithm(pathRenderer));
    }

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
  
        bestNode = aiAlgorithm.GetBestMove(currentNode, goalNode);

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

                if (pathRenderer.WasNodeAlreadyUsed(bestNode))
                {
                    Debug.Log("Gracz kończy ruch na używanym węźle - dodatkowy ruch!");
                    PerformAITurn();
                    return; 
                }
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
