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
    public float droppingRate;

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
        ObjectPooler.SharedInstance.DropByEnemy(gameObject.transform.position, droppingRate);
    }

    void FixedUpdate()
    {
        Vector3 direction;
        if ((Time.time - hitTime >= turnBack)&&(currentState ==States.recoil))
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
        AnalyticsResult analyticsResult = Analytics.CustomEvent("Collision", new Dictionary<string, object>
                    {
                        { "Stage", 1},
                        { "Collided with", "Ball"},
                        {"State", playerObject.actionState}
                    });
        Debug.Log("Collision with ball Result: " + analyticsResult);

        if (currentHP <= 0)
        {
            
            //Track the number of dead enemy based on the attack of the player
            AnalyticsResult killAnalytics = Analytics.CustomEvent("Enemy killed", new Dictionary<string, object>
                    {
                        { "Stage", 1},
                        { "Collided with", "Ball"},
                        {"State", playerObject.actionState}
                    });
            Debug.Log("Enemy killed Result: " + killAnalytics);


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
        if ((collision.gameObject.tag == "Enemy")&&(collider.currentState == States.recoil || collider.currentState == States.dead))
        {
            switch (currentState)
            {
                case States.dead:
                    break;

                case States.normal:
                    AnalyticsResult analyticsResult = Analytics.CustomEvent("Collision", new Dictionary<string, object>
                    {
                        { "Stage", 1},
                        { "Collided with", "Enemy"},
                        { "State", collider.currentState}
                    });
                    Debug.Log("Collision with enemy Result: " + analyticsResult);

                    hitTime = Time.time;
                    lastFrameVelocity = rb.velocity;
                    rb.velocity = collision.relativeVelocity;
                    currentState = States.recoil;
                    break;

                case States.recoil:
                    hitTime = Time.time;
                    break;

                case States.respawn:
                    break;

            }
        }
    }


}
