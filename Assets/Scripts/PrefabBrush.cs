using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class PrefabBrush : MonoBehaviour {
    [SerializeField] private float brushSize = 5f;
    [SerializeField] private float minimumDistance = 1f;
    [SerializeField] private Vector2 scale = new Vector2(.5f, 1.5f);
    [SerializeField] private float spawnDelay = 1f;
    [SerializeField] private int prefabDensity = 3;
    [SerializeField] [Range(0, 1)] private float maxSlopeRange = .9f;
    [SerializeField] private bool eraserOn = false;

    [SerializeField] private GameObject[] prefabs = null;

    [HideInInspector] public bool enableBrush = false;
    private GameObject parent = null;

    [SerializeField] private PrefabBrushSavefile saveFile = null;

    private Vector3 hitPoint = Vector3.zero;
    private float elapsed = 0;
    private Terrain[] terrains = null;

    private void OnEnable() {
        if (!Application.isEditor || Application.isPlaying) {
            Destroy(gameObject);
        }
        enableBrush = false;
        eraserOn = false;
        terrains = FindObjectsOfType<Terrain>();
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
        }

        if (!enableBrush) return;

        HandleUtility.AddDefaultControl(controlID);
        Tools.current = Tool.None;

        if (e.GetTypeForControl(controlID) == EventType.KeyDown && e.keyCode == KeyCode.RightBracket) {
            brushSize += 2;
        }
        else if (e.GetTypeForControl(controlID) == EventType.KeyDown && e.keyCode == KeyCode.LeftBracket && brushSize >= 0) {
            brushSize -= 2;
        }

        if (e.GetTypeForControl(controlID) == EventType.KeyDown && e.keyCode == KeyCode.E) {
            eraserOn = !eraserOn;
        }

        Vector3 mousePosition = e.mousePosition;
        float ppp = EditorGUIUtility.pixelsPerPoint;
        mousePosition.y = scene.camera.pixelHeight - mousePosition.y * ppp;
        mousePosition.x *= ppp;

        if (Physics.Raycast(scene.camera.ScreenPointToRay(mousePosition), out RaycastHit hit)) {
            hitPoint = hit.point;

            Terrain terrain = null;
            for (int i = 0; i < terrains.Length; i++) {
                if (terrains[i].gameObject == hit.collider.gameObject) {
                    terrain = terrains[i];
                    break;
                }
            }
            if (terrain == null) return;

            if (!((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)) {
                if (e.type != EventType.Layout && e.type != EventType.Repaint) e.Use();
                return;
            }

            if (eraserOn) {
                foreach (GameObject go in saveFile.MeshCollection.ToArray()) {
                    if (Vector3.Distance(go.transform.position, hitPoint) <= brushSize) {
                        saveFile.MeshCollection.Remove(go);
                        DestroyImmediate(go);
                    }
                }
                return;
            }

            if (parent == null) {
                parent = new GameObject("BrushedItems");
                elapsed = float.MaxValue;
                saveFile.MeshCollection = new List<GameObject>();
            }
            if (elapsed < spawnDelay && e.type == EventType.MouseDrag) {
                elapsed += Time.deltaTime;
                return;
            }

            for (int i = 0; i < prefabDensity; i++) {
                int randomPrefab = Random.Range(0, prefabs.Length);

                Vector3 randomPosition = hitPoint + Random.insideUnitSphere * brushSize;
                randomPosition = randomPosition.SetY(terrain.SampleHeight(randomPosition) + prefabs[randomPrefab].transform.position.y);

                Vector3 terrainSize = terrain.terrainData.size;
                float posX = -(terrain.GetPosition().x / terrainSize.x);
                float posZ = -(terrain.GetPosition().z / terrainSize.z);
                float yNormal = terrain.terrainData.GetInterpolatedNormal(posX + (randomPosition.x / terrainSize.x), posZ + (randomPosition.z / terrainSize.z)).y;
                if (yNormal < maxSlopeRange) {
                    continue;
                }
                bool tooClose = false;
                foreach (GameObject go in saveFile.MeshCollection) {
                    if (Vector3.Distance(go.transform.position, randomPosition) < minimumDistance) {
                        tooClose = true;
                    }
                }
                if (tooClose) continue;

                GameObject finalInstance = Instantiate(prefabs[randomPrefab]);
                finalInstance.transform.parent = parent.transform;
                finalInstance.transform.position = randomPosition;
                finalInstance.transform.eulerAngles = finalInstance.transform.eulerAngles.SetY(Random.Range(0, 360));
                finalInstance.transform.localScale *= Random.Range(scale.x, scale.y);
                saveFile.MeshCollection.Add(finalInstance);
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
}