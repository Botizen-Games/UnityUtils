using UnityEngine;
using UnityEditor;

namespace BG.UnityUtils.Editor
{
    [InitializeOnLoad]
    public static class HierarchyWindowGroupHeader
    {
        static HierarchyWindowGroupHeader()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (gameObject != null && gameObject.name.StartsWith("---", System.StringComparison.Ordinal))
            {
                var color = new Color(
                    EditorPrefs.GetFloat("HierarchyWindowGroupHeaderColorR"),
                    EditorPrefs.GetFloat("HierarchyWindowGroupHeaderColorG"),
                    EditorPrefs.GetFloat("HierarchyWindowGroupHeaderColorB"),
                    1f
                );
                EditorGUI.DrawRect(selectionRect, color);
                EditorGUI.DropShadowLabel(selectionRect, gameObject.name.Replace("-", "").ToUpperInvariant());
            }
        }
    }

    public class HierarchyWindowGroupHeaderEditorWindow : EditorWindow
    {
        private Color color
        {
            get
            {
                return new Color(
                    EditorPrefs.GetFloat("HierarchyWindowGroupHeaderColorR"),
                    EditorPrefs.GetFloat("HierarchyWindowGroupHeaderColorG"),
                    EditorPrefs.GetFloat("HierarchyWindowGroupHeaderColorB"),
                    1f
                );
            }
            set
            {
                EditorPrefs.SetFloat("HierarchyWindowGroupHeaderColorR", value.r);
                EditorPrefs.SetFloat("HierarchyWindowGroupHeaderColorG", value.g);
                EditorPrefs.SetFloat("HierarchyWindowGroupHeaderColorB", value.b);
            }
        }

        [MenuItem("Tools/Botizen Games/Hierarchy Window Group Header Settings...")]
        static void ShowWindow()
        {
            var window = GetWindow<HierarchyWindowGroupHeaderEditorWindow>();
            window.titleContent = new GUIContent("Hierarchy Window Group Header Settings");
            EditorWindow.GetWindowWithRect(typeof(HierarchyWindowGroupHeaderEditorWindow), new Rect(0, 0, 200, 50));
            window.Show();
        }

        void OnGUI()
        {
            color = EditorGUILayout.ColorField("Color", color);
        }
    }
}