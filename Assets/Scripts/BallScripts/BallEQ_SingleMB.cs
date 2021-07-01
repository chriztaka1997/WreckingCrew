using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallEQ_SingleMB : BallEquipMB
{
    public BallThrowMB ball;
    public float spinSlowFactor;

    public void Start()
    {
        ball.onCollisionDelegate += OnBallCollision;
    }

    public override void SetEquip(PlayerMB player)
    {
        base.SetEquip(player);
        ball.SetAnchor(player.gameObject);
    }

    public override bool AllReturned()
    {
        return ball.IsReturnedDistance();
    }

    public override void InitSpin()
    {
        float spinSpd = ball.GetConservedSpinSpd(player);
        ball.InitSpin(spinSpd, player);
    }

    public override void DoSpin(float dt)
    {
        ball.SpinBall(dt, player);
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

    public override bool IsStuck()
    {
        return ball.isStuck;
    }

    public override void SlowBalls(float slowFactor)
    {
        ball.InitSpin(ball.spinSpd * slowFactor, player);
    }
}
