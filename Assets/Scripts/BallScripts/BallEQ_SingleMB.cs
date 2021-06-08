using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallEQ_SingleMB : BallEquipMB
{
    public BallThrowMB ball;

    public override void SetEquip(PlayerMB player)
    {
        base.SetEquip(player);
        ball.SetAnchor(player.gameObject);
    }

    public override bool AllReturned()
    {
        return ball.IsReturnedDistance();
    }

    public override void DoSpin(float dt)
    {
        ball.SpinBall(dt);
    }

    public override void InitThrow()
    {
        ball.InitThrow(targetPos, aimTypeDirect);
    }

    public override void InitThrowCharge()
    {
        ball.InitThrowCharge();
    }

    public override void SetState(BallThrowMB.BallState ballState)
    {
        ball.state = ballState;
    }

    public override bool ThrowAngleCorrect()
    {
        return ball.ThrowAngleCorrect(targetPos, throwAngleWiggle, aimTypeDirect);
    }
}
