using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Analytics;

public class AnalyticsManager
{
    public static void TestAnalytics()
    {
        AnalyticsEvent.SocialShare("Real Share", "Totally Real Social Network");
    }

    public static Dictionary<string, object> MakeBaseDictionary()
    {
        GameManagerMB gmRef = GameManagerMB.instance;

        var dict = new Dictionary<string, object>();

        dict.Add("player stats", gmRef.player.stats.ToJsonString());
        dict.Add("stage name", gmRef.startingStage);
        dict.Add("level name", gmRef.stageData.currentLevel);
        //dict.Add("enemy spawn", gmRef.stageData.currentEnemySpawnData);
        dict.Add("upgrades", gmRef.upgradeMngr.UpgradeStringListJson());

        return dict;
    }

    public static void SpinStartAnalytics(float spinSpd)
    {
        var dict = MakeBaseDictionary();
        dict.Add("ball spin speed", Mathf.Abs(spinSpd));
        dict.Add("ball spin CCW?", spinSpd >= 0);
        AnalyticsResult analyticsResult = Analytics.CustomEvent("Spin Start", dict);
        Debug.Log("Spin Start Result: " + analyticsResult);
    }

    public static void ThrowAnalytics(float spinSpd)
    {
        var dict = MakeBaseDictionary();
        dict.Add("ball spin speed", Mathf.Abs(spinSpd));
        dict.Add("ball spin CCW?", spinSpd >= 0);
        AnalyticsResult analyticsResult = Analytics.CustomEvent("Throw", dict);
        Debug.Log("Throw Result: " + analyticsResult);
    }

    public static void PlayerStateChangeAnalytics(PlayerMB.ActionState previousState, float timeInState)
    {
        var dict = MakeBaseDictionary();
        dict.Add("player state", previousState);
        dict.Add("time in state", timeInState);
        AnalyticsResult analyticsResult = Analytics.CustomEvent("Player State Change", dict);
        Debug.Log("Player State Change Result: " + analyticsResult);
    }

    public static void GameStateChangeAnalytics(GameManagerMB.GameState previousState, float timeInState)
    {
        var dict = MakeBaseDictionary();
        dict.Add("game state", previousState);
        dict.Add("time in state", timeInState);
        AnalyticsResult analyticsResult = Analytics.CustomEvent("Game State Change", dict);
        Debug.Log("Game State Change Result: " + analyticsResult);
    }
}
