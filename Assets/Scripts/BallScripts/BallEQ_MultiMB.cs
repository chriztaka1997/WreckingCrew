using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEQ_MultiMB : BallEquipMB
{
    public List<BallThrowMB> balls;
    public bool spread;

    public void Start()
    {
        foreach (BallThrowMB ball in balls)
        {
            ball.onCollisionDelegate += OnBallCollision;
        }
    }

    public override void SetEquip(PlayerMB player)
    {
        base.SetEquip(player);
        foreach (BallThrowMB ball in balls)
        {
            ball.SetAnchor(player.gameObject);
        }
    }

    public override void ResetPos()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            BallThrowMB ball = balls[i];
            float angle = i * 360.0f / balls.Count;

            Vector3 pos = player.transform.position + (Quaternion.Euler(0, 0, angle) * Vector3.up * ball.chainLengthSet);
            pos.z = ball.fixedZ;
            ball.transform.position = pos;
        }
    }

    public override bool AllReturned()
    {
        bool allReturned = true;
        foreach (BallThrowMB ball in balls)
        {
            allReturned &= ball.IsReturnedDistance();
        }
        return allReturned;
    }

    public override void DoSpin(float dt)
    {
        foreach (BallThrowMB ball in balls)
        {
            ball.SpinBall(dt, player, this);
        }
    }

    public override void DoThrowPreRelease(float dt)
    {
        if (spread)
        {
            DoSpin(dt);
        }
        else
        {
            foreach (BallThrowMB ball in balls)
            {
                if (ball.state == BallThrowMB.BallState.spin &&
                    ball.ThrowAngleCorrect(targetPos, throwAngleWiggle, aimTypeDirect))
                {
                    ball.InitThrow(targetPos, aimTypeDirect);
                }
                else if (ball.state == BallThrowMB.BallState.spin) ball.SpinBall(dt, player, this);
            }
        }
    }

    public override void InitThrow()
    {
        if (spread)
        {
            BallThrowMB primary = GetPrimaryBall();
            foreach (BallThrowMB ball in balls)
            {
                if (ball == primary) ball.InitThrow(targetPos, aimTypeDirect);
                else ball.InitThrowTangent();
            }
        }
        else
        {
            foreach (BallThrowMB ball in balls)
            {
                if (ball.state != BallThrowMB.BallState.thrown)
                {
                    ball.InitThrow(targetPos, aimTypeDirect);
                }
            }
        }
    }

    public override void InitSpin()
    {
        float spinSpdAvg = 0f;
        foreach (BallThrowMB ball in balls)
        {
            spinSpdAvg += ball.GetConservedSpinSpd(player, this);
        }
        spinSpdAvg /= balls.Count;
        foreach (BallThrowMB ball in balls)
        {
            ball.InitSpin(spinSpdAvg, player, this);
        }
    }

    public override void SetState(BallThrowMB.BallState ballState)
    {
        foreach (BallThrowMB ball in balls)
        {
            ball.state = ballState;
        }
    }

    public override void InitNormal()
    {
        base.InitNormal();
        foreach (BallThrowMB ball in balls)
        {
            ball.isStuck = false;
        }
    }

    public override bool ThrowAngleCorrect()
    {
        if (spread) return GetPrimaryBall().ThrowAngleCorrect(targetPos, throwAngleWiggle, aimTypeDirect);
        else
        {
            int numNotThrown = balls.Count;
            foreach (BallThrowMB ball in balls)
            {
                if (ball.state == BallThrowMB.BallState.thrown) numNotThrown--;
                // return false if any remaining are not at the angle
                else if (!ball.ThrowAngleCorrect(targetPos, throwAngleWiggle, aimTypeDirect)) return false;
            }
            // still true if 1 not thrown, but is in angle
            return numNotThrown <= 1;
        }
    }

    public override (Vector2 source, Vector2 direction) GetThrowSource()
    {
        return balls[0].GetThrowSource(targetPos, aimTypeDirect);
    }

    public BallThrowMB GetPrimaryBall()
    {
        if (balls.Count == 0) return null;

        Vector2 centroid = Vector2.zero;
        foreach (BallThrowMB ball in balls)
        {
            centroid += (Vector2)ball.transform.position;
        }
        centroid /= balls.Count;
        BallThrowMB primary = null;
        float distToCentroid = float.MaxValue;

        foreach (BallThrowMB ball in balls)
        {
            float thisDist = Vector2.Distance(centroid, ball.transform.position);
            if (thisDist < distToCentroid)
            {
                distToCentroid = thisDist;
                primary = ball;
            }
        }
        return primary;

    }

    public override bool IsStuck()
    {
        foreach (BallThrowMB ball in balls)
        {
            if (ball.isStuck) return true;
        }
        return false;
    }

    public override float CalcSlowSpdMult(Enemymovement enemy)
    {
        float baseSlow = base.CalcSlowSpdMult(enemy);
        // split slowing among all balls
        return 1.0f - ((1.0f - baseSlow) / balls.Count);
    }

    public override void SlowBalls(float slowFactor)
    {
        float spinSpdAvg = 0f;
        foreach (BallThrowMB ball in balls)
        {
            spinSpdAvg += ball.GetConservedSpinSpd(player, this);
        }
        spinSpdAvg /= balls.Count;
        foreach (BallThrowMB ball in balls)
        {
            ball.InitSpin(spinSpdAvg * slowFactor, player, this);
        }
    }
}
