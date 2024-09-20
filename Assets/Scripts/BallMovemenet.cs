using UnityEngine;
using System.Collections.Generic;

public class BallMovement : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private GridManager gridManager;
    public GameObject ball;
    public float gridSize = 1f;
    public Vector2Int arenaSize = new Vector2Int(8, 10);
    public int goalWidth = 2;

    [SerializeField]
    private BallPathRenderer pathRenderer;

    private Node currentTemporaryNode;
    private Node confirmedNode;

    public Vector2 temporaryBallPos = Vector2.zero;

    public void BallInit()
    {
        confirmedNode = gridManager.GetNodeAtPosition(Vector2.zero);
        currentTemporaryNode = confirmedNode;
    }

    private void Update()
    {
        if (!gameController.isGameStarted) return;

        if (Input.anyKeyDown)
        {
            HandleBallMovement();
            ball.transform.position = currentTemporaryNode.Position;
        }
    }
    private bool HandleBallMovement()
    {
        bool didMove = false;

        currentTemporaryNode = confirmedNode;
        Vector2 direction = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector2.right;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            direction = new Vector2(-1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            direction = new Vector2(1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            direction = new Vector2(-1, -1);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            direction = new Vector2(1, -1);
        }
        if (direction != Vector2.zero)
        {
            didMove = TryMoveToNode(direction);
        }
        return didMove;
    }

    public bool TryMoveToNode(Vector2 direction)
    {
        Vector2 newPosition = confirmedNode.Position + direction * gridSize;
        Node targetNode = gridManager.GetNodeAtPosition(newPosition);

        temporaryBallPos = newPosition;

        if (confirmedNode.IsNeighbor(targetNode))
        {
            currentTemporaryNode = targetNode;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetConfirmedNode( ref Node node)
    {
        confirmedNode = node;
        ball.transform.position = node.Position;
        currentTemporaryNode = node;
    }

    public void ResetToLastConfirmedNode()
    {
        ball.transform.position = confirmedNode.Position;
        currentTemporaryNode = confirmedNode;
    }

    public ref Node GetTargetNode()
    {
        currentTemporaryNode = gridManager.GetNodeAtPosition(temporaryBallPos);
        return ref currentTemporaryNode;
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
