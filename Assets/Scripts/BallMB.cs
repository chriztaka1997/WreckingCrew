using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMB : MonoBehaviour
{
    public GameObject anchor;
    protected BallMB anchorBallRef;
    public Rigidbody2D thisRigidbody { get; protected set; }

    public float fixedZ;
    public float chainLengthSet;
    public float chainModulus; // (Force / len) or (mass / sec^2)
    public float maxForce;
    public float groundFrictionForce;
    public float groundedVelocity;



    public Transform thisTransform => gameObject.transform;
    public Transform anchorTransform => anchor.transform;


    public virtual void Start()
    {
        thisRigidbody = gameObject.GetComponent<Rigidbody2D>();

        Vector3 newPos = thisTransform.position;
        newPos.z = fixedZ;
        thisTransform.position = newPos;

        anchorBallRef = anchor.GetComponent<BallMB>();
    }

    public virtual void FixedUpdate()
    {
        UpdateForces();
    }

    protected void AddForceTowardsAnchor(float magnitude)
    {
        if (magnitude > maxForce) magnitude = maxForce;

        Vector2 chainForce = (anchorTransform.position - thisTransform.position).normalized * magnitude;

        thisRigidbody.AddForce(chainForce);

        if (anchorBallRef != null)
        {
            anchorBallRef.thisRigidbody.AddForce(-chainForce);
        }
    }

    public virtual void UpdateForces()
    {
        float chainLengthNow = Vector2.Distance(anchorTransform.position, thisTransform.position);

        float chainLengthDiff = (chainLengthNow - chainLengthSet);
        if (chainLengthDiff < 0f)
        {
            chainLengthDiff = 0f;
            if (thisRigidbody.velocity.magnitude <= groundedVelocity)
            {
                Vector2 looseDragForce = thisRigidbody.velocity.normalized * -groundFrictionForce;
                thisRigidbody.AddForce(looseDragForce);
            }
        }

        float chainForceMag = chainLengthDiff * chainModulus; // Force in (mass * len / sec^2)
        AddForceTowardsAnchor(chainForceMag);
    }
}
