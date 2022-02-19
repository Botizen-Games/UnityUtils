using UnityEngine;

namespace BG.UnityUtils.Runtime
{
    [System.Serializable]
    [CreateAssetMenu]
    public class SceneReference : ScriptableObject
    {
#if UNITY_EDITOR
        void OnValidate()
        {
            var buildScenes = UnityEditor.EditorBuildSettings.scenes;
            string sceneFileName;

            for (int i = 0; i < buildScenes.Length; i++)
            {
                sceneFileName = System.IO.Path.GetFileNameWithoutExtension(buildScenes[i].path);

                if (name.Equals(sceneFileName))
                {
                    return;
                }
            }

            Debug.LogError($"{name} doesn't exist in the build settings.", this);
        }
#endif
    }
}