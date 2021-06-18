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
    public GameObject wallHolder;

    public void Awake()
    {
        level = new LevelData(37, 37, 1, new Vector2(0, 0));
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
                            string name = string.Format("Wall Solid: ({0}, {1})", ix, iy);
                            GameObject go = Instantiate(wallSolidPF, wallHolder.transform);
                            go.name = name;
                            go.transform.position = level.WorldLocation(ix, iy);
                            wallsSolid.Add(go);
                        }
                        break;
                    case TileType.breakWall:
                        {
                            string name = string.Format("Wall Break: ({0}, {1})", ix, iy);
                            GameObject go = Instantiate(wallBreakPF, wallHolder.transform);
                            go.name = name;
                            go.transform.position = level.WorldLocation(ix, iy);
                            wallsBreak.Add(go);
                        }
                        break;
                    case TileType.enemySpawn:
                        break;
                    case TileType.playerSpawn:
                        {
                            string name = string.Format("PlayerUnit", ix, iy);
                            GameObject go = Instantiate(playerPF);
                            go.name = name;
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
            if (g != null)
            {
                if (!Application.isEditor)
                {
                    Destroy(g);
                }
                else
                {
                    DestroyImmediate(g);
                }
            }
        }
        foreach (GameObject g in wallsBreak)
        {
            if (g != null)
            {
                if (!Application.isEditor)
                {
                    Destroy(g);
                }
                else
                {
                    DestroyImmediate(g);
                }
            }
        }
        if (player != null)
        {
            if (!Application.isEditor)
            {
                Destroy(player);
            }
            else
            {
                DestroyImmediate(player);
            }
        }
    }

}
