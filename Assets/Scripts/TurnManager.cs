using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;

    private Node lastConfirmedNode;
    private Color playerColor = Color.blue;

    [SerializeField]
    private bool isPlayerTurn = true;
    public bool IsPlayerTurn { get => isPlayerTurn; set => isPlayerTurn = value; }

    public void TurnManagerInit()
    {
        lastConfirmedNode = gameController.BallMovement.GetConfirmedNode();
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
        if (Input.GetMouseButtonUp(0))
        {
            Node currentNode = gameController.BallMovement.GetConfirmedNode();
            Node targetNode = gameController.BallMovement.GetTargetNode();

            if (currentNode == null || targetNode == null)
                return;

            if (gameController.PathRenderer.IsMoveLegal(currentNode, targetNode))
            {
                gameController.BallMovement.MoveBallToNode(targetNode);
                gameController.PathRenderer.AddPosition(ref currentNode, ref targetNode, playerColor);
                lastConfirmedNode = targetNode;


                gameController.BallMovement.SetConfirmedNode(ref lastConfirmedNode);

               if(gameController.CheckIfGameEnded(ref lastConfirmedNode))
                {
                    return;
                }          

                if (gameController.PathRenderer.WasNodeAlreadyUsed(targetNode))
                {
                    gameController.UIManager.DisplayMessageBonusMove("BONUS MOVE");
                    Debug.Log("Gracz ko�czy ruch na u�ywanym w�le - dodatkowy ruch!");

                    return; 
                }
                else
                {
                    gameController.UIManager.ResetBonusMoveMessage();
                    gameController.UIManager.ResetMessage();
                    IsPlayerTurn = false;
                    HandleAITurn();
                }
            }
            else
            {
                gameController.UIManager.ResetBonusMoveMessage();
                gameController.UIManager.ResetMessage();
                gameController.BallMovement.ResetToLastConfirmedNode();
            }
        }
    }

    void HandleAITurn()
    {
        if (gameController.AIController != null)
        {
            gameController.AIController.PerformAITurn();

        }
        IsPlayerTurn = true;
    }

    #region ENTER Controls
    //void HandlePlayerTurn()
    //{
    //    if (Input.GetButtonDown("Submit"))
    //    {
    //        Node currentNode = ballMovement.GetConfirmedNode();
    //        Node targetNode = ballMovement.GetTargetNode();

    //        if (currentNode == null || targetNode == null)
    //            return;

    //        // Sprawdzanie, czy ruch jest legalny
    //        if (pathRenderer.IsMoveLegal(currentNode, targetNode))
    //        {
    //            // Wykonaj ruch
    //            pathRenderer.AddPosition(ref currentNode, ref targetNode, playerColor);
    //            lastConfirmedNode = targetNode;

    //            Debug.Log("Koniec ruchu gracza!");

    //            // Aktualizacja ostatniego potwierdzonego w�z�a
    //            ballMovement.SetConfirmedNode(ref lastConfirmedNode);

    //            // Sprawd�, czy gra zako�czy�a si�
    //            if (gameController.CheckIfGameEnded(ref lastConfirmedNode))
    //            {
    //                return;
    //            }

    //            // Sprawd�, czy gracz ko�czy ruch na u�ywanym w�le
    //            if (pathRenderer.WasNodeAlreadyUsed(targetNode))
    //            {
    //                uiManager.DisplayMessageBonusMove("Dodatkowy ruch!");
    //                Debug.Log("Gracz ko�czy ruch na u�ywanym w�le - dodatkowy ruch!");
    //                // Tutaj gracz mo�e wykona� kolejny ruch
    //                return;  // Wyj�cie, aby gracz m�g� wykona� kolejny ruch
    //            }
    //            else
    //            {
    //                uiManager.ResetBonusMoveMessage();
    //                uiManager.ResetMessage();
    //                // Tura gracza si� ko�czy
    //                IsPlayerTurn = false;
    //                HandleAITurn();
    //            }
    //        }
    //        else
    //        {
    //            uiManager.ResetBonusMoveMessage();
    //            uiManager.ResetMessage();
    //            // Ruch nielegalny, reset do ostatniego potwierdzonego w�z�a
    //            ballMovement.ResetToLastConfirmedNode();
    //        }
    //    }
    //}
    #endregion
}
