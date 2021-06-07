using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BallEquipMB : MonoBehaviour
{
    protected PlayerMB player;

    public virtual void SetEquip(PlayerMB player)
    {
        this.player = player;
    }

    public abstract void SetState(BallThrowMB.BallState ballState);

    public abstract bool ThrowAngleCorrect(Vector3 targetPos, float throwAngleWiggle, bool aimTypeDirect);

    public abstract void InitThrowCharge();

    public abstract void InitThrow(Vector3 targetPos, bool aimTypeDirect);

    public abstract void DoSpin(float dt);

    public abstract bool AllReturned();
}
