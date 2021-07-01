using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BallEquipMB : MonoBehaviour
{
    protected PlayerMB player;
    public float damageMult;
    public float spinSpdMinMult;
    public float spinSpdMaxMult;
    public float spinSpdRateMult;
    public float spinSlowFactor;

    protected Vector3 targetPos => player.targetPos;
    protected float throwAngleWiggle => player.throwAngleWiggle;
    protected bool aimTypeDirect => player.aimTypeDirect;

    public virtual void SetEquip(PlayerMB player)
    {
        this.player = player;
        ResetState();
        ResetPos();
    }

    public abstract void SetState(BallThrowMB.BallState ballState);

    public virtual void ResetState() => SetState(BallThrowMB.BallState.normal);

    public abstract void ResetPos();

    public abstract bool ThrowAngleCorrect();

    public abstract void InitSpin();

    public abstract void InitThrow();

    public abstract void DoSpin(float dt);

    public virtual void DoThrowPreRelease(float dt) => DoSpin(dt);

    public virtual void InitReturn() => SetState(BallThrowMB.BallState.returning);

    public abstract bool AllReturned();

    public virtual void InitNormal() => SetState(BallThrowMB.BallState.normal);

    public abstract bool IsStuck();

    public virtual void OnBallCollision(BallThrowMB ballRef, Collider2D collider)
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
                    ballRef.thisRigidbody.velocity = velocity * CalcSlowSpdMult(enemymovement);
                    break;
                case BallThrowMB.BallState.spin:
                    SlowBalls(CalcSlowSpdMult(enemymovement));
                    break;
            }
            float damage = CalcDamage(ballRef);
            enemymovement.CollisionWithBall(ballRef, damage);
        }
    }

    public virtual float CalcDamage(BallThrowMB ballRef)
    {
        float ballSpd = ballRef.thisRigidbody.velocity.magnitude;

        float spdDamage = player.stats.precision * ballSpd;
        float baseDamage = player.stats.attack;

        float totDamage = baseDamage + spdDamage;
        // multiply by local multiplier
        totDamage *= damageMult;

        // add ball special effect damage

        // multiply by ball state specific damage
        switch (ballRef.state)
        {
            case BallThrowMB.BallState.normal:
                totDamage *= player.stats.swingDmg;
                break;
            case BallThrowMB.BallState.thrown:
                totDamage *= player.stats.throwDmg;
                break;
            case BallThrowMB.BallState.spin:
                totDamage *= player.stats.spinDmg;
                break;
        }

        return totDamage;
    }

    public virtual float CalcSlowSpdMult(Enemymovement enemy)
    {
        float baseSlow = enemy.weight / 100.0f;
        float slowResFactor = 1 / (1 + player.stats.tenacity);
        float slowAmount = baseSlow * slowResFactor;
        if (slowAmount < 0) slowAmount = 0;
        if (slowAmount >= 1.0f) slowAmount = 1.0f;
        return 1 - slowAmount;
    }

    public abstract void SlowBalls(float slowFactor);
}
