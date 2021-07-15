using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallEQ_SingleMB : BallEquipMB
{
    public BallThrowMB ball;

    public void Start()
    {
        ball.onCollisionDelegate += OnBallCollision;
    }

    public override void SetEquip(PlayerMB player)
    {
        base.SetEquip(player);
        ball.SetAnchor(player.gameObject);
    }

    public override void ResetPos()
    {
        Vector3 pos = player.transform.position + new Vector3(ball.chainLengthSet, 0, 0);
        pos.z = ball.fixedZ;
        ball.transform.position = pos;
    }

    public override bool AllReturned()
    {
        return ball.IsReturnedDistance();
    }

    public override void InitSpin()
    {
        float spinSpd = ball.GetConservedSpinSpd(player, this);
        ball.InitSpin(spinSpd, player, this);
    }

    public override void DoSpin(float dt)
    {
        ball.SpinBall(dt, player, this);
    }

    public override void InitThrow()
    {
        ball.InitThrow(targetPos, aimTypeDirect);
    }

    public override void SetState(BallThrowMB.BallState ballState)
    {
        ball.state = ballState;
    }

    public override void InitNormal()
    {
        base.InitNormal();
        ball.isStuck = false;
    }

    public override bool ThrowAngleCorrect()
    {
        return ball.ThrowAngleCorrect(targetPos, throwAngleWiggle, aimTypeDirect);
    }

    public override (Vector2 source, Vector2 direction) GetThrowSource()
    {
        return ball.GetThrowSource(targetPos, aimTypeDirect);
    }

    public override bool IsStuck()
    {
        return ball.isStuck;
    }

    public override void SlowBalls(float slowFactor)
    {
        ball.InitSpin(ball.spinSpd * slowFactor, player, this);
    }
}
