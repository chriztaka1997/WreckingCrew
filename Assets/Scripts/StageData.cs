using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

[Serializable]
public class StageData
{
    public List<string> levelNames;
    public List<EnemySpawnData> enemySpawns;

    [JsonIgnore]
    public string currentLevel { get; private set; }
    [JsonIgnore]
    public EnemySpawnData currentEnemySpawnData { get; private set; }


    [JsonConstructor]
    public StageData(List<string> levelNames, List<EnemySpawnData> enemySpawns)
    {
        this.levelNames = new List<string>(levelNames);
        this.enemySpawns = new List<EnemySpawnData>(enemySpawns);

        RandomSelect();
    }

    public void RandomSelect()
    {
        System.Random rng = new System.Random();
        currentLevel = levelNames[Utils.BindRange(rng.Next(), 0, levelNames.Count)];
        currentEnemySpawnData = enemySpawns[Utils.BindRange(rng.Next(), 0, enemySpawns.Count)];

    }

    public static StageData Deserialize(string jsonStr)
    {
        try
        {
            return JsonConvert.DeserializeObject<StageData>(jsonStr);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public string ToJsonString()
    {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
    }
}
