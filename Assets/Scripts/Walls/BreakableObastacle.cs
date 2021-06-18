using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObastacle : MonoBehaviour
{
    public GameObject m;
    private int hitPoint = 3;//After hitPoint tims collision with the ball, the obstacle will be destroied.
    public uint collisionDirection;//2 -- horizontal, 1 -- vertical, 3 -- both
    private float collisionVelocityThreshold = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ball" && CollisionVelocityCheck(collision.rigidbody.velocity))
        {
            hitPoint--;
            if (hitPoint == 0)
                Destroy(this.gameObject);
            else if (hitPoint == 1)
                this.gameObject.GetComponent<Renderer>().material.color = Color.red;
            else if (hitPoint == 2)
                this.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        }
    }

    private bool CollisionVelocityCheck(Vector2 otherVelocity)
    {
        if (collisionDirection == 1)
        {
            return Math.Abs(otherVelocity.x) >= collisionVelocityThreshold;
        }
        else if (collisionDirection == 2)
        {
            return Math.Abs(otherVelocity.y) >= collisionVelocityThreshold;
        }
        else if (collisionDirection == 3)
        {
            return Math.Max(Math.Abs(otherVelocity.x), Math.Abs(otherVelocity.y)) >= collisionVelocityThreshold;
        }
        else
            return false;
    }
}
