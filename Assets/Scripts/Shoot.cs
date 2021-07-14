using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : Enemymovement
{
    public Bullet bullet;
    public float fireDelay = 0.5f;
    public float nextFireTime;
    public float numBallOnDeath = 7f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        nextFireTime = Time.time;
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

            Vector3 direction = (Quaternion.Euler(0, 0, angle) * Vector3.up);
            direction.Normalize();

            Rigidbody2D rb = temp.GetComponent<Rigidbody2D>();
            Bullet tempBull = temp.GetComponent<Bullet>();
            rb.velocity = direction * tempBull.moveSpeed;
        }
    }
}
