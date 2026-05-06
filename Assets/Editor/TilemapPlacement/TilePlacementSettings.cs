using UnityEngine;
using UnityEditor;

namespace TilePlacement.Editor {
    [CreateAssetMenu(
        fileName = "TilePlacementSettings",
        menuName = "Tile Placement/Settings")]
    public class TilePlacementSettings : ScriptableObject {
        private const string ASSET_PATH = "Assets/Editor/TilePlacement/Settings.asset";

        [Header("Grid")]
        public float gridSize = 1f;
        public float snapRotation = 0f;

        [Header("Placement")]
        public Transform parentContainer;
        public bool preventOverlap = true;
        public LayerMask paintLayers = ~0;

        [Header("Random Rotation")]
        public bool randomizeRotation = false;
        [Range(0f, 360f)]
        public float rotationStep = 90f;

        private static TilePlacementSettings _instance;

        public static TilePlacementSettings instance {
            get {
                if (_instance != null) return _instance;

                _instance = AssetDatabase.LoadAssetAtPath<TilePlacementSettings>(ASSET_PATH);
                if (_instance != null) return _instance;

                _instance = CreateInstance<TilePlacementSettings>();
                System.IO.Directory.CreateDirectory("Assets/Editor/TilePlacement");
                AssetDatabase.CreateAsset(_instance, ASSET_PATH);
                AssetDatabase.SaveAssets();
                return _instance;
            }
        }
    }
}