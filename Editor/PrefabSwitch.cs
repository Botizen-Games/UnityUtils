using UnityEngine;
using UnityEditor;

namespace BG.UnityUtils.Editor
{
    public class PrefabSwitch : EditorWindow
    {
        [SerializeField] private GameObject prefab;

        [MenuItem("UnityUtils/Prefab Switch", false, 1)]
        static void ShowWindow()
        {
            var window = GetWindow<PrefabSwitch>();
            window.titleContent = new GUIContent("Prefab Switch");
            window.Show();
        }

        void OnGUI()
        {
            prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

            if (GUILayout.Button("Switch"))
            {
                var selection = Selection.gameObjects;

                for (var i = selection.Length - 1; i >= 0; --i)
                {
                    var selected = selection[i];
                    var prefabType = PrefabUtility.GetPrefabAssetType(prefab);
                    GameObject newObject;

                    if (prefabType == PrefabAssetType.Regular)
                    {
                        newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    }
                    else
                    {
                        newObject = Instantiate(prefab);
                        newObject.name = prefab.name;
                    }

                    if (newObject == null)
                    {
                        Debug.LogError("Error instantiating prefab");
                        break;
                    }

                    Undo.RegisterCreatedObjectUndo(newObject, "Switch Prefab");
                    newObject.transform.parent = selected.transform.parent;
                    newObject.transform.localPosition = selected.transform.localPosition;
                    newObject.transform.localRotation = selected.transform.localRotation;
                    newObject.transform.localScale = selected.transform.localScale;
                    newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
                    Undo.DestroyObjectImmediate(selected);
                }
            }

            GUI.enabled = false;
            EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
        }
    }
}