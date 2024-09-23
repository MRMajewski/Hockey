using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static AIController;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI aiScoreText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI bonusMoveInfotext;
    [SerializeField] private TextMeshProUGUI aiTypeInfotext;
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
    public void DisplayMessageBonusMove(string message)
    {
        bonusMoveInfotext.text = message;
    }
    public void ResetMessage()
    {
        if (!infoText.text.Equals(""))
            infoText.text = "";
    }
    public void ResetBonusMoveMessage()
    {
        if (!bonusMoveInfotext.text.Equals(""))
            bonusMoveInfotext.text = "";
    }

    public void DisplayAITypeInfo(AIAlgorithmType aiType)
    {
        aiTypeInfotext.text = "AI TYPE: "+aiType.ToString();
    }
}