using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class HP_BarMB : MonoBehaviour
{
    public Slider hpSlider, delaySlider;
    public float delayTime; // sec of delay
    public float delaySliderSpd; // ratio of whole slider / sec
    public DateTime delayStart;
    public TextMeshProUGUI tmpro;

    private bool isDelay;

    public void InitHP(float hp)
    {
        hpSlider.maxValue = hp;
        delaySlider.maxValue = hp;
        Reset();
    }

    public void Reset()
    {
        SetHP(hpSlider.maxValue);
        isDelay = false;
    }

    public void SetHP(float hp, bool useDelay = true)
    {
        hpSlider.value = hp;
        if (hp >= delaySlider.value)
        {
            delaySlider.value = hp;
        }
        else if (useDelay)
        {
            delayStart = DateTime.Now;
            isDelay = true;
        }
        if (tmpro != null) tmpro.text = hpSlider.value.ToString("F0");
    }

    public void Update()
    {
        if (isDelay)
        {
            if ((DateTime.Now - delayStart).TotalSeconds >= delayTime)
                isDelay = false;
        }
        else
        {
            if (hpSlider.value < delaySlider.value)
            {
                float valDiff = Time.deltaTime * delaySliderSpd * delaySlider.maxValue;
                delaySlider.value -= valDiff;
                if (delaySlider.value < hpSlider.value) delaySlider.value = hpSlider.value;
            }
        }
    }
}
