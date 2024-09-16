using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
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

    private void HandleBallMovement()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetTargetPosition(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SetTargetPosition(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            SetTargetPosition(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SetTargetPosition(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            SetTargetPosition(new Vector3(1, 1, 0).normalized);
        }
        else if (Input.GetKeyDown(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            SetTargetPosition(new Vector3(-1, 1, 0).normalized);
        }
        else if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            SetTargetPosition(new Vector3(1, -1, 0).normalized);
        }
        else if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            SetTargetPosition(new Vector3(-1, -1, 0).normalized);
        }

        if (isMoving)
        {
            MoveBall();
        }
    }

    public void SetTargetPosition(Vector3 direction)
    {
        Vector3 newTarget = ball.transform.position + direction * gridSize;
        targetPosition = new Vector3(
            Mathf.Round(newTarget.x / gridSize) * gridSize,
            Mathf.Round(newTarget.y / gridSize) * gridSize,
            ball.transform.position.z
        );

        if (IsWithinArena(targetPosition))
        {
            isMoving = true;
        }
    }

    public void MoveBall()
    {
        ball.transform.position = targetPosition;
        isMoving = false;
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
        return Vector3.Distance(currentPosition, targetPosition) <= gridSize * Mathf.Sqrt(2);
    }
}