using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManagerMB : MonoBehaviour
{
    public static GameManagerMB instance;

    public PlayerMB player { get; private set; }
    public Enemyspawn enemyspawn;
    public GameObject playerPF;
    public BorderMB borderPF;

    public LevelManagerMB levelMngr;
    public LevelManagerMB levelMngrPF;

    public StageData stageData;
    public string startingStage;


    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    public void Start()
    {
        stageData = StagePaletteMB.instance.GetStageData(startingStage);
        stageData.RandomSelect();
        if (levelMngr == null)
        {
            LevelData level = LevelPaletteMB.instance.GetLevelData(stageData.currentLevel);
            if (level == null) level = LevelPaletteMB.instance.GetLevelData("default");
            levelMngr = Instantiate(levelMngrPF);
            levelMngr.name = "LevelManager";
            levelMngr.SetLevel(level);
        }
        if (player == null)
        {
            player = levelMngr.PlacePlayerPF(playerPF);
        }
        else levelMngr.PlacePlayer(player);

        enemyspawn.StartWave(player, stageData.currentEnemySpawnData, levelMngr);

        // set reference in other objects
        PlayerCameraMB.instance.target = player.gameObject;
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

    public void Update()
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
    }

    public enum GameState
    {
        countdown,
        spawning,
        complete,
    }
    
}
