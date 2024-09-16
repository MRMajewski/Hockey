using UnityEngine;

public class AIController : MonoBehaviour
{
    public BallMovement ballMovement;
    public TurnManager turnManager;

    [SerializeField]
    private Vector3 targetPosition;

    [SerializeField]
    private Vector3 newTargetPosition;

    private void Start()
    {
        SetTargetPositionForAI();
    }

    public void ResetAI()
    {
        newTargetPosition = Vector3.zero;
    }

    public void PerformAITurn()
    {
        Vector3 direction = (targetPosition - ballMovement.ball.transform.position).normalized;
        newTargetPosition = ballMovement.ball.transform.position + direction * ballMovement.gridSize;

        newTargetPosition = new Vector3(
            Mathf.Round(newTargetPosition.x / ballMovement.gridSize) * ballMovement.gridSize,
            Mathf.Round(newTargetPosition.y / ballMovement.gridSize) * ballMovement.gridSize,
            ballMovement.ball.transform.position.z
        );

        if (ballMovement.IsWithinArena(newTargetPosition))
        {
            turnManager.pathRenderer.AddPosition(ballMovement.ball.transform.position, newTargetPosition, Color.red);
            ballMovement.SetTargetPosition(newTargetPosition - ballMovement.ball.transform.position);
            ballMovement.MoveBall();
            Debug.Log($"AI Move: {ballMovement.ball.transform.position} to {newTargetPosition}");
        }
        else
        {
            Debug.Log("AI move out of bounds.");
        }

        turnManager.isPlayerTurn = true;
    }

    public void SetTargetPositionForAI()
    {
        float arenaBottom = -ballMovement.arenaSize.y / 2f * ballMovement.gridSize;
        targetPosition = new Vector3(0, arenaBottom, ballMovement.ball.transform.position.z);

        ballMovement.ball.transform.position = Vector3.zero;
    }
}