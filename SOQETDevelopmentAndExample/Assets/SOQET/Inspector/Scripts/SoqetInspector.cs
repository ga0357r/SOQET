using SOQET.Others;
using UnityEngine;
using UnityEngine.Events;
using SOQET.Debugging;

public class SoqetInspector : MonoBehaviour
{
    private static SoqetInspector instance;
    public static SoqetInspector Instance { get => instance; }

    #region Stories

    #region Fields
    [SerializeField] private Story currentStory;
    #endregion

    #region Properties
    public Story CurrentStory => currentStory;
    #endregion

    #endregion

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

        currentStory.Initialize();
    }

    public void OnApplicationQuit()
    {
        currentStory.OnApplicationQuit();
    }

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
        Objective currentObjective = currentStory.GetCurrentObjective();
        
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

    public void SubscribeToAllObjectivesOnCompleteEvents(UnityAction call)
    {
        foreach (Objective objective in currentStory.GetObjectives())
        {
            objective.OnObjectiveCompleted.AddListener(call);
            SOQET.Debugging.Debug.Log($"Subscribed to {objective} objective OnObjectiveCompleted event");
        }
    }
}
