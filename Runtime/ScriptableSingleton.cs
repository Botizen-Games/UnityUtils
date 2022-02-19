using UnityEngine;

namespace BG.UnityUtils
{
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        protected static T instance
        {
            get
            {
                if (!_instance)
                {
                    T[] objects = null;

#if UNITY_EDITOR
                    // If running game in editor, scriptable singleton assets will be found via AssetDatabase
                    string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}");
                    int count = guids.Length;
                    objects = new T[count];

                    for (int i = 0; i < count; i++)
                    {
                        objects[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]));
                    }
#else
                    // Get all assets of type T from Resources or loaded assets
                    objects = Resources.FindObjectsOfTypeAll<T>();
#endif
                    if (objects.Length == 0)
                    {
                        Debug.LogError($"No asset of type \"{typeof(T).Name}\" loaded in this project. Please create a new one and add it to the \"Preloaded Assets\" array in Edit > Project Settings > Player > Other Settings.");
                    }
                    else if (objects.Length > 1)
                    {
                        Debug.LogError($"There's more than one Singleton asset of type \"{typeof(T).Name}\" loaded in this project. Please remove other assets of that type from this project.");
                    }

                    _instance = objects.Length > 0 ? objects[0] : null;
                }

                return _instance;
            }
        }

        private static T _instance = null;
    }
}
