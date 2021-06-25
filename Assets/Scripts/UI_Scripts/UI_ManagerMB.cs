using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ManagerMB : MonoBehaviour
{
    public GameManagerMB gameMngr;

    public WaveProgressBarMB waveProgress;
    public GameObject nextButtonArea;


    void Start()
    {
        gameMngr = GameManagerMB.instance;
        StateChanged();
    }

    public void StateChanged()
    {
        switch (gameMngr.gameState)
        {
            case GameManagerMB.GameState.countdown:
                nextButtonArea.SetActive(false);
                waveProgress.SetState(WaveProgressBarMB.State.prep);
                break;
            case GameManagerMB.GameState.combat:
                nextButtonArea.SetActive(false);
                waveProgress.SetState(WaveProgressBarMB.State.wave);
                break;
            case GameManagerMB.GameState.complete:
                nextButtonArea.SetActive(true);
                waveProgress.SetState(WaveProgressBarMB.State.done);
                break;
        }
    }

    public void UpdateWaveProgress(float progress)
    {
        waveProgress.SetValue(progress);
    }

    
}
