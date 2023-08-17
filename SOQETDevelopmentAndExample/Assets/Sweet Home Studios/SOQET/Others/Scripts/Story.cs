using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using SOQET.Editor;
using SOQET.Debugging;
using SOQET.DataPersistence;
using SOQET.Security;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SOQET.Others
{
    /// <summary>
    /// Story Scriptable Object. Can be created in Project Window
    /// </summary>
    [CreateAssetMenu(fileName = "New Story", menuName = "SOQET/Story")]
    public sealed class Story : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Objectives to complete 
        /// </summary>
        [HideInInspector] [SerializeField] private List<Objective> objectives = new List<Objective>();

        /// <summary>
        /// Deleted objectives
        /// </summary>
        [HideInInspector] [SerializeField] private List<Objective> removedObjectives = new List<Objective>();
        [HideInInspector] [SerializeField] private int currentObjective;
        [HideInInspector] [SerializeField] private int defaultObjective;

        /// <summary>
        /// Is story started?
        /// </summary>
        [SerializeField] private bool isStarted;

        /// <summary>
        /// Is story completed?
        /// </summary>
        [SerializeField] private bool isCompleted;

        [SerializeField] private SoqetEditorSettings soqetEditorSettings = new SoqetEditorSettings();
        public UnityEvent OnStartStory = new UnityEvent();
        public UnityEvent OnStoryCompleted = new UnityEvent();
        private UnityAction QuestCompletedCallback;

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

        /// <summary>
        /// Create objective in Editor Window
        /// </summary>
        public void CreateObjective()
        {
#if UNITY_EDITOR
            Objective objective = MakeObjective();
            SetupCurrentAndDefaultObjectives(objective);
            AddObjective(objective);
            EditorUtility.SetDirty(this);


#endif
        }

        private void SetupCurrentAndDefaultObjectives(Objective objective)
        {
            if (objectives.Count.Equals(0))
            {

                defaultObjective = 1;
                currentObjective = defaultObjective;
            }

            else
            {
                defaultObjective = 1;
                currentObjective = defaultObjective;
            }
        }

        /// <summary>
        /// Delete objective in Editor Window
        /// </summary>
        public void DeleteObjective(Objective objectiveToDelete)
        {
#if UNITY_EDITOR
            RemoveQuests(objectiveToDelete);
            objectives.Remove(objectiveToDelete);
            removedObjectives.Add(objectiveToDelete);
            RestructureObjectives();
            EditorUtility.SetDirty(this);
#endif
        }


        private void RemoveQuests(Objective objective)
        {
            if (objective.GetQuests().Count().Equals(0))
            {
                return;
            }

            List<Quest> questsCopy = objective.GetQuests().ToList();
            List<Quest> removedQuestsCopy = objective.GetRemovedQuests().ToList();

            foreach (Quest questToRemove in objective.GetQuests())
            {
                questsCopy.Remove(questToRemove);
                removedQuestsCopy.Add(questToRemove);
            }

            objective.Quests = questsCopy.ToList();
            objective.RemovedQuests = removedQuestsCopy.ToList();
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
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (Objective objective in GetObjectives())
                {
                    if (AssetDatabase.GetAssetPath(objective) == "")
                    {
                        AssetDatabase.AddObjectToAsset(objective, this);
                    }
                }

                foreach (Objective objectiveToRemove in GetRemovedObjectives())
                {
                    AssetDatabase.RemoveObjectFromAsset(objectiveToRemove);
                }

                removedObjectives.Clear();
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

        public IEnumerable<Objective> GetRemovedObjectives()
        {
            return removedObjectives;
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

        public Objective GetCurrentObjectiveObject()
        {
            return GetObjective(currentObjective);
        }

        private bool StartNextObjective()
        {
            if (int.TryParse(GetCurrentObjectiveObject().NextObjective, out var nextObjectiveIndex))
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

            currentObjective = nextObjectiveIndex;
            Objective currentObjectObject = GetCurrentObjectiveObject();
            currentObjectObject.StartObjective();
            SOQET.Debugging.Debug.Log($"starting {currentObjectObject.name} objective succesful");
            return true;
        }

        /// <summary>
        /// Start Story. Invokes OnStartOStoryEvent here 
        /// </summary>
        private void StartStory()
        {
            if(!SoqetEditorSettings.EnableStory)
            {
                return;
            }

            if (isStarted)
            {
                SOQET.Debugging.Debug.Log($"{name} story already started");
                return;
            }

            isStarted = true;
            SOQET.Debugging.Debug.Log($"{name} story started");
            OnStartStory?.Invoke();
            Objective currentObjectiveObject = GetCurrentObjectiveObject();
            currentObjectiveObject.StartObjective();
            currentObjectiveObject.GetCurrentQuestObject().StartQuest();
        }

        /// <summary>
        /// Complete Story. Invokes OnCompleteStoryEvent here 
        /// </summary>
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
            if(!SoqetEditorSettings.EnableStory)
            {
                return;
            }

            foreach (Objective objective in GetObjectives())
            {
                objective.MarkAsIncomplete();
            }

            isStarted = false;
            isCompleted = false;
            SOQET.Debugging.Debug.Log($"{name} story marked incomplete");
        }

        public void OnApplicationQuit()
        {
            SaveStory();
        }

        private void SetAllObjectivesAndQuestsToDefault()
        {
            SetCurrentObjectiveToDefault();

            foreach (Objective objective in GetObjectives())
            {
                objective.SetCurrentQuestToDefault();
            }
        }

        public void Initialize()
        {
            HandleAllEventCompletions();
            StartStory();
        }

        public void Dispose()
        {
            foreach (var objective in GetObjectives())
            {
                foreach (var quest in objective.GetQuests())
                {
                    quest.OnQuestCompleted.RemoveListener(QuestCompletedCallback);
                }
            }
        }

        //Automatically Handle all Event Completion. 
        private void HandleAllEventCompletions()
        {
            var objectives = GetObjectives();
            foreach (var objective in GetObjectives())
            {
                foreach (var quest in objective.GetQuests())
                {
                    QuestCompletedCallback = () =>
                    {
                        EventTick(objective);
                    };

                    quest.OnQuestCompleted.AddListener(QuestCompletedCallback);
                }
            }
        }

        /// <summary>
        /// SOQET Event Tick Method. Called after each quest is completed
        /// </summary>
        /// <param name="objective"></param>
        private void EventTick(Objective objective)
        {
            bool nextQuestExists = objective.StartNextQuest();

            if (nextQuestExists)
            {
                return;
            }

            else if (!nextQuestExists)
            {
                objective.CompleteObjective();
                bool nextObjectiveExists = StartNextObjective();

                if (nextObjectiveExists)
                {
                    //start current quest
                    GetCurrentObjectiveObject().StartCurrentQuest();
                    return;
                }

                else if (!nextObjectiveExists)
                {
                    CompleteStory();
                }
            }

        }
        
        /// <summary>
        /// Save Story Data. Encrypted/Unencrypted
        /// </summary>
        public void SaveStory()
        {
#if UNITY_EDITOR
            if (!soqetEditorSettings.SaveState)
            {
                ResetStory();
            }

#else       
            if(soqetEditorSettings.SaveState)
            {
                //save with json utility
                SaveAndLoad.SaveDefaultJson(this);

                if(soqetEditorSettings.EncryptSaveFile)
                {
                    AESEncryption.EncryptFile(SaveAndLoad.GetSavePath());
                }
            }
#endif
        }

        /// <summary>
        /// Load Saved Story. Automatically called on Start
        /// </summary>
        public void LoadSavedStory()
        {   
            if(soqetEditorSettings.SaveState)
            {
                //Decrypt Save File
                if(soqetEditorSettings.EncryptSaveFile)
                {
                    AESEncryption.DecryptFile(SaveAndLoad.GetSavePath());
                }
                
                SaveAndLoad.LoadDefaultJson(this);

                //encrypt after loading
                if(soqetEditorSettings.EncryptSaveFile)
                {
                    AESEncryption.EncryptFile(SaveAndLoad.GetSavePath());
                }
            }
        }

        public void SetIsStarted(bool isStarted)
        {
            this.isStarted = isStarted;
        }

        public void SetIsCompleted(bool isCompleted)
        {
            this.isCompleted = isCompleted;
        }

        public void SetCurrentObjective(int currentObjective)
        {
            this.currentObjective = currentObjective;
        }

        public bool GetIsStarted()
        {
            return isStarted;
        }

        public bool GetIsCompleted()
        {
            return isCompleted;
        }

        public int GetCurrentObjective()
        {
            return currentObjective;
        }

        /// <summary>
        /// Reset the story to default. Can be called from inspector by right-clicking-> ResetStory.
        /// </summary>
        [ContextMenu("Reset Story")]
        public void ResetStory()
        {
            SetAllObjectivesAndQuestsToDefault();
            MarkAsIncomplete();
        }
    }
}