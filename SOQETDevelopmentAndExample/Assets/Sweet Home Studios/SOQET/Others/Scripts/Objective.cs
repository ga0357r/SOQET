using SOQET.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SOQET.Others
{
    /// <summary>
    /// Objective Scriptable Object
    /// </summary>
    public sealed class Objective : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Objective name
        /// </summary>
        [SerializeField] private string text;

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
            #if UNITY_EDITOR
                text = value;
                SetName(value);
                EditorUtility.SetDirty(this);
            #else
                text = value;
                SetName(value);
            #endif
            }
        }

        /// <summary>
        /// Quests to complete 
        /// </summary>
        [HideInInspector] [SerializeField] private List<Quest> quests = new List<Quest>();
        public List<Quest> Quests
        {
            get => quests;

            set => quests = value;
        }

        /// <summary>
        /// Deleted Quests
        /// </summary>
        [HideInInspector] [SerializeField] private List<Quest> removedQuests = new List<Quest>();
        public List<Quest> RemovedQuests
        {
            get => removedQuests;

            set => removedQuests = value;
        }

        /// <summary>
        /// Unique Identifier
        /// </summary>
        [HideInInspector] [SerializeField] private string id;
        public string ID { get => id; }

        [HideInInspector] [SerializeField] private string order;
        public string Order { get => order; set => order = value; }

        /// <summary>
        /// Is objective started?
        /// </summary>
        [SerializeField] private bool isStarted;
        public bool IsStarted { get => isStarted; set => isStarted = value; }

        /// <summary>
        /// Is objective completed?
        /// </summary>
        [SerializeField] private bool isCompleted;
        public bool IsCompleted { get => isCompleted; set => isCompleted = value; }

        [HideInInspector] [SerializeField] private string nextObjective;
        public string NextObjective { get => nextObjective; set => nextObjective = value; }

        /// <summary>
        /// Current quest
        /// </summary>
        [HideInInspector] [SerializeField] private int currentQuest;
        [HideInInspector] [SerializeField] private int defaultQuest;

        public UnityEvent OnStartObjective = new UnityEvent();
        public UnityEvent OnObjectiveCompleted = new UnityEvent();


#if UNITY_EDITOR
        public const float defaultSize = 100f;
        public const float widthMultiplier = 4;
        [HideInInspector] [SerializeField] private float heightMultiplier = 1;

        [HideInInspector] [SerializeField] private Rect rect = new Rect(0f, 0f, defaultSize, defaultSize);

        public Rect Rect => rect;
#endif

        public void SetRectPosition(Vector2 newPosition)
        {
            #if UNITY_EDITOR
                rect.position = newPosition;
                EditorUtility.SetDirty(this);
            #endif
        }

        public void SetName(string newName)
        {
            name = newName;
        }

        /// <summary>
        /// Generate Unique Identifier
        /// </summary>
        /// <returns></returns>
        private string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Create quest in Editor Window
        /// </summary>
        public void CreateQuest()
        {
            #if UNITY_EDITOR
                Quest quest = MakeQuest();
                SetupCurrentAndDefaultQuests();
                AddQuest(quest);
                EditorUtility.SetDirty(this);
            #endif
        }

        private void SetupCurrentAndDefaultQuests()
        {
            //SetupCurrentAndDefaultQuests to always be quest 1
            if (quests.Count.Equals(0))
                {
                    defaultQuest = 1;
                    currentQuest = defaultQuest;
                }

            else
            {
                defaultQuest = 1;
                currentQuest = defaultQuest;
            }
        }

        /// <summary>
        /// Delete quest in Editor Window
        /// </summary>
        public void DeleteQuest(Quest questToDelete)
        {
            #if UNITY_EDITOR
                quests.Remove(questToDelete);
                removedQuests.Add(questToDelete);
                RestructureQuests();
                ResizeObjectiveRect(false);
                EditorUtility.SetDirty(this);
            #endif
        }

        public void OnBeforeSerialize()
        {
            #if UNITY_EDITOR
                if (AssetDatabase.GetAssetPath(this) != "")
                {
                    foreach (Quest quest in GetQuests())
                    {
                        if (AssetDatabase.GetAssetPath(quest) == "")
                        {
                            AssetDatabase.AddObjectToAsset(quest, this);
                        }
                    }

                    foreach (Quest questToRemove in GetRemovedQuests())
                    {
                        AssetDatabase.RemoveObjectFromAsset(questToRemove);
                    }

                    removedQuests.Clear();
                }
            #endif
        }

        private void AddQuest(Quest quest)
        {
            #if UNITY_EDITOR
                quests.Add(quest);
                ResizeObjectiveRect();
                PositionQuest(quest);
            #endif
        }

        private void PositionQuest(Quest quest)
        {
            #if UNITY_EDITOR
                float xPos = rect.width / widthMultiplier;
                float yPos = Rect.height / heightMultiplier;
                int questIndex = quests.IndexOf(quest);
                float multiplier = questIndex + 1.3f;

                Vector2 position = new Vector2(xPos, yPos * multiplier);
                quest.SetRectPosition(position);
            #endif
        }

        public Quest MakeQuest()
        {
            Quest quest = CreateInstance<Quest>();
            quest.Initialize((quests.Count + 1).ToString(), (quests.Count + 2).ToString());
            return quest;
        }

        public void Initialize(string order, string nextObjective)
        {
            id = GenerateID();
            this.order = order;
            this.nextObjective = nextObjective;
            
            #if UNITY_EDITOR
                ResizeObjectiveRect();
            #endif
        }

        /// <summary>
        /// Rezise Objective Rect
        /// </summary>
        /// <param name="shouldIncrease"> true = increase. false = reduce</param>
        public void ResizeObjectiveRect(bool shouldIncrease = true)
        {
            #if UNITY_EDITOR
                if (shouldIncrease)
                {
                    heightMultiplier++;
                }

                else
                {
                    heightMultiplier--;
                }

                rect = new Rect(Rect.x, rect.y, defaultSize * widthMultiplier, defaultSize * heightMultiplier);
            #endif
        }

        public void SetCurrentQuestToDefault()
        {
            currentQuest = defaultQuest;
        }

        public void OnAfterDeserialize()
        {

        }

        private void RestructureQuests()
        {
            for (int i = 0; i < quests.Count; i++)
            {
                quests[i].Order = (i + 1).ToString();
                quests[i].NextQuest = (i + 2).ToString();

                #if UNITY_EDITOR
                    PositionQuest(quests[i]);
                #endif
            }
        }

        /// <summary>
        /// Start Objective. Invokes OnStartObjectiveEvent here 
        /// </summary>
        public void StartObjective()
        {
            if(!SoqetEditorSettings.EnableStory)
            {
                return;
            }

            if(isStarted)
            {
                SOQET.Debugging.Debug.Log($"{name} objective already started");
                return;
            }


            isStarted = true;
            SOQET.Debugging.Debug.Log($"{name} objective started");
            OnStartObjective?.Invoke();   
        }

        /// <summary>
        /// Complete Objective. Invokes OnCompleteObjectiveEvent here 
        /// </summary>
        public void CompleteObjective()
        {
            if(!SoqetEditorSettings.EnableStory)
            {
                return;
            }

            if(isCompleted)
            {
                SOQET.Debugging.Debug.Log($"{name} objective already complete");
                return;
            }

            foreach (Quest quest in GetQuests())
            {
                if (!quest.IsCompleted)
                {
                    return;
                }
            }


            isCompleted = true;
            SOQET.Debugging.Debug.Log($"{name} objective completed");
            OnObjectiveCompleted?.Invoke();   
        }

        public void MarkAsIncomplete()
        {
            if(!SoqetEditorSettings.EnableStory)
            {
                return;
            }

            foreach (Quest quest in GetQuests())
            {
                quest.MarkAsIncomplete();
            }

            isStarted = false;
            isCompleted = false;
            SOQET.Debugging.Debug.Log($"{name} objective marked incomplete");
        }

        public IEnumerable<Quest> GetQuests()
        {
            return quests;
        }

        public IEnumerable<Quest> GetRemovedQuests()
        {
            return removedQuests;
        }

        public Quest GetQuest(int index)
        {
            if (index > quests.Count)
            {
                return null;
            }

            else
            {
                return quests[index - 1];
            }
        }

        public Quest GetQuest(string name)
        {
            var getQuest = from quest
                           in quests
                           where quest.name == name
                           select quest;

            if (getQuest.Any())
            {
                return getQuest.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Start next quest automatically called during SOQET Event Tick 
        /// </summary>
        /// <returns></returns>
        public bool StartNextQuest()
        {
            Quest currentQuestObject = GetCurrentQuestObject();
            if (int.TryParse(currentQuestObject.NextQuest, out var nextQuestIndex))
            {

            }

            else
            {
                SOQET.Debugging.Debug.LogError("Parsing Failed");
                return false;
            }

            if (nextQuestIndex > quests.Count)
            {
                SOQET.Debugging.Debug.Log("starting next quest unsuccesful");
                return false;
            }

            currentQuest = nextQuestIndex;
            currentQuestObject = GetCurrentQuestObject();
            currentQuestObject.StartQuest();
            SOQET.Debugging.Debug.Log($"starting {currentQuestObject.name} quest succesful");
            return true;
        }

        public Quest GetCurrentQuestObject()
        {
            return GetQuest(currentQuest);
        }

        public float CalculateObjectiveProgress()
        {
            float progress = 0;

            var getCompletedQuests = from quest
                                     in GetQuests()
                                     where quest.IsCompleted == true
                                     select quest;

            progress = (float) getCompletedQuests.Count()/(float) GetQuests().Count();
            return progress;
        }

        public void SetCurrentQuest(int currentQuest)
        {
            this.currentQuest = currentQuest;
        }

        public int GetCurrentQuest()
        {
            return currentQuest;
        }

        public void StartCurrentQuest()
        {
            if(GetQuests().Count().Equals(0)) return;

            GetCurrentQuestObject().StartQuest();
        }
    }
}