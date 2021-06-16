using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemymovement : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;
    public float weight;
    
    private Rigidbody2D rb;
    private string BALL_TAG = "Ball";

    private Vector2 movement;
    private Vector3 fixedAway;
    private bool move = true;
    private bool hit =true;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    
    void FixedUpdate()
    {
        Vector3 direction;
        if (move)
        {
            direction = player.position - transform.position;
        }
        else
        {
            if (hit)
            {
                fixedAway = transform.position - player.position;
                hit = false;
            }
            direction = fixedAway;
        }

        direction.Normalize();
        movement = direction;
        
        moveEnemy(movement);
    }

    void moveEnemy(Vector2 direction)
    {
        rb.MovePosition((Vector2)transform.position + (direction * moveSpeed *Time.deltaTime));
    }

    public void CollisionWithBall(BallThrowMB ball, float damage)
    {
        // the ball is what was collided with
        // damage is the damage for the enemy to take
        //knockback to give player a room to re spin the ball
            //Idea for knocking back, using the same force or speed +
            //the motion will be perpendicular to the ball motion
        //if the health is zero then the enemy will drift along with collision off

        move = false;
        //rb.Collider.enabled = false;
        Destroy(gameObject, 1f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals(BALL_TAG))
        {
            move = false;
            Destroy(gameObject, 1f);
        }
    }

    /**
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Use if you want to have collision effect with other object

        if (collision.gameObject.tag.Equals(BALL_TAG))
        {
            move = false;
            CircleCollider2D m_collider = GetComponent<CircleCollider2D>();
            m_collider.isTrigger = true;
            Destroy(gameObject,1f);
        }
    }
    **/
}
