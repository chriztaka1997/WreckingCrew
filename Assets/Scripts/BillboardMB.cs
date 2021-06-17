using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardMB : MonoBehaviour
{
    private Transform camTransform;

    public void Start()
    {
        camTransform = Camera.main.transform;
    }

    public void LateUpdate()
    {
        transform.LookAt(transform.position + camTransform.forward);
    }
}
