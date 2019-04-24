using UnityEditor;
using UnityEngine;

public class AssetImporter : AssetPostprocessor {
    private void OnPreprocessModel() {
        ModelImporter importer = (ModelImporter)assetImporter;

        //Model
        importer.globalScale = 1;
        importer.importBlendShapes = false;
        importer.importVisibility = true;
        importer.importCameras = false;
        importer.importLights = false;
        importer.preserveHierarchy = false;
        importer.meshCompression = ModelImporterMeshCompression.Off;
        importer.isReadable = true;
        importer.optimizeMesh = true;
        importer.addCollider = false;
        importer.keepQuads = false;
        importer.weldVertices = false;
        importer.indexFormat = ModelImporterIndexFormat.Auto;
        importer.importNormals = ModelImporterNormals.Import;
        importer.normalCalculationMode = ModelImporterNormalCalculationMode.AreaAndAngleWeighted;
        importer.normalSmoothingSource = ModelImporterNormalSmoothingSource.PreferSmoothingGroups;
        importer.normalSmoothingAngle = 60;
        importer.importTangents = ModelImporterTangents.CalculateMikk;
        importer.swapUVChannels = false;
        importer.generateSecondaryUV = false;

        //Rig
        importer.animationType = ModelImporterAnimationType.None;

        //Animation
        importer.importConstraints = false;
        importer.importAnimation = false;

        //Materials
        importer.importMaterials = false;
        importer.useSRGBMaterialColor = false;
        importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
        foreach (string assetPath in importedAssets) {
            if (assetPath.EndsWith(".fbx")) {
                GameObject gameObject = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                GameObject instance = GameObject.Instantiate(gameObject);
                if (EditorUtility.DisplayDialog("Make it static?",
                    "Do you want to make the prefab " + gameObject.name + " static? It will be better on performance, but will restrict the object from moving!",
                    "Yes",
                    "No")) {
                    instance.isStatic = true;
                }
                PrefabUtility.SaveAsPrefabAsset(instance, "Assets/Resources/Prefabs/" + gameObject.name + "_prefab.prefab");
                GameObject.DestroyImmediate(instance);
            }
        }
    }
}