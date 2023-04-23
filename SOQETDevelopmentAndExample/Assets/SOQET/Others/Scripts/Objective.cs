using SOQET.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Debug = UnityEngine.Debug;

namespace SOQET.Others
{
    public class Objective : ScriptableObject, ISerializationCallbackReceiver
    {
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
                Undo.RecordObject(this, "Change quest node name");
                text = value;
                SetName(value);
                EditorUtility.SetDirty(this);
            #else
                text = value;
                SetName(value);
            #endif
            }
        }

        [SerializeField] private List<Quest> quests = new List<Quest>();

        [SerializeField] private string id;
        public string ID { get => id; }

        [SerializeField] private string order;
        public string Order { get => order; set => order = value; }

        [SerializeField] private bool isCompleted;
        public bool IsCompleted { get => isCompleted; set => isCompleted = value; }

        [SerializeField] private string nextObjective;
        public string NextObjective { get => nextObjective; set => nextObjective = value; }

        [SerializeField] private Quest currentQuest;
        [SerializeField] private Quest defaultQuest;

        [SerializeField] private Dictionary<string, Quest> questsDictionary = new Dictionary<string, Quest>();
        public UnityEvent OnObjectiveCompleted = new UnityEvent();

#if UNITY_EDITOR
        public const float defaultSize = 100f;
        public const float widthMultiplier = 4;
        [SerializeField] private float heightMultiplier = 1;

        [SerializeField] private Rect rect = new Rect(0f, 0f, defaultSize, defaultSize);

        public Rect Rect => rect;
#endif

        public void SetRectPosition(Vector2 newPosition)
        {
            #if UNITY_EDITOR
                Undo.RecordObject(this, "Change quest node position");
                rect.position = newPosition;
                EditorUtility.SetDirty(this);
            #endif
        }

        public void SetName(string newName)
        {
            name = newName;
        }

        private string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }

        public void CreateQuest()
        {
            #if UNITY_EDITOR
                Quest quest = MakeQuest();
                Undo.RegisterCreatedObjectUndo(quest, "Created new quest");
                Undo.RecordObject(this, "Created new quest");
                AddQuest(quest);
                EditorUtility.SetDirty(this);
            #endif
        }

        public void DeleteQuest(Quest questToDelete)
        {
            #if UNITY_EDITOR
                Undo.RecordObject(this, "Deleted Quest");
                quests.Remove(questToDelete);
                UpdateQuestsDictionary();
                RestructureQuests();
                ResizeObjectiveRect(false);
                Undo.DestroyObjectImmediate(questToDelete);
                EditorUtility.SetDirty(this);
            #endif
        }

        public void OnBeforeSerialize()
        {
            #if UNITY_EDITOR
                if (quests.Count.Equals(0))
                {
                    Quest quest = MakeQuest();
                    defaultQuest = quest;
                    currentQuest = defaultQuest;
                    AddQuest(quest);
                }

                if (AssetDatabase.GetAssetPath(this) != "")
                {
                    foreach (Quest quest in GetQuests())
                    {
                        if (AssetDatabase.GetAssetPath(quest) == "")
                        {
                            AssetDatabase.AddObjectToAsset(quest, this);
                        }
                    }
                }
            #endif
        }

        private void AddQuest(Quest quest)
        {
            #if UNITY_EDITOR
                quests.Add(quest);
                UpdateQuestsDictionary();
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

        private void UpdateQuestsDictionary()
        {
            #if UNITY_EDITOR
                questsDictionary.Clear();

                foreach (Quest quest in GetQuests())
                {
                    if (!questsDictionary.ContainsKey(quest.ID))
                    {
                        questsDictionary.Add($"{quest.ID}", quest);
                    }
                }
            #endif
        }

        public void SetCurrentQuestToDefault()
        {
            currentQuest = defaultQuest;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateQuestsDictionary();
        }
#endif

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

        [Conditional(SoqetEditorSettings.symbol)]
        public void CompleteObjective()
        {
            if(isCompleted)
            {
                Debug.Log($"{name} objective already complete");
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
            Debug.Log($"{name} objective completed");
            OnObjectiveCompleted?.Invoke();   
        }

        [Conditional(SoqetEditorSettings.symbol)]
        public void MarkAsIncomplete()
        {
            foreach (Quest quest in GetQuests())
            {
                quest.MarkAsIncomplete();
            }

            isCompleted = false;
            Debug.Log("Marked as Incomplete");
        }

        public IEnumerable<Quest> GetQuests()
        {
            return quests;
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

        public bool StartNextQuest()
        {
            if (int.TryParse(currentQuest.NextQuest, out var nextQuestIndex))
            {
                Debug.Log("Parsing Successful");
            }

            else
            {
                Debug.Log("Parsing Failed");
            }

            if (nextQuestIndex > quests.Count)
            {
                Debug.Log("starting next quest unsuccesful");
                return false;
            }

            nextQuestIndex -= 1;
            currentQuest = quests[nextQuestIndex];
            Debug.Log($"starting {currentQuest.name} quest succesful");
            return true;
        }

        public void BeginNextQuest()
        {
            if (int.TryParse(currentQuest.NextQuest, out var nextQuestIndex))
            {
                Debug.Log("Parsing Successful");
            }

            else
            {
                Debug.Log("Parsing Failed");
            }

            if (nextQuestIndex > quests.Count)
            {
                Debug.Log("starting next quest unsuccesful");
                return;
            }

            nextQuestIndex -= 1;
            currentQuest = quests[nextQuestIndex];
            Debug.Log("starting next quest succesful");
        }

        public Quest GetCurrentQuest()
        {
            return currentQuest;
        }
    }
}