using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderMB : MonoBehaviour
{
    public GameObject top, bot, left, right, background;
    public float thicknessZ;
    public float borderZ, backgroundZ;

    public void Init(Vector2 bottomLeft, float width, float height, float thickness)
    {
        top.transform.localScale = new Vector3(width + (2 * thickness), thickness, thicknessZ);
        bot.transform.localScale = new Vector3(width + (2 * thickness), thickness, thicknessZ);
        left.transform.localScale = new Vector3(thickness, height, thicknessZ);
        right.transform.localScale = new Vector3(thickness, height, thicknessZ);

        top.transform.position = new Vector3(bottomLeft.x + (width / 2), bottomLeft.y + height + (thickness / 2), borderZ);
        bot.transform.position = new Vector3(bottomLeft.x + (width / 2), bottomLeft.y - (thickness / 2), borderZ);
        left.transform.position = new Vector3(bottomLeft.x - (thickness / 2), bottomLeft.y + (height / 2), borderZ);
        right.transform.position = new Vector3(bottomLeft.x + width + (thickness / 2), bottomLeft.y + (height / 2), borderZ);

        background.transform.localScale = new Vector3((width + (2 * thickness)) / 10, 1, (height + (2 * thickness)) / 10);
        background.transform.position = new Vector3(bottomLeft.x + (width / 2), bottomLeft.y + (height / 2), backgroundZ);
    }

    public void Init(LevelData level) => Init(level.bottomLeft, level.levelScale * level.width, level.levelScale * level.height, level.levelScale);
}
