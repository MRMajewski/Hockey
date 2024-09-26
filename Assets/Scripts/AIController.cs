using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;

    [Header("Node references")]
    [SerializeField]
    private Node targetNode; 
    [SerializeField]
    private Node currentNode; 
    [SerializeField]
    private Node goalNode;  

    public Node GoalNode { get => goalNode; }

    private const float DiagonalMoveDistance = 1.414f;  
    private const float StraightMoveDistance = 1f;  

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
            this.aiAlgorithm = new GreedyAlgorithm(gameController.PathRenderer);
            SetBottomGoalOnly();
        }
        else if (algorithmType == AIAlgorithmType.Greedy)
        {
            this.aiAlgorithm = new GreedyAlgorithm(gameController.PathRenderer);
            SetRandomGoalNode();
        }
        else if (algorithmType == AIAlgorithmType.AStar)
        {
            this.aiAlgorithm = new AStarAlgorithm(gameController.GridManager, gameController.PathRenderer);
            SetGoalNodeBasedOnSituation();
        }
        else if (algorithmType == AIAlgorithmType.MinMax)
        {
            this.aiAlgorithm = new MinimaxAlgorithm(gameController.PathRenderer);
            SetGoalNodeBasedOnSituation();
        }
    }

    public void InitAI()
    {
        SetAIAlgorithm(algorithmType);
    }


    public void PerformAITurn()
    {
        currentNode = gameController.BallMovement.GetConfirmedNode();

        if (currentNode == null || goalNode == null)
        {
            return;
        }

        bool moveSuccessful = false;

        while (!moveSuccessful)
        {
            List<Node> neighbors = new List<Node>(currentNode.GetNeighbors());
            Node bestNode = aiAlgorithm.GetBestMove(currentNode, goalNode);

            if (bestNode != null)
            {
                Vector2 direction = bestNode.Position - currentNode.Position;

                float moveDistance = (Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0) ? DiagonalMoveDistance : StraightMoveDistance;

                Vector2 newPosition = currentNode.Position + direction.normalized * moveDistance;

                moveSuccessful = gameController.BallMovement.TryMoveToNode(newPosition - gameController.BallMovement.GetTargetNode().Position);

                if (moveSuccessful)
                {
                    gameController.PathRenderer.AddPosition(ref currentNode, ref bestNode, Color.red);
                    Debug.Log($"AI Move: {currentNode.Position} to {newPosition}");
                    gameController.BallMovement.SetConfirmedNode(ref bestNode);

                    if (gameController.PathRenderer.WasNodeAlreadyUsed(bestNode))
                    {
                        continue;  
                    }
                }
                else
                {
                    SwitchGoalNode();
                }
            }
            else
            {
                gameController.PlayerWinsDueToNoAiMoves();
                return;
            }
        }
        gameController.TurnManager.IsPlayerTurn = false;
        gameController.CheckIfGameEnded(ref currentNode);
    }

    public void SetGoalNodeForAI()
    {
        goalNode = gameController.BallMovement.GetNodeAtPosition(gameController.GridManager.GoalNodes[1].Position);
        Debug.Log($"AI GoalNode set to: {goalNode.Position}");
    }

    public void SetBottomGoalOnly()
    {
        goalNode = gameController.BallMovement.GetNodeAtPosition(gameController.GridManager.GoalNodes[1].Position);
        Debug.Log($"AI GoalNode set to: {goalNode.Position}");
    }
    private void SwitchGoalNode()
    {
        Node previousGoal = goalNode;

        if (goalNode == gameController.BallMovement.GetNodeAtPosition(gameController.GridManager.GoalNodes[1].Position))
        {
            goalNode = gameController.BallMovement.GetNodeAtPosition(gameController.GridManager.GoalNodes[0].Position);
        }
        else
        {
            goalNode = gameController.BallMovement.GetNodeAtPosition(gameController.GridManager.GoalNodes[1].Position);
        }
        Debug.Log($"AI GoalNode zmieniono z {previousGoal.Position} na {goalNode.Position}");
    }

    public void SetRandomGoalNode()
    {
        int randomGoalIndex = Random.Range(0, 2);

        goalNode = gameController.BallMovement.GetNodeAtPosition(gameController.GridManager.GoalNodes[randomGoalIndex].Position);
        Debug.Log($"AI GoalNode set to: {goalNode.Position}");
    }

    public void SetGoalNodeBasedOnSituation()
    {
        Node goalNodeTop = gameController.BallMovement.GetNodeAtPosition(gameController.GridManager.GoalNodes[0].Position);
        Node goalNodeBottom = gameController.BallMovement.GetNodeAtPosition(gameController.GridManager.GoalNodes[1].Position);
        float distanceToGoal1 = Vector2.Distance(currentNode.Position, goalNodeTop.Position);
        float distanceToGoal2 = Vector2.Distance(currentNode.Position, goalNodeBottom.Position);

        goalNode = distanceToGoal1 < distanceToGoal2 ? goalNodeTop : goalNodeBottom;
        Debug.Log($"AI GoalNode changed based on distance: {goalNode.Position}");
    }
}
