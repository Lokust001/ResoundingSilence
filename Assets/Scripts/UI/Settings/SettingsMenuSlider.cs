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

    public void SliderUpdated(Single value = -1)
    {
        if (Mathf.RoundToInt(value) != currentValue)
        {
            currentValue = Mathf.RoundToInt(value);
        }
        UpdateSliderAndField();
    }

    public void FieldUpdated(string newText = "")
    {
        if (int.TryParse(newText, out int res))
        {
            if (res >= 0 && res <= 100)
            {
                currentValue = res;
                UpdateSliderAndField();
            }
        }
    }

    private void UpdateSliderAndField()
    {
        slide.value = currentValue;
        field.text = $"{currentValue}";
    }
}
