using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridSizeSliders : MonoBehaviour
{
    [SerializeField]
    private ArenaGenerator arenaGenerator;

    [SerializeField] private Slider widthSlider;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private TextMeshProUGUI widthValueDisplayText;
    [SerializeField] private TextMeshProUGUI heightValueDisplayText;

    public void OnWidthSliderValueChanged(float value)
    {
        int evenValue = Mathf.RoundToInt(value / 2) * 2;
        widthSlider.value = evenValue;

        UpdateValueDisplay();
    }
    public void OnHeightSliderValueChanged(float value)
    {
        int evenValue = Mathf.RoundToInt(value / 2) * 2;
        heightSlider.value = evenValue;
        UpdateValueDisplay();
    }

    public void UpdateValueDisplay()
    {
        arenaGenerator.Height = (int)heightSlider.value;
        arenaGenerator.Width = (int)widthSlider.value;
        heightValueDisplayText.text = heightSlider.value.ToString();
        widthValueDisplayText.text = widthSlider.value.ToString();
    }
}
