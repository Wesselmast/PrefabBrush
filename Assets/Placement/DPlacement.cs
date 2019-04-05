using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DPlacementGUI))]
public class DPlacement : Editor {
    public override void OnInspectorGUI() {
        DPlacementGUI window = EditorWindow.GetWindow<DPlacementGUI>();
        Event e = Event.current;
        if (e.type != EventType.KeyDown) return;
        if (Event.current.keyCode == (KeyCode.T)) {
            Debug.Log("placed Object");
            PlaceDecall(window.objectToPlace);
        }
    }


    public void PlaceDecall(GameObject decallSelected) {
        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(Camera.current.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
                Instantiate(decallSelected, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                Debug.Log("Placed" + decallSelected.name + "at" + hit.point);
            }
        }
    }
}