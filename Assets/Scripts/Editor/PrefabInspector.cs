using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabBrush))]
public class PrefabInspector : Editor
{
    Vector2 scrolling = Vector2.zero;
    SerializedProperty meshes;
    int cubes = 3;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PrefabBrush brush = (PrefabBrush)target;
       
        EditorGUILayout.LabelField("PaintMode", EditorStyles.boldLabel);
        if (brush.enableBrush == true)
        {
            if (GUILayout.Button("Disable"))
            {
                brush.enableBrush = false;
                Debug.Log("Disabled Placement Mode");
            }
        }
        else
        {
            if (GUILayout.Button("Enable Paint"))
            {
                brush.enableBrush = true;
                Debug.Log("Enabled Placement Mode");
            }
        }

        EditorGUILayout.BeginVertical("Box");
        {
            scrolling = EditorGUILayout.BeginScrollView(scrolling, false, false, GUILayout.ExpandWidth(false), GUILayout.Height(130));
            {
                for (int i = 0; i < brush.meshList.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.ObjectField(brush.meshList[i], typeof(GameObject), true, GUILayout.Height(16.35f));
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(2.75f);
                }
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            GUILayout.BeginHorizontal("buttons");
            if (GUILayout.Button("+"))
            {
                brush.AddMesh(null);
            }
            if (GUILayout.Button("-"))
            {
                //delete mesh function needed
            }
            GUILayout.EndHorizontal();

        }
    }
}