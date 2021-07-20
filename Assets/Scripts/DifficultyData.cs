using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DifficultyData
{
    public float diffScale; // ratio to increase difficulty per stage completed
    [NonSerialized]
    public int stagesCompleted;

    public float diff => 1.0f + (diffScale * stagesCompleted);

    public void OnStageComplete() => stagesCompleted++;

    public void Reset() => stagesCompleted = 0;

    public void ScaleEnemy(Enemymovement enemy)
    {
        enemy.maxHP *= diff;
        enemy.ResetHP();
        enemy.attack *= diff;
        enemy.weight *= Mathf.Sqrt(diff); // scale with sqrt

        if (enemy is Shoot)
        {
            Shoot shoot = (Shoot)enemy;
            shoot.numBallOnDeath *= diff;
        }
    }

    public void ScaleBullet(Bullet bullet)
    {
        bullet.ScaleAttack(diff);
    }


}
