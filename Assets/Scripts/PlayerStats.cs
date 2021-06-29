using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerStats
{
    public float currentHP;
    public Stat maxHP;
    public Stat attack;
    public Stat precision;
    public Stat movSpd;
    public Stat dodge;
    public Stat healTick;
    public Stat tenacity;
    public Stat maxSpinSpd;
    public Stat minSpinSpd;
    public Stat spinSpdRate;


    [NonSerialized]
    public List<Stat> statsList;

    public void Init()
    {
        currentHP = maxHP;
        statsList = new List<Stat>();
        statsList.Add(maxHP);
        statsList.Add(attack);
        statsList.Add(precision);
        statsList.Add(movSpd);
        statsList.Add(dodge);
        statsList.Add(healTick);
        statsList.Add(tenacity);
        statsList.Add(maxSpinSpd);
        statsList.Add(minSpinSpd);
        statsList.Add(spinSpdRate);

        Reset();
    }

    public void Reset()
    {
        foreach (Stat s in statsList)
        {
            s.Reset();
        }

        currentHP = maxHP;
    }

    public void ChangeHP(float flatInc, float multInc)
    {
        float curHP_Ratio = currentHP / maxHP;
        maxHP.flatInc += flatInc;
        maxHP.multInc += multInc;
        currentHP = curHP_Ratio * maxHP;
        if (currentHP > maxHP) currentHP = maxHP;
    }
}

[Serializable]
public class Stat
{
    public float baseStat;
    [NonSerialized]
    public float flatInc;
    [NonSerialized]
    public float multInc;

    public float GetValue()
    {
        return (baseStat + flatInc) * (1 + multInc);
    }

    public void Reset()
    {
        flatInc = 0;
        multInc = 0;
    }

    public static implicit operator float(Stat s) => s.GetValue();
}
