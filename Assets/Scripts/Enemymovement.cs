using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemymovement : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;
    public float currentHP;
    public float maxHP;
    public float attack;
    public float weight;

    public HP_BarMB hpBar;
    
    private Rigidbody2D rb;
    //private string BALL_TAG = "Ball";

    public States currentState;
    //private bool gotHit;
    private float hitTime;
    private float turnBack = 1f;

    private Vector2 movement;
    //private Vector3 fixedAway;
    private Vector3 lastFrameVelocity; //use for recoil movement

    public enum States
    {
        normal,
        recoil,
        dead,
        respawn
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

        currentHP = maxHP;
        hpBar.InitHP(maxHP);
        currentState = States.normal;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    
    void FixedUpdate()
    {
        Vector3 direction;
        if ((Time.time - hitTime >= turnBack)&&(currentState ==States.recoil))
        {
            currentState = States.normal;
            moveSpeed = 2f;
        }

        switch (currentState)
        {
            case States.respawn:
                //direction = Vector3.zero;
                break;
            case States.normal:
                direction = player.position - transform.position;
                direction.Normalize();
                movement = direction;

                moveEnemy(movement);
                break;

            case States.recoil:
                break;
            case States.dead:
                break;

        }
    }

    void moveEnemy(Vector2 direction)
    {
        switch (currentState)
        {

            case States.recoil:
                float tempSpeed = 6f;
                direction.Normalize();
                rb.velocity = direction * tempSpeed;
                break;

            case States.dead:
                moveSpeed = 10f;
                direction.Normalize();
                rb.velocity = direction * moveSpeed;
                break;

            case States.normal:
                rb.MovePosition((Vector2)transform.position +
                    (direction * moveSpeed * Time.deltaTime));
                break;
        }
       
    }

    public void SetHP(float hp)
    {
        currentHP = hp;
        hpBar.SetHP(currentHP);
    }

    public void ResetHP() => SetHP(maxHP);

    public void AlterHP(float d_hp) => SetHP(currentHP + d_hp);

    public void CollisionWithBall(BallThrowMB ball, float damage)
    {
        // the ball is what was collided with
        // damage is the damage for the enemy to take
        //knockback to give player a room to re spin the ball
        //Idea for knocking back, using the same force or speed +
        //the motion will be perpendicular to the ball motion
        //if the health is zero then the enemy will drift along with collision off

        AlterHP(-damage);
        //gotHit = true;
        lastFrameVelocity = rb.velocity;

        if (currentHP <= 0)
        {
            //move = false;
            currentState = States.dead;
            Vector3 direction = transform.position - player.position;
            moveEnemy(direction);
            Destroy(gameObject, 1f);
        }
        else
        {
            hitTime = Time.time;
            currentState = States.recoil;
            Vector3 direction = transform.position - player.position;
            moveEnemy(direction);
        }


    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        currentState = States.recoil;
        Bounce(collision.contacts[0].normal);
    }

    private void Bounce(Vector3 collisionNormal)
    {
        var speed = lastFrameVelocity.magnitude;
        var direction = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);

        //Checking to make sure that the enemy does not got back
        if (Vector3.Dot(lastFrameVelocity.normalized, direction.normalized) == -1)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb.velocity = direction * (speed - 1f);
        }


    }


}
