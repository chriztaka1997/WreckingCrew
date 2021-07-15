using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : Enemymovement
{
    public Bullet bullet;
    public float fireDelay = 0.5f;
    public float nextFireTime;
    public float numBallOnDeath;
    public float bulletDeathTime;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        nextFireTime = Time.time;
        numBallOnDeath = 7f;
        deathTime = 0f;
        bulletDeathTime = 1f;
        //player = GameManagerMB.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextFireTime)
        {
            GameObject a = ObjectPooler.SharedInstance.GetPooledObject("Bullet");
            if(a != null)
            {
                a.SetActive(true);
                a.transform.position = transform.position;
                Rigidbody2D rb = a.GetComponent<Rigidbody2D>();
                Bullet b = a.GetComponent<Bullet>();
                b.deathTime = bulletDeathTime;
                rb.velocity = (playerObject.transform.position - transform.position).normalized * b.moveSpeed;
            }
            nextFireTime = Time.time + fireDelay;
        }
    }


    public override void killed()
    {
        base.killed();
        for (int i = 0; i <numBallOnDeath; i++)
        {
            Debug.Log("Init bullet on death #######################");
            float angle = i*360f/numBallOnDeath ;
            GameObject temp = ObjectPooler.SharedInstance.GetPooledObject("Bullet");
            temp.SetActive(true);
            temp.transform.position = transform.position;

            Vector3 direction = (Quaternion.Euler(0, 0, angle) * Vector3.up);

            Rigidbody2D rb = temp.GetComponent<Rigidbody2D>();
            Bullet tempBull = temp.GetComponent<Bullet>();
            tempBull.deathTime = bulletDeathTime;
            rb.velocity = direction * tempBull.moveSpeed;
        }
    }
}
