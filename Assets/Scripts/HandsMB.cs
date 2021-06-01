using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsMB : MonoBehaviour
{
    public GameObject anchor;

    public Vector2 targetAngle; // normalized vector
    public float fixedZ;
    public float maxAngSpeed; // deg / sec
    public float handDist;

    public MoveType moveType;

    const float kbdDist = 1.0f;

    public Transform thisTransform => gameObject.transform;
    public Transform anchorTransform => anchor.transform;

    public void Start()
    {
        Vector2 currAngle = ((Vector2)(thisTransform.position - anchorTransform.position)).normalized;
        if (currAngle.magnitude == 0) currAngle = Vector2.right;

        Vector3 newPos = anchorTransform.position + ((Vector3)currAngle * handDist);
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
            case MoveType.Kbd4Way:
                Kbd4WayTargetPos();
                break;
            case MoveType.Kbd2Way:
                Kbd2WayTargetPos();
                break;
        }

        UpdatePos(Time.fixedDeltaTime);
    }

    public void MouseTargetPos()
    {
        Vector2 mousePos = Utils.MousePosPlane();
        targetAngle = (mousePos - (Vector2)anchorTransform.position).normalized;
    }

    public void Kbd4WayTargetPos()
    {
        Vector2 targetPos = thisTransform.position;

        if (Input.GetKey(KeyCode.I))
            targetPos.y += kbdDist;
        if (Input.GetKey(KeyCode.K))
            targetPos.y -= kbdDist;
        if (Input.GetKey(KeyCode.L))
            targetPos.x += kbdDist;
        if (Input.GetKey(KeyCode.J))
            targetPos.x -= kbdDist;

        targetAngle = (targetPos - (Vector2)anchorTransform.position).normalized;
    }

    public void Kbd2WayTargetPos()
    {
        targetAngle = ((Vector2)thisTransform.position - (Vector2)anchorTransform.position).normalized;

        if (Input.GetKey(KeyCode.Period))
            targetAngle = Quaternion.Euler(0, 0, -90) * targetAngle;
        if (Input.GetKey(KeyCode.Comma))
            targetAngle = Quaternion.Euler(0, 0, 90) * targetAngle;
    }

    public void UpdatePos(float dt)
    {
        Vector2 currAngle = ((Vector2)(thisTransform.position - anchorTransform.position)).normalized;

        float diffAngleMag = Vector2.SignedAngle(currAngle, targetAngle);

        float maxAngleMag = maxAngSpeed * dt;

        if (Mathf.Abs(diffAngleMag) > maxAngleMag)
        {
            diffAngleMag = (diffAngleMag > 0) ? maxAngleMag : -maxAngleMag;
        }

        Vector2 newAng = Quaternion.Euler(0, 0, diffAngleMag) * currAngle;

        Vector3 newPos = anchorTransform.position + ((Vector3)newAng * handDist);

        thisTransform.position = newPos;
    }


    public enum MoveType
    {
        Mouse,
        Kbd4Way,
        Kbd2Way,
    }
}
