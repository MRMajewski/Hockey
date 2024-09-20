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

    private const string PlayerScoreKey = "PlayerScore";
    private const string AiScoreKey = "AiScore";

    [Header("UI references")]
    [SerializeField]
    private TextMeshProUGUI playerScoreText;
    [SerializeField]
    private TextMeshProUGUI aiScoreText;
    [SerializeField]
    private TextMeshProUGUI infoText;

    [SerializeField]
    private bool gameEnded = false;
    public bool isGameStarted = false;
    private int playerScore = 0;
    private int aiScore = 0;


    private void Start()
    {
        isGameStarted = false;
        LoadScores();
        UpdateScoreUI();
    }
    public List<Node> GetAllNodes()
    {
        return gridManager.GetAllNodes(); // Zak�adaj�c, �e GridManager jest przypisany w GameController
    }

    public void CheckIfGameEnded(ref Node nodeUnderChecking)
    {
        if (!isGameStarted) return;

        if (turnManager.IsPlayerTurn)
        {
            if (!CheckIfPlayerHasLegalMoves())
                return;
        }  

        if (CheckForWin(ref nodeUnderChecking))
        {
            if (turnManager.IsPlayerTurn)
            {
                playerScore++;
                infoText.text = "Player wins!";
                Debug.Log("Player wins!");
            }
            else
            {
                aiScore++;
                infoText.text = "AI wins!";
                Debug.Log("AI wins!");
            }
            UpdateScoreUI();
            SaveScores();
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
        PlayerPrefs.SetInt(PlayerScoreKey, 0);
        PlayerPrefs.SetInt(AiScoreKey, 0);

        playerScore = 0;
        aiScore = 0;
        UpdateScoreUI();
    }

    //private void EndTurn()
    //{
    //    turnManager.IsPlayerTurn = !turnManager.IsPlayerTurn;
    //    Debug.Log(turnManager.IsPlayerTurn ? "Player's turn" : "AI's turn");
    //}

    private bool CheckForWin(ref Node nodeUnderChecking)
    {
        if ((nodeUnderChecking.Position == aiController.GoalNode.Position))
            return true;
        else
            return false;
    }
    public bool CheckIfPlayerHasLegalMoves()
    {
        // Pobieramy obecny w�ze�, na kt�rym znajduje si� pi�ka (gracza)
        Node currentNode = ballMovement.GetConfirmedNode();
     //   if (currentNode == null) return;

        // Pobieramy s�siad�w obecnego w�z�a
        List<Node> neighbors = new List<Node>(currentNode.GetNeighbors());

        bool hasLegalMove = false;

        foreach (Node neighbor in neighbors)
        {
            // Sprawdzamy, czy ruch do s�siada jest legalny
            if (pathRenderer.IsMoveLegal(currentNode, neighbor))
            {
                hasLegalMove = true;
                break; // Je�li znajdziemy legalny ruch, przerywamy p�tl�
            }
        }

        // Je�li �aden s�siad nie jest dost�pny
        if (!hasLegalMove)
        {
            Debug.Log("Gracz nie ma �adnych legalnych ruch�w. AI wygrywa!");
            AiWinsDueToNoPlayerMoves();
        }

        return hasLegalMove;
    }

    public void PlayerWinsDueToNoAiMoves()
    {
        if (gameEnded)
            return; // Zapobiega podw�jnemu zako�czeniu gry

        playerScore++;  // Zwi�kszamy punkty gracza
        infoText.text = "Player wins! AI has no more moves!";
        Debug.Log("Player wins! AI has no more moves!");

        UpdateScoreUI(); // Aktualizujemy interfejs
        SaveScores();    // Zapisujemy wynik
        gameEnded = true;
        isGameStarted = false; // Ko�czymy gr�
    }
    public void AiWinsDueToNoPlayerMoves()
    {
        if (gameEnded)
            return; // Zapobiega podw�jnemu zako�czeniu gry

        aiScore++;  // Zwi�kszamy punkty AI
        infoText.text = "AI wins! Player has no more moves!";
        Debug.Log("AI wins! Player has no more moves!");

        UpdateScoreUI(); // Aktualizujemy interfejs
        SaveScores();    // Zapisujemy wynik
        gameEnded = true;
        isGameStarted = false; // Ko�czymy gr�
    }
    private void LoadScores()
    {
        playerScore = PlayerPrefs.GetInt(PlayerScoreKey, 0);
        aiScore = PlayerPrefs.GetInt(AiScoreKey, 0);
    }

    private void SaveScores()
    {
        PlayerPrefs.SetInt(PlayerScoreKey, playerScore);
        PlayerPrefs.SetInt(AiScoreKey, aiScore);
    }

    private void UpdateScoreUI()
    {
        if (playerScoreText != null)
        {
            playerScoreText.text = "Player Score: " + playerScore;
        }
        if (aiScoreText != null)
        {
            aiScoreText.text = "AI Score: " + aiScore;
        }
    }
}