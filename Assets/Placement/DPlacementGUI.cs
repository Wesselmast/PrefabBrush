using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class DPlacementGUI : EditorWindow {
    public GameObject objectToPlace;
    public static bool shouldPlaceDecall;

    [MenuItem("Window/Decal Placement")]
    public static void ShowWindow() {
        DPlacementGUI window = GetWindow<DPlacementGUI>("Decall Placement");
        window.minSize = new Vector2(200, 200);
    }

    private void OnGUI() {
        GUILayout.Label("placement", EditorStyles.boldLabel);
        GUILayout.Label("Scale", EditorStyles.foldout);
        GUILayout.Label("My Cube Prefab");
        objectToPlace = EditorGUILayout.ObjectField(objectToPlace, typeof(GameObject), true) as GameObject;

    }/*

        if (Decall.paintMode == true) {
            if (GUILayout.Button("Disable")) {
                Decall.paintMode = false;
                Debug.Log("Disabled Placement Mode");
            }
        }
        else {
            if (GUILayout.Button("Enable")) {
                Decall.paintMode = true;
                Debug.Log("Enabled Placement Mode");
            }
        }

        if (GUILayout.Button("Gerrit")) {
            Decall.Gerrit();
        }
    }

    void Update() {
        if (Decall.paintMode == true) {
            Tools.current = Tool.None;
        }

        if (Decall.paintMode == true && Decall.Click == true) {
            Decall.PlaceDecall(objectToPlace);
            Repaint();
            Decall.Click = false;
        }
    }
}*/
}