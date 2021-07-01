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

    public Dictionary<string, object> timeInGameState;
    public Dictionary<PlayerMB.ActionState, List<float>> timeInPlayerState;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        waveEndDict = new Dictionary<string, object>();

        timeInGameState = new Dictionary<string, object>();
        ResetTimeInGameState();

        timeInPlayerState = new Dictionary<PlayerMB.ActionState, List<float>>();
        ResetTimeInPlayerState();


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
        GameManagerMB gmRef = GameManagerMB.instance;

        //instance.waveEndDict["player stats"] = gmRef.player.stats.ToJsonString();
        instance.waveEndDict["stage name"] = gmRef.startingStage;
        instance.waveEndDict["level name"] = gmRef.stageData.currentLevel;
        instance.waveEndDict["time in wave"] = instance.timeInGameState[GameManagerMB.GameState.combat.ToString()];

        // maybe add enemy spawn later
        instance.waveEndDict["upgrades"] = gmRef.upgradeMngr.UpgradeStringListJson();
        instance.waveEndDict["upgrade selected"] = upgradeChosen;

        {
            AnalyticsResult analyticsResult = Analytics.CustomEvent("Wave End", instance.waveEndDict);
            Debug.Log("Wave End Result: " + analyticsResult);
        }

        // Send Action State Time
        {
            AnalyticsResult analyticsResult = Analytics.CustomEvent("Wave End: Player State Data", instance.GetPlayerStateTimeDict());
            Debug.Log("Wave End: Player State Data Results: " + analyticsResult);
        }

        instance.ResetTimeInGameState();
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
}
