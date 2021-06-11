using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemymovement : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;
    public float weight;
    private Rigidbody2D rb;
    private Vector2 movement;
    private string BALL_TAG = "Ball";
    private bool move = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        movement = direction;
    }
    
    void FixedUpdate()
    {
        if (move)
        {
            moveEnemy(movement);
        }
    }

    void moveEnemy(Vector2 direction)
    {
        rb.MovePosition((Vector2)transform.position + (direction * moveSpeed *Time.deltaTime));
    }

    public void CollisionWithBall(BallThrowMB ball, float damage)
    {
        // the ball is what was collided with
        // damage is the damage for the enemy to take

        move = false;
        Destroy(gameObject, 1f);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Use if you want to have collision effect with other object

        //if (collision.gameObject.tag.Equals(BALL_TAG))
        //{
        //    move = false;
        //    Destroy(gameObject,1f);
        //}
    }
}
