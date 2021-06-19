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

    //private List<string> STATES= new List<string>() { "Spawning", "Alive", "Recoil", "Dead" };
    public States currentState;
    //private string currentState = "Alive";
    private bool gotHit;
    private float hitTime;
    private float turnBack = 1f;

    private Vector2 movement;
    private Vector3 fixedAway;

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
        }

        switch (currentState)
        {
            case States.respawn:
                direction = Vector3.zero;
                break;
            case States.normal:
                direction = player.position - transform.position;
                break;
            default:
                if (gotHit)
                {
                    fixedAway = transform.position - player.position;
                    gotHit = false;
                }
                direction = fixedAway;
                break;
        }
        direction.Normalize();
        movement = direction;
        
        moveEnemy(movement);
    }

    void moveEnemy(Vector2 direction)
    {
        switch (currentState)
        {
            case States.dead:
                rb.velocity = direction * moveSpeed;
                break;

            default:
                if (currentState == States.recoil)
                {
                    rb.MovePosition((Vector2)transform.position +
                    (direction * 6 * Time.deltaTime));
                }
                else
                {
                    rb.MovePosition((Vector2)transform.position +
                    (direction * moveSpeed * Time.deltaTime));
                }
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
        gotHit = true;
        hitTime = Time.time;
        currentState = States.recoil;

        if (currentHP <= 0)
        {
            //move = false;
            currentState = States.dead;
            moveSpeed = 10f;
            Destroy(gameObject, 1f);
        }


    }

    
}
