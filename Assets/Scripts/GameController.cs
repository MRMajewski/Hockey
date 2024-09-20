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

    //[Header("bool references")]
    //private bool isPlayerTurn = true;
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
        return gridManager.GetAllNodes(); // Zak³adaj¹c, ¿e GridManager jest przypisany w GameController
    }

    public void CheckIfGameEnded(ref Node nodeUnderChecking)
    {
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