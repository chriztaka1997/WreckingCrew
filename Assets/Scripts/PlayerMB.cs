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

    public ActionState actionState;

    public MoveType moveType;

    public KeyManager throwChargeKey, throwFreeKey;

    const float kbdDist = 1.0f;

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
        throwChargeKey.Update();
        throwFreeKey.Update();

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

    

    public void UpdateAction(float dt)
    {
        switch (actionState)
        {
            case ActionState.normal:
                if (throwChargeKey.GetKeyDown)
                {
                    actionState = ActionState.throwCharge;
                    primaryBall.state = BallThrowMB.BallState.external;
                    primaryBall.InitThrowCharge();
                    break;
                }
                if (throwFreeKey.GetKeyDown)
                {
                    actionState = ActionState.thrown;
                    primaryBall.state = BallThrowMB.BallState.thrown;
                }
                break;
            case ActionState.throwCharge:
                if (!throwChargeKey.GetKey)
                {
                    if (primaryBall.ThrowAngleCorrect(targetPos))
                    {
                        actionState = ActionState.thrown;
                        primaryBall.state = BallThrowMB.BallState.thrown;
                        primaryBall.InitThrow(targetPos);
                    }
                    else
                    {
                        actionState = ActionState.throwPreRelease;
                    }
                }
                break;
            case ActionState.throwPreRelease:
                if (primaryBall.ThrowAngleCorrect(targetPos))
                {
                    actionState = ActionState.thrown;
                    primaryBall.state = BallThrowMB.BallState.thrown;
                    primaryBall.InitThrow(targetPos);
                }
                break;

            case ActionState.thrown:
                //if (Vector2.Dot(pBallRigidbody.velocity, (thisTransform.position - pBallTransform.position).normalized) >= 10.0f)
                if (throwChargeKey.GetKey)
                {
                    actionState = ActionState.returning;
                    primaryBall.state = BallThrowMB.BallState.returning;
                }
                break;
            case ActionState.returning:
                if (primaryBall.IsReturnedDistance())
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
                primaryBall.SpinBall(dt);
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


