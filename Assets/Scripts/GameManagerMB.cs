using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManagerMB : MonoBehaviour
{
    public static GameManagerMB instance;

    public GameState gameState;


    public PlayerMB player { get; private set; }
    public Enemyspawn enemyspawn;
    public GameObject playerPF;
    public BorderMB borderPF;

    public LevelManagerMB levelMngr;
    public LevelManagerMB levelMngrPF;

    public StageData stageData;
    public string startingStage;

    public float countDownTime;
    private DateTime countDownStart;


    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        InstantiatePlayer();

        gameState = GameState.countdown;
        countDownStart = DateTime.Now;
    }

    public void Start()
    {
        stageData = StagePaletteMB.instance.GetStageData(startingStage);
        stageData.RandomSelect();
        ReloadStageData();

        levelMngr.PlacePlayer(player);
    }

    public void Update()
    {
        KbdDebugCommands();

        UpdateState();
    }

    public void KbdDebugCommands()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.SetEquipBall("BallEQ_Single");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            player.SetEquipBall("BallEQ_TripleSpread");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            player.SetEquipBall("BallEQ_TripleRapid");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Game");
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            PrepNextWave();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            PrepNextStage();
        }
    }

    public void UpdateState()
    {
        switch (gameState)
        {
            case GameState.countdown:
                if ((DateTime.Now - countDownStart).TotalSeconds >= countDownTime)
                {
                    enemyspawn.StartWave(player, stageData.currentEnemySpawnData, levelMngr);
                    gameState = GameState.combat;
                }
                break;
            case GameState.combat:
                if (!enemyspawn.IsWave())
                {
                    gameState = GameState.complete;
                }
                break;
            case GameState.complete:
                break;
        }
    }

    public void InstantiatePlayer()
    {
        string name = string.Format("PlayerUnit");
        GameObject go = Instantiate(playerPF);
        go.name = name;
        go.transform.position = Vector3.zero;
        player = go.GetComponentInChildren<PlayerMB>();
    }

    public void SetLevel(LevelData level)
    {
        levelMngr.SetLevel(level);
        levelMngr.PlacePlayer(player);
    }

    public void ResetLevel()
    {
        levelMngr.ResetLevel();
        levelMngr.PlacePlayer(player);
    }

    public void ReloadStageData()
    {
        LevelData level = LevelPaletteMB.instance.GetLevelData(stageData.currentLevel);
        if (level == null) level = LevelPaletteMB.instance.GetLevelData("default");
        if (levelMngr == null)
        {
            levelMngr = Instantiate(levelMngrPF);
            levelMngr.name = "LevelManager";
        }
        SetLevel(level);
    }

    public void PrepNextWave()
    {
        if (gameState != GameState.complete) return;

        gameState = GameState.countdown;
        countDownStart = DateTime.Now;
        enemyspawn.AbortWave();
    }

    public void PrepNextStage()
    {
        if (gameState != GameState.complete) return;

        stageData.RandomSelect();
        ReloadStageData();

        PrepNextWave();
    }

    public enum GameState
    {
        countdown,
        combat,
        complete,
    }
    
}
