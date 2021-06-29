using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class UpgradeData
{
    public string name;
    public string title;
    public string desc;
    public Sprite sprite;
    public List<Data> data;
    public float weight = 1.0f;

    [Serializable]
    public struct Data
    {
        public string name;
        public float amount;
    }
}
