using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabBrush : MonoBehaviour {
    [SerializeField] private float brushSize = 5f;
    [SerializeField] private float scaleFactor = 1.5f;
    [SerializeField] private float spawnDelay = 1f;
    [SerializeField] private int prefabDensity = 3;

    public GameObject targetGround = null;
    [HideInInspector] public bool enableBrush = false;
    [HideInInspector] public List<GameObject> meshList;
    private GameObject parent = null;

    private Vector3 hitPoint = Vector3.zero;
    private float elapsed = 0;

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
        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Keyboard);

        if (e.GetTypeForControl(controlID) == EventType.KeyDown && e.keyCode == KeyCode.B) {
            enableBrush = !enableBrush;
            if (enableBrush) Debug.Log("Brush mode on");
            else Debug.Log("Brush mode off");
        }

        if (!enableBrush) return;

        HandleUtility.AddDefaultControl(controlID);
        Tools.current = Tool.None;

        if (!((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)) return;

        Vector3 mousePosition = e.mousePosition;
        float ppp = EditorGUIUtility.pixelsPerPoint;
        mousePosition.y = scene.camera.pixelHeight - mousePosition.y * ppp;
        mousePosition.x *= ppp;

        if (Physics.Raycast(scene.camera.ScreenPointToRay(mousePosition), out RaycastHit hit)) {
            if (hit.collider.gameObject != targetGround) return;
            if (parent == null) parent = new GameObject("BrushedItems");

            hitPoint = hit.point;

            if (elapsed < spawnDelay) {
                elapsed += Time.deltaTime;
            }
            else {
                for (int i = 0; i < prefabDensity; i++) {
                    GameObject prefab = (GameObject)Resources.Load("Example", typeof(GameObject));
                    GameObject prefabInstance = ((GameObject)PrefabUtility.InstantiatePrefab(prefab));

                    float randomNum = Random.Range(0f, 1f);
                    float circumfrence = randomNum * 2 * Mathf.PI;
                    float radius = brushSize * Mathf.Sqrt(randomNum);
                    float randomX = hitPoint.x + radius * Mathf.Cos(circumfrence);
                    float randomY = hitPoint.z + radius * Mathf.Sin(circumfrence);
                    Vector3 randomPosition = new Vector3(randomX, 0, randomY);
                    randomPosition = randomPosition.SetY(Terrain.activeTerrain.SampleHeight(randomPosition) + prefabInstance.transform.position.y);

                    prefabInstance.transform.position = randomPosition;
                    prefabInstance.transform.eulerAngles = prefabInstance.transform.eulerAngles.SetY(Random.Range(0, 360));

                    prefabInstance.transform.localScale *= randomNum * scaleFactor;
                    prefabInstance.transform.parent = parent.transform;
                }
                elapsed = 0;
            }
        }
        Event.current.Use();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(hitPoint, brushSize);
    }

    public void AddMesh(GameObject mesh) {
        meshList.Add(mesh);
    }
}