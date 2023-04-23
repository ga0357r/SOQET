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

        currentStory.SetupInternalStoryEvents();
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

    public void SubscribeToQuestEvent(string objectiveName, string questName, UnityAction call)
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

    public void SubscribeToObjectiveEvent(string objectiveName, UnityAction call)
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
}
