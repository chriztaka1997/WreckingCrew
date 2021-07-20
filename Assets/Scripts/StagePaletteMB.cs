using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StagePaletteMB : MonoBehaviour
{
    public static StagePaletteMB instance { get; private set; }

    public List<StageDataPair> stagePairs;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public StageData GetStageData(string name)
    {
        if (stagePairs.Exists(stage => stage.name == name))
        {
            return stagePairs.Find(stage => stage.name == name).data;
        }
        return null;
    }


    [Serializable]
    public struct StageDataPair
    {
        public string name;
        public StageData data;
    }
}
