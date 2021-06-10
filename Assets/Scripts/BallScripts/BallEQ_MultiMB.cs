using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEQ_MultiMB : BallEquipMB
{
    public List<BallThrowMB> balls;
    public bool spread;
    public float spinSlowFactor;

    private float spinSpdAvg;

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
            ball.SpinBall(dt);
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
                if (ball.state == BallThrowMB.BallState.external &&
                    ball.ThrowAngleCorrect(targetPos, throwAngleWiggle, aimTypeDirect))
                {
                    ball.InitThrow(targetPos, aimTypeDirect);
                }
                else if (ball.state == BallThrowMB.BallState.external) ball.SpinBall(dt);
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
                else ball.state = BallThrowMB.BallState.thrown;
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
        float tempSpinSpdAvg = 0f;
        foreach (BallThrowMB ball in balls)
        {
            tempSpinSpdAvg += ball.GetConservedSpinSpd();
        }
        tempSpinSpdAvg /= balls.Count;
        spinSpdAvg = tempSpinSpdAvg;
        foreach (BallThrowMB ball in balls)
        {
            ball.InitSpin(tempSpinSpdAvg);
        }
    }

    public override void SetState(BallThrowMB.BallState ballState)
    {
        foreach (BallThrowMB ball in balls)
        {
            ball.state = ballState;
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

    public override void OnBallCollision(BallThrowMB ballRef, Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Enemy")
        {
            switch (ballRef.state)
            {
                case BallThrowMB.BallState.normal:
                case BallThrowMB.BallState.thrown:
                    var velocity = ballRef.thisRigidbody.velocity;
                    ballRef.thisRigidbody.velocity = velocity * spinSlowFactor;
                    break;
                case BallThrowMB.BallState.external:
                    // split slowing among all balls
                    float groupSlowFactor = 1.0f - ((1.0f - spinSlowFactor) / balls.Count);
                    foreach (BallThrowMB ball in balls)
                    {
                        ball.InitSpin(ballRef.spinSpd * groupSlowFactor);
                    }
                    break;
            }
        }
    }
}
