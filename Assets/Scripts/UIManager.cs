using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AIController;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI aiScoreText;
    [SerializeField] private TextMeshProUGUI playerScoreValueText;
    [SerializeField] private TextMeshProUGUI aiScoreValueText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI bonusMoveInfotext;
    [SerializeField] private TextMeshProUGUI aiTypeInfotext;
    [Space]
    [SerializeField] private GameController gameController;

    [SerializeField] private UIButtonPermamentSelection AIButtonsSelection;

    [SerializeField] private Button firstAITypeButton;

    [SerializeField] private Vector3 enlargeScaleVector;

    public void UIInit()
    {
        firstAITypeButton.onClick.Invoke();
        UpdateScoreUI();
    }
    public void UpdateScoreUI()
    {

        int PlayerScore = gameController.ScoreManager.PlayerScore;
        int AIScore = gameController.ScoreManager.AIScore;

        if(PlayerScore!=AIScore)
        {
            if (PlayerScore > AIScore)
            {
                playerScoreText.rectTransform.localScale = enlargeScaleVector;
                playerScoreValueText.rectTransform.localScale = enlargeScaleVector;
            }
            else
            {
                aiScoreText.rectTransform.localScale = enlargeScaleVector;
                aiScoreValueText.rectTransform.localScale = enlargeScaleVector;
            }
        }
        else
        {
            playerScoreText.rectTransform.localScale = Vector3.one;
            playerScoreValueText.rectTransform.localScale = Vector3.one;
            aiScoreText.rectTransform.localScale = Vector3.one;
            aiScoreValueText.rectTransform.localScale = Vector3.one;
        }
        playerScoreValueText.text = PlayerScore.ToString();
        aiScoreValueText.text = AIScore.ToString();
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
}