using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI aiScoreText;
    [SerializeField] private TextMeshProUGUI infoText;
    [Space]
    [SerializeField] private ScoreManager scoreManager;
    public void UpdateScoreUI()
    {
        playerScoreText.text = scoreManager.PlayerScore.ToString();
        aiScoreText.text = scoreManager.AIScore.ToString();
    }

    public void DisplayMessage(string message)
    {
        infoText.text = message;
    }

    public void ResetMessage()
    {
        if (!infoText.text.Equals(""))
            infoText.text = "";
    }
}