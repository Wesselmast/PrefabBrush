using UnityEngine;

public class Decall : MonoBehaviour {
    public static void PlaceDecall(GameObject decallSelected) {
        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(Camera.current.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
                Instantiate(decallSelected, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                Debug.Log("Placed" + decallSelected.name + "at" + hit.point);
            }
        }
    }
}