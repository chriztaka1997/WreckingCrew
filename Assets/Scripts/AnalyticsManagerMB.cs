using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Analytics;

public class AnalyticsManagerMB : MonoBehaviour
{
    public static AnalyticsManagerMB instance { get; private set; }

    public DateTime waveStart, waveEnd;

    public Dictionary<string, object> waveEndDict;
    public bool ignoreNextWaveData;

    public Dictionary<string, object> timeInGameState;
    public Dictionary<PlayerMB.ActionState, List<float>> timeInPlayerState;

    public enum EnemyHitTypes {BallSpin, BallThrow, BallFree, EnemyRecoil, EnemyDead};


    public int enemiesProcessed;
    public Dictionary<string, object> enemyDataDict;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        waveEndDict = new Dictionary<string, object>();
        ignoreNextWaveData = false;

        timeInGameState = new Dictionary<string, object>();
        ResetTimeInGameState();

        timeInPlayerState = new Dictionary<PlayerMB.ActionState, List<float>>();
        ResetTimeInPlayerState();

        enemyDataDict = new Dictionary<string, object>();
        ResetEnemyData();


    }

    public void ResetTimeInGameState()
    {
        foreach (string s in Enum.GetNames(typeof(GameManagerMB.GameState)))
        {
            timeInGameState[s] = 0f;
        }
    }

    public void ResetTimeInPlayerState()
    {
        foreach (PlayerMB.ActionState s in Enum.GetValues(typeof(PlayerMB.ActionState)))
        {
            if (!timeInPlayerState.ContainsKey(s)) timeInPlayerState[s] = new List<float>();
            else timeInPlayerState[s].Clear();
        }
    }

    public void ResetEnemyData()
    {
        enemiesProcessed = 0;
        foreach (string s in Enum.GetNames(typeof(EnemyHitTypes)))
        {
            enemyDataDict[string.Format("{0} Collision", s)] = 0.0f;
            enemyDataDict[string.Format("{0} Killed", s)] = 0.0f;
        }
    }

    public Dictionary<string, object> GetPlayerStateTimeDict()
    {
        var retVal = new Dictionary<string, object>();
        float totalTime = 0f;
        foreach (PlayerMB.ActionState s in Enum.GetValues(typeof(PlayerMB.ActionState)))
        {
            float timeInState = 0f;
            totalTime += timeInState;
            foreach (float f in timeInPlayerState[s])
            {
                timeInState += f;
            }
            if (timeInPlayerState[s].Count != 0)
                retVal[string.Format("{0} Avg Time", s.ToString())] = timeInState / timeInPlayerState[s].Count;
        }
        retVal["Total Time"] = totalTime;

        return retVal;
    }

    public static void SendWaveEndAnalytics(string upgradeChosen)
    {
        if (instance.ignoreNextWaveData)
        {
            instance.ignoreNextWaveData = false;
            instance.ResetTimeInGameState();
            instance.ResetTimeInPlayerState();
            instance.ResetEnemyData();
            Debug.Log("Wave End: Data Ignored");
            return;
        }


        GameManagerMB gmRef = GameManagerMB.instance;

        //instance.waveEndDict["player stats"] = gmRef.player.stats.ToJsonString();
        instance.waveEndDict["stage name"] = gmRef.startingStage;
        instance.waveEndDict["level name"] = gmRef.stageData.currentLevel;
        instance.waveEndDict["time in wave"] = instance.timeInGameState[GameManagerMB.GameState.combat.ToString()];

        // maybe add enemy spawn later
        instance.waveEndDict["upgrades"] = gmRef.upgradeMngr.UpgradeStringListJson();
        instance.waveEndDict["upgrade selected"] = upgradeChosen;

        {
            AnalyticsResult analyticsResult = AnalyticsEvent.LevelComplete("Wave End", instance.waveEndDict);
            Debug.Log("Wave End Result: " + analyticsResult);
        }
        instance.ResetTimeInGameState();

        // Send Action State Time
        {
            AnalyticsResult analyticsResult = Analytics.CustomEvent("Wave End: Player State Data", instance.GetPlayerStateTimeDict());
            Debug.Log("Wave End: Player State Data Results: " + analyticsResult);
        }
        instance.ResetTimeInPlayerState();

        // Send Action State Time
        {
            AnalyticsResult analyticsResult = Analytics.CustomEvent("Wave End: Enemy Data", instance.enemyDataDict);
            Debug.Log("Wave End: Enemy Data Results: " + analyticsResult);
        }
        instance.ResetEnemyData();
    }

    public static void SpinStartAnalytics(float spinSpd)
    {
        //instance.SetWaveEndDictionary();
        //dict.Add("ball spin speed", Mathf.Abs(spinSpd));
        //dict.Add("ball spin CCW?", spinSpd >= 0);
        //AnalyticsResult analyticsResult = Analytics.CustomEvent("Spin Start", dict);
        //Debug.Log("Spin Start Result: " + analyticsResult);
    }

    public static void ThrowAnalytics(float spinSpd)
    {
        //var dict = MakeBaseDictionary();
        //dict.Add("ball spin speed", Mathf.Abs(spinSpd));
        //dict.Add("ball spin CCW?", spinSpd >= 0);
        //AnalyticsResult analyticsResult = Analytics.CustomEvent("Throw", dict);
        //Debug.Log("Throw Result: " + analyticsResult);
    }

    public static void PlayerStateChangeAnalytics(PlayerMB.ActionState previousState, float timeInState)
    {
        instance.timeInPlayerState[previousState].Add(timeInState);
    }

    public static void GameStateChangeAnalytics(GameManagerMB.GameState previousState, float timeInState)
    {
        instance.timeInGameState[previousState.ToString()] = (float)instance.timeInGameState[previousState.ToString()] + timeInState;
    }

    public static void EnemyDeathAnalytics(Dictionary<string, object> collisionAnalytics, Dictionary<string, object> killedAnalytics)
        => instance.LocalEnemyDeathAnalytics(collisionAnalytics, killedAnalytics);
    public void LocalEnemyDeathAnalytics(Dictionary<string, object> collisionAnalytics, Dictionary<string, object> killedAnalytics)
    {
        foreach (string s in Enum.GetNames(typeof(EnemyHitTypes)))
        {
            string collisionKey = string.Format("{0} Collision", s);
            string killedKey = string.Format("{0} Killed", s);
            enemyDataDict[collisionKey] = RunningAvg((float)enemyDataDict[collisionKey], enemiesProcessed, (int)collisionAnalytics[s]);
            enemyDataDict[killedKey] = RunningAvg((float)enemyDataDict[killedKey], enemiesProcessed, (int)collisionAnalytics[s]);
        }
        enemiesProcessed++;
    }

    public static float RunningAvg(float oldAvg, int oldCount, float newVal)
    {
        return ((oldAvg * oldCount) + newVal) / (oldCount + 1);
    }

    public static void PlayerDeathAnalytics(string cause)
    {
        GameManagerMB gmRef = GameManagerMB.instance;

        Dictionary<string, object> deadDict = new Dictionary<string, object>();

        deadDict["stage name"] = gmRef.startingStage;
        deadDict["level name"] = gmRef.stageData.currentLevel;
        deadDict["player maxHP"] = (float)gmRef.player.stats.maxHP;
        deadDict["death cause"] = cause;
        deadDict["level progress"] = gmRef.enemyspawn.GetWaveProgressRatio();
        deadDict["player state"] = gmRef.player.actionState;

        {
            AnalyticsResult analyticsResult = AnalyticsEvent.LevelFail("Player Died", deadDict);
            Debug.Log("Player Died Results: " + analyticsResult);
        }
        instance.ResetEnemyData();
    }

    public static void IgnoreNextWaveAnalytics() => instance.ignoreNextWaveData = true;
}
