using System;
using UnityEngine;
using UnityEditor;

namespace SOQET.Others
{
    [Serializable]
    public sealed class Reward : ScriptableObject
    {
        [SerializeField] private string text;

        public string Text
        {
            get => text;

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

        [SerializeField] private RewardType rewardType = RewardType.None;


        public RewardType CurrentRewardType => rewardType;

        [SerializeField] private int amount;
        public int Amount => amount;

#if UNITY_EDITOR
        public const float defaultSize = 100f;
        public const float widthMultiplier = 4;
        [HideInInspector][SerializeField] private float heightMultiplier = 1;

        [HideInInspector][SerializeField] private Rect rect = new Rect(0f, 0f, 200f, 100f);

        public Rect Rect { get => rect; }
#endif
        public enum RewardType
        {
            None,
            ExperiencePoints,
            ItemsAndEquipments,
            AchievementsAndTrophies
        }
        
        private void SetName(string newName)
        {
            name = newName;
        }

        public void SetRectPosition(Vector2 newPosition)
        {
#if UNITY_EDITOR
            rect.position = newPosition;
#endif
        }

        public void Initialize(string order)
        {
            id = GenerateID();
            this.order = order;
        }

        private string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}