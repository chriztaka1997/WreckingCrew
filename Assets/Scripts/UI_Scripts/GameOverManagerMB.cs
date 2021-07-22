using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameOverManagerMB : MonoBehaviour
{
    public static GameOverManagerMB instance;

    public string stageClearStr;
    public Text stageClearedText;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void Init(int stagesComplete)
    {
        stageClearedText.text = string.Format(stageClearStr, stagesComplete);
    }

    public void OnReturnToTitlePress()
    {
        GameManagerMB.instance.EndPause();
        SceneManager.LoadScene("start_sc");
    }

    public void OnKeepPlayingPress()
    {
        GameManagerMB.instance.player.ResetHP();
        GameManagerMB.instance.EndPause();
        instance = null;
        Destroy(gameObject);
    }


}
