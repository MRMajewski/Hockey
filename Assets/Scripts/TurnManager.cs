using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField]
    private BallMovement ballMovement;
    [SerializeField]
    private BallPathRenderer pathRenderer;
    [SerializeField]
    private AIController aiController;
    [SerializeField]
    private GameController gameController;

    private Node lastConfirmedNode;
    private Color playerColor = Color.blue;

    [SerializeField]
    private bool isPlayerTurn = true;

    public bool IsPlayerTurn { get => isPlayerTurn; set => isPlayerTurn = value; }

    public void TurnManagerInit()
    {
      //  Debug.Log(lastConfirmedNode.Position + " TurnManagerInit lastConfirmedNode");

        lastConfirmedNode = ballMovement.GetConfirmedNode(); // Ustawiamy pierwszy zatwierdzony wêze³

        Debug.Log(lastConfirmedNode.Position + " TurnManagerInit lastConfirmedNode");
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
            
                Debug.Log("Koniec tury gracza!");
                ballMovement.SetConfirmedNode(ref lastConfirmedNode);
            //    IsPlayerTurn = false;
                gameController.CheckIfGameEnded(ref lastConfirmedNode);
                IsPlayerTurn = false;
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
