using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrowMB : BallMB
{
    public BallState state;
    public float returnForceMag;
    public float lengthReturnedRatio; // at this ratio, the ball is returned
    public float throwChainLengthSet;
    public KeyCode throwKey;

    public override void Start()
    {
        base.Start();
        state = BallState.normal;
    }

    public override void UpdateForces()
    {
        switch (state)
        {
            case BallState.normal:
                if (Input.GetKey(throwKey))
                {
                    state = BallState.thrown;
                    break;
                }
                base.UpdateForces();
                break;
            case BallState.thrown:
                if (!Input.GetKey(throwKey))
                {
                    state = BallState.returning;
                    // only keep velocity in target direction
                    Vector2 towardsAnchor = (anchorTransform.position - thisTransform.position).normalized;
                    thisRigidbody.velocity = towardsAnchor * Vector2.Dot(thisRigidbody.velocity, towardsAnchor);
                    AddReturnForce();
                    break;
                }
                AddThrownForce();
                break;
            case BallState.returning:
                if ((anchorTransform.position - thisTransform.position).magnitude <= chainLengthSet * lengthReturnedRatio)
                {
                    state = BallState.normal;
                    base.UpdateForces();
                    break;
                }
                AddReturnForce();
                break;
        }
    }

    public void AddThrownForce()
    {
        float chainLengthNow = Vector2.Distance(anchorTransform.position, thisTransform.position);

        float chainLengthDiff = (chainLengthNow - throwChainLengthSet);
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

    public void AddReturnForce() => AddForceTowardsAnchor(returnForceMag);

    public enum BallState
    {
        normal, // like normal ball
        thrown, // no chain force
        returning, // on the way back to player with constant force
    }
}
