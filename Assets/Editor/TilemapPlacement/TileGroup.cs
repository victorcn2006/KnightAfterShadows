using UnityEngine;
using System.Collections.Generic;

namespace TilePlacement.Editor {
    [System.Serializable]
    public class TileGroup {
        public string name = "New Group";
        public Color accentColor = Color.gray;
        public bool collapsed = false;
        public List<TilePrefabEntry> tiles = new List<TilePrefabEntry>();
    }
}