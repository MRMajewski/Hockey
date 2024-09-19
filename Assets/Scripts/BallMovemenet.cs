using UnityEngine;
using System.Collections.Generic;

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
    public Vector2 lastPosition;
    [SerializeField]
    public Vector2 targetPosition;
    private bool isMoving = false;

    [SerializeField]
    private BallPathRenderer pathRenderer;

    [SerializeField]
    private Node currentNode;
    [SerializeField]
    private Node targetNode;
    [SerializeField]
    private Node lastNode;

    public void BallInit()
    {
        currentNode = gridManager.GetNodeAtPosition(ball.transform.position);
        targetPosition = currentNode.Position;
        lastNode = currentNode;
    }

    private void Update()
    {
        if (!gameController.isGameStarted) return;

        if (Input.anyKeyDown)
        {
            if (HandleBallMovement()) // Sprawdzamy, czy udało się poruszyć
            {
                MoveBall(); // Jeżeli ruch jest możliwy, wykonujemy przesunięcie
            }
            else
                MoveBall(false);
        }
    }

  

    private bool HandleBallMovement()
    {
        bool didMove = false;

        lastNode = currentNode;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) // Ruch w górę
        {
            didMove = TryMoveToNode(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) // Ruch w dół
        {
            didMove = TryMoveToNode(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) // Ruch w lewo
        {
            didMove = TryMoveToNode(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) // Ruch w prawo
        {
            didMove = TryMoveToNode(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.Q)) // Ruch po skosie w lewo-górę
        {
            didMove = TryMoveToNode(new Vector2(-1, 1));
        }
        else if (Input.GetKeyDown(KeyCode.E)) // Ruch po skosie w prawo-górę
        {
            didMove = TryMoveToNode(new Vector2(1, 1));
        }
        else if (Input.GetKeyDown(KeyCode.Z)) // Ruch po skosie w lewo-dół
        {
            didMove = TryMoveToNode(new Vector2(-1, -1));
        }
        else if (Input.GetKeyDown(KeyCode.C)) // Ruch po skosie w prawo-dół
        {
            didMove = TryMoveToNode(new Vector2(1, -1));
        }

        return didMove; // Zwracamy, czy ruch się udał
    }

    public bool TryMoveToNode(Vector2 direction)
    {
        // Sprawdzenie, czy ruch jest po skosie
        bool isDiagonalMove = Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0;

        // Jeśli ruch po skosie, przemnóż przez √2
        float distanceMultiplier = isDiagonalMove ? Mathf.Sqrt(2) : 1f;

        // Oblicz nową pozycję z uwzględnieniem długości ruchu
    //    Vector2 newPosition = currentNode.Position + direction * gridSize * distanceMultiplier;

        Vector2 newPosition = currentNode.Position + direction * gridSize;
        Debug.Log("new Position " + newPosition);

        lastNode = currentNode;
        Debug.Log("nlastNode " + lastNode);

        Node targetNode = gridManager.GetNodeAtPosition(newPosition);

        Debug.Log("targetNode " + targetNode);

        // Sprawdzenie, czy docelowy węzeł jest sąsiadem
        if (currentNode.IsNeighbor(targetNode))
        {
            this.targetNode = targetNode; // Ustawienie docelowego węzła
            isMoving = true;
            return true; // Ruch udany
        }
        else
        {
            Debug.Log("Invalid move: Not a neighbor node.");
            return false; // Ruch nieudany
        }
    }
    public void MoveBall(bool isMoveLegal = true)
    {
        if (isMoveLegal)
        {
            // Ustawiamy pozycję piłki na pozycję docelowego węzła
            ball.transform.position = targetNode.Position;

            // Ustawiamy nowy aktualny węzeł



           // currentNode = targetNode;



            Debug.Log($"MoveBall: {currentNode.Position}"); // Debugowanie
        }
        else
        {
          //  ball.transform.position = currentNode.Position;
            ball.transform.position = lastNode.Position;
            Debug.Log($"MoveBall (illegal): {currentNode.Position}"); // Debugowanie
        }
        isMoving = false;
        Debug.Log("Current node Pos " + currentNode.Position);
    }


    public Node GetTargetNode()
    {
        return targetNode;
    }
    public Node GetCurrentNode()
    {
      //  Vector2 currentNodePosition = GetClosestNodePosition((Vector2)ball.transform.position);
     //   currentNode = gridManager.GetNodeAtPosition(currentNodePosition);
        return currentNode;
    }

    public void SetCurrentNode(Node node)
    {
        currentNode = node;
        targetPosition = node.Position;
    }

    public Node GetLastNode()
    {
        return lastNode;
    }
    private Vector2 GetClosestNodePosition(Vector2 position)
    {
        Node closestNode = null;
        float closestDistance = Mathf.Infinity;

        foreach (var node in gridManager.GetAllNodes())
        {
            float distance = Vector2.Distance(position, node.Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode != null ? closestNode.Position : position;
    }
    public bool IsNeighborNode(Vector2 targetPosition)
    {
        Vector2 currentNodePosition = GetClosestNodePosition((Vector2)ball.transform.position);
        Node currentNode = gridManager.GetNodeAtPosition(currentNodePosition);

        if (currentNode != null)
        {
            List<Node> neighbors = currentNode.GetNeighbors();

            foreach (var neighbor in neighbors)
            {
                if (Vector2.Distance(targetPosition, neighbor.Position) <= gridSize * 0.5f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Node GetNodeAtPosition(Vector2 position)
    {
        Node closestNode = null;
        float closestDistance = Mathf.Infinity;

        foreach (Node node in gridManager.GetAllNodes())
        {
            float distance = Vector2.Distance(position, node.Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }
        return closestNode;
    }



}
