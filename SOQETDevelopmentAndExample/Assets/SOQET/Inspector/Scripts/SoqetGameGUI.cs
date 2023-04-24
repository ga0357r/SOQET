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

        private Objective currentObjective;
        private Quest currentQuest;

        private void Awake() 
        {
            //get the current quest and objective
            currentObjective = soqetInspector.CurrentStory.GetCurrentObjective();
            SOQET.Debugging.Debug.Log($"CurrentObjective:{currentObjective.name}");
            
            currentQuest = currentObjective.GetCurrentQuest();
            SOQET.Debugging.Debug.Log($"CurrentQuest:{currentQuest.name}");

            //currentObjectiveText.text = soqetInspector.CurrentStory.GetCurrentObjective().Text;
            //currentQuestText.text = soqetInspector.CurrentStory.GetCurrentObjective().GetCurrentQuest().Text;
        }

        private void Start() 
        {
            
        }
    }
}