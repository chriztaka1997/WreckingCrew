using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerStats
{
    public float currentHP;
    public float maxHP;
    public float attack;
    public float precision;
    public float movSpd;
    public float dodge;
    public float healTick;
    // list of buffs/debuffs
    // list of ball attributes
    // list of ball equips obtained

    public void Reset()
    {
        currentHP = maxHP;
    }

}
