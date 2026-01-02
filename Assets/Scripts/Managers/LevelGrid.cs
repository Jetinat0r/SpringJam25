using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

//Thanks to kurtdekker on Github: https://gist.github.com/kurtdekker/079cb9fbc6d2ecf0a13ba045e96d23d2
// @kurtdekker - ultra Cheesy grid with in-built editor for Unity3D
//
// To use:
//	make an empty game object
//	drag this script on it
// 	make a prefab out of it
//	select the prefab to edit the grid
//
// now you can cheesy-easy edit a smallish grid in the Unity editor window
//
public class LevelGrid : MonoBehaviour
{
    [Header("Cell values cycle through this list. MUST BE SINGLE CHARS!")]
    public List<string> cellValues = new List<string> { "F", "T", "*" };

    [Header("Actual saved payload. Use GetCell(x,y) to read!")]
    [Header("WARNING: changing this will nuke your data!")]
    public string data;

    public int width = 4;
    public int height = 4;

    public string GetCell(int x, int y)
    {
        int n = GetIndex(x, y);
        return data.Substring(n, 1);
    }

    public int GetIndex(int x, int y)
    {
        if (x < 0) return -1;
        if (y < 0) return -1;
        if (x >= width) return -1;
        if (y >= height) return -1;
        return x + y * width;
    }

    public void ToggleCell(int x, int y)
    {
        int n = GetIndex(x, y);
        if (n >= 0)
        {
            var cell = data.Substring(n, 1);

            int c = cellValues.IndexOf(cell);
            c++;
            if (c >= cellValues.Count) { c = 0; }

            cell = cellValues[c];

        #if UNITY_EDITOR
            Undo.RecordObject(this, "Toggle Cell");
        #endif
            // reassemble
            data = data.Substring(0, n) + cell + data.Substring(n + 1);
        #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
        #endif
        }
    }

    public bool IsValidCell(int x, int y)
    {
        return !(x < 0 || y < 0 || x >= width || y >= height);
    }

    public Vector2Int FindZeroRoomReferenceCell()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (GetCell(i, j) == "*")
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        Debug.LogWarning("NO ZERO ROOM REFERENCE CELL IN CURRENT LEVEL!");
        return new Vector2Int(0, 0);
    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (data == null || data.Length != (width * height))
        {
            Undo.RecordObject(this, "Resize");

            if (width < 1) width = 1;
            if (height < 1) height = 1;

            // make a default layout
            data = "";
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    string cell = "F";

                    data = data + cell;
                }
            }

            EditorUtility.SetDirty(this);
        }
    }

    public void Reset()
    {
        OnValidate();
    }
#endif



#if UNITY_EDITOR
    [CustomEditor(typeof(LevelGrid))]
    public class CheesyGridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var grid = (LevelGrid)target;

            EditorGUILayout.BeginVertical();

            GUILayout.Label("WARNING: Save and commit your prefab/scene OFTEN!");
            GUILayout.Label("F = NO ROOM");
            GUILayout.Label("T = ROOM");
            GUILayout.Label("* = (0, 0) ROOM");

            for (int y = 0; y < grid.width; y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < grid.height; x++)
                {
                    int n = grid.GetIndex(x, y);

                    var cell = grid.data.Substring(n, 1);

                    // hard-coded some cheesy color map - improve it by all means!
                    GUI.color = Color.gray;
                    if (cell == "F") GUI.color = Color.red;
                    if (cell == "T") GUI.color = Color.white;
                    if (cell == "*") GUI.color = Color.green;

                    if (GUILayout.Button(cell, GUILayout.Width(20)))
                    {
                        grid.ToggleCell(x, y);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUI.color = Color.yellow;

            GUILayout.Label("DANGER ZONE BELOW THIS AREA!");

            GUI.color = Color.white;

            EditorGUILayout.EndVertical();

            DrawDefaultInspector();
        }
    }
#endif
}
