using UnityEditor;
using UnityEngine;
using World;

[CustomEditor(typeof(TacticsGrid))]
public class GridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TacticsGrid grid = (TacticsGrid)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate map"))
            grid.GenerateMap();
        if (GUILayout.Button("Clear map"))
            grid.ClearMap();
        EditorGUILayout.EndHorizontal();
    }
}
