using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class BallMovement : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;

    [SerializeField]
    private Transform ball;
    [SerializeField]
    private Transform cursor;
    public float gridSize = 1f;
    public Vector2Int arenaSize = new Vector2Int(8, 10);
    public int goalWidth = 2;

    private Node currentTemporaryNode;
    private Node confirmedNode;

    public Vector2 temporaryBallPos = Vector2.zero;

    public float scaleFactor = 1.25f;  
    public float duration = 0.5f;

    private Tween cursorTween;
    private Vector3 originalCursorScale;

    private void Start()
    {
        originalCursorScale = cursor.transform.localScale;
    }
    public void BallInit()
    {
        confirmedNode = gameController.GridManager.GetNodeAtPosition(Vector2.zero);
        currentTemporaryNode = confirmedNode;
        ball.position = confirmedNode.Position;
    }

    #region WSAD Controls
    //private void Update()
    //{
    //    if (!gameController.isGameStarted) return;

    //    if (Input.anyKeyDown)
    //    {
    //        HandleBallMovement();
    //        ball.position = currentTemporaryNode.Position;
    //    }
    //}
    //private bool HandleBallMovement()
    //{
    //    bool didMove = false;

    //    currentTemporaryNode = confirmedNode;
    //    Vector2 direction = Vector2.zero;

    //    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
    //    {
    //        direction = Vector2.up;
    //    }
    //    else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
    //    {
    //        direction = Vector2.down;
    //    }
    //    else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
    //    {
    //        direction = Vector2.left;
    //    }
    //    else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
    //    {
    //        direction = Vector2.right;
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        direction = new Vector2(-1, 1);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        direction = new Vector2(1, 1);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        direction = new Vector2(-1, -1);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        direction = new Vector2(1, -1);
    //    }
    //    if (direction != Vector2.zero)
    //    {
    //        didMove = TryMoveToNode(direction);
    //    }
    //    return didMove;
    //}
    #endregion

    private void Update()
    {
        if (!gameController.isGameStarted) return;

        UpdateCursorPosition();
    }

    public void MoveBallToNode(Node targetNode)
    {
        currentTemporaryNode = targetNode;
        SetConfirmedNode(ref targetNode);
    }
    private void UpdateCursorPosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 gridPosition = GetClosestNodePosition(new Vector2(mousePosition.x, mousePosition.y));
        cursor.position = new Vector3(gridPosition.x, gridPosition.y, cursor.position.z);

        Node nodeUnderCursor = gameController.GridManager.GetNodeAtPosition(gridPosition);

        if (confirmedNode.IsNeighbor(nodeUnderCursor))
        {
            StartPulsating(); 
        }
        else
        {
            StopPulsating(); 
        }
    }

    private void StartPulsating()
    {
        if (cursorTween != null && cursorTween.IsPlaying())
        {
            return; 
        }

        cursorTween = cursor.transform.DOScale(originalCursorScale * scaleFactor, duration)
            .SetLoops(-1, LoopType.Yoyo)  
            .SetEase(Ease.InOutSine);  
    }

    private void StopPulsating()
    {

        if (cursorTween != null && cursorTween.IsPlaying())
        {
            cursorTween.Kill(); 
            cursor.transform.localScale = originalCursorScale; 
        }
    }

    public void SetConfirmedNode( ref Node node)
    {
        confirmedNode = node;
        ball.position = node.Position;
        currentTemporaryNode = node;
    }

    public void ResetToLastConfirmedNode()
    {
        ball.position = confirmedNode.Position;
        currentTemporaryNode = confirmedNode;
    }

    public ref Node GetTargetNode()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 targetPosition = new Vector2(mousePosition.x, mousePosition.y);

        currentTemporaryNode = gameController.GridManager.GetNodeAtPosition(targetPosition);
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

        foreach (var node in gameController.GridManager.Nodes)
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
        Node currentNode = gameController.GridManager.GetNodeAtPosition(currentNodePosition);

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

        foreach (Node node in gameController.GridManager.Nodes)
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
    public bool TryMoveToNode(Vector2 direction)
    {
        Vector2 newPosition = confirmedNode.Position + direction * gridSize;
        Node targetNode = gameController.GridManager.GetNodeAtPosition(newPosition);

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
}
