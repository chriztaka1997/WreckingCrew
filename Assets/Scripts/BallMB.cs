using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMB : MonoBehaviour
{
    public GameObject anchor;
    private BallMB anchorBallRef;
    public Rigidbody2D thisRigidbody;

    public float fixedZ;
    public float chainLengthSet;
    public float chainModulus; // (Force / len) or (mass / sec^2)
    public float maxForce;
    public float groundFrictionForce;
    public float groundedVelocity;



    public Transform thisTransform => gameObject.transform;
    public Transform anchorTransform => anchor.transform;


    public void Start()
    {
        thisRigidbody = gameObject.GetComponent<Rigidbody2D>();

        Vector3 newPos = thisTransform.position;
        newPos.z = fixedZ;
        thisTransform.position = newPos;

        anchorBallRef = anchor.GetComponent<BallMB>();
    }

    public void FixedUpdate()
    {
        UpdateForces();
    }

    public void UpdateForces()
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
        if (chainForceMag > maxForce) chainForceMag = maxForce;

        Vector2 chainForce = anchorTransform.position - thisTransform.position;
        chainForce.Normalize();
        chainForce *= chainForceMag;

        thisRigidbody.AddForce(chainForce);

        if (anchorBallRef != null)
        {
            anchorBallRef.thisRigidbody.AddForce(-chainForce);
        }
    }
}
