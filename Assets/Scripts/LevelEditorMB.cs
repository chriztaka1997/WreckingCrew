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


    public void ResetLevel()
    {
        gameManager.ResetLevel();
        gameManager.level = level;
        gameManager.InitFromLevel(level);
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
    }
}

#endif