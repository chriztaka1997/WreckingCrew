using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerMB : MonoBehaviour
{
    public BallEquipMB ballEquip;
    public HP_BarMB hpBar;

    public string startBallEquipName;

    public PlayerStats stats;

    public Vector3 targetPos;
    public float fixedZ;
    public float maxSpeed; // units per sec

    public ActionState actionState;

    public MoveType moveType;

    public bool aimTypeDirect; // true means aimed directly at cursor
    public float throwAngleWiggle; // degrees either way

    public KeyManager throwKey, spinKey;
    private List<KeyManager> keyManagers;

    public PlayerEffectManagerMB effectManager;

    public float knockBackDist;
    public float knockBackDuration; // in seconds
    public float iframeDuration; // in seconds
    private DateTime hitTime;
    private Vector2 knockbackStartPoint;
    private Vector2 knockbackEndPoint;

    public Rigidbody2D  thisRigidbody { get; protected set; }
    public CircleCollider2D thisCollider { get; protected set; }
    const float kbdDist = 1.0f;

    public Transform thisTransform => gameObject.transform;

    public void Start()
    {
        SetEquipBall(startBallEquipName);

        thisCollider = gameObject.GetComponent<CircleCollider2D>();
        thisRigidbody = gameObject.GetComponent<Rigidbody2D>();

        Vector3 newPos = thisTransform.position;
        newPos.z = fixedZ;
        thisTransform.position = newPos;

        stats.Reset();
        hpBar.InitHP(stats.currentHP);

        keyManagers = new List<KeyManager> { throwKey, spinKey };

        effectManager.Init(gameObject.GetComponent<MeshRenderer>());

        actionState = ActionState.normal;

    }

    public void FixedUpdate()
    {
        UpdateKeys();
        UpdateTargetPos();
        UpdateAction(Time.fixedDeltaTime);
    }

    public void SetEquipBall(string name)
    {
        if (ballEquip != null) Destroy(ballEquip.gameObject);
        BallEquipMB ballEquipPF = PrefabPaletteMB.instance.GetBallEQ_Prefab(name);
        if (ballEquipPF == null)
        {
            Debug.Log(string.Format("Ball equip with the name \"{0}\" could not be found", name));
            return;
        }
        ballEquip = Instantiate(ballEquipPF, transform.parent);
        ballEquip.SetEquip(this);
    }

    public void MouseTargetPos()
    {
        targetPos = Utils.MousePosPlane();
        targetPos.z = fixedZ;
    }

    public void KbdTargetPos()
    {
        targetPos = thisTransform.position;
        targetPos.z = fixedZ;

        if (Input.GetKey(KeyCode.W))
            targetPos.y += kbdDist;
        if (Input.GetKey(KeyCode.S))
            targetPos.y -= kbdDist;
        if (Input.GetKey(KeyCode.D))
            targetPos.x += kbdDist;
        if (Input.GetKey(KeyCode.A))
            targetPos.x -= kbdDist;
    }

    public void UpdateKeys()
    {
        foreach (KeyManager keyM in keyManagers)
        {
            keyM.Update();
        }
    }

    public void UpdateTargetPos()
    {
        switch (moveType)
        {
            case MoveType.Mouse:
                MouseTargetPos();
                break;
            case MoveType.Kbd:
                KbdTargetPos();
                break;
        }
    }    

    public void UpdatePos(float dt)
    {
        float maxDist = maxSpeed * dt;
        if (Vector2.Distance(targetPos, thisTransform.position) <= maxDist)
        {
            thisRigidbody.MovePosition(targetPos);
        }
        else
        {
            Vector3 changeVec = targetPos - thisTransform.position;
            changeVec.Normalize();
            changeVec *= maxDist;
            thisRigidbody.MovePosition(thisTransform.position + changeVec);
        }
    }

    public void UpdateKnockback()
    {
        double timeSinceHit = (DateTime.Now - hitTime).TotalSeconds;
        double kb_ratio = timeSinceHit / knockBackDuration;
        thisRigidbody.MovePosition(Vector2.Lerp(knockbackStartPoint, knockbackEndPoint, (float)kb_ratio));
    }

    public void UpdateAction(float dt)
    {
        switch (actionState)
        {
            case ActionState.normal:
                if (spinKey.GetKeyDown)
                {
                    actionState = ActionState.moveSpin;
                    ballEquip.InitSpin();
                    break;
                }
                if (throwKey.GetKeyDown)
                {
                    actionState = ActionState.throwCharge;
                    ballEquip.InitSpin();
                    break;
                }
                break;
            case ActionState.moveSpin:
                if (!spinKey.GetKey)
                {
                    actionState = ActionState.normal;
                    ballEquip.InitNormal();
                    break;
                }
                if (throwKey.GetKeyDown)
                {
                    actionState = ActionState.throwCharge;
                    break;
                }
                break;
            case ActionState.throwCharge:
                if (spinKey.GetKeyDown)
                {
                    actionState = ActionState.moveSpin;
                    break;
                }
                if (!throwKey.GetKey)
                {
                    if (ballEquip.ThrowAngleCorrect())
                    {
                        actionState = ActionState.thrown;
                        ballEquip.InitThrow();
                    }
                    else
                    {
                        actionState = ActionState.throwPreRelease;
                    }
                    break;
                }
                break;
            case ActionState.throwPreRelease:
                if (ballEquip.ThrowAngleCorrect())
                {
                    actionState = ActionState.thrown;
                    ballEquip.InitThrow();
                }
                break;

            case ActionState.thrown:
                // suggestion: maybe add way to set return without button in range
                if (throwKey.GetKeyDown || spinKey.GetKeyDown)
                {
                    actionState = ActionState.returning;
                    ballEquip.InitReturn();
                }
                break;
            case ActionState.returning:
                if (ballEquip.AllReturned())
                {
                    actionState = ActionState.normal;
                    ballEquip.InitNormal();
                    break;
                }
                break;
            case ActionState.knockback:
                if ((DateTime.Now - hitTime).TotalSeconds >= knockBackDuration)
                {
                    actionState = ActionState.iframes;
                    effectManager.ChangeState(PlayerEffectManagerMB.State.iframe);
                    goto case ActionState.iframes;
                }
                break;
            case ActionState.iframes:
                if ((DateTime.Now - hitTime).TotalSeconds >= iframeDuration)
                {
                    actionState = ActionState.normal;
                    effectManager.ChangeState(PlayerEffectManagerMB.State.normal);
                }
                break;
        }
        switch (actionState)
        {
            case ActionState.normal:
                UpdatePos(dt);
                break;
            case ActionState.moveSpin:
                UpdatePos(dt);
                ballEquip.DoSpin(dt);
                break;
            case ActionState.throwCharge:
                ballEquip.DoSpin(dt);
                break;
            case ActionState.throwPreRelease:
                ballEquip.DoThrowPreRelease(dt);
                break;
            case ActionState.thrown:
            case ActionState.returning:
                UpdatePos(dt);
                break;
            case ActionState.knockback:
                UpdateKnockback();
                break;
            case ActionState.iframes:
                UpdatePos(dt);
                break;
        }
    }

    public void ResetActionState()
    {
        ballEquip.ResetState();
        actionState = ActionState.normal;
    }

    public bool StartHit()
    {
        if (actionState == ActionState.knockback) return false;

        ResetActionState();
        actionState = ActionState.knockback;
        hitTime = DateTime.Now;
        return true;
    }

    public void SetHP(float hp)
    {
        stats.currentHP = hp;
        hpBar.SetHP(stats.currentHP);
    }

    public void ResetHP() => SetHP(stats.maxHP);

    public void AlterHP(float d_hp) => SetHP(stats.currentHP + d_hp);

    public bool CheckDeath()
    {
        if (stats.currentHP <= 0)
        {
            print("Died :(");
            stats.Reset();
            ResetHP();
            return true;
        }
        return false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        Vector2 hitDir = (transform.position - collision.transform.position).normalized;
        switch (tag)
        {
            case "Enemy":
                if (actionState == ActionState.knockback || actionState == ActionState.iframes) break;
                Enemymovement enemy = collision.gameObject.GetComponent<Enemymovement>();
                knockbackStartPoint = transform.position;
                knockbackEndPoint = knockbackStartPoint + (hitDir * knockBackDist);
                hitTime = DateTime.Now;
                actionState = ActionState.knockback;
                ballEquip.InitNormal();
                effectManager.ChangeState(PlayerEffectManagerMB.State.damaged);

                float damageTaken = 40.0f; // Set from enemy later
                AlterHP(-damageTaken);

                CheckDeath();
                break;
        }
    }


    public enum MoveType
    {
        Mouse,
        Kbd,
    }

    public enum ActionState
    {
        normal, // neutral state
        moveSpin, // spinning ball while moving
        throwCharge, // chargeup to throw
        throwPreRelease, // waiting to throw in direction
        thrown, // ball(s) being thrown
        returning, // ball(s) on the way back to player with constant force
        knockback, // hit back by enemy
        iframes, // invincible state after being hit
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerMB))]
public class PlayerMB_Editor : Editor
{
    public PlayerMB targetRef => (PlayerMB)target;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Editor");

        if (GUILayout.Button("Set ball to single"))
        {
            targetRef.SetEquipBall("BallEQ_Single");
        }
        if (GUILayout.Button("Set ball to triple spread"))
        {
            targetRef.SetEquipBall("BallEQ_TripleSpread");
        }
        if (GUILayout.Button("Set ball to triple rapid"))
        {
            targetRef.SetEquipBall("BallEQ_TripleRapid");
        }
    }
}
#endif