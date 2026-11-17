/*
* Author: Tyler
* Contributors:
* Last Modified:11/17/2026
* Summary: The sliders for the settings menu
* To Do:   N/A
*/

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuSlider : MonoBehaviour
{
    [SerializeField]
    private Slider slide;

    [SerializeField]
    private TMP_InputField field;

    private int currentValue = -1;

    [SerializeField]
    private int defaultValue = -1;

    /// <summary>
    /// Set to default at start
    /// </summary>
    private void Awake()
    {
        currentValue = defaultValue;
        UpdateSliderAndField();
    }

    /// <summary>
    /// Triggers when the slider gets updated
    /// </summary>
    /// <param name="value"></param>
    public void SliderUpdated(Single value = -1)
    {
        if (Mathf.RoundToInt(value) != currentValue)
        {
            currentValue = Mathf.RoundToInt(value);
        }
        UpdateSliderAndField();
    }

    /// <summary>
    /// Triggers when the text field gets updated
    /// </summary>
    /// <param name="newText"></param>
    public void FieldUpdated(string newText = "")
    {
        if (int.TryParse(newText, out int res))
        {
            if (res >= 0 && res <= 100)
            {
                currentValue = res;
                
            }
        }
        UpdateSliderAndField();
    }

    /// <summary>
    /// Force updates the slider and text field
    /// </summary>
    private void UpdateSliderAndField()
    {
        slide.value = currentValue;
        field.text = $"{currentValue}";
    }
}
