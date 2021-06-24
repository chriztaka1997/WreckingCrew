using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallSpawner
{

    public static GameObject SpawnWall(Vector2 pos, float length, float width, Transform parent = null)
    {
        GameObject wallPrefab = Resources.Load("RandomWall") as GameObject;
        GameObject wall = (parent == null) ? GameObject.Instantiate(wallPrefab) : GameObject.Instantiate(wallPrefab, parent);
        wall.transform.position = new Vector3(pos.x, pos.y, -0.5f);
        wall.transform.localScale = new Vector3(length, width, 1.0f);
        return wall;
    }

    public static BreakableObastacle SpawnObstacle(Vector2 pos, float length, float width, Transform parent = null)
    {
        BreakableObastacle wallPrefab = Resources.Load("RandomObstacle", typeof(BreakableObastacle)) as BreakableObastacle;
        BreakableObastacle wall = (parent == null) ? GameObject.Instantiate(wallPrefab) : GameObject.Instantiate(wallPrefab, parent);
        wall.transform.position = new Vector3(pos.x, pos.y, -0.5f);
        wall.transform.localScale = new Vector3(length, width, 1.0f);
        return wall;
    }
}
