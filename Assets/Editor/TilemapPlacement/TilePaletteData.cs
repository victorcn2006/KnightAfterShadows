using UnityEngine;
using System.Collections.Generic;

namespace TilePlacement.Editor {
    [CreateAssetMenu(
        fileName = "TilePalette",
        menuName = "Tile Placement/Palette")]
    public class TilePaletteData : ScriptableObject {
        public List<TileGroup> groups = new List<TileGroup>();
    }
}