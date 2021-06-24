using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

[Serializable]
public class StageData
{
    public List<string> levelNames;
    [JsonIgnore]
    public string currentLevel { get; private set; }

    [JsonConstructor]
    public StageData(List<string> levelNames)
    {
        this.levelNames = new List<string>(levelNames);
        ChooseLevel();
    }

    public string ChooseLevel()
    {
        System.Random rng = new System.Random();
        currentLevel = levelNames[Utils.BindRange(rng.Next(), 0, levelNames.Count)];
        return currentLevel;
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
