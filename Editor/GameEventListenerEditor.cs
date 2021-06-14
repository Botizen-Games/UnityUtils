using UnityEngine;
using UnityEditor;

namespace BG.UnityUtils.Editor
{
    [CustomEditor(typeof(GameEventListener))]
    public class GameEventListenerEditor : UnityEditor.Editor
    {
        private GameEventListener gameEventListener;
        private SerializedProperty list;
        private int listSize;

        void OnEnable()
        {
            gameEventListener = (GameEventListener)target;
            list = serializedObject.FindProperty("eventAndResponses");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            listSize = list.arraySize;
            listSize = EditorGUILayout.IntField("Size", listSize);
            EditorGUILayout.Space();

            if (listSize < 0)
            {
                return;
            }

            if (listSize == 0)
            {
                if (GUILayout.Button("Add New Event"))
                {
                    gameEventListener.eventAndResponses.Add(new EventAndResponse());
                }
            }

            if (listSize != list.arraySize)
            {
                while (listSize > list.arraySize)
                {
                    list.InsertArrayElementAtIndex(list.arraySize);
                }
                while (listSize < list.arraySize)
                {
                    list.DeleteArrayElementAtIndex(list.arraySize - 1);
                }
            }

            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty eventAndResponses = list.GetArrayElementAtIndex(i);
                SerializedProperty _name = eventAndResponses.FindPropertyRelative("Name");
                SerializedProperty _event = eventAndResponses.FindPropertyRelative("Event");
                SerializedProperty responseType = eventAndResponses.FindPropertyRelative("ResponseType");
                SerializedProperty response = eventAndResponses.FindPropertyRelative("Response");
                SerializedProperty responseForString = eventAndResponses.FindPropertyRelative("responseForString");
                SerializedProperty responseForInt = eventAndResponses.FindPropertyRelative("responseForInt");
                SerializedProperty responseForFloat = eventAndResponses.FindPropertyRelative("responseForFloat");
                SerializedProperty responseForBool = eventAndResponses.FindPropertyRelative("responseForBool");
                SerializedProperty responseForGameObject = eventAndResponses.FindPropertyRelative("responseForGameObject");

                string nameLabel = string.IsNullOrWhiteSpace(_name.stringValue) ? "Unnamed Event" : $"{_name.stringValue}";
                eventAndResponses.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(eventAndResponses.isExpanded, nameLabel);

                if (eventAndResponses.isExpanded)
                {
                    _name.stringValue = EditorGUILayout.TextField("Event Name", _name.stringValue);
                    _event.objectReferenceValue = EditorGUILayout.ObjectField("Event", _event.objectReferenceValue, typeof(GameEvent), false);
                    EventAndResponse.ResponseTypes type = (EventAndResponse.ResponseTypes)EditorGUILayout.EnumFlagsField("Response Types", (EventAndResponse.ResponseTypes)System.Enum.ToObject(typeof(EventAndResponse.ResponseTypes), responseType.intValue));
                    responseType.intValue = (int)type;

                    EditorGUILayout.PropertyField(response);

                    if (type.HasFlag(EventAndResponse.ResponseTypes.String)) EditorGUILayout.PropertyField(responseForString);
                    if (type.HasFlag(EventAndResponse.ResponseTypes.Int)) EditorGUILayout.PropertyField(responseForInt);
                    if (type.HasFlag(EventAndResponse.ResponseTypes.Float)) EditorGUILayout.PropertyField(responseForFloat);
                    if (type.HasFlag(EventAndResponse.ResponseTypes.Bool)) EditorGUILayout.PropertyField(responseForBool);
                    if (type.HasFlag(EventAndResponse.ResponseTypes.GameObject)) EditorGUILayout.PropertyField(responseForGameObject);
                }

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Add New Event"))
                {
                    gameEventListener.eventAndResponses.Add(new EventAndResponse());
                }

                string removeLabel = string.IsNullOrWhiteSpace(_name.stringValue) ? "Remove Event" : $"Remove {_name.stringValue}";

                if (GUILayout.Button(removeLabel))
                {
                    list.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(25);
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }
    }
}