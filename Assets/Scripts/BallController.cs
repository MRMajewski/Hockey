using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject ball; // Odniesienie do obiektu pi³ki
    public float gridSize = 1f; // Rozmiar kratki (1x1)
    public Vector2Int arenaSize = new Vector2Int(8, 10); // Rozmiar planszy bez bramek
    public int goalWidth = 2; // Szerokoœæ bramki (2 kratki)
  
    private bool isMoving = false;

    [SerializeField]
    private BallPathRenderer pathRenderer;

    [SerializeField]
    private Vector3 currentPosition;
    [SerializeField]
    private Vector3 lastPosition;
    [SerializeField]
    private Vector3 targetPosition;

    private void Start()
    {
        // Ustawienie pocz¹tkowej pozycji celu na obecn¹ pozycjê pi³ki
        targetPosition = ball.transform.position;
        lastPosition = transform.position;
    }

    private void Update()
    {
        HandleBallMovement();
   //   currentPosition = transform.position;

    }

    private void HandleBallMovement()
    {
        lastPosition = ball.transform.position;
        // Sprawdzanie wejœcia od u¿ytkownika i ustawienie kierunku
        if (Input.GetKeyDown(KeyCode.W)) // Ruch w górê
        {
            SetTargetPosition(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.S)) // Ruch w dó³
        {
            SetTargetPosition(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.A)) // Ruch w lewo
        {
            SetTargetPosition(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D)) // Ruch w prawo
        {
            SetTargetPosition(Vector3.right);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) // Ruch w górê i w prawo
        {
            SetTargetPosition(new Vector3(1, 1, 0).normalized);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) // Ruch w górê i w lewo
        {
            SetTargetPosition(new Vector3(-1, 1, 0).normalized);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)) // Ruch w dó³ i w prawo
        {
            SetTargetPosition(new Vector3(1, -1, 0).normalized);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) // Ruch w dó³ i w lewo
        {
            SetTargetPosition(new Vector3(-1, -1, 0).normalized);
        }

        // Poruszanie pi³ki w kierunku celu
        if (isMoving)
        {
            MoveBall();
        }
    }

    private void SetTargetPosition(Vector3 direction)
    {
        // Ustawienie celu na najbli¿sze przeciêcie kratki w danym kierunku
        Vector3 newTarget = ball.transform.position + direction * gridSize;
        targetPosition = new Vector3(
            Mathf.Round(newTarget.x / gridSize) * gridSize,
            Mathf.Round(newTarget.y / gridSize) * gridSize,
            ball.transform.position.z
        );

        // Sprawdzenie czy nowa pozycja mieœci siê w obszarze gry
        if (IsWithinArena(targetPosition))
        {
            isMoving = true;
        }
    }

    private void MoveBall()
    {
        // Natychmiastowe przesuniêcie pi³ki do celu
        ball.transform.position = targetPosition;


        //currentPosition = targetPosition;
        ////    if (currentPosition != lastPosition)
        //if (currentPosition != targetPosition)
        //{
        // Powiadom BallPathRenderer o zmianie pozycji

     //   pathRenderer.AddPosition(lastPosition, targetPosition);
       // pathRenderer.AddPosition(lastPosition, currentPosition);
            lastPosition = currentPosition;
            lastPosition = currentPosition;
      //  }


        isMoving = false;
    }

    private bool IsWithinArena(Vector3 position)
    {
        // Sprawdzenie, czy pozycja mieœci siê w zakresie kratek, uwzglêdniaj¹c bramki
        float arenaLeft = -arenaSize.x / 2f * gridSize;
        float arenaRight = arenaSize.x / 2f * gridSize;
        float arenaBottom = -arenaSize.y / 2f * gridSize;
        float arenaTop = arenaSize.y / 2f * gridSize;

        // Uwzglêdnianie bramek: Sprawdzenie, czy pozycja jest w obrêbie kratek
        bool withinVerticalBounds = position.y >= arenaBottom && position.y <= arenaTop;
        bool withinHorizontalBounds = position.x >= arenaLeft && position.x <= arenaRight;

        // Sprawdzenie, czy pozycja jest w obrêbie bramek
        bool withinTopGoal = position.y >= arenaTop && (position.x >= arenaLeft + (gridSize * (arenaSize.x / 2 - goalWidth / 2)) && position.x <= arenaRight - (gridSize * (arenaSize.x / 2 - goalWidth / 2)));
        bool withinBottomGoal = position.y <= arenaBottom && (position.x >= arenaLeft + (gridSize * (arenaSize.x / 2 - goalWidth / 2)) && position.x <= arenaRight - (gridSize * (arenaSize.x / 2 - goalWidth / 2)));

        return (withinVerticalBounds && withinHorizontalBounds) || withinTopGoal || withinBottomGoal;
    }
}