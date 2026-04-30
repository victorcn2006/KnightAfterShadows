using UnityEngine;
using UnityEditor;
using System;

namespace TilePlacement.Editor {
    public class TileAddGroupWindow : EditorWindow {
        private string _groupName = "New Group";
        private Color _accentColor = new Color(0.4f, 0.7f, 0.4f);
        private Action<TileGroup> _onCreate;

        public static void Open(Action<TileGroup> onCreate) {
            TileAddGroupWindow win = CreateInstance<TileAddGroupWindow>();
            win._onCreate = onCreate;
            win.titleContent = new GUIContent("Add Group");
            win.minSize = new Vector2(280, 120);
            win.maxSize = new Vector2(280, 120);
            win.ShowUtility();
        }

        private void OnGUI() {
            GUILayout.Space(10);

            GUILayout.Label("Group Name", EditorStyles.boldLabel);
            _groupName = EditorGUILayout.TextField(_groupName);

            GUILayout.Space(6);
            _accentColor = EditorGUILayout.ColorField("Accent Color", _accentColor);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(70))) {
                Close();
            }

            GUI.enabled = !string.IsNullOrEmpty(_groupName.Trim());
            if (GUILayout.Button("Create", GUILayout.Width(70))) {
                _onCreate?.Invoke(new TileGroup {
                    name = _groupName.Trim(),
                    accentColor = _accentColor
                });
                Close();
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }
    }
}