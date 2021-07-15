using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoPointSpriteMB : MonoBehaviour
{
    public GameObject topObj;
    public GameObject botObj;
    public Vector2 topPos;
    public Vector2 botPos;
    public float fixedZ;

    public int height; // unity units height at y scale = 1

    public void LateUpdate()
    {
        if (topObj != null) topPos = topObj.transform.position;
        if (botObj != null) botPos = botObj.transform.position;
        PointTo(topPos, botPos);
    }

    public void SetPos(Vector2 topPos, Vector2 botPos)
    {
        this.topPos = topPos;
        this.botPos = botPos;
        topObj = null;
        botObj = null;
    }

    public void PointTo(Vector2 _topPos, Vector2 _botPos)
    {
        Vector3 newPos = (_topPos + _botPos) / 2.0f;
        newPos.z = fixedZ;
        transform.position = newPos;

        Vector3 botToTop = _topPos - _botPos;
        botToTop.z = 0;

        Vector3 scale = transform.localScale;
        scale.y = botToTop.magnitude / height;
        transform.localScale = scale;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, botToTop);
    }
}
