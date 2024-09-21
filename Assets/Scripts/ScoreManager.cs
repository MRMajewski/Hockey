using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class ScoreManager: MonoBehaviour
{
    private const string PlayerScoreKey = "PlayerScore";
    private const string AiScoreKey = "AiScore";
    private int playerScore = 0;
    private int aiScore = 0;
    public int PlayerScore => playerScore;
    public int AIScore => aiScore;

    public void LoadScores()
    {
        playerScore = PlayerPrefs.GetInt(PlayerScoreKey, 0);
        aiScore = PlayerPrefs.GetInt(AiScoreKey, 0);
    }

    public void SaveScores()
    {
        PlayerPrefs.SetInt(PlayerScoreKey, playerScore);
        PlayerPrefs.SetInt(AiScoreKey, aiScore);
    }

    public void ResetScores()
    {
        playerScore = 0;
        aiScore = 0;
        SaveScores();
    }

    public void PlayerWinsUpdateScore()
    {
        playerScore++;
        SaveScores();
    }

    public void AIWinsUpdateScore()
    {
        aiScore++;
        SaveScores();
    }
}
