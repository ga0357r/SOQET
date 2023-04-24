using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UnityEngine.Events;
using SOQET.Editor;
using SOQET.Debugging;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Debug = UnityEngine.Debug;

namespace SOQET.Others
{
    [CreateAssetMenu(fileName = "New Story", menuName = "SOQET/Story")]
    public class Story : ScriptableObject, ISerializationCallbackReceiver
    {
        [HideInInspector] private List<Objective> objectives = new List<Objective>();
        [HideInInspector] private Objective currentObjective;
        [HideInInspector] private Objective defaultObjective;
        [SerializeField] private bool isCompleted;
        [SerializeField] private SoqetEditorSettings soqetEditorSettings = new SoqetEditorSettings();
        public UnityEvent OnStoryCompleted = new UnityEvent();

#if UNITY_EDITOR
        private void OnValidate()
        {
            SOQET.Debugging.Debug.EnableDebug = soqetEditorSettings.EnableDebug;
            SoqetEditorSettings.EnableStory = soqetEditorSettings.GetEnableStory();
        }
#endif

        public SoqetEditorSettings GetSoqetEditorSettings()
        {
            return soqetEditorSettings;
        }

        private void SetCurrentObjectiveToDefault()
        {
            currentObjective = defaultObjective;
        }

        public void CreateObjective()
        {
#if UNITY_EDITOR
            Objective objective = MakeObjective();
            Undo.RegisterCreatedObjectUndo(objective, "Created new objective");
            Undo.RecordObject(this, "Created new objective");
            AddObjective(objective);
            EditorUtility.SetDirty(this);
#endif
        }

        public void DeleteObjective(Objective objectiveToDelete)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Deleted Objective");
            objectives.Remove(objectiveToDelete);
            RestructureObjectives();
            Undo.DestroyObjectImmediate(objectiveToDelete);
            EditorUtility.SetDirty(this);
#endif
        }

        private void RestructureObjectives()
        {
            for (int i = 0; i < objectives.Count; i++)
            {
                objectives[i].Order = (i + 1).ToString();
                objectives[i].NextObjective = (i + 2).ToString();
            }
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (objectives.Count.Equals(0))
            {
                Objective objective = MakeObjective();
                defaultObjective = objective;
                currentObjective = defaultObjective;
                AddObjective(objective);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (Objective objective in GetObjectives())
                {
                    if (AssetDatabase.GetAssetPath(objective) == "")
                    {
                        AssetDatabase.AddObjectToAsset(objective, this);
                    }
                }
            }

            soqetEditorSettings.EnableDebug = SOQET.Debugging.Debug.EnableDebug;
            soqetEditorSettings.SetEnableStory(SoqetEditorSettings.EnableStory);

#endif
        }

        private void AddObjective(Objective objective)
        {
#if UNITY_EDITOR
            objectives.Add(objective);
#endif
        }

        private Objective MakeObjective()
        {
            Objective objective = CreateInstance<Objective>();
            objective.Initialize((objectives.Count + 1).ToString(), (objectives.Count + 2).ToString());

#if UNITY_EDITOR
            PositionObjectiveRect(objective);
#endif

            return objective;
        }

        private void PositionObjectiveRect(Objective objective)
        {
#if UNITY_EDITOR
            Objective previousObjective = null;

            if (int.TryParse(objective.NextObjective, out int nextObjective))
            {
                previousObjective = GetObjective(nextObjective - 2);
            }

            if (!previousObjective)
            {
                return;
            }

            else
            {
                float xPos = previousObjective.Rect.x;
                float yPos = previousObjective.Rect.y;
                float offset = Objective.defaultSize * Objective.widthMultiplier + 50f;

                Vector2 position = new Vector2(xPos + offset, yPos);
                objective.SetRectPosition(position);
            }
#endif
        }

        public void OnAfterDeserialize()
        {
 
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }

        public Objective GetObjective(int index)
        {

            if (index > objectives.Count || objectives.Count == 0)
            {
                return null;
            }

            else
            {
                return objectives[index - 1];
            }
        }

        public Objective GetObjective(string name)
        {
            var getObjective = from objective
                           in objectives
                               where objective.name == name
                               select objective;

            if (getObjective.Any())
            {
                return getObjective.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public Objective GetCurrentObjective()
        {
            return currentObjective;
        }

        private bool StartNextObjective()
        {
            if (int.TryParse(currentObjective.NextObjective, out var nextObjectiveIndex))
            {

            }

            else
            {
                SOQET.Debugging.Debug.LogError("Parsing Failed");
            }

            if (nextObjectiveIndex > objectives.Count)
            {
                SOQET.Debugging.Debug.Log("starting next objective unsuccesful");
                return false;
            }

            nextObjectiveIndex -= 1;
            currentObjective = objectives[nextObjectiveIndex];
            SOQET.Debugging.Debug.Log($"starting {currentObjective.name} objective succesful");
            return true;
        }

        private void CompleteStory()
        {
            if(!SoqetEditorSettings.EnableStory)
            {
                return;
            }

            if (isCompleted)
            {
                SOQET.Debugging.Debug.Log($"{name} story already complete");
                return;
            }

            foreach (Objective objective in GetObjectives())
            {
                if (!objective.IsCompleted)
                {
                    return;
                }
            }

            isCompleted = true;
            SOQET.Debugging.Debug.Log($"{name} story completed");
            OnStoryCompleted?.Invoke();
        }

        private void MarkAsIncomplete()
        {
            foreach (Objective objective in GetObjectives())
            {
                objective.MarkAsIncomplete();
            }

            isCompleted = false;
            SOQET.Debugging.Debug.Log($"{name} story marked incomplete");
        }

        public void OnApplicationQuit()
        {
#if UNITY_EDITOR
            if (!soqetEditorSettings.SaveState)
            {
                SetCurrentObjectiveToDefault();
                currentObjective.SetCurrentQuestToDefault();
                MarkAsIncomplete();
            }
#endif
        }

        public void SetupInternalStoryEvents()
        {
            var objectives = GetObjectives();
            foreach (var objective in GetObjectives())
            {
                foreach (var quest in objective.GetQuests())
                {
                    quest.OnQuestCompleted.AddListener
                    (
                        () => 
                        {
                            bool nextQuestExists = objective.StartNextQuest();

                            if(nextQuestExists)
                            {
                                return;
                            }

                            else if(!nextQuestExists)
                            {
                                objective.CompleteObjective();
                                bool nextObjectiveExists = StartNextObjective();

                                if(nextObjectiveExists)
                                {
                                    return;
                                }

                                else if(!nextObjectiveExists)
                                {
                                    CompleteStory();
                                }
                            }
                        }
                    );
                }
            }

            SOQET.Debugging.Debug.Log("Setup Internal Story Events");
        }
    }
}