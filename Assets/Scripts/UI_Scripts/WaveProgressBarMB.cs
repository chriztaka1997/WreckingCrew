using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveProgressBarMB : MonoBehaviour
{
    public Slider waveSlider;
    public TextMeshProUGUI tmpro;

    public string prepText, waveText, doneText;

    public State state;

    public void Start()
    {
        waveSlider.maxValue = 1.0f;
        SetState(State.prep);
    }

    public void SetValue(float progress)
    {
        waveSlider.value = progress;
    }

    public void SetState(State state)
    {
        this.state = state;
        switch (state)
        {
            case State.prep:
                tmpro.text = prepText;
                waveSlider.value = 0.0f;
                break;
            case State.wave:
                tmpro.text = waveText;
                break;
            case State.done:
                tmpro.text = doneText;
                waveSlider.value = 1.0f;
                break;
        }
    }

    public enum State
    {
        prep,
        wave,
        done,
    }
}
