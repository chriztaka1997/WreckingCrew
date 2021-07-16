using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : Enemymovement
{
    public float numBallOnDeath;


    public override void Start()
    {
        base.Start();
        deathTime = 0f;
        numBallOnDeath = 7f;
    }

    public override void killed()
    {
        base.killed();
        for (int i = 0; i < numBallOnDeath; i++)
        {
            float angle = i * 360f / numBallOnDeath;
            GameObject temp = ObjectPooler.SharedInstance.GetPooledObject("Bullet");
            temp.SetActive(true);
            temp.transform.position = transform.position;

            Vector3 direction = (Quaternion.Euler(0, 0, angle) * Vector3.up);

            Rigidbody2D rb = temp.GetComponent<Rigidbody2D>();
            Bullet tempBull = temp.GetComponent<Bullet>();
            rb.velocity = direction * tempBull.moveSpeed;
        }
    }
}
