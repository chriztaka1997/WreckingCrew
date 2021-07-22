using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ManagerMB : MonoBehaviour
{
    public GameManagerMB gameMngr;

    public WaveProgressBarMB waveProgress;
    public GameObject nextButtonArea;
    public UpgradeSelectorMB upgradeSelector;

    public PauseManagerMB pausePF;
    public GameOverManagerMB gameOverPF;

    public TextMeshProUGUI stagesText;

    public bool isPaused => PauseManagerMB.instance != null;
    public bool isGameOver => GameOverManagerMB.instance != null;
    public bool isTutorial => TutorialMB.instance != null && TutorialMB.instance.textObj.activeSelf;
    public bool isUI_Interactable => !isPaused && !isGameOver && !isTutorial;


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

        UpdateStagesText();
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

    public void UpdateStagesText()
    {
        stagesText.text = string.Format("Stages Cleared:\n{0}", gameMngr.difficulty.stagesCompleted);
    }

    public void OnUpgradeSelected(string name)
    {
        upgradeSelector.gameObject.SetActive(false);
        gameMngr.UpgradeSelected(name);
    }

    public void OnGameOver(int stagesComplete)
    {
        if (!isGameOver)
        {
            GameManagerMB.instance.StartPause();
            GameOverManagerMB gom = Instantiate(gameOverPF);
            gom.Init(stagesComplete);
        }
    }

    public void OnNextStagePress()
    {
        if (isUI_Interactable)
        {
            gameMngr.PrepNextStage();
        }
    }

    public void OnNextWavePress()
    {
        if (isUI_Interactable)
        {
            gameMngr.PrepNextWave();
        }
    }

    public void OnPausePress()
    {
        if (isPaused)
        {
            PauseManagerMB.instance.OnUnpausePress();
        }
        else if (isUI_Interactable)
        {
            GameManagerMB.instance.StartPause();
            Instantiate(pausePF);
        }
    }

    
}
