using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Enemymovement : MonoBehaviour
{
    public Transform player;
    public PlayerMB playerObject;
    public float moveSpeed = 5f;
    public float currentHP;
    public float maxHP;
    public float attack;
    public float weight;
    public float fixedDamage = 5; //Fixed enemy collision damage

    public HP_BarMB hpBar;
    public States currentState;
    public float turnBack = 1f;
    public float deathTime = 1f;
    public float knockBackSpeed = 6f;

    [SerializeField]
    private Rigidbody2D rb;
    private float hitTime;
    private Vector2 movement;
    private Vector3 lastFrameVelocity; //use for recoil movement


    //This is to send to the analytics
    public Dictionary<string, object> collisionAnalytics = new Dictionary<string, object>
    {
        {"Stage", 1 },{"BallSpin",0},{"BallThrow",0},{"BallFree",0},
        {"EnemyRecoil",0},{"EnemyDead",0}
    };
    public Dictionary<string, object> killedAnalytics = new Dictionary<string, object>
    {
        {"Stage", 1 },{"BallSpin",0},{"BallThrow",0},{"BallFree",0},
        {"EnemyRecoil",0},{"EnemyDead",0}
    };
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
        playerObject = player.GetComponent<PlayerMB>();
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnDestroy()
    {
        ObjectPooler.SharedInstance.SpawnCoin(gameObject.transform.position);
        ObjectPooler.SharedInstance.SpawnHealthPotion(gameObject.transform.position);
    }

    void FixedUpdate()
    {
        Vector3 direction;
        if ((Time.time - hitTime >= turnBack) && (currentState == States.recoil))
        {
            currentState = States.normal;
            rb.velocity = lastFrameVelocity;
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
                direction.Normalize();
                rb.velocity = direction * knockBackSpeed;
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
        lastFrameVelocity = rb.velocity;

        //Track enemy collision with ball
        switch (playerObject.actionState)
        {
            case PlayerMB.ActionState.normal:
                collisionAnalytics["BallFree"] = (int)collisionAnalytics["BallFree"] + 1;
                break;
            case PlayerMB.ActionState.moveSpin:
                collisionAnalytics["BallSpin"] = (int)collisionAnalytics["BallSpin"] + 1;
                break;
            case PlayerMB.ActionState.throwCharge:
                collisionAnalytics["BallSpin"] = (int)collisionAnalytics["BallSpin"] + 1;
                break;
            case PlayerMB.ActionState.throwPreRelease:
                collisionAnalytics["BallSpin"] = (int)collisionAnalytics["BallSpin"] + 1;
                break;
            case PlayerMB.ActionState.thrown:
                collisionAnalytics["BallThrow"] = (int)collisionAnalytics["BallThrow"] + 1;
                break;
            case PlayerMB.ActionState.returning:
                collisionAnalytics["BallFree"] = (int)collisionAnalytics["BallFree"] + 1;
                break;
            case PlayerMB.ActionState.iframes:
                collisionAnalytics["BallFree"] = (int)collisionAnalytics["BallFree"] + 1;
                break;
            case PlayerMB.ActionState.knockback:
                collisionAnalytics["BallFree"] = (int)collisionAnalytics["BallFree"] + 1;
                break;
        }

        if (currentHP <= 0)
        {

            //Track the number of dead enemy based on the attack of the player
            switch (playerObject.actionState)
            {
                case PlayerMB.ActionState.normal:
                    killedAnalytics["BallFree"] = (int)killedAnalytics["BallFree"] + 1;
                    break;
                case PlayerMB.ActionState.moveSpin:
                    killedAnalytics["BallSpin"] = (int)killedAnalytics["BallSpin"] + 1;
                    break;
                case PlayerMB.ActionState.throwCharge:
                    killedAnalytics["BallSpin"] = (int)killedAnalytics["BallSpin"] + 1;
                    break;
                case PlayerMB.ActionState.throwPreRelease:
                    killedAnalytics["BallSpin"] = (int)killedAnalytics["BallSpin"] + 1;
                    break;
                case PlayerMB.ActionState.thrown:
                    killedAnalytics["BallThrow"] = (int)killedAnalytics["BallThrow"] + 1;
                    break;
                case PlayerMB.ActionState.returning:
                    killedAnalytics["BallFree"] = (int)killedAnalytics["BallFree"] + 1;
                    break;
                case PlayerMB.ActionState.iframes:
                    killedAnalytics["BallFree"] = (int)killedAnalytics["BallFree"] + 1;
                    break;
                case PlayerMB.ActionState.knockback:
                    killedAnalytics["BallFree"] = (int)killedAnalytics["BallFree"] + 1;
                    break;
            }


            currentState = States.dead;
            Vector3 direction = transform.position - player.position;
            moveEnemy(direction);
            Destroy(gameObject, deathTime);
        }
        else
        {


            hitTime = Time.time;
            lastFrameVelocity = rb.velocity;
            currentState = States.recoil;
            Vector3 direction = transform.position - player.position;
            moveEnemy(direction);
        }


    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Enemymovement collider = collision.gameObject.GetComponent<Enemymovement>();
        if ((collision.gameObject.tag == "Enemy") && (collider.currentState == States.recoil || collider.currentState == States.dead))
        {
            //This is for ana
            switch (collider.currentState)
            {
                case States.recoil:
                    collisionAnalytics["EnemyRecoil"] = (int)collisionAnalytics["EnemyRecoil"]+1;
                    break;
                case States.dead:
                    collisionAnalytics["EnemyDead"] = (int)collisionAnalytics["EnemyDead"] + 1;
                    break;
            }
            switch (currentState)
            {
                case States.dead:
                    break;

                case States.normal:


                    // Collision with recoil or dead enemy will effect the hp
                    //check if the enemy hp is 0 or below zero 
                    //if it is, destroy
                    //if not, change state to recoil
                    AlterHP(-fixedDamage);
                    if (currentHP <= 0)
                    {

                        //Track the number of dead enemy based on the attack of the player
                        switch (collider.currentState)
                        {
                            case States.recoil:
                                killedAnalytics["EnemyRecoil"] = (int)killedAnalytics["EnemyRecoil"] + 1;
                                break;
                            case States.dead:
                                killedAnalytics["EnemyDead"] = (int)killedAnalytics["EnemyDead"] + 1;
                                break;
                        }


                        currentState = States.dead;
                        Vector3 direction = transform.position - player.position;
                        moveEnemy(direction);
                        Destroy(gameObject, deathTime);
                    }
                    else
                    {
                        hitTime = Time.time;
                        lastFrameVelocity = rb.velocity;
                        rb.velocity = collision.relativeVelocity;
                        currentState = States.recoil;
                    }

                    break;

                case States.recoil:
                    AlterHP(-fixedDamage);

                    if (currentHP <= 0)
                    {

                        //Track the number of dead enemy based on the attack of the player
                        switch (collider.currentState)
                        {
                            case States.recoil:
                                killedAnalytics["EnemyRecoil"] = (int)killedAnalytics["EnemyRecoil"] + 1;
                                break;
                            case States.dead:
                                killedAnalytics["EnemyDead"] = (int)killedAnalytics["EnemyDead"] + 1;
                                break;
                        }



                        currentState = States.dead;
                        Vector3 direction = transform.position - player.position;
                        moveEnemy(direction);
                        Destroy(gameObject, deathTime);
                    }
                    else
                    {
                        hitTime = Time.time;
                    }
                    break;

                case States.respawn:
                    break;

            }
        }
    }
}
