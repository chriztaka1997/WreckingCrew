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

    private MeshRenderer mRenderer;
    private DateTime timeChanged;
    public TimeSpan timeSinceChange => timeChanged - DateTime.Now;

    
    public void Init(MeshRenderer meshRenderer)
    {
        mRenderer = meshRenderer;
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
                break;
            case State.iframe:
                int flashNum = (int)(timeSinceChange.TotalSeconds / iframeFlashPeriod);
                bool isIframeMat = (flashNum % 2) == 0;
                mRenderer.material = isIframeMat ? iframeMat : normalMat;
                break;
        }
    }

    public void ChangeState(State state)
    {
        this.state = state;
        timeChanged = DateTime.Now;
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
