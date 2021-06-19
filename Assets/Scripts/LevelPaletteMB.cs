using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LevelPaletteMB : MonoBehaviour
{
    public static LevelPaletteMB instance { get; private set; }

    public List<LevelJsonPair> jsonPairs;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public string GetJsonString(string name)
    {
        if (jsonPairs.Exists(ballEQ => ballEQ.name == name))
        {
            return jsonPairs.Find(ballEQ => ballEQ.name == name).json;
        }
        return null;
    }

    public LevelData GetLevelData(string name) => LevelData.Deserialize(GetJsonString(name));
}

[Serializable]
public struct LevelJsonPair
{
    public string name;
    [TextArea]
    public string json;
}
