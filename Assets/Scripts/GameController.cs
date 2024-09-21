using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    [Header("Main components references")]
    [SerializeField]
    private GameObject ball;
    [SerializeField]
    private BallPathRenderer pathRenderer;
    [SerializeField]
    private BallMovement ballMovement;
    [SerializeField]
    private ArenaGenerator arenaGenerator;
    [SerializeField]
    private AIController aiController;
    [SerializeField]
    private TurnManager turnManager;
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private ScoreManager scoreManager;

    [SerializeField]
    private bool gameEnded = false;
    public bool isGameStarted = false;

    private void Start()
    {
        isGameStarted = false;
        scoreManager.LoadScores();
        uiManager.UpdateScoreUI();
    }
    public List<Node> GetAllNodes()
    {
        return gridManager.GetAllNodes(); 
    }

    public void CheckIfGameEnded(ref Node nodeUnderChecking)
    {
        if (!isGameStarted) return;

        if (turnManager.IsPlayerTurn)
        {
            if (!CheckIfPlayerHasLegalMoves())
                return;
        }

        if (gameEnded)
            return;

        if (CheckForWin(ref nodeUnderChecking))
        {
            string displayText;
            if (turnManager.IsPlayerTurn)
            {
                scoreManager.PlayerWinsUpdateScore();
                displayText = "Player wins!";
            }
            else
            {
                scoreManager.AIWinsUpdateScore();

                displayText = "AI wins!";
            }

            uiManager.DisplayMessage(displayText);
            uiManager.UpdateScoreUI();

            gameEnded = true;
            isGameStarted = false;
            return;
        }
    }

    public void StartGame()
    {
           if (isGameStarted)
               return;

        isGameStarted = true;
        pathRenderer.ClearPaths();
        arenaGenerator.GenerateArena();
        ballMovement.BallInit();
        aiController.SetGoalNodeForAI();
        turnManager.TurnManagerInit();
        turnManager.IsPlayerTurn = true;
        gameEnded = false;
      
        Debug.Log("Game Started");
    }

    public void ResetGame()
    {
        pathRenderer.ClearPaths();
        ballMovement.BallInit();
    }

    public void ResetScores()
    {
        scoreManager.ResetScores();
        uiManager.UpdateScoreUI();
    }

    private bool CheckForWin(ref Node nodeUnderChecking)
    {
        if ((nodeUnderChecking.Position == aiController.GoalNode.Position))
            return true;
        else
            return false;
    }
    public bool CheckIfPlayerHasLegalMoves()
    {
        // Pobieramy obecny wêze³, na którym znajduje siê pi³ka (gracza)
        Node currentNode = ballMovement.GetConfirmedNode();
     //   if (currentNode == null) return;

        // Pobieramy s¹siadów obecnego wêz³a
        List<Node> neighbors = new List<Node>(currentNode.GetNeighbors());

        bool hasLegalMove = false;

        foreach (Node neighbor in neighbors)
        {
            // Sprawdzamy, czy ruch do s¹siada jest legalny
            if (pathRenderer.IsMoveLegal(currentNode, neighbor))
            {
                hasLegalMove = true;
                break; // Jeœli znajdziemy legalny ruch, przerywamy pêtlê
            }
        }

        // Jeœli ¿aden s¹siad nie jest dostêpny
        if (!hasLegalMove)
        {
            Debug.Log("Gracz nie ma ¿adnych legalnych ruchów. AI wygrywa!");
            AiWinsDueToNoPlayerMoves();
        }

        return hasLegalMove;
    }

    public void PlayerWinsDueToNoAiMoves()
    {
        if (gameEnded)
            return; // Zapobiega podwójnemu zakoñczeniu gry
        scoreManager.PlayerWinsUpdateScore();

        uiManager.DisplayMessage("Player wins! AI has no more moves!");
      //  playerScore++;  // Zwiêkszamy punkty gracza
     //   infoText.text = "Player wins! AI has no more moves!";
        Debug.Log("Player wins! AI has no more moves!");

      //  UpdateScoreUI(); // Aktualizujemy interfejs
     //   SaveScores();    // Zapisujemy wynik
        gameEnded = true;
        isGameStarted = false; // Koñczymy grê
    }
    public void AiWinsDueToNoPlayerMoves()
    {
        if (gameEnded)
            return; // Zapobiega podwójnemu zakoñczeniu gry

        scoreManager.AIWinsUpdateScore();
        //  aiScore++;  // Zwiêkszamy punkty AI

        uiManager.DisplayMessage("AI wins! Player has no more moves!");
       // infoText.text = "AI wins! Player has no more moves!";
        Debug.Log("AI wins! Player has no more moves!");

     //   UpdateScoreUI(); // Aktualizujemy interfejs
     //   SaveScores();    // Zapisujemy wynik
        gameEnded = true;
        isGameStarted = false; // Koñczymy grê
    }

}