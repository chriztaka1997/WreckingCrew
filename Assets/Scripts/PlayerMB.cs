using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMB : MonoBehaviour
{
    public BallEquipMB ballEquip;

    public Vector3 targetPos;
    public float fixedZ;
    public float maxSpeed; // units per sec

    public ActionState actionState;

    public MoveType moveType;

    public bool aimTypeDirect; // true means aimed directly at cursor
    public float throwAngleWiggle; // degrees either way

    public KeyManager throwChargeKey, throwFreeKey;

    const float kbdDist = 1.0f;

    public Transform thisTransform => gameObject.transform;

    public void Start()
    {
        SetEquipBall("BallEQ_Single");

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

    public void SetEquipBall(string name)
    {
        if (ballEquip != null) Destroy(ballEquip.gameObject);
        ballEquip = Instantiate(PrefabPaletteMB.instance.GetBallEQ_Prefab(name));
        if (ballEquip != null) ballEquip.SetEquip(this);
        else Debug.Log(string.Format("Ball equip with the name \"{0}\" could not be found", name));
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
                    ballEquip.SetState(BallThrowMB.BallState.external);
                    ballEquip.InitThrowCharge();
                    break;
                }
                if (throwFreeKey.GetKeyDown)
                {
                    actionState = ActionState.thrown;
                    ballEquip.SetState(BallThrowMB.BallState.thrown);
                }
                break;
            case ActionState.throwCharge:
                if (!throwChargeKey.GetKey)
                {
                    if (ballEquip.ThrowAngleCorrect(targetPos, throwAngleWiggle, aimTypeDirect))
                    {
                        actionState = ActionState.thrown;
                        ballEquip.SetState(BallThrowMB.BallState.thrown);
                        ballEquip.InitThrow(targetPos, aimTypeDirect);
                    }
                    else
                    {
                        actionState = ActionState.throwPreRelease;
                    }
                }
                break;
            case ActionState.throwPreRelease:
                if (ballEquip.ThrowAngleCorrect(targetPos, throwAngleWiggle, aimTypeDirect))
                {
                    actionState = ActionState.thrown;
                    ballEquip.SetState(BallThrowMB.BallState.thrown);
                    ballEquip.InitThrow(targetPos, aimTypeDirect);
                }
                break;

            case ActionState.thrown:
                // suggestion: maybe add way to set return without button in range
                if (throwChargeKey.GetKey)
                {
                    actionState = ActionState.returning;
                    ballEquip.SetState(BallThrowMB.BallState.returning);
                }
                break;
            case ActionState.returning:
                if (ballEquip.AllReturned())
                {
                    actionState = ActionState.normal;
                    ballEquip.SetState(BallThrowMB.BallState.normal);
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
                ballEquip.DoSpin(dt);
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


