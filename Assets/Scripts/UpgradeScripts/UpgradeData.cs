using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UpgradeData
{
    public string name;
    public string title;
    public string desc;
    public List<Data> data;

    [Serializable]
    public struct Data
    {
        public string name;
        public float amount;
    }
}
