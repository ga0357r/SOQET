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
        /// Complete Current Quest in Current Objective. All subscribers are automatically notified.
        /// </summary>
        /// <param name="objectiveName"> Current Objective Name</param>
        /// <param name="questName">Current Quest Name</param>
        public void CompletePlayerQuest(string objectiveName, string questName)
        {
            Objective objective = CurrentStory.GetObjective(objectiveName);
            Objective currentObjective = CurrentStory.GetCurrentObjectiveObject();

            if (objective && !objective.IsCompleted)
            {
                if (currentObjective && objective.name != currentObjective.name)
                {
                    SOQET.Debugging.Debug.LogError($"SOQET currently only supports linear storytelling");
                    return;
                }

                Quest quest = objective.GetQuest(questName);
                Quest currentQuest = currentObjective.GetCurrentQuestObject();

                if (quest && !quest.IsCompleted)
                {
                    if (currentQuest && quest.name != currentQuest.name)
                    {
                        SOQET.Debugging.Debug.LogError($"SOQET currently only supports linear storytelling");
                        return;
                    }

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

        /// <summary>
        /// Complete Current Quest from Current Objective. All subscribers are automatically notified.
        /// </summary>
        /// <param name="questNumber"> Current Quest Number</param>
        public void CompletePlayerQuest(int questNumber)
        {
            Objective currentObjective = currentStory.GetCurrentObjectiveObject();

            if (currentObjective && !currentObjective.IsCompleted)
            {
                Quest quest = currentObjective.GetQuest(questNumber);
                Quest currentQuest = currentObjective.GetCurrentQuestObject();

                if (quest && !quest.IsCompleted)
                {
                    if (currentQuest && quest.name != currentQuest.name)
                    {
                        SOQET.Debugging.Debug.LogError($"SOQET currently only supports linear storytelling");
                        return;
                    }

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

        /// <summary>
        /// Subscribe to a quest's OnCompleteEvent
        /// </summary>
        /// <param name="objectiveName">Quest is in which objective?</param>
        /// <param name="questName"> Which quest?</param>
        /// <param name="call"> Run this method after quest is completed</param>
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

        /// <summary>
        /// Unsubscribe from a quest's OnCompleteEvent. Not Unsubscribing can cause memory problems
        /// </summary>
        /// <param name="objectiveName">Quest is in which objective?</param>
        /// <param name="questName">Which quest?</param>
        /// <param name="call">Remove this method</param>
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

        /// <summary>
        /// Subscribe to a quest's OnStartEvent
        /// </summary>
        /// <param name="objectiveName">Quest is in which objective?</param>
        /// <param name="questName">Which quest?</param>
        /// <param name="call">Run this method after quest is started</param>
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

        /// <summary>
        /// Unsubscribe from a quest's OnStartEvent. Not Unsubscribing can cause memory problems
        /// </summary>
        /// <param name="objectiveName">Quest is in which objective?</param>
        /// <param name="questName">Which quest?</param>
        /// <param name="call">Remove this method</param>
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

        /// <summary>
        /// Subscribe to an objective's OnCompleteEvent
        /// </summary>
        /// <param name="objectiveName">Which objective?</param>
        /// <param name="call">Run this method after objective is completed</param>
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

        /// <summary>
        /// Unsubscribe from an objective's OnCompleteEvent. Not Unsubscribing can cause memory problems
        /// </summary>
        /// <param name="objectiveName">which objective?</param>
        /// <param name="call">Remove this method</param>
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

        /// <summary>
        /// Subscribe to an objective's OnStartEvent
        /// </summary>
        /// <param name="objectiveName">which objective?</param>
        /// <param name="call">Run this method after objective is started</param>
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

        /// <summary>
        /// Unsubscribe from a quest's OnCompleteEvent. Not Unsubscribing can cause memory problems
        /// </summary>
        /// <param name="objectiveName">which objective?</param>
        /// <param name="call">Remove this method</param>
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

        /// <summary>
        /// Subscribe to all quest OnCompleteEvents
        /// </summary>
        /// <param name="call">Run this method after each quest is completed</param>
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

        /// <summary>
        /// Unsubscribe from all quests OnCompleteEvents. Not Unsubscribing can cause memory problems
        /// </summary>
        /// <param name="call">Remove this method</param>
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

        /// <summary>
        /// Subscribe to all quests OnStartEvents
        /// </summary>
        /// <param name="call">Run this method after each quest is started</param>
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

        /// <summary>
        /// Unsubscribe from all quests OnStartEvents. Not Unsubscribing can cause memory problems
        /// </summary>
        /// <param name="call">Remove this method</param>
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

        /// <summary>
        /// Subscribe to all objective? OnCompleteEvents
        /// </summary>
        /// <param name="call">Run this method after each objective? is completed</param>
        public void SubscribeToAllObjectivesOnCompleteEvents(UnityAction call)
        {
            foreach (Objective objective in currentStory.GetObjectives())
            {
                objective.OnObjectiveCompleted.AddListener(call);
                SOQET.Debugging.Debug.Log($"Subscribed to {objective} objective OnObjectiveCompleted event");
            }
        }

        /// <summary>
        /// Unsubscribe from all objective OnCompleteEvents. Not Unsubscribing can cause memory problems
        /// </summary>
        /// <param name="call">Remove this method</param>
        public void UnsubscribeFromAllObjectivesOnCompleteEvents(UnityAction call)
        {
            foreach (Objective objective in currentStory.GetObjectives())
            {
                objective.OnObjectiveCompleted.RemoveListener(call);
                SOQET.Debugging.Debug.Log($"Unsubscribed from {objective} objective OnObjectiveCompleted event");
            }
        }

        /// <summary>
        /// Subscribe to all objective OnStartEvents
        /// </summary>
        /// <param name="call">Run this method after each objective is started</param>
        public void SubscribeToAllObjectivesOnStartEvents(UnityAction call)
        {
            foreach (Objective objective in currentStory.GetObjectives())
            {
                objective.OnStartObjective.AddListener(call);
                SOQET.Debugging.Debug.Log($"Subscribed to {objective} objective OnStartObjective event");
            }
        }

        /// <summary>
        /// Unsubscribe from all objective OnStartEvents. Not Unsubscribing can cause memory problems
        /// </summary>
        /// <param name="call">Remove this method</param>
        public void UnsubscribeFromAllObjectivesOnStartEvents(UnityAction call)
        {
            foreach (Objective objective in currentStory.GetObjectives())
            {
                objective.OnStartObjective.RemoveListener(call);
                SOQET.Debugging.Debug.Log($"Unsubscribed from {objective} objective OnStartObjective event");
            }
        }

        /// <summary>
        /// Subscribe to current story OnStartEvent
        /// </summary>
        /// <param name="call">Run this method after story is started</param>
        public void SubscribeToOnStartStoryEvent(UnityAction call)
        {
            currentStory.OnStartStory.AddListener(call);
            SOQET.Debugging.Debug.Log($"Subscribed to {currentStory} story OnStartStory event");
        }

        /// <summary>
        /// Unsubscribe from current story OnStartEvent. Not Unsubscribing can cause memory problems
        /// </summary>
        /// <param name="call">Remove this method</param>
        public void UnsubscribeFromOnStartStoryEvent(UnityAction call)
        {
            currentStory.OnStartStory.RemoveListener(call);
            SOQET.Debugging.Debug.Log($"Unsubscribed from {currentStory} story OnStartStory event");
        }

        /// <summary>
        /// Subscribe to all story OnCompleteEvent
        /// </summary>
        /// <param name="call">Run this method after story is completed</param>
        public void SubscribeToOnStoryCompletedEvent(UnityAction call)
        {
            currentStory.OnStoryCompleted.AddListener(call);
            SOQET.Debugging.Debug.Log($"Subscribed to {currentStory} story OnStoryCompleted event");
        }

        /// <summary>
        /// Unsubscribe from story OnCompleteEvent. Not Unsubscribing can cause memory problems
        /// </summary>
        /// <param name="call">Remove this method</param>
        public void UnsubscribeFromOnStoryCompletedEvent(UnityAction call)
        {
            currentStory.OnStoryCompleted.RemoveListener(call);
            SOQET.Debugging.Debug.Log($"Unsubscribed from {currentStory} story OnStoryCompleted event");
        }

        /// <summary>
        /// Subscribe to all OnCompleteEvents and OnStartEvents
        /// </summary>
        /// <param name="call">Run this method after each quest,objective,story is completed</param>
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

        /// <summary>
        /// Subscribe from all OnCompleteEvents and OnStartEvents. Not Unsubscribing can cause memory problems
        /// </summary>
        /// <param name="call">Remove this method</param>
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