using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject ball;
    [SerializeField]
    private BallPathRenderer pathRenderer;
    [SerializeField]
    private BallMovement ballMovement;
    [SerializeField]
    private TextMeshProUGUI playerScoreText;
    [SerializeField]
    private TextMeshProUGUI aiScoreText;
    [SerializeField]
    private TextMeshProUGUI infoText;
    [SerializeField]
    private ArenaGenerator arenaGenerator;
    [SerializeField]
    private AIController aiController;

    [SerializeField]
    private TurnManager turnManager;

    private bool isPlayerTurn = true;
    [SerializeField]
    private bool gameEnded = false;
    public bool isGameStarted = false;

    private int playerScore = 0;
    private int aiScore = 0;

    private const string PlayerScoreKey = "PlayerScore";
    private const string AiScoreKey = "AiScore";

    [SerializeField]
    private GridManager gridManager;

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
    private void Update()
    {
        //   if (!isGameStarted || gameEnded)
        //      return;

        //if (CheckForWin())
        //{
        //    if (isPlayerTurn)
        //    {
        //        playerScore++;
        //        infoText.text = "Player wins!";
        //        Debug.Log("Player wins!");
        //    }
        //    else
        //    {
        //        aiScore++;
        //        infoText.text = "AI wins!";
        //        Debug.Log("AI wins!");
        //    }
        //    SaveScores();
        //    gameEnded = true;
        //    isGameStarted = false;
        //    return;
        //}



        //if (isPlayerTurn)
        //{
        //    if (Input.GetButtonDown("Submit"))
        //    {
        //        Debug.Log("Dzia³a Enter");
        //        EndTurn();
        //    }
        //}
        //else
        //{
        //    aiController.PerformAITurn();
        //    EndTurn();
        //}
    }

    public void StartGame()
    {
        //   if (isGameStarted)
        //       return;

        isGameStarted = true;
        ResetGame();
        Debug.Log(" arenaGenerator.GenerateArena()");
        arenaGenerator.GenerateArena();
        Debug.Log("  aiController.SetGoalNodeForAI();");
        aiController.SetGoalNodeForAI();
        Debug.Log("   ballMovement.BallInit(); ");
        ballMovement.BallInit();
        Debug.Log("    aiController.SetGoalNodeForAI(); ");
        aiController.SetGoalNodeForAI();
        Debug.Log("      turnManager.TurnManagerInit(); ");
        turnManager.TurnManagerInit();
        isPlayerTurn = true;
        gameEnded = false;
        Debug.Log("Game Started");
    }

    public void ResetGame()
    {
        ballMovement.lastPosition = Vector3.zero;
        ballMovement.targetPosition = Vector3.zero;
        ball.transform.position = Vector3.zero;
        pathRenderer.ClearPaths();
        //
        //    aiController.ResetAI();
        //   aiController.SetGoalNodeForAI();

        //      LoadScores();
        //     UpdateScoreUI();
    }

    public void ResetScores()
    {
        PlayerPrefs.SetInt(PlayerScoreKey, 0);
        PlayerPrefs.SetInt(AiScoreKey, 0);

        playerScore = 0;
        aiScore = 0;
        UpdateScoreUI();
    }

    private void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        Debug.Log(isPlayerTurn ? "Player's turn" : "AI's turn");
    }

    private bool CheckForWin()
    {
        Vector3 ballPosition = ball.transform.position;

        bool onTopRow = Mathf.Abs(ballPosition.y - (ballMovement.arenaSize.y / 2f * ballMovement.gridSize)) < ballMovement.gridSize / 2;
        bool onBottomRow = Mathf.Abs(ballPosition.y + (ballMovement.arenaSize.y / 2f * ballMovement.gridSize)) < ballMovement.gridSize / 2;

        return onTopRow || onBottomRow;
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