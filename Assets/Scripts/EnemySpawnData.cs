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


}
