using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Prefab Brush Save File", menuName = "PrefabBrush/Save File")]
public class PrefabBrushSavefile : ScriptableObject {
    [HideInInspector] public List<GameObject> MeshCollection = new List<GameObject>();
}
