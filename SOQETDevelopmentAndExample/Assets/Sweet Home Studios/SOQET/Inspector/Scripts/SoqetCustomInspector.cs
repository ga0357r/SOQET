#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SOQET.Others;
using SOQET.Debugging;

namespace SOQET.Inspector
{
    /// <summary>
    /// Custom Inspector for SOQET
    /// </summary>
    [CustomEditor(typeof(SoqetInspector))]
    public sealed class SoqetCustomInspector : UnityEditor.Editor
    {
        private SoqetInspector storyManager;
        private Story story;
        private SerializedProperty storySerializedProperty;

        private void OnEnable()
        {
            storyManager = (SoqetInspector)target;
            storySerializedProperty = serializedObject.FindProperty("currentStory");
            story = storyManager.CurrentStory;
            SOQET.Debugging.Debug.Log("On Enable");
        }

        /// <summary>
        /// Displays Current Story Information
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField
            (
                storySerializedProperty,
                new GUIContent("Current Story"),
                GUILayout.Height(20)
            );

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                OnEnable();
            }

            bool showFoldout = (story != null);
            EditorGUILayout.Separator();
            EditorGUILayout.Foldout(showFoldout, "Current Story Information");

            if (story)
            {
                EditorGUILayout.Space();

                foreach (var objective in story.GetObjectives())
                {
                    EditorGUILayout.TextField($"Objective {objective.Order}", objective.name);

                    foreach (var quest in objective.GetQuests())
                    {
                        EditorGUILayout.TextField($"Quest {quest.Order}", quest.name);
                    }

                    EditorGUILayout.Space();
                }

                return;
            }

            SOQET.Debugging.Debug.Log("story is null");
        }
    }
}
#endif