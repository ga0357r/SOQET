using SOQET.Others;
using UnityEngine;
using UnityEngine.Events;
using SOQET.Debugging;

namespace SOQET.Inspector
{
    /// <summary>
    /// SOQET Inspector. Only 1 per scene
    /// </summary>
    public sealed class SoqetInspector : MonoBehaviour
    {
        private static SoqetInspector instance;
        public static SoqetInspector Instance { get => instance; }

        #region Stories

        #region Fields
        /// <summary>
        /// Current Story to complete
        /// </summary>
        [SerializeField] private Story currentStory;
        #endregion

        #region Properties
        public Story CurrentStory => currentStory;
        #endregion

        #endregion

        private void OnEnable()
        {
            //subcribe to any story event here
            currentStory.Initialize();
        }

        private void OnDisable()
        {
            //unsubscribe from any story event here
            currentStory.Dispose();
        }

        private void Awake()
        {
            if (instance)
            {
                Destroy(instance);
            }

            else
            {
                instance = this;
            }
        }

        private void Start()
        {
            //load story data
            if (currentStory.GetSoqetEditorSettings().SaveState)
            {
                currentStory.LoadSavedStory();
            }
        }

        public void OnApplicationQuit()
        {
            currentStory.OnApplicationQuit();
        }

        /// <summary>
        /// Complete Player Quest. All subscribers are notified
        /// </summary>
        /// <param name="objectiveName"> </param>
        /// <param name="questName"></param>
        public void CompletePlayerQuest(string objectiveName, string questName)
        {
            Objective objective = CurrentStory.GetObjective(objectiveName);

            if (objective && !objective.IsCompleted)
            {
                Quest quest = objective.GetQuest(questName);

                if (quest && !quest.IsCompleted)
                {
                    quest.CompleteQuest();
                }

                else
                {
                    if (!quest)
                    {
                        SOQET.Debugging.Debug.LogError($"{questName} quest not found");
                    }

                    else if (quest.IsCompleted)
                    {
                        SOQET.Debugging.Debug.LogError($"{questName} quest already complete");
                    }
                }
            }

            else
            {
                if (!objective)
                {
                    SOQET.Debugging.Debug.LogError($"{objectiveName} objective not found");
                }

                else if (objective.IsCompleted)
                {
                    SOQET.Debugging.Debug.LogError($"{objectiveName} objective already complete");
                }
            }
        }

        public void CompletePlayerQuest(int questNumber)
        {
            //use current objective
            Objective currentObjective = currentStory.GetCurrentObjectiveObject();

            if (currentObjective && !currentObjective.IsCompleted)
            {
                Quest quest = currentObjective.GetQuest(questNumber);

                if (quest && !quest.IsCompleted)
                {
                    quest.CompleteQuest();
                }

                else
                {
                    if (!quest)
                    {
                        SOQET.Debugging.Debug.LogError($"{quest} quest not found");
                    }

                    else if (quest.IsCompleted)
                    {
                        SOQET.Debugging.Debug.LogError($"{quest} quest already complete");
                    }
                }
            }

            else
            {
                if (!currentObjective)
                {
                    SOQET.Debugging.Debug.LogError($"{currentObjective} objective not found");
                }

                else if (currentObjective.IsCompleted)
                {
                    SOQET.Debugging.Debug.LogError($"{currentObjective} objective already complete");
                }
            }
        }

        public void SubscribeToQuestOnCompleteEvent(string objectiveName, string questName, UnityAction call)
        {
            Objective objective = CurrentStory.GetObjective(objectiveName);

            if (objective)
            {
                Quest quest = objective.GetQuest(questName);

                if (quest)
                {
                    quest.OnQuestCompleted.AddListener(call);
                    SOQET.Debugging.Debug.Log($"Subscribed to {questName} quest OnQuestCompleted event");
                }

                else
                {
                    SOQET.Debugging.Debug.LogError($"{questName} quest not found");
                }
            }

            else
            {
                SOQET.Debugging.Debug.LogError($"{objectiveName} objective not found");
            }
        }

        public void UnsubscribeFromQuestOnCompleteEvent(string objectiveName, string questName, UnityAction call)
        {
            Objective objective = CurrentStory.GetObjective(objectiveName);

            if (objective)
            {
                Quest quest = objective.GetQuest(questName);

                if (quest)
                {
                    quest.OnQuestCompleted.RemoveListener(call);
                    SOQET.Debugging.Debug.Log($"Unsubscribed from {questName} quest OnQuestCompleted event");
                }

                else
                {
                    SOQET.Debugging.Debug.LogError($"{questName} quest not found");
                }
            }

            else
            {
                SOQET.Debugging.Debug.LogError($"{objectiveName} objective not found");
            }
        }

        public void SubscribeToQuestOnStartEvent(string objectiveName, string questName, UnityAction call)
        {
            Objective objective = CurrentStory.GetObjective(objectiveName);

            if (objective)
            {
                Quest quest = objective.GetQuest(questName);

                if (quest)
                {
                    quest.OnStartQuest.AddListener(call);
                    SOQET.Debugging.Debug.Log($"Subscribed to {questName} quest OnStartQuest event");
                }

                else
                {
                    SOQET.Debugging.Debug.LogError($"{questName} quest not found");
                }
            }

            else
            {
                SOQET.Debugging.Debug.LogError($"{objectiveName} objective not found");
            }
        }

        public void UnsubscribeFromQuestOnStartEvent(string objectiveName, string questName, UnityAction call)
        {
            Objective objective = CurrentStory.GetObjective(objectiveName);

            if (objective)
            {
                Quest quest = objective.GetQuest(questName);

                if (quest)
                {
                    quest.OnStartQuest.RemoveListener(call);
                    SOQET.Debugging.Debug.Log($"Unsubscribed from {questName} quest OnStartQuest event");
                }

                else
                {
                    SOQET.Debugging.Debug.LogError($"{questName} quest not found");
                }
            }

            else
            {
                SOQET.Debugging.Debug.LogError($"{objectiveName} objective not found");
            }
        }


        public void SubscribeToObjectiveOnCompleteEvent(string objectiveName, UnityAction call)
        {
            Objective objective = CurrentStory.GetObjective(objectiveName);

            if (objective)
            {
                objective.OnObjectiveCompleted.AddListener(call);
                SOQET.Debugging.Debug.Log($"Subscribed to {objectiveName} objective OnObjectiveCompleted event");
            }

            else
            {
                SOQET.Debugging.Debug.LogError($"{objectiveName} objective not found");
            }
        }

        public void UnsubscribeFromObjectiveOnCompleteEvent(string objectiveName, UnityAction call)
        {
            Objective objective = CurrentStory.GetObjective(objectiveName);

            if (objective)
            {
                objective.OnObjectiveCompleted.RemoveListener(call);
                SOQET.Debugging.Debug.Log($"Unsubscribed from {objectiveName} objective OnObjectiveCompleted event");
            }

            else
            {
                SOQET.Debugging.Debug.LogError($"{objectiveName} objective not found");
            }
        }

        public void SubscribeToObjectiveOnStartEvent(string objectiveName, UnityAction call)
        {
            Objective objective = CurrentStory.GetObjective(objectiveName);

            if (objective)
            {
                objective.OnStartObjective.AddListener(call);
                SOQET.Debugging.Debug.Log($"Subscribed to {objectiveName} objective OnStartObjective event");
            }

            else
            {
                SOQET.Debugging.Debug.LogError($"{objectiveName} objective not found");
            }
        }

        public void UnsubscribeFromObjectiveOnStartEvent(string objectiveName, UnityAction call)
        {
            Objective objective = CurrentStory.GetObjective(objectiveName);

            if (objective)
            {
                objective.OnStartObjective.RemoveListener(call);
                SOQET.Debugging.Debug.Log($"Unsubscribed from {objectiveName} objective OnStartObjective event");
            }

            else
            {
                SOQET.Debugging.Debug.LogError($"{objectiveName} objective not found");
            }
        }

        public void SubscribeToAllQuestsOnCompleteEvents(UnityAction call)
        {
            foreach (Objective objective in currentStory.GetObjectives())
            {
                foreach (Quest quest in objective.GetQuests())
                {
                    quest.OnQuestCompleted.AddListener(call);
                    SOQET.Debugging.Debug.Log($"Subscribed to {quest} quest OnQuestCompleted event");
                }
            }
        }

        public void UnsubscribeFromAllQuestsOnCompleteEvents(UnityAction call)
        {
            foreach (Objective objective in currentStory.GetObjectives())
            {
                foreach (Quest quest in objective.GetQuests())
                {
                    quest.OnQuestCompleted.RemoveListener(call);
                    SOQET.Debugging.Debug.Log($"unsubscribed from {quest} quest OnQuestCompleted event");
                }
            }
        }

        public void SubscribeToAllQuestsOnStartEvents(UnityAction call)
        {
            foreach (Objective objective in currentStory.GetObjectives())
            {
                foreach (Quest quest in objective.GetQuests())
                {
                    quest.OnStartQuest.AddListener(call);
                    SOQET.Debugging.Debug.Log($"Subscribed to {quest} quest OnStartQuest event");
                }
            }
        }

        public void UnsubscribeFromAllQuestsOnStartEvents(UnityAction call)
        {
            foreach (Objective objective in currentStory.GetObjectives())
            {
                foreach (Quest quest in objective.GetQuests())
                {
                    quest.OnStartQuest.RemoveListener(call);
                    SOQET.Debugging.Debug.Log($"Unsubscribed from {quest} quest OnStartQuest event");
                }
            }
        }

        public void SubscribeToAllObjectivesOnCompleteEvents(UnityAction call)
        {
            foreach (Objective objective in currentStory.GetObjectives())
            {
                objective.OnObjectiveCompleted.AddListener(call);
                SOQET.Debugging.Debug.Log($"Subscribed to {objective} objective OnObjectiveCompleted event");
            }
        }

        public void UnsubscribeFromAllObjectivesOnCompleteEvents(UnityAction call)
        {
            foreach (Objective objective in currentStory.GetObjectives())
            {
                objective.OnObjectiveCompleted.RemoveListener(call);
                SOQET.Debugging.Debug.Log($"Unsubscribed from {objective} objective OnObjectiveCompleted event");
            }
        }

        public void SubscribeToAllObjectivesOnStartEvents(UnityAction call)
        {
            foreach (Objective objective in currentStory.GetObjectives())
            {
                objective.OnStartObjective.AddListener(call);
                SOQET.Debugging.Debug.Log($"Subscribed to {objective} objective OnStartObjective event");
            }
        }

        public void UnsubscribeFromAllObjectivesOnStartEvents(UnityAction call)
        {
            foreach (Objective objective in currentStory.GetObjectives())
            {
                objective.OnStartObjective.RemoveListener(call);
                SOQET.Debugging.Debug.Log($"Unsubscribed from {objective} objective OnStartObjective event");
            }
        }

        public void SubscribeToOnStartStoryEvent(UnityAction call)
        {
            currentStory.OnStartStory.AddListener(call);
            SOQET.Debugging.Debug.Log($"Subscribed to {currentStory} story OnStartStory event");
        }

        public void UnsubscribeFromOnStartStoryEvent(UnityAction call)
        {
            currentStory.OnStartStory.RemoveListener(call);
            SOQET.Debugging.Debug.Log($"Unsubscribed from {currentStory} story OnStartStory event");
        }

        public void SubscribeToOnStoryCompletedEvent(UnityAction call)
        {
            currentStory.OnStoryCompleted.AddListener(call);
            SOQET.Debugging.Debug.Log($"Subscribed to {currentStory} story OnStoryCompleted event");
        }

        public void UnsubscribeFromOnStoryCompletedEvent(UnityAction call)
        {
            currentStory.OnStoryCompleted.RemoveListener(call);
            SOQET.Debugging.Debug.Log($"Unsubscribed from {currentStory} story OnStoryCompleted event");
        }

        public void SubscribeToAllStoryEvents(UnityAction call)
        {
            //OnStart
            SubscribeToOnStartStoryEvent(call);
            SubscribeToAllObjectivesOnStartEvents(call);
            SubscribeToAllQuestsOnStartEvents(call);

            //OnComplete
            SubscribeToOnStoryCompletedEvent(call);
            SubscribeToAllObjectivesOnCompleteEvents(call);
            SubscribeToAllQuestsOnCompleteEvents(call);
            SOQET.Debugging.Debug.Log($"Subscribed to all {currentStory} story events");
        }

        public void UnsubscribeFromAllStoryEvents(UnityAction call)
        {
            //OnStart
            UnsubscribeFromOnStartStoryEvent(call);
            UnsubscribeFromAllObjectivesOnStartEvents(call);
            UnsubscribeFromAllQuestsOnStartEvents(call);

            //OnComplete
            UnsubscribeFromOnStoryCompletedEvent(call);
            UnsubscribeFromAllObjectivesOnCompleteEvents(call);
            UnsubscribeFromAllQuestsOnCompleteEvents(call);
            SOQET.Debugging.Debug.Log($"Unsubscribed from all {currentStory} story story events");
        }
    }
}