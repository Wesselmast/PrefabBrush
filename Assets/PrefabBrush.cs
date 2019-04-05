using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabBrush : MonoBehaviour {
    private void OnEnable() {
        if (!Application.isEditor) {
            Destroy(this);
        }
        SceneView.onSceneGUIDelegate += OnScene;
    }

    private void OnDisable() {
        SceneView.onSceneGUIDelegate -= OnScene;
    }

    private static void OnScene(SceneView scene) {
        if (Event.current.keyCode == KeyCode.Q) {
            Vector3 mousePosition = Event.current.mousePosition;
            float ppp = EditorGUIUtility.pixelsPerPoint;
            mousePosition.y = scene.camera.pixelHeight - mousePosition.y * ppp;
            mousePosition.x *= ppp;
            if (Physics.Raycast(scene.camera.ScreenPointToRay(mousePosition), out RaycastHit hit)) {
                GameObject prefab = (GameObject)Resources.Load("Example", typeof(GameObject));
                GameObject prefabInstance = ((GameObject)PrefabUtility.InstantiatePrefab(prefab));
                prefabInstance.transform.position = hit.point;
            }
            Event.current.Use();
        }
    }
}