using UnityEngine;

namespace TilePlacement.Editor {
    [System.Serializable]
    public class TilePrefabEntry {
        public string displayName = "Tile";
        public GameObject prefab;
        public Texture2D thumbnail;
        public string[] tags;
        public Vector3 placementOffset;
        public float randomYRotMin = 0f;
        public float randomYRotMax = 0f;

        [Range(0.1f, 10f)]
        public float weight = 1f;
    }
}
