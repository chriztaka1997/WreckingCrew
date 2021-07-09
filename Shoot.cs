using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Bullet bullet;
    public float nextFireTime;
    public float fireDelay = 0.5f;
    public PlayerMB player;

    // Start is called before the first frame update
    void Start()
    {
        nextFireTime = Time.time;
        player = GameManagerMB.instance.player;
        Debug.Log(player.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (nextFireTime < Time.time)
        {
            Vector3 direction = player.transform.position - transform.position;
            
            Bullet a= Instantiate(bullet, transform.position, transform.rotation);
            a.player = player.transform;
            nextFireTime = Time.time + fireDelay;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
