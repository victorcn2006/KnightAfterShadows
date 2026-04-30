using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace TilePlacement.Editor {
    public class TilePaletteWindow : EditorWindow {
        // ?? Layout ???????????????????????????????????????????
        private const float TILE_SIZE = 64f;
        private const float TILE_PAD = 4f;
        private const float GROUP_HDR_H = 28f;
        private const float TOOLBAR_H = 38f;
        private const float SEARCH_H = 28f;
        private const float PREVIEW_H = 96f;
        private const float STATUS_H = 22f;

        // ?? State ????????????????????????????????????????????
        private TilePaletteData _palette;
        private TilePrefabEntry _selected;
        private TilePlacementTool _activeTool = TilePlacementTool.Place;
        private Vector2 _scrollPos;
        private string _searchQuery = "";
        private bool _showPreview = true;

        // Inline rename
        private TileGroup _renamingGroup;
        private string _renameBuffer;

        // Colors
        private static readonly Color C_BG_DARK = new Color(0.18f, 0.18f, 0.20f);
        private static readonly Color C_BG_DARKER = new Color(0.13f, 0.13f, 0.15f);
        private static readonly Color C_SEPARATOR = new Color(0.10f, 0.10f, 0.11f);
        private static readonly Color C_TILE_SEL = new Color(0.22f, 0.46f, 0.82f, 0.35f);
        private static readonly Color C_TILE_SEL_B = new Color(0.30f, 0.58f, 1.00f);
        private static readonly Color C_TILE_HOV = new Color(1f, 1f, 1f, 0.06f);
        private static readonly Color C_TOOLBAR = new Color(0.15f, 0.15f, 0.17f);

        // ?? Open ?????????????????????????????????????????????
        [MenuItem("Tools/Tile Palette %#t")]
        public static void Open() {
            TilePaletteWindow w = GetWindow<TilePaletteWindow>("Tile Palette");
            w.minSize = new Vector2(240, 420);
            w.Show();
        }

        // ?? Unity callbacks ??????????????????????????????????
        private void OnEnable() {
            LoadOrCreatePalette();
            SceneView.duringSceneGui += OnSceneGUI;
            wantsMouseMove = true;
        }

        private void OnDisable() {
            SceneView.duringSceneGui -= OnSceneGUI;
            if (_palette != null)
                EditorUtility.SetDirty(_palette);
        }

        private void OnGUI() {
            // ?? Toolbar ??????????????????????????????????????
            DrawToolbar(new Rect(0, 0, position.width, TOOLBAR_H));

            // ?? Search ???????????????????????????????????????
            DrawSearchBar(new Rect(0, TOOLBAR_H, position.width, SEARCH_H));

            // ?? Scroll area ??????????????????????????????????
            float topY = TOOLBAR_H + SEARCH_H;
            float bottomH = STATUS_H + (_showPreview ? PREVIEW_H : 0);
            float scrollH = position.height - topY - bottomH;
            DrawPaletteGroups(new Rect(0, topY, position.width, scrollH));

            // ?? Preview ??????????????????????????????????????
            if (_showPreview)
                DrawPreview(new Rect(0, topY + scrollH, position.width, PREVIEW_H));

            // ?? Status bar ???????????????????????????????????
            DrawStatusBar(new Rect(0, position.height - STATUS_H, position.width, STATUS_H));
        }

        // ????????????????????????????????????????????????????
        //  TOOLBAR
        // ????????????????????????????????????????????????????
        private void DrawToolbar(Rect r) {
            EditorGUI.DrawRect(r, C_TOOLBAR);

            float btnH = 24f;
            float btnY = r.y + (r.height - btnH) * 0.5f;
            float x = r.x + 6f;

            DrawToolBtn(new Rect(x, btnY, 68, btnH), "?  Place", TilePlacementTool.Place);
            DrawToolBtn(new Rect(x + 72, btnY, 68, btnH), "?  Erase", TilePlacementTool.Erase);
            DrawToolBtn(new Rect(x + 144, btnY, 54, btnH), "?  Pick", TilePlacementTool.Pick);

            // Right side buttons
            float rx = r.xMax - 6f;

            rx -= 24f;
            if (GUI.Button(new Rect(rx, btnY, 24, btnH),
                    new GUIContent(_showPreview ? "?" : "?", "Toggle preview"),
                    EditorStyles.miniButton))
                _showPreview = !_showPreview;

            rx -= 26f;
            if (GUI.Button(new Rect(rx, btnY, 24, btnH),
                    new GUIContent("+", "Add new group"),
                    EditorStyles.miniButton))
                OpenAddGroupWindow();
        }

        private void DrawToolBtn(Rect r, string label, TilePlacementTool tool) {
            bool active = _activeTool == tool;
            if (active) {
                EditorGUI.DrawRect(r, new Color(0.25f, 0.50f, 0.88f, 0.45f));
                DrawBorder(r, C_TILE_SEL_B);
            }
            if (GUI.Button(r, label, EditorStyles.miniButton))
                _activeTool = tool;
        }

        // ????????????????????????????????????????????????????
        //  SEARCH BAR
        // ????????????????????????????????????????????????????
        private void DrawSearchBar(Rect r) {
            EditorGUI.DrawRect(r, C_BG_DARK);
            DrawSeparator(new Rect(r.x, r.yMax - 1, r.width, 1));

            Rect inner = new Rect(r.x + 8, r.y + 5, r.width - 16, r.height - 10);
            _searchQuery = EditorGUI.TextField(inner, _searchQuery);

            if (string.IsNullOrEmpty(_searchQuery)) {
                GUIStyle ph = new GUIStyle(EditorStyles.label) {
                    normal = { textColor = new Color(0.45f, 0.45f, 0.45f) },
                    fontSize = 11
                };
                GUI.Label(inner, "  Search tiles…", ph);
            }
        }

        // ????????????????????????????????????????????????????
        //  PALETTE GROUPS
        // ????????????????????????????????????????????????????
        private void DrawPaletteGroups(Rect scrollRect) {
            bool searching = !string.IsNullOrEmpty(_searchQuery);

            // Measure total content height
            float totalH = 0f;
            foreach (TileGroup g in _palette.groups) {
                totalH += GROUP_HDR_H + 1f;
                if (!g.collapsed || searching) {
                    int cols = TileColumns(scrollRect.width);
                    int rows = Mathf.CeilToInt(
                        (float)VisibleTiles(g).Count / cols);
                    totalH += rows * (TILE_SIZE + TILE_PAD) + TILE_PAD * 2f;
                }
            }
            totalH += 36f; // add-group button

            Rect view = new Rect(0, 0, scrollRect.width - 14f, totalH);
            _scrollPos = GUI.BeginScrollView(scrollRect, _scrollPos, view);

            float y = 0f;
            for (int i = 0; i < _palette.groups.Count; i++) {
                TileGroup g = _palette.groups[i];
                y = DrawGroupHeader(g, i, y, view.width);
                if (!g.collapsed || searching)
                    y = DrawTileGrid(g, y, view.width);
            }

            if (GUI.Button(new Rect(TILE_PAD, y + 4, view.width - TILE_PAD * 2, 26),
                    "+ Add Group"))
                OpenAddGroupWindow();

            GUI.EndScrollView();
        }

        private float DrawGroupHeader(TileGroup g, int index, float y, float w) {
            Rect r = new Rect(0, y, w, GROUP_HDR_H);
            EditorGUI.DrawRect(r, C_BG_DARK);
            DrawSeparator(new Rect(r.x, r.yMax, r.width, 1));

            // Accent strip
            EditorGUI.DrawRect(new Rect(r.x, r.y, 3, r.height), g.accentColor);

            // Collapse arrow
            Rect arrowR = new Rect(r.x + 8, r.y + 8, 12, 12);
            if (GUI.Button(arrowR, g.collapsed ? "?" : "?", EditorStyles.miniLabel)) {
                g.collapsed = !g.collapsed;
                EditorUtility.SetDirty(_palette);
            }

            // Name — double-click to rename
            Rect nameR = new Rect(r.x + 26, r.y, r.width - 96, r.height);
            if (_renamingGroup == g) {
                GUI.SetNextControlName("GroupRenameField");
                _renameBuffer = GUI.TextField(
                    new Rect(nameR.x, nameR.y + 5, nameR.width - 4, 18),
                    _renameBuffer);
                GUI.FocusControl("GroupRenameField");

                Event ev = Event.current;
                if (ev.type == EventType.KeyDown) {
                    if (ev.keyCode == KeyCode.Return) {
                        if (!string.IsNullOrWhiteSpace(_renameBuffer))
                            g.name = _renameBuffer.Trim();
                        _renamingGroup = null;
                        EditorUtility.SetDirty(_palette);
                        ev.Use();
                    } else if (ev.keyCode == KeyCode.Escape) {
                        _renamingGroup = null;
                        ev.Use();
                    }
                }
            } else {
                GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel) {
                    fontSize = 11,
                    normal = { textColor = Color.white }
                };
                GUI.Label(nameR, g.name, nameStyle);

                Event ev = Event.current;
                if (ev.type == EventType.MouseDown
                    && ev.clickCount == 2
                    && nameR.Contains(ev.mousePosition)) {
                    _renamingGroup = g;
                    _renameBuffer = g.name;
                    ev.Use();
                }
            }

            // Count badge
            GUIStyle cntStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(0.45f, 0.45f, 0.45f) } };
            GUI.Label(new Rect(r.xMax - 58, r.y, 28, r.height),
                g.tiles.Count.ToString(), cntStyle);

            // Add tile button
            if (GUI.Button(new Rect(r.xMax - 46, r.y + 4, 18, 20), "+", EditorStyles.miniButton))
                OpenAddTileWindow(g);

            // Context menu button
            if (GUI.Button(new Rect(r.xMax - 24, r.y + 4, 18, 20), "?", EditorStyles.miniButton))
                ShowGroupContextMenu(g, index);

            return y + GROUP_HDR_H + 1f;
        }

        private float DrawTileGrid(TileGroup g, float y, float w) {
            List<TilePrefabEntry> tiles = VisibleTiles(g);
            int cols = TileColumns(w);
            float curX = TILE_PAD;
            float curY = y + TILE_PAD;

            for (int i = 0; i < tiles.Count; i++) {
                DrawTileCell(new Rect(curX, curY, TILE_SIZE, TILE_SIZE), tiles[i], g);
                curX += TILE_SIZE + TILE_PAD;
                if (curX + TILE_SIZE > w - TILE_PAD) {
                    curX = TILE_PAD;
                    curY += TILE_SIZE + TILE_PAD;
                }
            }

            if (curX > TILE_PAD) curY += TILE_SIZE + TILE_PAD;
            return curY + TILE_PAD;
        }

        private void DrawTileCell(Rect r, TilePrefabEntry tile, TileGroup group) {
            bool isSelected = _selected == tile;
            bool isHovered = r.Contains(Event.current.mousePosition);

            if (isSelected) {
                EditorGUI.DrawRect(r, C_TILE_SEL);
                DrawBorder(r, C_TILE_SEL_B, 2);
            } else if (isHovered) {
                EditorGUI.DrawRect(r, C_TILE_HOV);
            }

            // Thumbnail
            Rect thumbR = new Rect(r.x + 4, r.y + 4, r.width - 8, r.height - 18);
            Texture2D tex = tile.thumbnail;
            if (tex == null && tile.prefab != null) {
                tex = AssetPreview.GetAssetPreview(tile.prefab);
                if (tex == null)
                    tex = AssetPreview.GetMiniThumbnail(tile.prefab);
            }

            if (tex != null) {
                GUI.DrawTexture(thumbR, tex, ScaleMode.ScaleToFit);
            } else {
                EditorGUI.DrawRect(thumbR, new Color(0.22f, 0.22f, 0.25f));
                GUIStyle noTex = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = 9 };
                GUI.Label(thumbR, tile.prefab == null ? "No Prefab" : "Loading…", noTex);
            }

            // Label
            GUIStyle lblStyle = new GUIStyle(EditorStyles.miniLabel) {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 9,
                normal = { textColor = isSelected
                    ? Color.white
                    : new Color(0.72f, 0.72f, 0.72f) }
            };
            GUI.Label(new Rect(r.x + 2, r.yMax - 16, r.width - 4, 14),
                tile.displayName, lblStyle);

            // Mouse events
            Event ev = Event.current;
            if (ev.type == EventType.MouseDown && r.Contains(ev.mousePosition)) {
                if (ev.button == 0) {
                    SelectTile(tile);
                    ev.Use();
                } else if (ev.button == 1) {
                    ShowTileContextMenu(tile, group);
                    ev.Use();
                }
            }

            if (isHovered)
                Repaint();
        }

        // ????????????????????????????????????????????????????
        //  PREVIEW PANEL
        // ????????????????????????????????????????????????????
        private void DrawPreview(Rect r) {
            DrawSeparator(new Rect(r.x, r.y, r.width, 1));
            EditorGUI.DrawRect(r, C_BG_DARKER);

            if (_selected == null) {
                GUIStyle empty = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = 10 };
                GUI.Label(r, "Select a tile to preview", empty);
                return;
            }

            float sz = r.height - 12f;
            Rect thumbR = new Rect(r.x + 8, r.y + 6, sz, sz);
            Rect infoR = new Rect(thumbR.xMax + 10, r.y + 8,
                                    r.width - sz - 26, r.height - 16);

            Texture2D tex = _selected.thumbnail;
            if (tex == null && _selected.prefab != null)
                tex = AssetPreview.GetAssetPreview(_selected.prefab);

            if (tex != null) {
                EditorGUI.DrawRect(thumbR, new Color(0.2f, 0.2f, 0.22f));
                GUI.DrawTexture(thumbR, tex, ScaleMode.ScaleToFit);
            }

            GUIStyle h1 = new GUIStyle(EditorStyles.boldLabel) {
                fontSize = 12,
                normal = { textColor = Color.white }
            };
            GUIStyle h2 = new GUIStyle(EditorStyles.miniLabel) {
                normal = { textColor = new Color(0.50f, 0.50f, 0.50f) }
            };

            GUI.Label(new Rect(infoR.x, infoR.y, infoR.width, 18),
                _selected.displayName, h1);
            GUI.Label(new Rect(infoR.x, infoR.y + 18, infoR.width, 14),
                _selected.prefab != null
                    ? AssetDatabase.GetAssetPath(_selected.prefab)
                    : "—", h2);

            string tagStr = _selected.tags != null
                ? string.Join(", ", _selected.tags)
                : "";
            GUI.Label(new Rect(infoR.x, infoR.y + 32, infoR.width, 14),
                string.IsNullOrEmpty(tagStr) ? "No tags" : tagStr, h2);

            if (_selected.prefab != null
                && GUI.Button(new Rect(infoR.x, infoR.y + 50, 60, 18),
                    "Ping", EditorStyles.miniButton))
                EditorGUIUtility.PingObject(_selected.prefab);
        }

        // ????????????????????????????????????????????????????
        //  STATUS BAR
        // ????????????????????????????????????????????????????
        private void DrawStatusBar(Rect r) {
            DrawSeparator(new Rect(r.x, r.y, r.width, 1));
            EditorGUI.DrawRect(r, C_BG_DARKER);

            int total = _palette.groups.Sum(g => g.tiles.Count);

            GUIStyle st = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(0.50f, 0.50f, 0.50f) } };

            GUI.Label(new Rect(r.x + 8, r.y + 3, 110, r.height),
                "Tool: " + _activeTool, st);
            GUI.Label(new Rect(r.x + 120, r.y + 3, 140, r.height),
                "Selected: " + (_selected != null ? _selected.displayName : "None"), st);
            GUI.Label(new Rect(r.xMax - 70, r.y + 3, 64, r.height),
                total + " tiles", st);
        }

        // ????????????????????????????????????????????????????
        //  SCENE GUI — painting
        // ????????????????????????????????????????????????????
        private void OnSceneGUI(SceneView sv) {
            if (_selected == null) return;

            Event ev = Event.current;

            if (ev.type == EventType.KeyDown && ev.keyCode == KeyCode.Escape) {
                _selected = null;
                Repaint();
                ev.Use();
                return;
            }

            Ray ray = HandleUtility.GUIPointToWorldRay(ev.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f)) return;

            TilePlacementSettings s = TilePlacementSettings.instance;
            Vector3 snapped = SnapToGrid(hit.point, s.gridSize);

            // Cursor indicator
            Handles.color = _activeTool == TilePlacementTool.Erase
                ? new Color(1f, 0.25f, 0.25f, 0.85f)
                : new Color(0.30f, 0.65f, 1.00f, 0.85f);

            Handles.DrawWireCube(
                snapped + Vector3.up * 0.05f,
                new Vector3(s.gridSize, 0.1f, s.gridSize));

            sv.Repaint();

            // Click / drag to paint
            if ((ev.type == EventType.MouseDown || ev.type == EventType.MouseDrag)
                && ev.button == 0 && !ev.alt) {
                if (_activeTool == TilePlacementTool.Place)
                    PlaceTile(snapped);
                else if (_activeTool == TilePlacementTool.Erase)
                    EraseTile(snapped);

                HandleUtility.AddDefaultControl(
                    GUIUtility.GetControlID(FocusType.Passive));
                ev.Use();
            }
        }

        // ????????????????????????????????????????????????????
        //  PLACEMENT LOGIC
        // ????????????????????????????????????????????????????
        private void PlaceTile(Vector3 pos) {
            if (_selected?.prefab == null) return;

            TilePlacementSettings s = TilePlacementSettings.instance;

            if (s.preventOverlap) {
                Collider[] cols = Physics.OverlapBox(
                    pos + Vector3.up * 0.5f,
                    new Vector3(s.gridSize * 0.45f, 0.4f, s.gridSize * 0.45f));
                foreach (Collider c in cols)
                    if (c.CompareTag("PlacedTile")) return;
            }

            float yRot = s.snapRotation;
            if (s.randomizeRotation)
                yRot = Mathf.Round(Random.Range(
                    _selected.randomYRotMin,
                    _selected.randomYRotMax) / s.rotationStep) * s.rotationStep;

            Undo.SetCurrentGroupName("Place Tile");
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(_selected.prefab);
            go.transform.SetPositionAndRotation(
                pos + _selected.placementOffset,
                Quaternion.Euler(0, yRot, 0));

            if (s.parentContainer != null)
                go.transform.SetParent(s.parentContainer);

            go.tag = "PlacedTile";
            Undo.RegisterCreatedObjectUndo(go, "Place Tile");
        }

        private void EraseTile(Vector3 pos) {
            TilePlacementSettings s = TilePlacementSettings.instance;
            Collider[] cols = Physics.OverlapBox(
                pos + Vector3.up * 0.5f,
                new Vector3(s.gridSize * 0.45f, 0.4f, s.gridSize * 0.45f));

            foreach (Collider c in cols) {
                if (c.CompareTag("PlacedTile")) {
                    Undo.DestroyObjectImmediate(c.gameObject);
                    break;
                }
            }
        }

        private Vector3 SnapToGrid(Vector3 pos, float gridSize) {
            return new Vector3(
                Mathf.Round(pos.x / gridSize) * gridSize,
                pos.y,
                Mathf.Round(pos.z / gridSize) * gridSize);
        }

        // ????????????????????????????????????????????????????
        //  CONTEXT MENUS
        // ????????????????????????????????????????????????????
        private void ShowGroupContextMenu(TileGroup g, int index) {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Rename"), false, () => {
                _renamingGroup = g;
                _renameBuffer = g.name;
            });
            menu.AddItem(new GUIContent("Add Tile"), false, () => OpenAddTileWindow(g));

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Move Up"), false, () => {
                if (index > 0) {
                    (_palette.groups[index], _palette.groups[index - 1]) =
                        (_palette.groups[index - 1], _palette.groups[index]);
                    EditorUtility.SetDirty(_palette);
                }
            });
            menu.AddItem(new GUIContent("Move Down"), false, () => {
                if (index < _palette.groups.Count - 1) {
                    (_palette.groups[index], _palette.groups[index + 1]) =
                        (_palette.groups[index + 1], _palette.groups[index]);
                    EditorUtility.SetDirty(_palette);
                }
            });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Delete Group"), false, () => {
                if (EditorUtility.DisplayDialog(
                        "Delete Group",
                        $"Delete \"{g.name}\" and all its tiles?",
                        "Delete", "Cancel")) {
                    _palette.groups.Remove(g);
                    EditorUtility.SetDirty(_palette);
                }
            });

            menu.ShowAsContext();
        }

        private void ShowTileContextMenu(TilePrefabEntry tile, TileGroup group) {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Select"), false,
                () => SelectTile(tile));
            menu.AddItem(new GUIContent("Ping in Project"), false,
                () => EditorGUIUtility.PingObject(tile.prefab));

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Remove from Group"), false, () => {
                group.tiles.Remove(tile);
                if (_selected == tile) _selected = null;
                EditorUtility.SetDirty(_palette);
            });

            menu.ShowAsContext();
        }

        // ????????????????????????????????????????????????????
        //  HELPERS
        // ????????????????????????????????????????????????????
        private void SelectTile(TilePrefabEntry tile) {
            _selected = tile;
            _activeTool = TilePlacementTool.Place;
            Repaint();
        }

        private void OpenAddGroupWindow() {
            TileAddGroupWindow.Open(group => {
                _palette.groups.Add(group);
                EditorUtility.SetDirty(_palette);
                Repaint();
            });
        }

        private void OpenAddTileWindow(TileGroup group) {
            TileAddTileWindow.Open(group, tile => {
                group.tiles.Add(tile);
                EditorUtility.SetDirty(_palette);
                Repaint();
            });
        }

        private List<TilePrefabEntry> VisibleTiles(TileGroup g) {
            if (string.IsNullOrEmpty(_searchQuery)) return g.tiles;
            string q = _searchQuery.ToLower();
            return g.tiles.Where(t =>
                t.displayName.ToLower().Contains(q) ||
                (t.tags != null && t.tags.Any(tag => tag.ToLower().Contains(q)))
            ).ToList();
        }

        private int TileColumns(float width) {
            return Mathf.Max(1,
                Mathf.FloorToInt((width - TILE_PAD * 2) / (TILE_SIZE + TILE_PAD)));
        }

        private void LoadOrCreatePalette() {
            const string path = "Assets/Editor/TilePlacement/DefaultPalette.asset";
            _palette = AssetDatabase.LoadAssetAtPath<TilePaletteData>(path);
            if (_palette != null) return;

            _palette = CreateInstance<TilePaletteData>();
            _palette.groups = new List<TileGroup>
            {
                new TileGroup { name = "Terrain",    accentColor = new Color(0.20f, 0.70f, 0.30f) },
                new TileGroup { name = "Props",      accentColor = new Color(0.80f, 0.60f, 0.20f) },
                new TileGroup { name = "Structures", accentColor = new Color(0.30f, 0.50f, 0.90f) },
            };

            System.IO.Directory.CreateDirectory("Assets/Editor/TilePlacement");
            AssetDatabase.CreateAsset(_palette, path);
            AssetDatabase.SaveAssets();
        }

        private static void DrawBorder(Rect r, Color c, float t = 1f) {
            EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, t), c);
            EditorGUI.DrawRect(new Rect(r.x, r.yMax - t, r.width, t), c);
            EditorGUI.DrawRect(new Rect(r.x, r.y, t, r.height), c);
            EditorGUI.DrawRect(new Rect(r.xMax - t, r.y, t, r.height), c);
        }

        private static void DrawSeparator(Rect r) {
            EditorGUI.DrawRect(r, C_SEPARATOR);
        }
    }
}