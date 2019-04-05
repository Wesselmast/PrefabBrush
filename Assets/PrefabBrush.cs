using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabBrush : MonoBehaviour {

    [SerializeField] private GameObject targetGround = null;
    [SerializeField] private bool enableBrush = false;

    private GameObject parent = null;

    private void OnEnable() {
        if (!Application.isEditor) {
            Destroy(this);
        }
        enableBrush = false;
        SceneView.onSceneGUIDelegate += OnScene;
    }

    private void OnDisable() {
        SceneView.onSceneGUIDelegate -= OnScene;
    }

    private void OnScene(SceneView scene) {
        if (!enableBrush) return;
        if (Event.current.keyCode == KeyCode.Q) {
            Vector3 mousePosition = Event.current.mousePosition;
            float ppp = EditorGUIUtility.pixelsPerPoint;
            mousePosition.y = scene.camera.pixelHeight - mousePosition.y * ppp;
            mousePosition.x *= ppp;
            if (Physics.Raycast(scene.camera.ScreenPointToRay(mousePosition), out RaycastHit hit)) {
                if (hit.collider.gameObject != targetGround) return;
                if (parent == null) {
                    parent = new GameObject("Parent");
                }
                GameObject prefab = (GameObject)Resources.Load("Example", typeof(GameObject));
                GameObject prefabInstance = ((GameObject)PrefabUtility.InstantiatePrefab(prefab));
                prefabInstance.transform.position = hit.point;
                prefabInstance.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                prefabInstance.transform.parent = parent.transform;
            }
        }
    }
}