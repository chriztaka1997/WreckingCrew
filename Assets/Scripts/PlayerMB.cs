using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMB : MonoBehaviour
{
    public Vector3 targetPos;
    public float fixedZ;
    public float maxSpeed; // units per sec

    public MoveType moveType;

    const float kbdDist = 1.0f;

    public Transform thisTransform => gameObject.transform;

    public void Start()
    {
        Vector3 newPos = thisTransform.position;
        newPos.z = fixedZ;
        thisTransform.position = newPos;
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

        UpdatePos(Time.fixedDeltaTime);
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

    public enum MoveType
    {
        Mouse,
        Kbd,
    }
}


