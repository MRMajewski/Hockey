using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public BallMovement ballMovement;
    public BallPathRenderer pathRenderer;

    private Vector3 lastPosition;
    private Color playerColor = Color.blue;
    private Color aiColor = Color.red;

    public bool isPlayerTurn = true;

    void Start()
    {
        lastPosition = ballMovement.ball.transform.position;
    }

    void Update()
    {
        if (isPlayerTurn)
        {
            HandlePlayerTurn();
        }
    }

    void HandlePlayerTurn()
    {
        if (Input.GetButtonDown("Submit"))
        {

            Debug.Log("Dzia³a Enter");
            Vector3 currentPosition = ballMovement.ball.transform.position;

            //if (IsNeighborNode(roundedTarget))
            //{
            //    targetPosition = roundedTarget;
            //    isMoving = true;
            //}
            if (ballMovement.IsNeighborNode(lastPosition))
              //  if (ballMovement.IsNeighborNode(currentPosition))
            {
             //   ballMovement.RemoveNeighborConnection(previousNode, currentNode);
                pathRenderer.AddPosition(lastPosition, currentPosition, playerColor);
                lastPosition = currentPosition;
           //     isPlayerTurn = false;
                //   PerformAITurn();
            }


            //if (IsMoveValid(lastPosition, currentPosition))
            //{
            //    pathRenderer.AddPosition(lastPosition, currentPosition, playerColor);
            //    lastPosition = currentPosition;
            //    isPlayerTurn = false;
            // //   PerformAITurn();
            //}
            else
            {
                ballMovement.ball.transform.position = lastPosition;
            }
        }
    }

    void PerformAITurn()
    {
        AIController aiController = FindObjectOfType<AIController>();
        if (aiController != null)
        {
            aiController.PerformAITurn();
        }

        isPlayerTurn = true;
    }

    //bool IsMoveValid(Vector3 startPos, Vector3 endPos)
    //{
    //    float distanceX = Mathf.Abs(endPos.x - startPos.x);
    //    float distanceY = Mathf.Abs(endPos.y - startPos.y);
    //    float distanceZ = Mathf.Abs(endPos.z - startPos.z);

    //    return (distanceX <= 1f && distanceY <= 1f && distanceZ <= 1f);
    //}
}