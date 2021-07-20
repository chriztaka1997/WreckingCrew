using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class PauseManagerMB : MonoBehaviour
{
    public static PauseManagerMB instance;

    public Slider camSizeSlider;
    public float camSizeMin;
    public float camSizeMax;

    public Button resetLevelButton;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        camSizeSlider.value = Mathf.InverseLerp(camSizeMin, camSizeMax, Camera.main.orthographicSize);

        resetLevelButton.interactable = TutorialMB.instance == null;
    }

    public void OnCamSizeSliderChange()
    {
        Camera.main.orthographicSize = Mathf.Lerp(camSizeMin, camSizeMax, camSizeSlider.value);
    }

    public void OnUnstuckPress()
    {
        GameManagerMB.instance.levelMngr.PlacePlayer(GameManagerMB.instance.player);
        OnUnpausePress();
    }

    public void OnRestartPress()
    {
        GameManagerMB.instance.ChangeState(GameManagerMB.GameState.countdown);
        GameManagerMB.instance.enemyspawn.AbortWave();
        OnUnpausePress();
    }

    public void OnUnpausePress()
    {
        GameManagerMB.instance.EndPause();
        instance = null;
        Destroy(gameObject);
    }

    public void OnReturnToMenuButton()
    {
        SceneManager.LoadScene("start_sc");
    }


}
