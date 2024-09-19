using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public BallMovement ballMovement;
    public BallPathRenderer pathRenderer;
    public AIController aiController;

    public GameController gameController;

    private Node lastConfirmedNode;
    private Color playerColor = Color.blue;

    public bool isPlayerTurn = true;


    public void TurnManagerInit()
    {
        lastConfirmedNode = ballMovement.GetConfirmedNode(); // Ustawiamy pierwszy zatwierdzony wêze³
        isPlayerTurn = true;
    }
    void Update()
    {
        if (isPlayerTurn)
        HandlePlayerTurn();
    }

    void HandlePlayerTurn()
    {
        if (Input.GetButtonDown("Submit"))
        {

            Node currentNode = ballMovement.GetConfirmedNode(); ; // Tymczasowy wêze³ pi³ki
            Node targetNode = ballMovement.GetTargetNode(); ;

            if (pathRenderer.IsMoveLegal(currentNode, targetNode))
                {
                pathRenderer.AddPosition(ref currentNode, ref targetNode, playerColor);
                lastConfirmedNode = targetNode;
                isPlayerTurn = false;
                Debug.Log("Koniec tury gracza!");
                ballMovement.SetConfirmedNode(ref lastConfirmedNode);

                gameController.CheckIfGameEnded();

                PerformAITurn();
            }
            else
            {
                ballMovement.ResetToLastConfirmedNode();
            }
        }
    }

    void PerformAITurn()
    {
        if (aiController != null)
        {
            aiController.PerformAITurn();

            Debug.Log("Koniec tury AI!");
        }

        isPlayerTurn = true;
    }
}
