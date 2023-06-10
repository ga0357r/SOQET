using System;
using UnityEngine;
using UnityEngine.Events;
using SOQET.Editor;
using System.Collections.Generic;
using static SOQET.Others.Reward;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SOQET.Others
{
    public sealed class Quest : ScriptableObject, ISerializationCallbackReceiver
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
                text = value;
                SetName(value);
                EditorUtility.SetDirty(this);
#else
                text = value;
                SetName(value);
#endif
            }
        }

        [HideInInspector][SerializeField] private string id;
        public string ID { get => id; }

        [HideInInspector][SerializeField] private string order;
        public string Order { get => order; set => order = value; }

        [SerializeField] private bool isStarted;
        public bool IsStarted { get => isStarted; set => isStarted = value; }

        [SerializeField] private bool isCompleted;
        public bool IsCompleted { get => isCompleted; set => isCompleted = value; }

        [HideInInspector][SerializeField] private string nextQuest;
        public string NextQuest { get => nextQuest; set => nextQuest = value; }

        public UnityEvent OnStartQuest = new UnityEvent();
        public UnityEvent OnQuestCompleted = new UnityEvent();

#if UNITY_EDITOR
        [HideInInspector][SerializeField] private Rect rect = new Rect(0f, 0f, 200f, 100f);

        public Rect Rect { get => rect; }
#endif

        [SerializeField] private List<Reward> rewards = new List<Reward>();

        [HideInInspector][SerializeField] private List<Reward> removedRewards = new List<Reward>();
        public List<Reward> RemovedRewards
        {
            get => removedRewards;

            set => removedRewards = value;
        }


        public void SetRectPosition(Vector2 newPosition)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Change quest node position");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
#endif
        }

        public void Initialize(string order, string nextQuest)
        {
            id = GenerateID();
            this.order = order;
            this.nextQuest = nextQuest;
        }

        public void SetName(string newName)
        {
            name = newName;
        }

        private string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }

        public void StartQuest()
        {
            if (!SoqetEditorSettings.EnableStory)
            {
                return;
            }

            if (isStarted)
            {
                SOQET.Debugging.Debug.Log($"{name} quest already started");
                return;
            }

            isStarted = true;
            SOQET.Debugging.Debug.Log($"{name} quest started");
            OnStartQuest?.Invoke();
        }

        public void CompleteQuest()
        {
            if (!SoqetEditorSettings.EnableStory)
            {
                return;
            }

            if (isCompleted)
            {
                SOQET.Debugging.Debug.Log($"{name} quest already complete");
                return;
            }

            isCompleted = true;
            SOQET.Debugging.Debug.Log($"{name} quest completed");
            OnQuestCompleted?.Invoke();
        }

        public void MarkAsIncomplete()
        {
            if (!SoqetEditorSettings.EnableStory)
            {
                return;
            }

            isStarted = false;
            isCompleted = false;
            SOQET.Debugging.Debug.Log($"{name} quest marked incomplete");
        }

        public void GiveRewards()
        {
            foreach (Reward reward in GetRewards())
            {
                switch (reward.CurrentRewardType)
                {
                    case RewardType.None:
                        break;
                    case RewardType.ExperiencePoints:
                        //run event here that gives player experience points
                        //give player experience points
                        //reward.ReleaseReward?.Invoke()
                        break;
                    case RewardType.ItemsAndEquipments:
                        //run event here that gives player items and equipments
                        //give player items and equipments
                        break;

                    case RewardType.AchievementsAndTrophies:
                        //run event here that gives achievemtns and trophies
                        //give achievemtns and trophies
                        break;
                    default:
                        Console.WriteLine("Unknown reward type!");
                        break;
                }
            }
        }

        public void CreateReward()
        {
#if UNITY_EDITOR

            Reward reward = MakeReward();
            AddReward(reward);
#endif
        }

        private Reward MakeReward()
        {
            Reward reward = CreateInstance<Reward>();
            reward.Initialize((rewards.Count + 1).ToString());
            return reward;
        }

        private void AddReward(Reward reward)
        {
#if UNITY_EDITOR
            rewards.Add(reward);
            PositionRewardRect(reward);
#endif
        }

        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }

        private void RestructureRewards()
        {
            for (int i = 0; i < GetRewards().Count(); i++)
            {
                rewards[i].Order = (i + 1).ToString();
            }
        }


        private void PositionRewardRect(Reward reward)
        {
#if UNITY_EDITOR
            float xPos = reward.Rect.x;
            float yPos = reward.Rect.y;
            //float offset = Reward.defaultSize * Reward.widthMultiplier + 50f;

           // Vector2 position = new Vector2(xPos + offset, yPos);
            //reward.SetRectPosition(position);
#endif
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (Reward reward in GetRewards())
                {
                    if (AssetDatabase.GetAssetPath(reward) == "")
                    {
                        AssetDatabase.AddObjectToAsset(reward, this);
                    }
                }

                foreach (Reward rewardToRemove in GetRemovedRewards())
                {
                    AssetDatabase.RemoveObjectFromAsset(rewardToRemove);
                }

                removedRewards.Clear();
            }
#endif
        }

        public void OnAfterDeserialize()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Reward> GetRemovedRewards()
        {
            return removedRewards;
        }
    }
}
