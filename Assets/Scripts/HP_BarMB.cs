using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HP_BarMB : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI tmpro;

    public void InitHP(float hp)
    {
        slider.maxValue = hp;
        Reset();
    }

    public void Reset()
    {
        SetHP(slider.maxValue);
    }

    public void SetHP(float hp)
    {
        slider.value = hp;
        tmpro.text = slider.value.ToString("N0");
    }
}
