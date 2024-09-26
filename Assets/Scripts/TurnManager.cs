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
                    Debug.Log("Gracz koñczy ruch na u¿ywanym wêŸle - dodatkowy ruch!");

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

    //            // Aktualizacja ostatniego potwierdzonego wêz³a
    //            ballMovement.SetConfirmedNode(ref lastConfirmedNode);

    //            // SprawdŸ, czy gra zakoñczy³a siê
    //            if (gameController.CheckIfGameEnded(ref lastConfirmedNode))
    //            {
    //                return;
    //            }

    //            // SprawdŸ, czy gracz koñczy ruch na u¿ywanym wêŸle
    //            if (pathRenderer.WasNodeAlreadyUsed(targetNode))
    //            {
    //                uiManager.DisplayMessageBonusMove("Dodatkowy ruch!");
    //                Debug.Log("Gracz koñczy ruch na u¿ywanym wêŸle - dodatkowy ruch!");
    //                // Tutaj gracz mo¿e wykonaæ kolejny ruch
    //                return;  // Wyjœcie, aby gracz móg³ wykonaæ kolejny ruch
    //            }
    //            else
    //            {
    //                uiManager.ResetBonusMoveMessage();
    //                uiManager.ResetMessage();
    //                // Tura gracza siê koñczy
    //                IsPlayerTurn = false;
    //                HandleAITurn();
    //            }
    //        }
    //        else
    //        {
    //            uiManager.ResetBonusMoveMessage();
    //            uiManager.ResetMessage();
    //            // Ruch nielegalny, reset do ostatniego potwierdzonego wêz³a
    //            ballMovement.ResetToLastConfirmedNode();
    //        }
    //    }
    //}
    #endregion
}
