using Controller;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PlayerReferences))]
public class PlayerReferencesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var references = (PlayerReferences)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Build References"))
            references.BuildReferences();
        EditorGUILayout.EndHorizontal();
    }
}
