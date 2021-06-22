#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditorMB : MonoBehaviour
{
    public GameManagerMB gameManager;
    public LevelData level;

    public LevelTile tileToAdd;

    public float levelScale; // tile length in unity units
    public Vector2 anchor;
    public int width;
    public int height;

    [TextArea]
    public string json;
    private string internalJson;

    public EditorLabelMB editorLabelPF;
    public GameObject labelsObject;

    public Color EnemySpawnColor;
    public Color PlayerSpawnColor;


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
        level.tiles[x, y] = tileToAdd;
        ResetLevel();
    }

    public void PlaceTileRect(int x, int y, int width, int height)
    {
        DeserializeInteral();
        for (int ix = x; ix < level.width && ix < x + width; ix++)
        {
            for (int iy = y; iy < level.height && iy < y + height; iy++)
            {
                level.tiles[ix, iy] = tileToAdd;
            }
        }
        ResetLevel();
    }

    public void MakeNewLevel()
    {
        level = new LevelData(width, height, levelScale, anchor);
        ResetLevel();
    }

    public void LoadLevelJson()
    {
        level = LevelData.Deserialize(json);
        ResetLevel();
    }

    public void PrintJson()
    {
        DeserializeInteral();
        json = level.ToJsonString();
    }

    public void CleanScene()
    {
        Utils.TryDestroy(gameManager.levelMngr?.gameObject);
        Utils.TryDestroy(gameManager.player?.gameObject);
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

        if (GUILayout.Button("Make new level"))
        {
            targetRef.MakeNewLevel();
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