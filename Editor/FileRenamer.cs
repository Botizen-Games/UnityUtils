using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUILayout;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace BG.UnityUtils.Editor
{
    public class FileRenamer : EditorWindow
    {
        private Vector2 scrollPos;

        private enum Action
        {
            Replace,
            Affix,
            LetterCasing,
            Insert,
            Delete
        }

        private Action action;
        private string path;
        private Object pathObject;
        private bool includeSubdirectories;
        private string[] files;
        private string[] modifiedFiles = new string[0];

        #region Replace
        private string oldString;
        private string newString;
        private bool caseSensitive;
        private bool replaceOnlyWholeWords;
        #endregion

        #region Affix
        private string prefix;
        private string suffix;
        #endregion

        #region Letter Casing
        private enum LetterCasingOptions
        {
            ConvertToUppercase,
            ConvertToLowercase,
            ConvertToOnlyFirstCharacterWithUppercase,
            ConvertFirstCharacterToUppercase,
            InvertCase,
            ConvertEveryFirstCharacterOfWordToUppercase
        }

        private LetterCasingOptions letterCasingOptions;
        #endregion

        #region Insert
        private string insertText;
        private int insertPosition;
        #endregion

        #region Delete
        private enum DeleteOptions
        {
            FixedLength,
            DynamicLength
        }

        private DeleteOptions deleteOptions;

        #region FixedLength
        private int startCharacterPosition;
        private int numberOfCharacters;
        #endregion

        #region DynamicLength
        private int numberOfCharactersToKeepFromStart;
        private int numberOfCharactersToKeepFromEnd;
        #endregion
        #endregion

        [MenuItem("UnityUtils/File Renamer", false, -10)]
        static void ShowWindow()
        {
            var window = GetWindow<FileRenamer>();
            window.titleContent = new GUIContent("File Renamer");
            window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 500f, 500f);
            window.Show();
        }

        void OnGUI()
        {
            pathObject = ObjectField("Path", (Object)pathObject, typeof(Object), false);
            includeSubdirectories = Toggle("Include subdirectories", includeSubdirectories);

            if (pathObject == null)
            {
                return;
            }

            path = AssetDatabase.GetAssetPath(pathObject);

            if (AssetDatabase.IsValidFolder(path))
            {
                var f = includeSubdirectories ? Directory.GetFiles(path, "*.*", SearchOption.AllDirectories) : Directory.GetFiles(path);
                files = f.Where(p => Path.GetExtension(p) != ".meta").ToArray();
            }
            else
            {
                files = new string[] { path };
            }

            action = (Action)EnumPopup("Action", action);
            Space(10);

            switch (action)
            {
                case Action.Replace:
                    oldString = TextField("Old", oldString);
                    newString = TextField("New", newString);
                    caseSensitive = Toggle("Case sensitive", caseSensitive);
                    replaceOnlyWholeWords = Toggle("Replace only whole words", replaceOnlyWholeWords);

                    if (GUILayout.Button("Stage Changes"))
                    {
                        Replace(oldString, newString, caseSensitive, replaceOnlyWholeWords);
                    }
                    break;
                case Action.Affix:
                    prefix = TextField("Prefix", prefix);
                    suffix = TextField("Suffix", suffix);

                    if (GUILayout.Button("Stage Changes"))
                    {
                        Affix(prefix, suffix);
                    }
                    break;
                case Action.LetterCasing:
                    letterCasingOptions = (LetterCasingOptions)EnumPopup("Options", letterCasingOptions);

                    if (GUILayout.Button("Stage Changes"))
                    {
                        modifiedFiles = new string[files.Length];

                        switch (letterCasingOptions)
                        {
                            case LetterCasingOptions.ConvertToUppercase:
                                ConvertToUppercase();
                                break;
                            case LetterCasingOptions.ConvertToLowercase:
                                ConvertToLowercase();
                                break;
                            case LetterCasingOptions.ConvertToOnlyFirstCharacterWithUppercase:
                                ConvertToOnlyFirstCharacterWithUppercase();
                                break;
                            case LetterCasingOptions.ConvertFirstCharacterToUppercase:
                                ConvertFirstCharacterToUppercase();
                                break;
                            case LetterCasingOptions.InvertCase:
                                InvertCase();
                                break;
                            case LetterCasingOptions.ConvertEveryFirstCharacterOfWordToUppercase:
                                ConvertEveryFirstCharacterOfWordToUppercase();
                                break;
                        }
                    }

                    break;
                case Action.Insert:
                    insertText = TextField("Text", insertText);
                    insertPosition = IntField("At character position", insertPosition);

                    if (GUILayout.Button("Stage Changes"))
                    {
                        Insert(insertText, insertPosition);
                    }
                    break;
                case Action.Delete:
                    deleteOptions = (DeleteOptions)EnumPopup("Options", deleteOptions);

                    switch (deleteOptions)
                    {
                        case DeleteOptions.FixedLength:
                            startCharacterPosition = IntField("From character position", startCharacterPosition);
                            numberOfCharacters = IntField("Number of characters", numberOfCharacters);

                            if (GUILayout.Button("Stage Changes"))
                            {
                                DeleteFixedLength(startCharacterPosition, numberOfCharacters);
                            }
                            break;
                        case DeleteOptions.DynamicLength:
                            numberOfCharactersToKeepFromStart = IntField("Start", numberOfCharactersToKeepFromStart);
                            numberOfCharactersToKeepFromEnd = IntField("End", numberOfCharactersToKeepFromEnd);

                            if (GUILayout.Button("Stage Changes"))
                            {
                                DeleteDynamicLength(numberOfCharactersToKeepFromStart, numberOfCharactersToKeepFromEnd);
                            }
                            break;
                    }
                    break;
            }

            if (files == null)
            {
                Debug.LogWarning("No files loaded");
                return;
            }

            if (GUILayout.Button("Apply Changes"))
            {
                if (files.Length != modifiedFiles.Length)
                {
                    Debug.Log("Make sure to stage changes before applying!");
                    return;
                }

                if (EditorUtility.DisplayDialog("Apply Changes", "Are you sure you want to rename these files?", "Yes", "Cancel"))
                {
                    ApplyChanges(path);
                }
            }

            #region File View
            scrollPos = BeginScrollView(scrollPos, false, false);
            BeginHorizontal();
            BeginVertical();
            LabelField("Current Name", new GUIStyle(EditorStyles.boldLabel));
            Space(5);
            for (int i = 0; i < files.Length; i++)
            {
                LabelField(GetFileNameAtPath(files[i]));
            }
            EndVertical();
            BeginVertical();
            LabelField("New Name", new GUIStyle(EditorStyles.boldLabel));
            Space(5);

            if (modifiedFiles.Length == files.Length)
            {
                for (int i = 0; i < modifiedFiles.Length; i++)
                {
                    if (modifiedFiles[i].Equals(GetFileNameAtPath(files[i])))
                    {
                        LabelField(modifiedFiles[i]);
                    }
                    else
                    {
                        LabelField(modifiedFiles[i], new GUIStyle(EditorStyles.whiteLabel));
                    }
                }
            }
            EndVertical();
            EndHorizontal();
            EndScrollView();
            #endregion
        }

        void ApplyChanges(string path)
        {
            AssetDatabase.StartAssetEditing();

            for (int i = 0; i < files.Length; i++)
            {
                if (GetFileNameAtPath(files[i]).Equals(modifiedFiles[i]))
                {
                    continue;
                }

                string result = AssetDatabase.RenameAsset(files[i], Path.GetFileNameWithoutExtension(modifiedFiles[i]));

                if (!string.IsNullOrEmpty(result))
                {
                    Debug.Log($"Failed to rename {files[i]}");
                }

                EditorUtility.DisplayProgressBar("Apply Changes", $"Renaming {files[i]}", (float)(i + 1) / files.Length);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void Replace(string oldString, string newString, bool caseSensitive, bool replaceOnlyWholeWords)
        {
            if (string.IsNullOrEmpty(oldString))
            {
                Debug.Log("Old string value cannot be empty.");
                return;
            }

            if (string.IsNullOrEmpty(newString))
            {
                newString = "";
            }

            modifiedFiles = new string[files.Length];
            string result;

            for (int i = 0; i < files.Length; i++)
            {
                string currentName = GetFileNameAtPath(files[i]);
                string[] currentNameExtensionSplit = currentName.Split('.');
                string currentNameNoExtension = currentNameExtensionSplit[0];
                string extension = "";

                for (int n = 1; n < currentNameExtensionSplit.Length; n++)
                {
                    extension += currentNameExtensionSplit[n];

                    if (n < currentNameExtensionSplit.Length - 1)
                    {
                        extension += ".";
                    }
                }

                RegexOptions regexOptions = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                oldString = replaceOnlyWholeWords ? string.Format(@"\b{0}\b", oldString) : oldString;
                result = Regex.Replace(currentNameNoExtension, oldString, newString, regexOptions) + "." + extension;
                modifiedFiles[i] = result;
            }
        }

        void Affix(string prefix, string suffix)
        {
            modifiedFiles = new string[files.Length];
            string result;

            for (int i = 0; i < files.Length; i++)
            {
                result = prefix;
                string currentName = GetFileNameAtPath(files[i]);
                string[] currentNameExtensionSplit = currentName.Split('.');

                for (int s = 0; s < currentNameExtensionSplit.Length - 1; s++)
                {
                    result += s < currentNameExtensionSplit.Length - 2 ? currentNameExtensionSplit[s] + "." : currentNameExtensionSplit[s];
                }
                result += suffix + "." + currentNameExtensionSplit[currentNameExtensionSplit.Length - 1];
                modifiedFiles[i] = result;
            }
        }

        #region Letter Casing Methods
        void ConvertToUppercase()
        {
            string result;

            for (int i = 0; i < files.Length; i++)
            {
                result = "";
                string currentName = GetFileNameAtPath(files[i]);
                string[] currentNameExtensionSplit = currentName.Split('.');

                for (int s = 0; s < currentNameExtensionSplit.Length - 1; s++)
                {
                    result += currentNameExtensionSplit[s].ToUpper() + ".";
                }
                result += currentNameExtensionSplit[currentNameExtensionSplit.Length - 1];
                modifiedFiles[i] = result;
            }
        }

        void ConvertToLowercase()
        {
            string result;

            for (int i = 0; i < files.Length; i++)
            {
                result = "";
                string currentName = GetFileNameAtPath(files[i]);
                string[] currentNameExtensionSplit = currentName.Split('.');

                for (int s = 0; s < currentNameExtensionSplit.Length - 1; s++)
                {
                    result += currentNameExtensionSplit[s].ToLower() + ".";
                }
                result += currentNameExtensionSplit[currentNameExtensionSplit.Length - 1];
                modifiedFiles[i] = result;
            }
        }

        void ConvertToOnlyFirstCharacterWithUppercase()
        {
            string result;

            for (int i = 0; i < files.Length; i++)
            {
                string currentName = GetFileNameAtPath(files[i]);
                result = currentName.ToLower();
                result = result.Substring(0, 1).ToUpper() + result.Substring(1, result.Length - 1);
                modifiedFiles[i] = result;
            }
        }

        void ConvertFirstCharacterToUppercase()
        {
            string result;

            for (int i = 0; i < files.Length; i++)
            {
                string currentName = GetFileNameAtPath(files[i]);
                result = currentName.Substring(0, 1).ToUpper() + currentName.Substring(1, currentName.Length - 1);
                modifiedFiles[i] = result;
            }
        }

        void InvertCase()
        {
            string result;

            for (int i = 0; i < files.Length; i++)
            {
                string currentName = GetFileNameAtPath(files[i]);
                char[] currentCharArray = currentName.ToCharArray();
                string[] currentNameExtensionSplit = currentName.Split('.');
                string extension = currentNameExtensionSplit[currentNameExtensionSplit.Length - 1];
                char[] newCharArray = new char[currentCharArray.Length - extension.Length];
                string currentNameNoExtension = "";

                for (int n = 0; n < currentNameExtensionSplit.Length - 1; n++)
                {
                    currentNameNoExtension += currentNameExtensionSplit[n] + ".";
                }

                currentCharArray = currentNameNoExtension.ToCharArray();

                for (int c = 0; c < currentCharArray.Length; c++)
                {
                    newCharArray[c] = char.IsUpper(currentCharArray[c]) ? char.ToLower(currentCharArray[c]) : char.ToUpper(currentCharArray[c]);
                }

                result = new string(newCharArray) + extension;
                modifiedFiles[i] = result;
            }
        }

        void ConvertEveryFirstCharacterOfWordToUppercase()
        {
            string result;

            for (int i = 0; i < files.Length; i++)
            {
                result = "";
                string currentName = GetFileNameAtPath(files[i]);
                string[] words = currentName.Split(' ');

                for (int w = 0; w < words.Length; w++)
                {
                    result += words[w].Substring(0, 1).ToUpper() + words[w].Substring(1, words[w].Length - 1);

                    if (w < words.Length - 1)
                    {
                        result += " ";
                    }
                }
                modifiedFiles[i] = result;
            }
        }
        #endregion

        void Insert(string insertText, int insertPosition)
        {
            if (string.IsNullOrEmpty(insertText))
            {
                Debug.Log("Text cannot be empty.");
                return;
            }

            if (insertPosition < 1)
            {
                Debug.Log("Position must not be less than 1.");
                return;
            }

            modifiedFiles = new string[files.Length];
            string result;

            for (int i = 0; i < files.Length; i++)
            {
                string currentName = GetFileNameAtPath(files[i]);
                int _insertPosition = insertPosition;

                if (insertPosition > currentName.LastIndexOf('.'))
                {
                    _insertPosition = currentName.LastIndexOf('.') + 1;
                }

                result = currentName.Insert(_insertPosition - 1, insertText);
                modifiedFiles[i] = result;
            }
        }

        #region Delete Methods
        void DeleteFixedLength(int startCharacterPosition, int numberOfCharacters)
        {
            if (startCharacterPosition < 1)
            {
                Debug.Log("Position must not be less than 1.");
                return;
            }

            if (numberOfCharacters < 0)
            {
                Debug.Log("Number of characters must not be negative.");
                return;
            }

            modifiedFiles = new string[files.Length];
            string result;

            for (int i = 0; i < files.Length; i++)
            {
                string currentName = GetFileNameAtPath(files[i]);
                string[] currentNameExtensionSplit = currentName.Split('.');
                string extension = currentNameExtensionSplit[currentNameExtensionSplit.Length - 1];
                string currentNameNoExtension = "";

                for (int n = 0; n < currentNameExtensionSplit.Length - 1; n++)
                {
                    currentNameNoExtension += currentNameExtensionSplit[n];

                    if (n < currentNameExtensionSplit.Length - 2)
                    {
                        currentNameNoExtension += ".";
                    }
                }

                if (startCharacterPosition > currentNameNoExtension.Length)
                {
                    modifiedFiles[i] = currentName;
                    continue;
                }

                if (startCharacterPosition + numberOfCharacters > currentNameNoExtension.Length)
                {
                    if (startCharacterPosition == 1)
                    {
                        modifiedFiles[i] = currentName;
                        continue;
                    }

                    result = currentNameNoExtension.Remove(startCharacterPosition - 1) + "." + extension;
                }
                else
                {
                    result = currentNameNoExtension.Remove(startCharacterPosition - 1, numberOfCharacters) + "." + extension;
                }
                modifiedFiles[i] = result;
            }
        }

        void DeleteDynamicLength(int numberOfCharactersToKeepFromStart, int numberOfCharactersToKeepFromEnd)
        {
            if (numberOfCharactersToKeepFromStart < 0 || numberOfCharactersToKeepFromEnd < 0)
            {
                Debug.Log("Numbers cannot be negative.");
                return;
            }

            if (numberOfCharactersToKeepFromStart == 0 && numberOfCharactersToKeepFromEnd == 0)
            {
                return;
            }

            modifiedFiles = new string[files.Length];
            string result;

            for (int i = 0; i < files.Length; i++)
            {
                string currentName = GetFileNameAtPath(files[i]);
                string[] currentNameExtensionSplit = currentName.Split('.');
                string extension = currentNameExtensionSplit[currentNameExtensionSplit.Length - 1];
                string currentNameNoExtension = "";

                for (int n = 0; n < currentNameExtensionSplit.Length - 1; n++)
                {
                    currentNameNoExtension += currentNameExtensionSplit[n];

                    if (n < currentNameExtensionSplit.Length - 2)
                    {
                        currentNameNoExtension += ".";
                    }
                }

                if (numberOfCharactersToKeepFromStart + numberOfCharactersToKeepFromEnd >= currentNameNoExtension.Length)
                {
                    modifiedFiles[i] = currentName;
                    continue;
                }

                result = currentNameNoExtension.Substring(0, numberOfCharactersToKeepFromStart) + currentNameNoExtension.Substring(currentNameNoExtension.Length - numberOfCharactersToKeepFromEnd) + "." + extension;
                modifiedFiles[i] = result;
            }
        }
        #endregion

        static string GetFileNameAtPath(string path)
        {
            string[] pathSplit;

            if (path.Contains(Path.DirectorySeparatorChar.ToString()))
            {
                pathSplit = path.Split(Path.DirectorySeparatorChar);
            }
            else
            {
                pathSplit = path.Split(Path.AltDirectorySeparatorChar);
            }

            return pathSplit[pathSplit.Length - 1];
        }
    }
}