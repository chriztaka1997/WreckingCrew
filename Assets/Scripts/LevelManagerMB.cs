using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManagerMB : MonoBehaviour
{
    public BorderMB borderPF;
    private BorderMB border;
    public LevelData level;
    public LevelTile[,] currentTiles;

    public List<Vector2Int> enemySpawnCoords;
    public Vector2Int playerSpawnCoords;

    public List<GameObject> wallsSolid;
    public List<GameObject> wallsBreak;
    private GameObject wallSolidHolder, wallBreakHolder;

    public LevelAnalysis.Graph la_graph { get; private set; }
    public List<TileType> traversable;

    [TextArea]
    public string jsonString;

    public void Awake()
    {
        transform.position = Vector3.zero;
    }

    public void Start()
    {
        LevelData storedLevel = LevelData.Deserialize(jsonString);
        if (storedLevel != null)
        {
            SetLevel(storedLevel);
        }
    }

    public void SetLevel(LevelData level)
    {
        this.level = new LevelData(level);
        InitFromLevel(this.level);
        jsonString = this.level.ToJsonString();
    }

    // doesn't init player
    public void InitFromLevel(LevelData level)
    {
        ResetLevel();


        Utils.TryDestroy(wallSolidHolder);
        wallSolidHolder = new GameObject("WallSolidHolder");
        wallSolidHolder.transform.position = Vector3.zero;
        wallSolidHolder.transform.SetParent(transform);

        Utils.TryDestroy(wallBreakHolder);
        wallBreakHolder = new GameObject("WallBreakHolder");
        wallBreakHolder.transform.position = Vector3.zero;
        wallBreakHolder.transform.SetParent(transform);

        Utils.TryDestroy(border?.gameObject);
        border = Instantiate(borderPF, transform);
        border.name = "Border";
        border.Init(level);

        enemySpawnCoords.Clear();

        currentTiles = (LevelTile[,])level.tiles.Clone();

        for (int ix = 0; ix < level.width; ix++)
        {
            for (int iy = 0; iy < level.height; iy++)
            {
                currentTiles[ix, iy] = new LevelTile(level.tiles[ix, iy]);
                switch (level.tiles[ix, iy].tileType)
                {
                    case TileType.empty:
                        break;
                    case TileType.solidWall:
                        {
                            string name = string.Format("Wall Solid: ({0}, {1})", ix, iy);
                            GameObject go = WallSpawner.SpawnWall(level.WorldLocation(ix, iy), level.levelScale, level.levelScale, wallSolidHolder.transform);
                            go.name = name;
                            wallsSolid.Add(go);
                        }
                        break;
                    case TileType.breakWall:
                        {
                            string name = string.Format("Wall Break: ({0}, {1})", ix, iy);
                            BreakableObastacle wall = WallSpawner.SpawnObstacle(level.WorldLocation(ix, iy), level.levelScale, level.levelScale, wallBreakHolder.transform);
                            GameObject go = wall.gameObject;
                            go.name = name;
                            go.transform.position = level.WorldLocation(ix, iy);
                            int _ix = ix, _iy = iy; // because c# is a janky language
                            wall.doOnBroken += () => OnWallBreak(_ix, _iy);
                            wallsBreak.Add(go);
                        }
                        break;
                    case TileType.waterHazard:
                        {
                            string name = string.Format("Water Hazard: ({0}, {1})", ix, iy);
                            GameObject go = WallSpawner.SpawnWater(level.WorldLocation(ix, iy), level.levelScale, level.levelScale, wallBreakHolder.transform).gameObject;
                            go.name = name;
                            go.transform.position = level.WorldLocation(ix, iy);
                            wallsBreak.Add(go);
                        }
                        break;
                    case TileType.enemySpawn:
                        enemySpawnCoords.Add(new Vector2Int(ix, iy));
                        break;
                    case TileType.playerSpawn:
                        {
                            playerSpawnCoords = new Vector2Int(ix, iy);
                        }
                        break;
                }
            }
        }

        OnTileChange();
    }

    public void PlacePlayer(PlayerMB player)
    {
        player.transform.position = level.WorldLocation(playerSpawnCoords);
        player.ballEquip?.ResetPos();
    }

    public void ResetLevel()
    {
        foreach (GameObject g in wallsSolid)
        {
            Utils.TryDestroy(g);
        }
        wallsSolid.Clear();
        foreach (GameObject g in wallsBreak)
        {
            Utils.TryDestroy(g);
        }
        wallsBreak.Clear();
    }

    public void OnWallBreak(int x, int y)
    {
        if (currentTiles[x, y].tileType == TileType.breakWall)
        {
            currentTiles[x, y].tileType = TileType.empty;
            OnTileChange();
            TutorialMB.SignalTutorial("wall");
        }
    }

    public void OnTileChange()
    {
        la_graph = new LevelAnalysis.Graph(currentTiles, traversable);
    }
}
