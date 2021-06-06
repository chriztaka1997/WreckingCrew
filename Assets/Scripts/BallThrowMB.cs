using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrowMB : BallMB
{
    public BallState state;
    public float returnForceMag;
    public float throwChainLengthSet;

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
                base.UpdateForces();
                break;
            case BallState.external:
                break;
            case BallState.thrown:
                AddThrownForce();
                break;
            case BallState.returning:
                AddReturnForce();
                break;
        }
    }

    public void AddThrownForce()
    {
        float chainLengthNow = Vector2.Distance(anchorTransform.position, thisTransform.position);

        float chainLengthDiff = (chainLengthNow - throwChainLengthSet);
        if (chainLengthDiff < 0f) chainLengthDiff = 0f;

        float chainForceMag = chainLengthDiff * chainModulus; // Force in (mass * len / sec^2)
        AddForceTowardsAnchor(chainForceMag);
    }

    public void AddExternSpinForce() => base.UpdateForces();

    public void AddReturnForce() => AddForceTowardsAnchor(returnForceMag);

    public enum BallState
    {
        normal, // like normal ball
        external, // no forces, implied velocity control externally
        thrown, // no chain force
        returning, // on the way back to player with constant force
    }
}
