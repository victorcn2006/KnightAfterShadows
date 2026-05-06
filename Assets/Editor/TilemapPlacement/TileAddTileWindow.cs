using UnityEngine;
using UnityEditor;
using System;

namespace TilePlacement.Editor {
    public class TileAddTileWindow : EditorWindow {
        private TileGroup _targetGroup;
        private TilePrefabEntry _entry = new TilePrefabEntry();
        private string _tagsRaw = "";
        private Action<TilePrefabEntry> _onAdd;
        private Vector2 _scroll;

        public static void Open(TileGroup group, Action<TilePrefabEntry> onAdd) {
            TileAddTileWindow win = CreateInstance<TileAddTileWindow>();
            win._targetGroup = group;
            win._onAdd = onAdd;
            win._entry = new TilePrefabEntry();
            win.titleContent = new GUIContent("Add Tile — " + group.name);
            win.minSize = new Vector2(300, 280);
            win.ShowUtility();
        }

        private void OnGUI() {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            GUILayout.Space(8);

            // Drag-and-drop zone
            Rect dropRect = GUILayoutUtility.GetRect(0, 56, GUILayout.ExpandWidth(true));
            DrawDropZone(dropRect);

            GUILayout.Space(6);

            EditorGUILayout.LabelField("Prefab", EditorStyles.boldLabel);
            _entry.prefab = (GameObject)EditorGUILayout.ObjectField(
                _entry.prefab, typeof(GameObject), false);

            EditorGUILayout.LabelField("Display Name", EditorStyles.boldLabel);
            _entry.displayName = EditorGUILayout.TextField(_entry.displayName);

            EditorGUILayout.LabelField("Custom Thumbnail (optional)");
            _entry.thumbnail = (Texture2D)EditorGUILayout.ObjectField(
                _entry.thumbnail, typeof(Texture2D), false);

            EditorGUILayout.LabelField("Tags (comma-separated)");
            _tagsRaw = EditorGUILayout.TextField(_tagsRaw);

            EditorGUILayout.LabelField("Placement Offset");
            _entry.placementOffset = EditorGUILayout.Vector3Field("", _entry.placementOffset);

            EditorGUILayout.MinMaxSlider(
                "Random Rotation",
                ref _entry.randomYRotMin,
                ref _entry.randomYRotMax,
                0f, 360f);

            EditorGUILayout.LabelField(
                $"  {_entry.randomYRotMin:F0}° – {_entry.randomYRotMax:F0}°",
                EditorStyles.miniLabel);

            EditorGUILayout.EndScrollView();

            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(70))) {
                Close();
            }

            GUI.enabled = _entry.prefab != null
                          && !string.IsNullOrEmpty(_entry.displayName.Trim());

            if (GUILayout.Button("Add", GUILayout.Width(70))) {
                if (!string.IsNullOrEmpty(_tagsRaw))
                    _entry.tags = _tagsRaw.Split(
                        new[] { ',' },
                        StringSplitOptions.RemoveEmptyEntries);

                if (string.IsNullOrEmpty(_entry.displayName.Trim()) && _entry.prefab != null)
                    _entry.displayName = _entry.prefab.name;

                _onAdd?.Invoke(_entry);
                Close();
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.Space(6);
        }

        private void DrawDropZone(Rect r) {
            GUI.Box(r, "Drag a prefab here");

            Event ev = Event.current;
            if (!r.Contains(ev.mousePosition)) return;

            if (ev.type == EventType.DragUpdated) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                ev.Use();
            } else if (ev.type == EventType.DragPerform) {
                DragAndDrop.AcceptDrag();
                foreach (UnityEngine.Object obj in DragAndDrop.objectReferences) {
                    if (obj is GameObject go) {
                        _entry.prefab = go;
                        _entry.displayName = go.name;
                        break;
                    }
                }
                ev.Use();
            }
        }
    }
}