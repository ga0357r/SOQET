using System;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine.Events;

namespace SOQET.Others
{
    [Serializable]
    public sealed class Reward
    {
        [SerializeField] private string name;

        public string Name 
        {
            get => name;
        }

        [SerializeField] private RewardType rewardType;

        public RewardType CurrentRewardType 
        { 
            get => rewardType;  
        }

        [SerializeField] private int amount;
        public int Amount => amount;
        public UnityEvent ReleaseReward = new UnityEvent();

        public enum RewardType
        {
            None,
            ExperiencePoints,
            ItemsAndEquipments,
            AchievementsAndTrophies
        }
    }
}