using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SOQET.Others;
using SOQET.Debugging;

namespace SOQET.Inspector
{
    public class SoqetGameGUI : MonoBehaviour
    {
        [SerializeField] private SoqetInspector soqetInspector;
        [SerializeField] private TextMeshProUGUI currentObjectiveText,currentQuestText;
        [SerializeField] private Slider currentObjectiveProgressSlider;

        [SerializeField] private Objective currentObjective;
        [SerializeField] private Quest currentQuest;
        [SerializeField] private int keyPressCounter = 0;

        private void OnEnable()
        {
            //subcribe to any story event here
            soqetInspector.SubscribeToAllStoryEvents(
                () =>
                {
                    GetCurrentObjectiveAndQuest();
                    UpdateGUI();
                });
        }

        private void OnDisable() 
        {
            //unsubscribe from any story event here
        }

        private void GetCurrentObjectiveAndQuest()
        {
            currentObjective = soqetInspector.CurrentStory.GetCurrentObjective();
            currentQuest = currentObjective.GetCurrentQuest();
        }

        private void Start() 
        {
            

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
                soqetInspector.CompletePlayerQuest(++keyPressCounter);
            }    
        }
    }
}