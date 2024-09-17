using UnityEngine;
using System.Collections.Generic;  // Dodaj to, jeœli nie jest dodane

public class BallMovement : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private GridManager gridManager;  // Referencja do GridManager
    public GameObject ball;
    public float gridSize = 1f;
    public Vector2Int arenaSize = new Vector2Int(8, 10);
    public int goalWidth = 2;
    [SerializeField]
    public Vector3 lastPosition;
    [SerializeField]
    public Vector3 targetPosition;
    private bool isMoving = false;

    [SerializeField]
    private BallPathRenderer pathRenderer;

    private void Start()
    {
        targetPosition = ball.transform.position;
        lastPosition = ball.transform.position;
    }

    private void Update()
    {
        if (!gameController.isGameStarted) return;
        HandleBallMovement();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Vector3 currentPosition = ball.transform.position;

            if (IsValidMove(currentPosition, targetPosition))
            {
                pathRenderer.AddPosition(lastPosition, currentPosition, Color.blue);
                lastPosition = currentPosition;
            }
            else
            {
                ball.transform.position = lastPosition;
                targetPosition = lastPosition;
                isMoving = false;
            }
        }
    }

    private Vector3 GetClosestNodePosition(Vector3 position)
    {
        Node closestNode = null;
        float closestDistance = Mathf.Infinity;

        foreach (var node in gridManager.GetAllNodes())
        {
            float distance = Vector3.Distance(position, node.Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode != null ? closestNode.Position : position;
    }

    private Vector3 GetClosestGoalNodePosition(Vector3 position)
    {
        Node closestGoalNode = null;
        float closestDistance = Mathf.Infinity;

        foreach (var node in gridManager.GetGoalNodes())
        {
            float distance = Vector3.Distance(position, node.Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGoalNode = node;
            }
        }

        return closestGoalNode != null ? closestGoalNode.Position : position;
    }

    private void HandleBallMovement()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) // Ruch w górê
        {
            SetTargetPosition(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) // Ruch w dó³
        {
            SetTargetPosition(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) // Ruch w lewo
        {
            SetTargetPosition(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) // Ruch w prawo
        {
            SetTargetPosition(Vector3.right);
        }

        if (isMoving)
        {
            MoveBall();
        }
    }

    public void SetTargetPosition(Vector3 direction)
    {
        Vector3 newTarget = ball.transform.position + direction * gridSize;
        targetPosition = GetClosestNodePosition(new Vector3(
            Mathf.Round(newTarget.x / gridSize) * gridSize,
            Mathf.Round(newTarget.y / gridSize) * gridSize,
            ball.transform.position.z
        ));

        if (IsWithinArena(targetPosition) || IsGoalPosition(targetPosition))
        {
            isMoving = true;
        }
    }

    private bool IsGoalPosition(Vector3 position)
    {
        foreach (var goalNode in gridManager.GetGoalNodes())
        {
            if (Vector3.Distance(position, goalNode.Position) <= gridSize)
            {
                return true;
            }
        }
        return false;
    }

    public void MoveBall()
    {
        ball.transform.position = Vector3.MoveTowards(ball.transform.position, targetPosition, gridSize);
        if (Vector3.Distance(ball.transform.position, targetPosition) < 0.01f)
        {
            ball.transform.position = targetPosition;
            isMoving = false;
            // Odtwórz dŸwiêk ruchu
            // ballAudioSource.PlayOneShot(moveSound);
            // SprawdŸ, czy pi³ka trafi³a do bramki
            // CheckGoal();
        }
    }

    public bool IsWithinArena(Vector3 position)
    {
        float arenaLeft = -arenaSize.x / 2f * gridSize;
        float arenaRight = arenaSize.x / 2f * gridSize;
        float arenaBottom = -arenaSize.y / 2f * gridSize;
        float arenaTop = arenaSize.y / 2f * gridSize;

        bool withinVerticalBounds = position.y > arenaBottom - gridSize && position.y < arenaTop + gridSize;
        bool withinHorizontalBounds = position.x >= arenaLeft && position.x <= arenaRight;

        bool withinTopGoal = position.y >= arenaTop && (position.x >= arenaLeft + (gridSize * (arenaSize.x / 2 - goalWidth / 2)) && position.x <= arenaRight - (gridSize * (arenaSize.x / 2 - goalWidth / 2)));
        bool withinBottomGoal = position.y <= arenaBottom && (position.x >= arenaLeft + (gridSize * (arenaSize.x / 2 - goalWidth / 2)) && position.x <= arenaRight - (gridSize * (arenaSize.x / 2 - goalWidth / 2)));

        return (withinVerticalBounds && withinHorizontalBounds) || withinTopGoal || withinBottomGoal;
    }

    private bool IsValidMove(Vector3 currentPosition, Vector3 targetPosition)
    {
        // SprawdŸ, czy targetPosition jest w pobli¿u najbli¿szego wêz³a lub bramki
        Vector3 closestNode = GetClosestNodePosition(targetPosition);
        Vector3 closestGoalNode = GetClosestGoalNodePosition(targetPosition);

        // Mo¿na dostosowaæ tolerancjê, np. `gridSize * Mathf.Sqrt(2)` dla wêz³ów i bramek
        bool isNearNode = Vector3.Distance(targetPosition, closestNode) <= gridSize * 0.5f;
        bool isNearGoalNode = Vector3.Distance(targetPosition, closestGoalNode) <= gridSize * 0.5f;

        return isNearNode || isNearGoalNode;
    }
}