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

    public bool tryLoadEditorLevel;

    public GameState gameState;

    public DifficultyData difficulty;

    public UI_ManagerMB uiMngr;

    public TutorialMB tutorial;
    public TutorialMB tutorialPF;

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

    private DateTime pauseStartTime;


    public void Awake()
    {
        Time.timeScale = 1.0f; // just in case

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        InstantiatePlayer();

        gameState = GameState.countdown;
        stateChangeTime = DateTime.Now;

        upgradeMngr = new UpgradeManager();
    }

    public void Start()
    {
        if (PersistentObjMB.IsStartInTutorial())
        {
            StartTutorial();
            return;
        }

        SetStage(startingStage);

#if UNITY_EDITOR
        if (LevelEditorMB.instance.json.Length != 0 && tryLoadEditorLevel)
        {
            SetLevel(LevelData.Deserialize(LevelEditorMB.instance.json));
            levelMngr.PlacePlayer(player);
        }
#endif
    }

    public void Update()
    {
        KbdDebugCommands();

        UpdateState();
        UpdateUI();
    }

    public void SetStage(string stageName)
    {
        stageData = StagePaletteMB.instance.GetStageData(stageName);
        stageData.RandomSelect();
        ReloadStageData();
        levelMngr.PlacePlayer(player);
    }

    public void KbdDebugCommands()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    player.SetEquipBall("BallEQ_Single");
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    player.SetEquipBall("BallEQ_TripleSpread");
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    player.SetEquipBall("BallEQ_TripleRapid");
        //}
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Game");
        }
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    PrepNextWave();
        //}
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    PrepNextStage();
        //}
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            enemyspawn.AbortWave();
            ChangeState(GameState.preupgrade);
            AnalyticsManagerMB.IgnoreNextWaveAnalytics();
        }
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    StartTutorial();
        //}
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
                    TutorialMB.SignalTutorial("wave");
                }
                break;
            case GameState.preupgrade:
                if ((DateTime.Now - stateChangeTime).TotalSeconds >= preUpgradeTime)
                {
                    ChangeState(GameState.upgrade);
                    DisplayUpgradeSelector();
                    StartPause();
                }
                break;
            case GameState.upgrade:
                break;
            case GameState.complete:
                break;
        }
    }

    public void StartPause()
    {
        Time.timeScale = 0;
        pauseStartTime = DateTime.Now;
    }

    public void EndPause()
    {
        Time.timeScale = 1.0f;
        player.actionStateChangeTime += DateTime.Now - pauseStartTime;
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
        AnalyticsManagerMB.GameStateChangeAnalytics(gameState, (float)(DateTime.Now - stateChangeTime).TotalSeconds);

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

    public void ResetPlayer()
    {
        //if (player != null)
        //{
        //    Destroy(player.transform.parent.gameObject);
        //}

        //InstantiatePlayer();
        upgradeMngr.ResetUpgrades(player);
        upgradeMngr.ClearUpgrades();
    }

    public void SetLevel(LevelData level)
    {
        levelMngr.SetLevel(level);
        levelMngr.PlacePlayer(player);
        ObjectPooler.SharedInstance.DestroyAllPooledObjects();
    }

    public void ResetLevel()
    {
        levelMngr.SetLevel(levelMngr.level);
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
        TutorialMB.SignalTutorial("next", false);

        ChangeState(GameState.countdown);
        enemyspawn.AbortWave();
    }

    public void PrepNextStage()
    {
        if (gameState != GameState.complete) return;
        TutorialMB.SignalTutorial("next", false);

        stageData.RandomSelect();
        ReloadStageData();

        PrepNextWave();

        difficulty.OnStageComplete();
        uiMngr.UpdateStagesText();
    }

    public void AddUpgrade(string name)
    {
        upgradeMngr.AddUpgrade(name, player);
    }

    public void UpgradeSelected(string name)
    {
        if (name != "") AddUpgrade(name);
        ChangeState(GameState.complete);
        EndPause();

        AnalyticsManagerMB.SendWaveEndAnalytics(name);
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

    public void StartTutorial()
    {
        if (tutorial == null)
        {
            tutorial = Instantiate(tutorialPF);
        }
        tutorial.StartTutorial();
    }

    public void OnGameOver()
    {
        uiMngr.OnGameOver(difficulty.stagesCompleted);
    }

    public void PlayerContinue()
    {
        player.ResetHP();
    }

    public enum GameState
    {
        countdown,
        combat,
        preupgrade,
        upgrade,
        complete,
        tutorial,
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