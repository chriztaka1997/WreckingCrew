#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System;

public class LevelEditorMB : MonoBehaviour
{
    public static LevelEditorMB instance { get; private set; }
    public GameManagerMB gameManager;
    public LevelData level;

    public LevelTile tileToAdd;

    public float tileSize; // tile length in unity units
    public Vector2 center;
    public int width;
    public int height;

    [TextArea]
    public string json;
    [SerializeField]
    [HideInInspector]
    public string internalJson;

    [TextArea]
    public string sheetsImportJson;

    public EditorLabelMB editorLabelPF;
    public GameObject labelsObject;

    public Color EnemySpawnColor;
    public Color PlayerSpawnColor;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        CleanScene();
    }


    public void ResetLevel()
    {
        if (gameManager.levelMngr == null)
        {
            gameManager.levelMngr = Instantiate(gameManager.levelMngrPF);
            gameManager.levelMngr.name = "LevelManager";
        }
        gameManager.levelMngr.SetLevel(level);
        AddLabels();
        SerializeInternal();
    }

    public void DeserializeSheetsJson()
    {
        LevelTile[,] tiles = JsonConvert.DeserializeObject<LevelTile[,]>(sheetsImportJson, new SheetsLevelTileConverter());
        Vector2 anchor = center - new Vector2(width * tileSize / 2, height * tileSize / 2) + new Vector2(tileSize / 2, tileSize / 2);
        level = new LevelData(tiles, tileSize, anchor);

        ResetLevel();
    }

    public void SerializeInternal()
    {
        internalJson = level.ToJsonString();
    }

    public void DeserializeInteral()
    {
        level = LevelData.Deserialize(internalJson);
        if (level == null)
        {
            MakeNewLevel();
        }
        AddLabels();
    }

    public void PlaceTile(int x, int y)
    {
        DeserializeInteral();
        level.tiles[x, y] = new LevelTile(tileToAdd);
        ResetLevel();
    }

    public void PlaceTileRect(int x, int y, int width, int height)
    {
        DeserializeInteral();
        for (int ix = x; ix < level.width && ix < x + width; ix++)
        {
            for (int iy = y; iy < level.height && iy < y + height; iy++)
            {
                level.tiles[ix, iy] = new LevelTile(tileToAdd);
            }
        }
        ResetLevel();
    }

    public void MirrorLeftRight()
    {
        DeserializeInteral();
        for (int ix = 0; ix < level.width / 2; ix++)
        {
            for (int iy = 0; iy < level.height; iy++)
            {
                level.tiles[width - ix - 1, iy] = new LevelTile(level.tiles[ix,iy]);
            }
        }
        ResetLevel();
    }

    public void MirrorBottomTop()
    {
        DeserializeInteral();
        for (int ix = 0; ix < level.width; ix++)
        {
            for (int iy = 0; iy < level.height / 2; iy++)
            {
                level.tiles[ix, height - iy - 1] = new LevelTile(level.tiles[ix, iy]);
            }
        }
        ResetLevel();
    }

    public void ChangeTileSize()
    {
        DeserializeInteral();
        level.SetLevelScale(tileSize);
        ResetLevel();
    }

    public void MakeNewLevel()
    {
        level = new LevelData(width, height, tileSize, center);
        ResetLevel();
    }

    public void LoadLevelJson()
    {
        level = LevelData.Deserialize(json);
        ResetLevel();
    }

    public void PrintJson()
    {
        level = LevelData.Deserialize(internalJson);
        if (level == null)
        {
            MakeNewLevel();
        }
        json = level.ToJsonString();
    }

    public void CleanScene()
    {
        if (gameManager.levelMngr != null)
            Utils.TryDestroy(gameManager.levelMngr.gameObject);
        Utils.TryDestroy(labelsObject);
    }

    public GameObject GetEditorObject()
    {
        if (labelsObject == null)
        {
            labelsObject = new GameObject("Labels Object");
            labelsObject.transform.SetParent(transform);
        }
        return labelsObject;
    }

    public void AddPlayerSpawn(int x, int y)
    {
        EditorLabelMB label = Instantiate(editorLabelPF, GetEditorObject().transform);
        string name = string.Format("Player Spawn: ({0}, {1})", x, y);
        label.name = name;
        label.Init("Player Spawn", level.levelScale, level.WorldLocation(x, y), PlayerSpawnColor);
    }

    public void AddEnemySpawn(int x, int y)
    {
        EditorLabelMB label = Instantiate(editorLabelPF, GetEditorObject().transform);
        string name = string.Format("Enemy Spawn: ({0}, {1})", x, y);
        label.name = name;
        label.Init("Enemy Spawn", level.levelScale, level.WorldLocation(x, y), EnemySpawnColor);
    }

    public void AddLabels()
    {
        Utils.TryDestroy(labelsObject);
        for (int ix = 0; ix < level.width; ix++)
        {
            for (int iy = 0; iy < level.height; iy++)
            {
                switch (level.tiles[ix, iy].tileType)
                {
                    case TileType.enemySpawn:
                        AddEnemySpawn(ix, iy);
                        break;
                    case TileType.playerSpawn:
                        AddPlayerSpawn(ix, iy);
                        break;
                }
            }
        }
    }
}

public class SheetsLevelTileConverter : JsonConverter<LevelTile>
{
    public override void WriteJson(JsonWriter writer, LevelTile value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }

    public override LevelTile ReadJson(JsonReader reader, Type objectType, LevelTile existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string s = (string)reader.Value;
        TileType tt;
        if (!Enum.TryParse(s, out tt))
        {
            tt = TileType.empty;
        }
        return new LevelTile(tt);
    }
}

[CustomEditor(typeof(LevelEditorMB))]
public class LevelEditorMB_Editor : Editor
{
    public LevelEditorMB targetRef => (LevelEditorMB)target;

    public int x;
    public int y;
    public int placeWidth;
    public int placeHeight;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Editor");

        x = EditorGUILayout.IntSlider("x", x, 0, targetRef.width - 1);
        y = EditorGUILayout.IntSlider("y", y, 0, targetRef.height - 1);
        placeWidth = EditorGUILayout.IntSlider("Block Width", placeWidth, 1, targetRef.width);
        placeHeight = EditorGUILayout.IntSlider("Block Height", placeHeight, 1, targetRef.height);

        EditorGUILayout.LabelField("Editor");

        if (GUILayout.Button("Place Single Tile"))
        {
            targetRef.PlaceTile(x, y);
        }

        if (GUILayout.Button("Place Rect"))
        {
            targetRef.PlaceTileRect(x, y, placeWidth, placeHeight);
        }

        if (GUILayout.Button("Mirror Left Right"))
        {
            targetRef.MirrorLeftRight();
        }

        if (GUILayout.Button("Mirror Bottom Top"))
        {
            targetRef.MirrorBottomTop();
        }

        if (GUILayout.Button("Make new level"))
        {
            targetRef.MakeNewLevel();
        }

        if (GUILayout.Button("Change Tile Size"))
        {
            targetRef.ChangeTileSize();
        }

        if (GUILayout.Button("Load From Google Sheets Json"))
        {
            targetRef.DeserializeSheetsJson();
        }

        if (GUILayout.Button("Deserialize Json"))
        {
            targetRef.LoadLevelJson();
        }

        if (GUILayout.Button("Print Json"))
        {
            targetRef.PrintJson();
        }

        if (GUILayout.Button("Clean Scene"))
        {
            targetRef.CleanScene();
        }
    }
}

#endif