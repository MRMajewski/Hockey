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

    [SerializeField]
    private UIManager uiManager;

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
        if (isPlayerTurn && gameController.isGameStarted && !gameController.gameEnded)
        HandlePlayerTurn();
    }
    void HandlePlayerTurn()
    {
        if (Input.GetButtonDown("Submit"))
        {
            Node currentNode = ballMovement.GetConfirmedNode();
            Node targetNode = ballMovement.GetTargetNode();

            if (currentNode == null || targetNode == null)
                return;

            // Sprawdzanie, czy ruch jest legalny
            if (pathRenderer.IsMoveLegal(currentNode, targetNode))
            {
                // Wykonaj ruch
                pathRenderer.AddPosition(ref currentNode, ref targetNode, playerColor);
                lastConfirmedNode = targetNode;

                Debug.Log("Koniec ruchu gracza!");

                // Aktualizacja ostatniego potwierdzonego w�z�a
                ballMovement.SetConfirmedNode(ref lastConfirmedNode);

                // Sprawd�, czy gra zako�czy�a si�
                gameController.CheckIfGameEnded(ref lastConfirmedNode);

                // Sprawd�, czy gracz ko�czy ruch na u�ywanym w�le
                if (pathRenderer.WasNodeAlreadyUsed(targetNode))
                {
                    uiManager.DisplayMessage("Dodatkowy ruch!");
                    Debug.Log("Gracz ko�czy ruch na u�ywanym w�le - dodatkowy ruch!");
                    // Tutaj gracz mo�e wykona� kolejny ruch
                    return;  // Wyj�cie, aby gracz m�g� wykona� kolejny ruch
                }
                else
                {

                    uiManager.ResetMessage();
                    // Tura gracza si� ko�czy
                    IsPlayerTurn = false;
                    HandleAITurn();
                }
            }
            else
            {
                uiManager.ResetMessage();
                // Ruch nielegalny, reset do ostatniego potwierdzonego w�z�a
                ballMovement.ResetToLastConfirmedNode();
            }
        }
    }

    void HandleAITurn()
    {
        if (aiController != null)
        {
            aiController.PerformAITurn();

            Debug.Log("Koniec tury AI!");

        }
        IsPlayerTurn = true;
    }
}
