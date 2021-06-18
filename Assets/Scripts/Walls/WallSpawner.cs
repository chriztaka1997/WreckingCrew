using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallSpawner
{

    public static void SpawnWall(float x, float y, float length, float width)
    {
        GameObject wallPrefab = Resources.Load("RandomWall") as GameObject;
        GameObject.Instantiate(wallPrefab);
        wallPrefab.transform.position = new Vector3(x, y, -0.5f);
        wallPrefab.transform.localScale = new Vector3(length, width, 1.0f);
    }

    public static void SpawnObstacle(float x, float y, float length, float width)
    {
        GameObject wallPrefab = Resources.Load("RandomObstacle") as GameObject;
        GameObject.Instantiate(wallPrefab);
        wallPrefab.transform.position = new Vector3(x, y, -0.5f);
        wallPrefab.transform.localScale = new Vector3(length, width, 1.0f);
    }
}
