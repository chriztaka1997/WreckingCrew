using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraMB : MonoBehaviour
{
    public static PlayerCameraMB instance;

    public float planeZ;
    public GameObject target;
    public float speedPerDist; // (units / sec) / unit apart
    [Header("Info Only")]
    public float planePosX;
    public float planePosY; // pos on z plane


    public Vector3 targetPos => target.transform.position;
    public int width => Camera.main.pixelWidth;
    public int height => Camera.main.pixelHeight;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    public void Start()
    {
        target = GameManagerMB.instance.player.gameObject;
    }

    public void FixedUpdate()
    {
        UpdatePosition(Time.fixedDeltaTime);
    }

    public void UpdatePosition(float dt)
    {
        UpdatePlanePos();

        Vector3 toTarget = new Vector3(targetPos.x, targetPos.y, planeZ) - new Vector3(planePosX, planePosY, planeZ);
        Vector3 moveVec = toTarget.normalized * toTarget.magnitude * speedPerDist;

        transform.position = transform.position + moveVec;
    }

    private void UpdatePlanePos()
    {
        planePosX = transform.position.x;
        planePosY = transform.position.y + (transform.position.z - planeZ) * Mathf.Tan(Mathf.Deg2Rad * transform.rotation.eulerAngles.x);
    }

}
