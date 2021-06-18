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
    private string BALL_TAG = "Ball";
    private List<string> STATES= new List<string>() { "Spawning", "Alive", "Recoil", "Dead" };
    private string currentState = "Alive";
    private bool gotHit;
    private float hitTime;
    private float turnBack = 1f;

    private Vector2 movement;
    private Vector3 fixedAway;
    private bool move = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

        currentHP = maxHP;
        hpBar.InitHP(maxHP);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    
    void FixedUpdate()
    {
        Vector3 direction;
        if ((Time.time - hitTime >= turnBack)&&!(currentState ==STATES[1]))
        {
            currentState = STATES[1];
        }

        //When nothing is happen to the enemy
        if (currentState == STATES[1])
        {
            direction = player.position - transform.position;
        }

        //When the ball hits the enemy
        else 
        {
            if (gotHit)
            {
                fixedAway = transform.position - player.position;
                gotHit = false;
            }
            direction = fixedAway;
        }

        direction.Normalize();
        movement = direction;
        
        moveEnemy(movement);
    }

    void moveEnemy(Vector2 direction)
    {
        rb.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
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
        gotHit = true;
        hitTime = Time.time;
        currentState = STATES[2];

        if (currentHP <= 0)
        {
            move = false;
            currentState = STATES[3];
            moveSpeed = 10f;
            Destroy(gameObject, 1f);
        }


    }

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals(BALL_TAG))
        {
            gotHit = true;
        }
    }
    */

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
