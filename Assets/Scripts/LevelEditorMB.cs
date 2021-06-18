#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditorMB : MonoBehaviour
{
    public GameManagerMB gameManager;
    public LevelData level => gameManager.level;

    public LevelTile tileToAdd;

    public KeyCode placeTileKey;

    public float levelScale; // tile length in unity units
    public Vector2 anchor;
    public int width;
    public int height;

    public void ResetLevel() => gameManager.ResetLevel();

    public void Update()
    {
        if (Input.GetKeyDown(placeTileKey))
        {
            var coords = level.GridLocation(Utils.MousePosPlane());
            if (coords == null) return;
            level.tiles[coords.Value.x, coords.Value.y] = tileToAdd;
            gameManager.InitFromLevel(level);
        }
    }

    public void MakeNewLevel()
    {
        LevelData level = new LevelData(width, height, levelScale, anchor);
    }
}

[CustomEditor(typeof(LevelEditorMB))]
public class LevelEditorMB_Editor : Editor
{
    public LevelEditorMB targetRef => (LevelEditorMB)target;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Editor");

        if (GUILayout.Button("Make new level"))
        {
            targetRef.MakeNewLevel();
        }
    }
}

#endif