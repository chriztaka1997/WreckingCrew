using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerEffectManagerMB : MonoBehaviour
{
    public Material damageMat, iframeMat;
    private Material normalMat;

    public float iframeFlashPeriod; // in seconds
    public State state;

    public float damagedDuration;
    public float iframeDuration;

    private MeshRenderer mRenderer;
    private float timeIframeChanged;
    public float timeSinceIframeChange => Time.time - timeIframeChanged;

    private float timeStateChanged;
    public float timeSinceStateChange => Time.time - timeStateChanged;


    public void Init(MeshRenderer meshRenderer, float damagedDuration, float iframeDuration)
    {
        mRenderer = meshRenderer;
        this.damagedDuration = damagedDuration;
        this.iframeDuration = iframeDuration;
        normalMat = mRenderer.material;
        state = State.normal;
    }

    public void Update()
    {
        switch (state)
        {
            case State.normal:
                break;
            case State.damaged:
                if (timeSinceStateChange > damagedDuration)
                {
                    ChangeState(State.normal);
                }
                break;
            case State.iframe:
                int flashNum = (int)(timeSinceIframeChange / iframeFlashPeriod);
                bool isIframeMat = (flashNum % 2) == 0;
                mRenderer.material = isIframeMat ? iframeMat : normalMat;
                if (timeSinceStateChange > iframeDuration)
                {
                    ChangeState(State.normal);
                }
                break;
        }
    }

    public void ChangeState(State state)
    {
        this.state = state;
        timeIframeChanged = Time.time;
        timeStateChanged = Time.time;
        switch (state)
        {
            case State.normal:
                mRenderer.material = normalMat;
                break;
            case State.damaged:
                mRenderer.material = damageMat;
                break;
            case State.iframe:
                mRenderer.material = iframeMat;
                break;
        }

        
    }

    public enum State
    {
        normal,
        damaged,
        iframe,
    }
}
