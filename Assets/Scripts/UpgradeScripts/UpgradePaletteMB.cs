using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UpgradePaletteMB : MonoBehaviour
{
    public static UpgradePaletteMB instance { get; private set; }

    public List<UpgradeData> upgrades;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public UpgradeData GetUpgradeData(string name)
    {
        if (upgrades.Exists(upd => upd.name == name))
        {
            return upgrades.Find(upd => upd.name == name);
        }
        return null;
    }

    public Upgrade GetUpgrade(string name)
    {
        try
        {
            return Upgrade.UpgradeFactory(GetUpgradeData(name));
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public UpgradeData GetRandomWeighted(System.Random rng = null)
    {
        if (rng == null) rng = new System.Random();

        float totWeight = 0;
        foreach (UpgradeData upd in upgrades)
        {
            totWeight += upd.weight;
        }
        float randWeight = (float)(rng.NextDouble() * totWeight);
        foreach (UpgradeData upd in upgrades)
        {
            randWeight -= upd.weight;
            if (randWeight <= 0) return upd;
        }
        return upgrades[upgrades.Count - 1];
    }
}
