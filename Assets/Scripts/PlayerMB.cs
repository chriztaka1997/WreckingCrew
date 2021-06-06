using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMB : MonoBehaviour
{
    public BallThrowMB primaryBall;

    public Vector3 targetPos;
    public float fixedZ;
    public float maxSpeed; // units per sec
    public float throwAngleWiggle; // degrees either way
    public float lengthReturnedRatio; // at this ratio, the ball is returned
    public float minSpinSpd;
    public bool aimTypeDirect; // true means aimed directly at cursor

    public ActionState actionState;

    public MoveType moveType;

    public KeyCode throwChargeKey, throwFreeKey;

    const float kbdDist = 1.0f;
    private bool spinDirCCW;
    private float spinDist, spinSpd;

    public Transform thisTransform => gameObject.transform;
    public Transform pBallTransform => primaryBall.transform;
    public Rigidbody2D pBallRigidbody => primaryBall.thisRigidbody;

    public void Start()
    {
        Vector3 newPos = thisTransform.position;
        newPos.z = fixedZ;
        thisTransform.position = newPos;

        actionState = ActionState.normal;
    }

    public void FixedUpdate()
    {
        switch (moveType)
        {
            case MoveType.Mouse:
                MouseTargetPos();
                break;
            case MoveType.Kbd:
                KbdTargetPos();
                break;
        }
        UpdateAction(Time.fixedDeltaTime);
    }

    public void MouseTargetPos()
    {
        targetPos = Utils.MousePosPlane();
        targetPos.z = fixedZ;
    }

    public void KbdTargetPos()
    {
        targetPos = thisTransform.position;
        targetPos.z = fixedZ;

        if (Input.GetKey(KeyCode.W))
            targetPos.y += kbdDist;
        if (Input.GetKey(KeyCode.S))
            targetPos.y -= kbdDist;
        if (Input.GetKey(KeyCode.D))
            targetPos.x += kbdDist;
        if (Input.GetKey(KeyCode.A))
            targetPos.x -= kbdDist;
    }

    public void UpdatePos(float dt)
    {
        float maxDist = maxSpeed * dt;
        if (Vector2.Distance(targetPos, thisTransform.position) <= maxDist)
        {
            thisTransform.position = targetPos;
        }
        else
        {
            Vector3 changeVec = targetPos - thisTransform.position;
            changeVec.Normalize();
            changeVec *= maxDist;
            thisTransform.position = thisTransform.position + changeVec;
        }
    }

    public bool ThrowAngleCorrect()
    {
        float targetAng = spinDirCCW ? 90.0f : -90.0f;
        Vector2 throwTrajectory = aimTypeDirect ? targetPos - pBallTransform.position : targetPos - thisTransform.position;
        float angle = targetAng - Vector2.SignedAngle(pBallTransform.position - thisTransform.position, throwTrajectory);
        return Mathf.Abs(angle) <= throwAngleWiggle;
    }

    public void InitThrowCharge()
    {
        Vector2 playerToBall = pBallTransform.position - thisTransform.position;
        spinDist = playerToBall.magnitude;
        Vector2 tangentCCW = Quaternion.AngleAxis(90, new Vector3(0, 0, 1)) * playerToBall.normalized;
        float spinCcwAmount = Vector2.Dot(pBallRigidbody.velocity, tangentCCW);
        Vector2 spinVel = spinCcwAmount * tangentCCW;
        spinSpd = spinVel.magnitude;
        if (spinSpd < minSpinSpd) spinSpd = minSpinSpd;
        spinDirCCW = spinCcwAmount >= 0;
    }

    public void InitThrow()
    {
        Vector2 throwTrajectory = aimTypeDirect ? targetPos - pBallTransform.position : targetPos - thisTransform.position;
        Vector2 throwVec = throwTrajectory.normalized * spinSpd;
        pBallRigidbody.velocity = throwVec;
        pBallRigidbody.angularVelocity = 0;
    }

    public void SpinBall(float dt)
    {
        Vector2 playerToBall = pBallTransform.position - thisTransform.position;
        Vector2 tangentCCW = Quaternion.AngleAxis(90, new Vector3(0, 0, 1)) * playerToBall.normalized;
        Vector2 spinVel = tangentCCW * spinSpd;
        if (!spinDirCCW) spinVel = -spinVel;
        pBallRigidbody.velocity = spinVel;

        primaryBall.AddExternSpinForce();
    }

    public void UpdateAction(float dt)
    {
        switch (actionState)
        {
            case ActionState.normal:
                if (Input.GetKeyDown(throwChargeKey))
                {
                    actionState = ActionState.throwCharge;
                    primaryBall.state = BallThrowMB.BallState.external;
                    InitThrowCharge();
                    break;
                }
                if (Input.GetKeyDown(throwFreeKey))
                {
                    actionState = ActionState.thrown;
                    primaryBall.state = BallThrowMB.BallState.thrown;
                }
                break;
            case ActionState.throwCharge:
                if (!Input.GetKey(throwChargeKey))
                {
                    if (ThrowAngleCorrect())
                    {
                        actionState = ActionState.thrown;
                        primaryBall.state = BallThrowMB.BallState.thrown;
                        InitThrow();
                    }
                    else
                    {
                        actionState = ActionState.throwPreRelease;
                    }
                }
                break;
            case ActionState.throwPreRelease:
                if (ThrowAngleCorrect())
                {
                    actionState = ActionState.thrown;
                    primaryBall.state = BallThrowMB.BallState.thrown;
                    InitThrow();
                }
                break;

            case ActionState.thrown:
                //if (Vector2.Dot(pBallRigidbody.velocity, (thisTransform.position - pBallTransform.position).normalized) >= 10.0f)
                if (Input.GetKey(throwChargeKey))
                {
                    actionState = ActionState.returning;
                    primaryBall.state = BallThrowMB.BallState.returning;
                }
                break;
            case ActionState.returning:
                if ((thisTransform.position - pBallTransform.position).magnitude <= primaryBall.chainLengthSet * lengthReturnedRatio)
                {
                    actionState = ActionState.normal;
                    primaryBall.state = BallThrowMB.BallState.normal;
                    break;
                }
                break;
        }
        switch (actionState)
        {
            case ActionState.normal:
                UpdatePos(dt);
                break;
            case ActionState.throwCharge:
            case ActionState.throwPreRelease:
                SpinBall(dt);
                break;
            case ActionState.thrown:
            case ActionState.returning:
                UpdatePos(dt);
                break;
        }
    }


    public enum MoveType
    {
        Mouse,
        Kbd,
    }

    public enum ActionState
    {
        normal, // neutral state
        throwCharge, // chargeup to throw
        throwPreRelease, // waiting to throw in direction
        thrown, // ball(s) being thrown
        returning, // ball(s) on the way back to player with constant force
    }
}


