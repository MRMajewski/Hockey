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
    [SerializeField]
    private UIManager uIManager;

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

    [SerializeField]
    private AIAlgorithmType algorithmType = AIAlgorithmType.Simple;
    public enum AIAlgorithmType
    {
        Simple,
        Greedy,
        AStar, 
        MinMax
    }

    public void SetAIAlgorithmFromButton(int index)
    {
        algorithmType = (AIAlgorithmType)index;     
    }

    public void SetAIAlgorithm(AIAlgorithmType algorithmType)
    {
        if (algorithmType == AIAlgorithmType.Simple)
        {
            this.aiAlgorithm = new GreedyAlgorithm(pathRenderer);
            SetBottomGoalOnly();
        }
        else if (algorithmType == AIAlgorithmType.Greedy)
        {
            this.aiAlgorithm = new GreedyAlgorithm(pathRenderer);
            SetRandomGoalNode();
        }
        else if (algorithmType == AIAlgorithmType.AStar)
        {
            this.aiAlgorithm = new AStarAlgorithm(gridManager, pathRenderer);
            SetGoalNodeBasedOnSituation();
        }
        else if (algorithmType == AIAlgorithmType.MinMax)
        {
            this.aiAlgorithm = new MinimaxAlgorithm(pathRenderer);
            SetGoalNodeBasedOnSituation();
        }
        uIManager.DisplayAITypeInfo(algorithmType);
    }

    public void InitAI()
    {
        SetAIAlgorithm(algorithmType);
        uIManager.DisplayAITypeInfo(algorithmType);
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


    public void SetGoalNodeForAI()
    {
        goalNode = ballMovement.GetNodeAtPosition(gridManager.GoalNodes[1].Position);
        Debug.Log($"AI GoalNode set to: {goalNode.Position}");
    }

    public void SetBottomGoalOnly()
    {
        goalNode = ballMovement.GetNodeAtPosition(gridManager.GoalNodes[1].Position);
        Debug.Log($"AI GoalNode set to: {goalNode.Position}");
    }

    public void SetRandomGoalNode()
    {
        int randomGoalIndex = Random.Range(0, 2);

        goalNode = ballMovement.GetNodeAtPosition(gridManager.GoalNodes[randomGoalIndex].Position);
        Debug.Log($"AI GoalNode set to: {goalNode.Position}");
    }

    public void SetGoalNodeBasedOnSituation()
    {
        Node goalNodeTop = ballMovement.GetNodeAtPosition(gridManager.GoalNodes[0].Position);
        Node goalNodeBottom = ballMovement.GetNodeAtPosition(gridManager.GoalNodes[1].Position);
        float distanceToGoal1 = Vector2.Distance(currentNode.Position, goalNodeTop.Position);
        float distanceToGoal2 = Vector2.Distance(currentNode.Position, goalNodeBottom.Position);

        // Wybierz bramkę, która jest bliżej
        goalNode = distanceToGoal1 < distanceToGoal2 ? goalNodeTop : goalNodeBottom;
        Debug.Log($"AI GoalNode changed based on distance: {goalNode.Position}");
    }
}
