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
    public Vector2 anchor; // center of bottom left tile

    public int width => tiles.GetLength(0);
    public int height => tiles.GetLength(1);
    [JsonIgnore]
    public Vector2 bottomLeft => anchor - new Vector2(levelScale / 2, levelScale / 2);
    [JsonIgnore]
    public Vector2 topRight => bottomLeft + new Vector2(levelScale * width, levelScale * height);
    [JsonIgnore]
    public Vector2 center => bottomLeft + new Vector2(width * levelScale / 2, height * levelScale / 2);

    [JsonConstructor]
    public LevelData(LevelTile[,] tiles, float levelScale, Vector2 anchor)
    {
        this.tiles = tiles;
        this.levelScale = levelScale;
        this.anchor = anchor;
    }

    public LevelData(int width, int height, float levelScale, Vector2 center)
    {
        tiles = new LevelTile[width, height];
        this.levelScale = levelScale;
        anchor = center - new Vector2(width * levelScale / 2, height * levelScale / 2) + new Vector2(levelScale / 2, levelScale / 2);

        for (int ix = 0; ix < width; ix++)
        {
            for (int iy = 0; iy < height; iy++)
            {
                tiles[ix, iy] = new LevelTile(TileType.empty);
            }
        }
    }

    // copy constructor
    public LevelData(LevelData other) : this(other.tiles, other.levelScale, other.anchor)
    { }

    public static LevelData Deserialize(string jsonStr)
    {
        try
        {
            return JsonConvert.DeserializeObject<LevelData>(jsonStr);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public void SetLevelScale(float levelScale)
    {
        Vector2 oldCenter = center;
        this.levelScale = levelScale;
        anchor = oldCenter - new Vector2(width * levelScale / 2, height * levelScale / 2) + new Vector2(levelScale / 2, levelScale / 2);
    }

    public Vector2 WorldLocation(Vector2Int coords) => WorldLocation(coords.x, coords.y);
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

    [JsonConstructor]
    public LevelTile(TileType tileType)
    {
        this.tileType = tileType;
    }

    public LevelTile(LevelTile other)
    {
        tileType = other.tileType;
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
    waterHazard,
}