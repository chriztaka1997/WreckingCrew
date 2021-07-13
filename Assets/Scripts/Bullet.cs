using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public PlayerMB player;
    public float moveSpeed = 7f;
    public float attack = 5f;
    public float deathTime = 2f;
    private Rigidbody2D rb;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent< Rigidbody2D > ();
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        rb.velocity = direction * moveSpeed;
        Destroy(gameObject, deathTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
