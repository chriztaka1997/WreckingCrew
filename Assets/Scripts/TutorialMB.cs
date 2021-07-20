using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class TutorialMB : MonoBehaviour
{
    public static TutorialMB instance;

    public List<TutorialEntry> entries;
    public GameObject textObj;
    public GameObject hintObj;

    public Text titleTxt;
    public Text descriptionText;

    public float timeDelay;
    private float timeSignaled;
    private bool signaled;

    private int curInd;
    public TutorialEntry curEntry => entries[curInd];


    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        curInd = 0;
        HideWindow();
        hintObj.SetActive(false);
        signaled = false;
    }

    public void Update()
    {
        if (signaled && Time.time - timeSignaled >= timeDelay)
        {
            NextItem();
            signaled = false;
        }
    }

    public void StartTutorial()
    {
        GameManagerMB.instance.enemyspawn.AbortWave();
        GameManagerMB.instance.SetStage("tutorial");
        GameManagerMB.instance.ChangeState(GameManagerMB.GameState.tutorial);

        LoadCurrentEntry();
        ShowWindow();
        hintObj.SetActive(true);
    }

    public void ShowWindow()
    {
        textObj.SetActive(true);
    }

    public void HideWindow()
    {
        textObj.SetActive(false);
    }

    public static void SignalTutorial(string name, bool delay = true)
    {
        if (instance == null) return;
        if (instance.curEntry.name == name)
        {
            if (!delay)
            {
                instance.NextItem();
                return;
            }
            if (!instance.signaled)
            {
                instance.timeSignaled = Time.time;
            }
            instance.signaled = true;
        }
    }

    public void NextItem()
    {
        curInd++;
        if (curInd >= entries.Count)
        {
            QuitTutorial();
            Destroy(gameObject);
            return;
        }
        else
        {
            LoadCurrentEntry();
            ShowWindow();
        }
    }

    public void QuitTutorial()
    {
        GameManagerMB.instance.ResetPlayer();
        GameManagerMB.instance.SetStage(GameManagerMB.instance.startingStage);
        GameManagerMB.instance.ChangeState(GameManagerMB.GameState.countdown);
    }

    public void LoadCurrentEntry()
    {
        titleTxt.text = curEntry.title;
        descriptionText.text = curEntry.description;
        DoEvent();
    }

    public void DoEvent()
    {
        switch (curEntry.name)
        {
            case "spin":
                break;
            case "throw":
                break;
            case "wall":
                GameManagerMB.instance.ResetLevel();
                break;
            case "item":
                ObjectPooler.SharedInstance.SpawnHealthPotion(GameManagerMB.instance.levelMngr.level.WorldLocation(5, 9));
                ObjectPooler.SharedInstance.SpawnCoin(GameManagerMB.instance.levelMngr.level.WorldLocation(13, 9));
                break;
            case "wave":
                GameManagerMB.instance.ChangeState(GameManagerMB.GameState.countdown);
                break;
            case "next":
                break;
        }
    }
}

[Serializable]
public class TutorialEntry
{
    public string name;
    public string title;
    [TextArea]
    public string description;
}
