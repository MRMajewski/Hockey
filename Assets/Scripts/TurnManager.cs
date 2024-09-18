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

            Node previousNode = ballMovement.GetNodeAtPosition(lastPosition);
            Node currentNode = ballMovement.GetNodeAtPosition(currentPosition);

//
       //     if (ballMovement.IsNeighborNode(lastPosition) && (pathRenderer.IsMoveLegal(previousNode, currentNode)))



          if (pathRenderer.IsMoveLegal(previousNode, currentNode))
                {

                pathRenderer.AddPosition(previousNode, currentNode, playerColor);
                //test!
            //    pathRenderer.MarkConnectionAsUsed(previousNode, currentNode);

              //  ballMovement.RemoveNeighborConnection(previousNode, currentNode);

                lastPosition = currentPosition;

            }
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
     //       aiController.PerformAITurn();
        }

        isPlayerTurn = true;
    }

}