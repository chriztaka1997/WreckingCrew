using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallEQ_SingleMB : BallEquipMB
{
    public BallThrowMB ball;
    public float spinSlowFactor;
    private float spinSpd;

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
        spinSpd = ball.GetConservedSpinSpd();
        ball.InitSpin(spinSpd);
    }

    public override void DoSpin(float dt)
    {
        ball.SpinBall(dt);
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

    public override void OnBallCollision(BallThrowMB ballRef, Collider2D collider)
    {
        string tag = collider.gameObject.tag;
        if (tag == "Enemy")
        {
            Enemymovement enemymovement = collider.gameObject.GetComponent<Enemymovement>();
            switch (ballRef.state)
            {
                case BallThrowMB.BallState.normal:
                case BallThrowMB.BallState.thrown:
                    var velocity = ballRef.thisRigidbody.velocity;
                    ballRef.thisRigidbody.velocity = velocity * CalcSloSpdMult(enemymovement);
                    break;
                case BallThrowMB.BallState.external:
                    ballRef.InitSpin(ballRef.spinSpd * CalcSloSpdMult(enemymovement));
                    break;
            }
            float damage = CalcDamage(ballRef);
            enemymovement.CollisionWithBall(ballRef, damage);
        }
    }

    public override bool IsStuck()
    {
        return ball.isStuck;
    }
}
