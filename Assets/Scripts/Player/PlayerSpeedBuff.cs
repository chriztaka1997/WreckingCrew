using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerSpeedBuff
{
    public GameObject particleHold, particleActive;
    public float duration;
    public State state;
    public float startTime;

    public bool isActive => state == State.active;

    public void Init()
    {
        state = State.inactive;

        particleHold.SetActive(false);
        particleActive.SetActive(false);
    }

    // returns whether spinning started
    public bool GetBuff(PlayerMB.ActionState playerState)
    {
        switch (state)
        {
            case State.inactive:
                if (playerState == PlayerMB.ActionState.moveSpin ||
                    playerState == PlayerMB.ActionState.throwCharge)
                {
                    state = State.active;
                    startTime = Time.time;
                    particleActive.SetActive(true);
                    particleHold.SetActive(false);
                    return true;
                }
                else
                {
                    state = State.holding;
                    particleActive.SetActive(false);
                    particleHold.SetActive(true);
                }
                break;
            case State.holding:
                break;
            case State.active:
                startTime = Time.time;
                break;
        }
        return false;
    }

    public void Update()
    {
        switch (state)
        {
            case State.inactive:
                break;
            case State.holding:
                break;
            case State.active:
                if (Time.time >= startTime + duration)
                {
                    state = State.inactive;
                    particleActive.SetActive(false);
                    particleHold.SetActive(false);
                }
                break;
        }
    }

    // returns whether spinning started
    public bool SpinStart()
    {
        switch (state)
        {
            case State.inactive:
                break;
            case State.holding:
                state = State.active;
                startTime = Time.time;
                particleActive.SetActive(true);
                particleHold.SetActive(false);
                return true;
            case State.active:
                break;
        }
        return false;
    }


    public enum State
    {
        inactive,
        holding,
        active,
    }
}
