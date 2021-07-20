using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PrefabPaletteMB : MonoBehaviour
{
    public static PrefabPaletteMB instance { get; private set; }

    public List<NamedBallEquip> ballEQ_Prefabs;
    public BallThrowMB ballThrowPF;

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

    public BallEquipMB GetBallEQ_Prefab(string name)
    {
        if (ballEQ_Prefabs.Exists(ballEQ => ballEQ.name == name))
        {
            return ballEQ_Prefabs.Find(ballEQ => ballEQ.name == name).ballEquip;
        }
        return null;
    }

    [Serializable]
    public struct NamedBallEquip
    {
        public string name;
        public BallEquipMB ballEquip;
    }
}
