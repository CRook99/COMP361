using UnityEditor;
using UnityEngine;
using World;

[CustomEditor(typeof(MapParser))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var parser = (MapParser)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate map"))
            parser.GenerateMap();
        if (GUILayout.Button("Clear map"))
            parser.ClearMap();
        EditorGUILayout.EndHorizontal();
    }
}
