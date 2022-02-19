using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace BG.UnityUtils.Editor
{
    public class ObjectDirtier : EditorWindow
    {
        private string searchInFolders;
        private SortedDictionary<string, string> allTypes;
        private string ignoreExtensions;
        private int ignoreTypesMask;
        private int dialogMaxPathDisplayCount = 50;

        [MenuItem("Tools/Botizen Games/Object Dirtier", false, -10)]
        static void ShowWindow()
        {
            var window = GetWindow<ObjectDirtier>();
            window.titleContent = new GUIContent("Object Dirtier");
            window.minSize = new Vector2(300, 150);
            window.Show();
        }

        void OnEnable()
        {
            LoadConfig();
            allTypes = GetAllTypesInProject();
        }

        void OnGUI()
        {
            searchInFolders = EditorGUILayout.TextField("Search in folders", searchInFolders);
            ignoreExtensions = EditorGUILayout.TextField("Ignore extensions", ignoreExtensions);

            EditorGUILayout.BeginHorizontal();
            ignoreTypesMask = EditorGUILayout.MaskField(new GUIContent("Ignore types"), ignoreTypesMask, allTypes.Values.ToArray());

            if (GUILayout.Button(new GUIContent($"\u21ba", "Refresh list"), GUILayout.Width(25)))
            {
                allTypes = GetAllTypesInProject();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Set Dirty All"))
            {
                DirtyObjectsAll();
                SaveConfig();
            }

            if (GUILayout.Button("Set Dirty Selected"))
            {
                DirtyObjectsSelected();
                SaveConfig();
            }
            EditorGUILayout.EndVertical();
        }

        void DirtyObjectsAll()
        {
            List<Object> objectsToSetDirty = new List<Object>();
            string[] searches = new string[] { };
            string[] extensions = ignoreExtensions.Split(',');

            if (string.IsNullOrEmpty(searchInFolders) || string.IsNullOrWhiteSpace(searchInFolders))
            {
                searches = new string[] { "Assets" };
            }
            else
            {
                searches = searchInFolders.Split(',');
            }

            var ignoreTypes = GetTypesFromMask(allTypes.Keys.ToArray(), ignoreTypesMask);
            string[] objectGUIDs = AssetDatabase.FindAssets("", searches);
            int count = 0;
            string paths = "";
            bool hasIgnoredType;
            bool emptyIgnoreExtensions = string.IsNullOrEmpty(ignoreExtensions);
            bool pathContainsIgnoredExtension;

            for (int i = 0; i < objectGUIDs.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(objectGUIDs[i]);
                Object o = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                hasIgnoredType = ignoreTypes.Any(x => o.GetType().Equals(x));
                pathContainsIgnoredExtension = extensions.Any(x => assetPath.Contains(x));

                if (hasIgnoredType)
                {
                    continue;
                }

                if (!emptyIgnoreExtensions && pathContainsIgnoredExtension)
                {
                    continue;
                }

                count++;
                objectsToSetDirty.Add(o);

                if (count < dialogMaxPathDisplayCount)
                {
                    paths += assetPath + "\n";
                }

                if (i == objectGUIDs.Length - 1 && count >= dialogMaxPathDisplayCount)
                {
                    paths += "...";
                }
            }

            if (EditorUtility.DisplayDialog("Set Objects Dirty", $"You are about to dirty {count} objects. Continue?\n\n{paths}", "Yes", "Cancel"))
            {
                AssetDatabase.StartAssetEditing();

                for (int i = 0; i < objectsToSetDirty.Count; i++)
                {
                    EditorUtility.SetDirty(objectsToSetDirty[i]);
                    EditorUtility.DisplayProgressBar("Setting objects dirty...", $"Dirtying {objectsToSetDirty[i].name}", (float)(i + 1) / objectGUIDs.Length);
                }

                EditorUtility.ClearProgressBar();
                AssetDatabase.SaveAssets();
                AssetDatabase.StopAssetEditing();
                Debug.Log($"Dirtied {count} objects.");
            }
        }

        void DirtyObjectsSelected()
        {
            if (Selection.objects.Length == 0)
            {
                Debug.Log("No objects selected.");
                return;
            }

            int count = 0;
            AssetDatabase.StartAssetEditing();
            foreach (Object o in Selection.objects)
            {
                count++;
                EditorUtility.SetDirty(o);
                EditorUtility.DisplayProgressBar("Setting objects dirty...", $"Dirtying {o.name}", (float)count / Selection.objects.Length);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.StopAssetEditing();
            Debug.Log($"Dirtied {count} objects.");
        }

        static SortedDictionary<string, string> GetAllTypesInProject()
        {
            SortedDictionary<string, string> types = new SortedDictionary<string, string>();
            string[] paths = AssetDatabase.GetAllAssetPaths();

            for (int i = 0; i < paths.Length; i++)
            {
                var o = AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object));
                var typeAssemblyName = o.GetType().AssemblyQualifiedName;
                var typeName = o.GetType().FullName;

                if (!types.ContainsKey(typeAssemblyName))
                {
                    types.Add(typeAssemblyName, typeName);
                }
            }

            return types;
        }

        static string[] GetSelectedTypes(string[] types, int mask)
        {
            List<string> selectedTypes = new List<string>();

            for (int i = 0; i < types.Length; i++)
            {
                if ((mask & (1 << i)) == 1 << i)
                {
                    selectedTypes.Add(types[i]);
                }
            }

            return selectedTypes.ToArray();
        }

        static System.Type[] GetTypesFromMask(string[] types, int mask)
        {
            var selectedTypes = GetSelectedTypes(types, mask);
            var _types = new System.Type[selectedTypes.Length];

            for (int i = 0; i < selectedTypes.Length; i++)
            {
                _types[i] = System.Type.GetType(selectedTypes[i]);
            }

            return _types;
        }

        void LoadConfig()
        {
            searchInFolders = EditorPrefs.GetString($"BG_UU_ObjectDirtier_Search_{Application.persistentDataPath.GetHashCode()}");
            ignoreExtensions = EditorPrefs.GetString($"BG_UU_ObjectDirtier_IgnoreExt_{Application.persistentDataPath.GetHashCode()}");
            ignoreTypesMask = EditorPrefs.GetInt($"BG_UU_ObjectDirtier_IgnoreTypes_{Application.persistentDataPath.GetHashCode()}");
        }

        void SaveConfig()
        {
            EditorPrefs.SetString($"BG_UU_ObjectDirtier_Search_{Application.persistentDataPath.GetHashCode()}", searchInFolders);
            EditorPrefs.SetString($"BG_UU_ObjectDirtier_IgnoreExt_{Application.persistentDataPath.GetHashCode()}", ignoreExtensions);
            EditorPrefs.SetInt($"BG_UU_ObjectDirtier_IgnoreTypes_{Application.persistentDataPath.GetHashCode()}", ignoreTypesMask);
        }
    }
}