using DG.Tweening;
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

    [Header("Tween referencs")]
    [SerializeField] private Vector3 enlargeScaleVector;

    public CanvasGroup bonusMoveText;
    public float appearDuration = 0.2f; 
    public float blinkDuration = 0.1f;  
    public int blinkCount = 3;          
    public float disappearDuration = 0.2f; 

    [Space]
    public float infoTextAppearDuration = 0.5f;     
    public float infoTextPulseDuration = 0.3f;     

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
        InfoTextAnimation();
    }
    public void DisplayMessageBonusMove(string message)
    { 
        bonusMoveText.alpha = 0;
        bonusMoveText.DOKill();

        Sequence blinkSequence = DOTween.Sequence();

        blinkSequence.Append(bonusMoveText.DOFade(1, appearDuration));

        blinkSequence.Append(bonusMoveText.DOFade(0, blinkDuration)
            .SetLoops(blinkCount * 2, LoopType.Yoyo)); 

        blinkSequence.Append(bonusMoveText.DOFade(0, disappearDuration));
    }

    public void ResetMessage()
    {
        if (infoText.text.Equals(""))
            return;

        DOTween.KillAll();

        Sequence resetSequence = DOTween.Sequence();
        resetSequence.Append(infoText.DOFade(0, infoTextAppearDuration));  // Fade-in

        resetSequence.OnComplete(() => infoText.text = "") ;
        resetSequence.OnComplete(() => DOTween.Kill(infoText.rectTransform));
    }
    public void ResetBonusMoveMessage()
    {
        if (bonusMoveText.alpha != 0)
            bonusMoveText.alpha = 0;
    }

    public void InfoTextAnimation()
    {
        infoText.DOKill();

        infoText.rectTransform.localScale = Vector3.zero;  

        Sequence winSequence = DOTween.Sequence();

        winSequence.Append(infoText.DOFade(1, infoTextAppearDuration));  
        winSequence.Join(infoText.rectTransform.DOScale(1.2f, infoTextAppearDuration).SetEase(Ease.OutBack));

        winSequence.Append(infoText.rectTransform.DOScale(1f, infoTextPulseDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
    }
}