using UnityEngine;
using UnityEditor;

namespace BG.UnityUtils.Editor
{
    public class ObjectSelectionCounter : EditorWindow
    {
        [MenuItem("UnityUtils/Count Object Selection", false, 1)]
        static void CountObjectSelection()
        {
            GetWindow<ObjectSelectionCounter>();
        }

        void OnGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
        }
    }
}