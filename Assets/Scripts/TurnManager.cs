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
        lastConfirmedNode = ballMovement.GetConfirmedNode();
        isPlayerTurn = true;
    }

    public void SwitchTurn()
    {
        IsPlayerTurn = !IsPlayerTurn;
        Debug.Log(IsPlayerTurn ? "Player's turn" : "AI's turn");
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
            Node currentNode = ballMovement.GetConfirmedNode(); ; 
            Node targetNode = ballMovement.GetTargetNode(); ;

            if (pathRenderer.IsMoveLegal(currentNode, targetNode))
                {
                pathRenderer.AddPosition(ref currentNode, ref targetNode, playerColor);
                lastConfirmedNode = targetNode;
            
                Debug.Log("Koniec tury gracza!");
                ballMovement.SetConfirmedNode(ref lastConfirmedNode);
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
        IsPlayerTurn = true;
    }
}
