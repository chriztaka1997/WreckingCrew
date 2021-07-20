using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ManagerMB : MonoBehaviour
{
    public GameManagerMB gameMngr;

    public WaveProgressBarMB waveProgress;
    public GameObject nextButtonArea;
    public UpgradeSelectorMB upgradeSelector;

    public PauseManagerMB pausePF;


    void Start()
    {
        gameMngr = GameManagerMB.instance;
        StateChanged();
    }

    public void StateChanged()
    {
        DeactivateTogglable();
        switch (gameMngr.gameState)
        {
            case GameManagerMB.GameState.countdown:
                waveProgress.SetState(WaveProgressBarMB.State.prep);
                break;
            case GameManagerMB.GameState.combat:
                waveProgress.SetState(WaveProgressBarMB.State.wave);
                break;
            case GameManagerMB.GameState.preupgrade:
            case GameManagerMB.GameState.upgrade:
                waveProgress.SetState(WaveProgressBarMB.State.done);
                break;
            case GameManagerMB.GameState.complete:
                nextButtonArea.SetActive(true);
                waveProgress.SetState(WaveProgressBarMB.State.done);
                break;
            case GameManagerMB.GameState.tutorial:
                waveProgress.SetState(WaveProgressBarMB.State.done);
                break;
        }
    }

    public void DeactivateTogglable()
    {
        nextButtonArea.SetActive(false);
        upgradeSelector.gameObject.SetActive(false);
    }

    public void UpdateWaveProgress(float progress)
    {
        waveProgress.SetValue(progress);
    }

    public void DisplayUpgradeSelector(List<UpgradeData> upgrades)
    {
        upgradeSelector.gameObject.SetActive(true);
        upgradeSelector.SetButtons(upgrades);
    }

    public void OnUpgradeSelected(string name)
    {
        upgradeSelector.gameObject.SetActive(false);
        gameMngr.UpgradeSelected(name);
    }

    public void OnPausePress()
    {
        if (PauseManagerMB.instance != null)
        {
            PauseManagerMB.instance.OnUnpausePress();
        }
        else
        {
            GameManagerMB.instance.StartPause();
            Instantiate(pausePF);
        }
    }

    
}
