using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManagerMB : MonoBehaviour
{
    public PlayerMB player { get; private set; }
    public GameObject playerPF;
    public GameObject wallSolidPF;
    public GameObject wallBreakPF;
    public LevelData level;

    public List<GameObject> wallsSolid;
    public List<GameObject> wallsBreak;

    public void Awake()
    {
        InitFromLevel(level);
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.SetEquipBall("BallEQ_Single");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            player.SetEquipBall("BallEQ_TripleSpread");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            player.SetEquipBall("BallEQ_TripleRapid");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Game");
        }
    }

    public void InitFromLevel(LevelData level)
    {
        for (int ix = 0; ix < level.width; ix++)
        {
            for (int iy = 0; iy < level.height; iy++)
            {
                switch (level.tiles[ix, iy].tileType)
                {
                    case TileType.empty:
                        break;
                    case TileType.solidWall:
                        {
                            GameObject go = Instantiate(wallSolidPF);
                            go.transform.position = level.WorldLocation(ix, iy);
                            wallsSolid.Add(go);
                        }
                        break;
                    case TileType.breakWall:
                        {
                            GameObject go = Instantiate(wallBreakPF);
                            go.transform.position = level.WorldLocation(ix, iy);
                            wallsBreak.Add(go);
                        }
                        break;
                    case TileType.enemySpawn:
                        break;
                    case TileType.playerSpawn:
                        {
                            GameObject go = Instantiate(playerPF);
                            go.transform.position = Vector3.zero;
                            player = go.GetComponentInChildren<PlayerMB>();
                            player.transform.position = level.WorldLocation(ix, iy);
                        }
                        break;
                }
            }
        }
    }


    public void ResetLevel()
    {
        foreach (GameObject g in wallsSolid)
        {
            if (g != null) Destroy(g);
        }
        foreach (GameObject g in wallsBreak)
        {
            if (g != null) Destroy(g);
        }
        if (player != null) Destroy(player);
    }
}
