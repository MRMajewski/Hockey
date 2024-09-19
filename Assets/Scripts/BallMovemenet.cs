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
    //[SerializeField]
    //public Vector2 lastPosition;
    //[SerializeField]
    //public Vector2 targetPosition;
    private bool isMoving = false;

    [SerializeField]
    private BallPathRenderer pathRenderer;


    public Node currentTemporaryNode; // Węzeł tymczasowy, podczas ruchu
    public Node confirmedNode; // Ostateczny węzeł po zatwierdzeniu ruchu


    public Vector2 temporaryBallPos = Vector2.zero;
    //[SerializeField]
    //private Node currentNode;
    //[SerializeField]
    //private Node targetNode;
    //[SerializeField]
    //private Node lastNode;

    public void BallInit()
    {
        confirmedNode = gridManager.GetNodeAtPosition(Vector2.zero); // Początkowy węzeł piłki
        currentTemporaryNode = confirmedNode;
        //currentNode = gridManager.GetNodeAtPosition(ball.transform.position);
        //targetPosition = currentNode.Position;
        //lastNode = currentNode;
    }

    private void Update()
    {
        if (!gameController.isGameStarted) return;

        if (Input.anyKeyDown)
        {
            HandleBallMovement();
            Debug.Log("currentTemporaryNode Position Przed MoveBall" + currentTemporaryNode.Position);
            MoveBall();
            Debug.Log("currentTemporaryNode Position Po MoveBall" + currentTemporaryNode.Position);


        }
    }

  

    private bool HandleBallMovement()
    {
        bool didMove = false;

        currentTemporaryNode = confirmedNode;
        //  lastNode = currentNode;
        Vector2 direction = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) // Ruch w górę
        {
            direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) // Ruch w dół
        {
            direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) // Ruch w lewo
        {
            direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) // Ruch w prawo
        {
            direction =Vector2.right;
        }
        else if (Input.GetKeyDown(KeyCode.Q)) // Ruch po skosie w lewo-górę
        {
            direction = new Vector2(-1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.E)) // Ruch po skosie w prawo-górę
        {
            direction = new Vector2(1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Z)) // Ruch po skosie w lewo-dół
        {
            direction = new Vector2(-1, -1);
        }
        else if (Input.GetKeyDown(KeyCode.C)) // Ruch po skosie w prawo-dół
        {
            direction = new Vector2(1, -1);
        }

        if (direction != Vector2.zero)
        {
            didMove = TryMoveToNode(direction);
        }

        return didMove; // Zwracamy, czy ruch się udał
    }

    public bool TryMoveToNode(Vector2 direction)
    {
        bool isDiagonalMove = Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0;

        float distanceMultiplier = isDiagonalMove ? Mathf.Sqrt(2) : 1f;

        Vector2 newPosition = confirmedNode.Position + direction * gridSize;
        Node targetNode = gridManager.GetNodeAtPosition(newPosition);

        Debug.Log("currentTemporaryNode Position" + currentTemporaryNode.Position);
        temporaryBallPos = newPosition;
        // Sprawdzenie, czy docelowy węzeł jest sąsiadem
        if (confirmedNode.IsNeighbor(targetNode))
        {
            Debug.Log("currentTemporaryNode Position" + currentTemporaryNode.Position);
            currentTemporaryNode = targetNode;
            Debug.Log("currentTemporaryNode Position" + currentTemporaryNode.Position);

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
        ball.transform.position = currentTemporaryNode.Position;
        Debug.Log("currentTemporaryNode Position w MoveBall" + currentTemporaryNode.Position);

    }
    public void SetConfirmedNode(ref Node node)
    {
        confirmedNode = node;
        ball.transform.position = node.Position; // Ustawiamy piłkę w ostatecznym węźle
        currentTemporaryNode = node; // Synchronizujemy tymczasowy węzeł z zatwierdzonym
    }

    public void ResetToLastConfirmedNode()
    {
        ball.transform.position = confirmedNode.Position;
        currentTemporaryNode = confirmedNode; // Resetujemy tymczasową pozycję do zatwierdzonej
    }


    public ref Node GetTargetNode()
    {
        currentTemporaryNode = gridManager.GetNodeAtPosition(temporaryBallPos);
        Debug.Log("GetTargetNode() " + currentTemporaryNode);
        return  ref currentTemporaryNode;
    }
    public ref Node GetConfirmedNode()
    {
        return ref confirmedNode;
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
