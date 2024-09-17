using UnityEngine;

public class AIController : MonoBehaviour
{
    public BallMovement ballMovement;
    public TurnManager turnManager;

    [SerializeField]
    private Vector3 targetPosition;

    private void Start()
    {
        SetTargetPositionForAI();
    }

    public void ResetAI()
    {
        ballMovement.ball.transform.position = Vector3.zero;
        SetTargetPositionForAI();
    }

    public void PerformAITurn()
    {
        // Oblicz kierunek do celu
        Vector3 direction = (targetPosition - ballMovement.ball.transform.position).normalized;
        // Oblicz now� pozycj�
        Vector3 newTargetPosition = ballMovement.ball.transform.position + direction * ballMovement.gridSize;
        // Zaokr�glij do najbli�szego pola siatki
        newTargetPosition = new Vector3(
            Mathf.Round(newTargetPosition.x / ballMovement.gridSize) * ballMovement.gridSize,
            Mathf.Round(newTargetPosition.y / ballMovement.gridSize) * ballMovement.gridSize,
            ballMovement.ball.transform.position.z
        );

        // Sprawd�, czy nowa pozycja jest w obr�bie areny
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
        // Oblicz pozycj� celu na podstawie bramki przeciwnika
        float arenaBottom = -ballMovement.arenaSize.y / 2f * ballMovement.gridSize;
        targetPosition = new Vector3(0, arenaBottom, ballMovement.ball.transform.position.z);

        // Ustaw pi�k� na �rodku areny
        ballMovement.ball.transform.position = Vector3.zero;
    }
}