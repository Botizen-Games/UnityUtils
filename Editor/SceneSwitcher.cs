using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace BG.UnityUtils.Editor
{
    public class SceneSwitcher : EditorWindow
    {
        private Vector2 scrollPos;

        [MenuItem("Tools/Botizen Games/Scene Switcher", false, 0)]
        static void Init()
        {
            var window = (SceneSwitcher)GetWindow(typeof(SceneSwitcher), false, "Scene Switcher");
            window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 300f);
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);
            GUILayout.Label("Scenes In Build", EditorStyles.boldLabel);

            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];

                if (scene.enabled)
                {
                    var sceneName = Path.GetFileNameWithoutExtension(scene.path);
                    var pressed = GUILayout.Button(i + ": " + sceneName, new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft });

                    if (pressed)
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(scene.path);
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        [MenuItem("Tools/Botizen Games/Open All Build Scenes")]
        public static void OpenAllBuildScenes()
        {
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];

                if (scene.enabled)
                {
                    EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
                }
            }
        }

        [MenuItem("Tools/Botizen Games/Close Multi-scene")]
        public static void CloseMultiscene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                for (int i = EditorSceneManager.sceneCount - 1; i > 0; i--)
                {
                    EditorSceneManager.CloseScene(EditorSceneManager.GetSceneAt(i), true);
                }
            }
        }
    }
}