using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManagerMB : MonoBehaviour
{
    public static GameManagerMB instance;

    public GameState gameState;

    public UI_ManagerMB uiMngr;

    public PlayerMB player { get; private set; }
    public Enemyspawn enemyspawn;
    public GameObject playerPF;
    public BorderMB borderPF;

    public LevelManagerMB levelMngr;
    public LevelManagerMB levelMngrPF;

    public UpgradeManager upgradeMngr;

    public StageData stageData;
    public string startingStage;

    public float countDownTime;
    public float preUpgradeTime;
    private DateTime stateChangeTime;


    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        InstantiatePlayer();

        gameState = GameState.countdown;
        stateChangeTime = DateTime.Now;

        upgradeMngr = new UpgradeManager();
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
        UpdateUI();
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
                if ((DateTime.Now - stateChangeTime).TotalSeconds >= countDownTime)
                {
                    enemyspawn.StartWave(player, stageData.currentEnemySpawnData, levelMngr);
                    ChangeState(GameState.combat);
                }
                break;
            case GameState.combat:
                if (!enemyspawn.IsWave())
                {
                    ChangeState(GameState.preupgrade);
                }
                break;
            case GameState.preupgrade:
                if ((DateTime.Now - stateChangeTime).TotalSeconds >= preUpgradeTime)
                {
                    ChangeState(GameState.upgrade);
                    DisplayUpgradeSelector();
                    Time.timeScale = 0;
                }
                break;
            case GameState.upgrade:
                break;
            case GameState.complete:
                break;
        }
    }

    public void UpdateUI()
    {
        if (gameState == GameState.combat)
        {
            uiMngr.UpdateWaveProgress(enemyspawn.GetWaveProgressRatio());
        }
    }

    public void ChangeState(GameState newState)
    {
        gameState = newState;
        stateChangeTime = DateTime.Now;
        uiMngr.StateChanged();
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

        ChangeState(GameState.countdown);
        enemyspawn.AbortWave();
    }

    public void PrepNextStage()
    {
        if (gameState != GameState.complete) return;

        stageData.RandomSelect();
        ReloadStageData();

        PrepNextWave();
    }

    public void AddUpgrade(string name)
    {
        upgradeMngr.AddUpgrade(name, player);
    }

    public void UpgradeSelected(string name)
    {
        if (name != "") AddUpgrade(name);
        ChangeState(GameState.complete);
        Time.timeScale = 1.0f;
    }

    public void DisplayUpgradeSelector()
    {
        System.Random rng = new System.Random();
        List<UpgradeData> upgrades = new List<UpgradeData>();
        for (int i = 0; i < UpgradeSelectorMB.numUpgrades; i++)
        {
            upgrades.Add(UpgradePaletteMB.instance.GetRandomWeighted(rng));
        }
        uiMngr.DisplayUpgradeSelector(upgrades);
    }

    public enum GameState
    {
        countdown,
        combat,
        preupgrade,
        upgrade,
        complete,
    }
    
}


#if UNITY_EDITOR
[CustomEditor(typeof(GameManagerMB))]
public class GameManagerMB_Editor : Editor
{
    public GameManagerMB targetRef => (GameManagerMB)target;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Editor");

        if (GUILayout.Button("hp_small"))
        {
            targetRef.AddUpgrade("hp_small");
        }
    }
}
#endif