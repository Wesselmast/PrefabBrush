using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Leeg))]
public class NewScript : Editor {
    private Vector3 position;
    public Vector3 Position {
        get {
            return position;
        }
    }

    private void OnSceneGUI() {
        Tools.current = Tool.None;
        if (Event.current == null) return;
        position = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(Event.current.mousePosition);

        if (!Event.current.Equals(Event.KeyboardEvent("return"))) return;
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
    }
}