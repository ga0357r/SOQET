using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SOQET.Others;
using SOQET.Debugging;
using UnityEngine.Events;

namespace SOQET.Inspector
{
    public sealed class SoqetGameGUI : MonoBehaviour
    {
        private SoqetInspector soqetInspector;
        [SerializeField] private TextMeshProUGUI currentObjectiveText,currentQuestText,gameCompletionText;
        [SerializeField] private Slider currentObjectiveProgressSlider;

        private Objective currentObjective;
        private Quest currentQuest;
        [SerializeField] private int keyPressCounter = 1;
        private UnityAction UpdateGUICallback;

        private void OnEnable()
        {
            //subcribe to any story event here
            UpdateGUICallback = ()=>
            {
                GetCurrentObjectiveAndQuest();
                UpdateGUI();
            };

            soqetInspector.SubscribeToAllStoryEvents(UpdateGUICallback);
            soqetInspector.SubscribeToAllObjectivesOnCompleteEvents(ResetKeyPressCounter);
            soqetInspector.SubscribeToOnStoryCompletedEvent(EnableGameCompletionGUI);
        }

        private void Awake() 
        {
            if(soqetInspector == null)
            {
                soqetInspector = FindObjectOfType<SoqetInspector>();
            }
        }

        private void OnDisable() 
        {
            //unsubscribe from any story event here
            soqetInspector.UnsubscribeFromAllStoryEvents(UpdateGUICallback);
            soqetInspector.UnsubscribeFromAllObjectivesOnCompleteEvents(ResetKeyPressCounter);
            soqetInspector.UnsubscribeFromOnStoryCompletedEvent(EnableGameCompletionGUI);
        }

        private void GetCurrentObjectiveAndQuest()
        {
            currentObjective = soqetInspector.CurrentStory.GetCurrentObjectiveObject();
            currentQuest = currentObjective.GetCurrentQuestObject();
        }

        private void UpdateGUI()
        {
            //Update GUI here
            SOQET.Debugging.Debug.Log("Update GUI here");
            ShowCurrentObjectiveAndQuest();
            ShowObjectiveProgress();
        }

        private void ShowCurrentObjectiveAndQuest()
        {
            currentObjectiveText.text = $"Current Objective: {currentObjective.Text}";
            currentQuestText.text = $"Current Quest: {currentQuest.Text}";
        }

        private void ShowObjectiveProgress()
        {
            float currentObjectiveProgress = currentObjective.CalculateObjectiveProgress();
            currentObjectiveProgressSlider.value = currentObjectiveProgress;
        }

        private void Update() 
        {
            if (Input.GetKeyUp(KeyCode.C))
            {
                //increment keypress
                soqetInspector.CompletePlayerQuest(keyPressCounter);
                keyPressCounter++;
            }    
        }

        private void ResetKeyPressCounter()
        {
            keyPressCounter = 0;
        }

        private void EnableGameCompletionGUI()
        {
            gameCompletionText.gameObject.SetActive(true);
        }
    }
}