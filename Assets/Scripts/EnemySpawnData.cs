using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EnemySpawnData
{
    public List<EnemyTypeSpawn> enemyTypes;
    public float timeSpawn; // in seconds
    public int maxEnemies;

    [Serializable]
    public struct EnemyTypeSpawn
    {
        public int totalEnemies;
        public Enemymovement enemyPF;
    }

    public EnemySpawnData(EnemySpawnData other)
    {
        enemyTypes = new List<EnemyTypeSpawn>(other.enemyTypes);
        timeSpawn = other.timeSpawn;
        maxEnemies = other.maxEnemies;
    }


}
