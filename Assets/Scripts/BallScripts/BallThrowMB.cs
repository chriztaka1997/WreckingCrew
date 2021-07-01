using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrowMB : BallMB
{
    public BallState state;
    public float returnForceMag;
    public float throwChainLengthSet;
    public float stuckDuration; // seconds until considered stuck


    public float lengthReturnedRatio; // at this ratio, the ball is returned
    public float spinSpd { get; private set; }

    public Action<BallThrowMB, Collider2D> onCollisionDelegate;

    public bool spinDirCCW => spinSpd >= 0;

    private Vector2 lastAng;
    //private DateTime stuckStart;
    public bool isStuck;

    public override void Start()
    {
        base.Start();
        state = BallState.normal;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateIsStuck();
    }

    public override void UpdateForces()
    {
        switch (state)
        {
            case BallState.normal:
                base.UpdateForces();
                break;
            case BallState.spin:
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

    public void AddReturnForce() => AddForceTowardsAnchor(returnForceMag);

    public bool ThrowAngleCorrect(Vector3 targetPos, float throwAngleWiggle, bool aimTypeDirect)
    {
        float targetAng = spinDirCCW ? 90.0f : -90.0f;
        Vector2 throwTrajectory = aimTypeDirect ? targetPos - thisTransform.position : targetPos - anchorTransform.position;
        float angle = targetAng - Vector2.SignedAngle(thisTransform.position - anchorTransform.position, throwTrajectory);
        return Mathf.Abs(angle) <= throwAngleWiggle;
    }

    public float GetTangentSpd()
    {
        Vector2 playerToBall = thisTransform.position - anchorTransform.position;
        Vector2 tangentCCW = Quaternion.AngleAxis(90, new Vector3(0, 0, 1)) * playerToBall.normalized;
        float spinCcwAmount = Vector2.Dot(thisRigidbody.velocity, tangentCCW);
        Vector2 tanVel = spinCcwAmount * tangentCCW;
        float tanSpd = tanVel.magnitude;
        if (spinCcwAmount < 0) tanSpd *= -1;
        return tanSpd;
    }

    public float GetConservedSpinSpd(PlayerMB player, BallEquipMB ballEquip)
    {
        Vector2 playerToBall = thisTransform.position - anchorTransform.position;
        Vector2 tangentCCW = Quaternion.AngleAxis(90, new Vector3(0, 0, 1)) * playerToBall.normalized;
        float spinCcwAmount = Vector2.Dot(thisRigidbody.velocity, tangentCCW);
        Vector2 tanVel = spinCcwAmount * tangentCCW;
        float tanSpd = tanVel.magnitude;
        // Converving momentum doesnt work now
        //tanSpd *= playerToBall.magnitude / chainLengthSet; // conserve momentum to chain length

        float minSpinSpd = player.stats.minSpinSpd * ballEquip.spinSpdMinMult;
        float maxSpinSpd = player.stats.maxSpinSpd * ballEquip.spinSpdMaxMult;

        if (tanSpd < minSpinSpd) tanSpd = minSpinSpd;
        if (tanSpd > maxSpinSpd) tanSpd = maxSpinSpd;
        if (spinCcwAmount < 0) tanSpd *= -1;

        AnalyticsManagerMB.SpinStartAnalytics(tanSpd);

        return tanSpd;
    }

    public void InitSpin(float spinSpd, PlayerMB player, BallEquipMB ballEquip)
    {
        state = BallState.spin;

        this.spinSpd = spinSpd;

        float minSpinSpd = player.stats.minSpinSpd * ballEquip.spinSpdMinMult;
        float maxSpinSpd = player.stats.maxSpinSpd * ballEquip.spinSpdMaxMult;

        if (Mathf.Abs(spinSpd) < minSpinSpd)
        {
            this.spinSpd = spinDirCCW ? minSpinSpd : -minSpinSpd;
        }
        if (Mathf.Abs(spinSpd) > maxSpinSpd)
        {
            this.spinSpd = spinDirCCW ? maxSpinSpd : -maxSpinSpd;
        }

        isStuck = false;
        lastAng = transform.position - anchorTransform.position; // intentionally reversed, will not trigger isStuck
    }

    public void InitThrow(Vector3 targetPos, bool aimTypeDirect)
    {
        state = BallState.thrown;

        Vector2 throwTrajectory = aimTypeDirect ? targetPos - thisTransform.position : targetPos - anchorTransform.position;
        Vector2 throwVec = throwTrajectory.normalized * Mathf.Abs(spinSpd);
        thisRigidbody.velocity = throwVec;


        AnalyticsManagerMB.ThrowAnalytics(spinSpd);
    }

    public void InitThrowTangent()
    {
        state = BallState.thrown;

        Vector2 playerToBall = thisTransform.position - anchorTransform.position;
        Vector2 tangentCCW = Quaternion.AngleAxis(90, new Vector3(0, 0, 1)) * playerToBall.normalized;
        float spinCcwAmount = Vector2.Dot(thisRigidbody.velocity, tangentCCW);
        Vector2 throwTrajectory = (spinCcwAmount >= 0) ? tangentCCW : tangentCCW * -1;
        Vector2 throwVec = throwTrajectory.normalized * Mathf.Abs(spinSpd);
        thisRigidbody.velocity = throwVec;
    }

    public void SpinBall(float dt, PlayerMB player, BallEquipMB ballEquip)
    {
        float minSpinSpd = player.stats.minSpinSpd * ballEquip.spinSpdMinMult;
        float maxSpinSpd = player.stats.maxSpinSpd * ballEquip.spinSpdMaxMult;

        if (Mathf.Abs(spinSpd) < maxSpinSpd)
        {
            float spinSpdRate = player.stats.spinSpdRate * ballEquip.spinSpdRateMult;

            spinSpd += spinSpdRate * dt * (spinDirCCW ? 1 : -1);
            if (Mathf.Abs(spinSpd) < minSpinSpd)
            {
                spinSpd = spinDirCCW ? minSpinSpd : -minSpinSpd;
            }
            if (Mathf.Abs(spinSpd) > maxSpinSpd)
            {
                spinSpd = spinDirCCW ? maxSpinSpd : -maxSpinSpd;
            }
        }

        Vector2 playerToBall = thisTransform.position - anchorTransform.position;
        Vector2 tangentCCW = Quaternion.AngleAxis(90, new Vector3(0, 0, 1)) * playerToBall.normalized;
        Vector2 spinVel = tangentCCW * spinSpd;
        thisRigidbody.velocity = spinVel;

        base.UpdateForces();
    }

    public bool IsReturnedDistance()
    {
        return (anchorTransform.position - thisTransform.position).magnitude <= chainLengthSet * lengthReturnedRatio;
    }

    public void UpdateIsStuck()
    {
        if (state == BallState.spin)
        {
            Vector2 toCurrent = anchorTransform.position - thisTransform.position;
            float expectedAngle = Mathf.Abs(Time.fixedDeltaTime * spinSpd / toCurrent.magnitude);
            float actualAngle = Vector2.Angle(toCurrent, lastAng);
            isStuck = actualAngle <= expectedAngle / 2;
            lastAng = anchorTransform.position - transform.position;
        }
        else isStuck = false;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        onCollisionDelegate?.Invoke(this, collider);
    }

    public enum BallState
    {
        normal, // like normal ball
        spin, // no forces, implied velocity control externally
        thrown, // no chain force
        returning, // on the way back to player with constant force
    }
}
