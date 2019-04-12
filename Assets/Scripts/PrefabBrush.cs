using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabBrush : MonoBehaviour {
    [SerializeField] private float brushSize = 5f;
    [SerializeField] private float minimumDistance = 1f;
    [SerializeField] private Vector2 scale = new Vector2(.5f, 1.5f);
    [SerializeField] private float spawnDelay = 1f;
    [SerializeField] private int prefabDensity = 3;
    [SerializeField] private bool eraserOn = false;

    [SerializeField] private GameObject[] prefabs = null;

    public GameObject targetGround = null;
    [HideInInspector] public bool enableBrush = false;
    //[HideInInspector] public List<GameObject> meshList = new List<GameObject>();
    private GameObject parent = null;

    private List<GameObject> meshCollection = new List<GameObject>();

    private Vector3 hitPoint = Vector3.zero;
    private float elapsed = 0;

    private void OnEnable() {
        if (!Application.isEditor) {
            Destroy(this);
        }
        enableBrush = false;
        eraserOn = false;
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

        if (e.GetTypeForControl(controlID) == EventType.KeyDown && e.keyCode == KeyCode.RightBracket) {
            brushSize += 2;
        }
        else if (e.GetTypeForControl(controlID) == EventType.KeyDown && e.keyCode == KeyCode.LeftBracket) {
            brushSize -= 2;
        }



        if (e.GetTypeForControl(controlID) == EventType.KeyDown && e.keyCode == KeyCode.E) {
            eraserOn = !eraserOn;
            if (enableBrush) Debug.Log("Eraser mode on");
            else Debug.Log("Eraser mode off");
        }

        Vector3 mousePosition = e.mousePosition;
        float ppp = EditorGUIUtility.pixelsPerPoint;
        mousePosition.y = scene.camera.pixelHeight - mousePosition.y * ppp;
        mousePosition.x *= ppp;

        if (Physics.Raycast(scene.camera.ScreenPointToRay(mousePosition), out RaycastHit hit)) {
            hitPoint = hit.point;

            if (hit.collider.gameObject != targetGround) return;

            if (!((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)) {
                if (e.type != EventType.Layout && e.type != EventType.Repaint) e.Use();
                return;
            }

            if (eraserOn) {
                foreach (GameObject go in meshCollection.ToArray()) {
                    if (Vector3.Distance(go.transform.position, hitPoint) <= brushSize) {
                        meshCollection.Remove(go);
                        DestroyImmediate(go);
                    }
                }
                return;
            }

            if (parent == null) {
                parent = new GameObject("BrushedItems");
                elapsed = float.MaxValue;
                meshCollection = new List<GameObject>();
            }
            if (elapsed < spawnDelay && e.type == EventType.MouseDrag) {
                elapsed += Time.deltaTime;
                return;
            }
            for (int i = 0; i < prefabDensity; i++) {
                GameObject prefabInstance = Instantiate(prefabs[Random.Range(0, prefabs.Length)]);

                Vector3 randomPosition = hitPoint + Random.insideUnitSphere * brushSize;
                randomPosition = randomPosition.SetY(Terrain.activeTerrain.SampleHeight(randomPosition) + prefabInstance.transform.position.y);

                prefabInstance.transform.position = randomPosition;
                prefabInstance.transform.eulerAngles = prefabInstance.transform.eulerAngles.SetY(Random.Range(0, 360));

                prefabInstance.transform.localScale *= Random.Range(scale.x, scale.y);
                prefabInstance.transform.parent = parent.transform;

                foreach (GameObject go in meshCollection) {
                    if (Vector3.Distance(go.transform.position, prefabInstance.transform.position) < minimumDistance) {
                        DestroyImmediate(prefabInstance);
                        break;
                    }
                }
                if (prefabInstance != null) meshCollection.Add(prefabInstance);
            }
            elapsed = 0;
        }
    }

    private void OnDrawGizmos() {
        if (!enableBrush) return;
        if (eraserOn) Gizmos.color = Color.red;
        else Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(hitPoint, brushSize);
    }

    // public void AddMesh(GameObject mesh) {
    //    meshList.Add(mesh);
    //}
}