using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BallEquipMB : MonoBehaviour
{
    protected PlayerMB player;

    protected Vector3 targetPos => player.targetPos;
    protected float throwAngleWiggle => player.throwAngleWiggle;
    protected bool aimTypeDirect => player.aimTypeDirect;

    public virtual void SetEquip(PlayerMB player)
    {
        this.player = player;
    }

    public abstract void SetState(BallThrowMB.BallState ballState);

    public virtual void ResetState() => SetState(BallThrowMB.BallState.normal);

    public abstract bool ThrowAngleCorrect();

    public abstract void InitSpin();

    public abstract void InitThrow();

    public abstract void DoSpin(float dt);

    public virtual void DoThrowPreRelease(float dt) => DoSpin(dt);

    public virtual void InitReturn() => SetState(BallThrowMB.BallState.returning);

    public abstract bool AllReturned();

    public virtual void InitNormal() => SetState(BallThrowMB.BallState.normal);

    public abstract void OnBallCollision(BallThrowMB ballRef, Collision2D collision);
}
