using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

[Serializable]
public class LevelData
{
    public LevelTile[,] tiles;
    public float levelScale; // tile length in unity units
    public Vector2 anchor;

    public int width => tiles.GetLength(0);
    public int height => tiles.GetLength(1);

    public LevelData(int width, int height, float levelScale, Vector2 center)
    {
        tiles = new LevelTile[width, height];
        this.levelScale = levelScale;
        anchor = center - new Vector2(width * levelScale * 0.5f, height * levelScale * 0.5f) + new Vector2(levelScale * 0.5f, levelScale * 0.5f);

        for (int ix = 0; ix < width; ix++)
        {
            for (int iy = 0; iy < height; iy++)
            {
                tiles[ix, iy] = new LevelTile(TileType.empty);
            }
        }
    }

    public static LevelData Deserialize(string jsonStr)
    {
        return JsonConvert.DeserializeObject<LevelData>(jsonStr);
    }

    public Vector2 WorldLocation(int x, int y)
    {
        return anchor + new Vector2(x * levelScale, y * levelScale);
    }

    public (int x, int y)? GridLocation(Vector2 location)
    {
        Vector2 baseLocation = (location - anchor) / levelScale;
        (int x, int y) = (Mathf.RoundToInt(baseLocation.x), Mathf.RoundToInt(baseLocation.y));
        if (x < 0 || x >= width || y < 0 || y >= height) return null;
        return (x, y);
        
    }

    public string ToJsonString()
    {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
    }
}

[Serializable]
public class LevelTile
{
    public TileType tileType;

    public LevelTile(TileType tileType)
    {
        this.tileType = tileType;
    }
}

[Serializable]
public enum TileType
{
    empty,
    solidWall,
    breakWall,
    enemySpawn,
    playerSpawn,
}