using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Bullet bullet;
    public float fireDelay = 0.5f;
    public float nextFireTime;
    public PlayerMB player;

    // Start is called before the first frame update
    void Start()
    {
        nextFireTime = Time.time;
        player = GameManagerMB.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextFireTime)
        {
            Bullet a = Instantiate(bullet, transform);
            a.player = player;
            nextFireTime = Time.time + fireDelay;
        }
    }


}
