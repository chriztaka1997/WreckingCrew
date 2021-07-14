using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : Enemymovement
{
    public Bullet bullet;
    public float fireDelay = 0.5f;
    public float nextFireTime;
    //public PlayerMB player;

    // Start is called before the first frame update
    void Start()
    {
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


}
