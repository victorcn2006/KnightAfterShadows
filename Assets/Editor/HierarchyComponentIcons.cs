using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class HierarchyComponentIcons
{
    private const string MENU_NAME = "Tools/Hierarchy Icons/Enable";
    private const string PREF_KEY = "HierarchyComponentIcons_Enabled";

    static bool Enabled
    {
        get => EditorPrefs.GetBool(PREF_KEY, true);
        set => EditorPrefs.SetBool(PREF_KEY, value);
    }

    static HierarchyComponentIcons()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        EditorApplication.delayCall += UpdateMenuCheck;
    }

    [MenuItem(MENU_NAME)]
    private static void Toggle()
    {
        Enabled = !Enabled;
        UpdateMenuCheck();
        EditorApplication.RepaintHierarchyWindow();
    }

    [MenuItem(MENU_NAME, true)]
    private static bool ToggleValidate()
    {
        Menu.SetChecked(MENU_NAME, Enabled);
        return true;
    }

    private static void UpdateMenuCheck()
    {
        Menu.SetChecked(MENU_NAME, Enabled);
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        if (!Enabled) return;

        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go == null) return;

        var components = go.GetComponents<Component>();

        float x = selectionRect.xMax;
        int maxIcons = 5;
        int count = 0;

        for (int i = components.Length - 1; i >= 0; i--)
        {
            if (count >= maxIcons) break;

            var comp = components[i];
            if (comp == null || comp is Transform) continue;

            Texture icon = AssetPreview.GetMiniThumbnail(comp);
            if (icon == null) continue;

            Rect rect = new Rect(x - 16, selectionRect.y, 16, 16);
            GUI.DrawTexture(rect, icon);
            GUI.Label(rect, new GUIContent("", comp.GetType().Name));

            x -= 18;
            count++;
        }

        if (count == 0)
        {
            Transform transform = go.transform;
            Texture icon = AssetPreview.GetMiniThumbnail(transform);

            if (icon != null)
            {
                Rect rect = new Rect(x - 16, selectionRect.y, 16, 16);
                GUI.DrawTexture(rect, icon);
                GUI.Label(rect, new GUIContent("", "Transform"));
            }
        }
    }
}