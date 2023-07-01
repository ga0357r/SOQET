using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SoqetInspector soqetInspector;
    private bool horizontalCompleted = false;
    private bool verticalCompleted = false;
    private bool movementTutorialCompleted = false;

    private void OnEnable()
    {
        //subscribe to the Press the directional keys to move quest
        soqetInspector.SubscribeToQuestOnCompleteEvent("Complete the Tutorial", "Press the directional keys to move", PrintLog);
    }

    private void OnDisable() 
    {
        //unsubscribe from the Press the directional keys to move quest
        soqetInspector.UnsubscribeFromQuestOnCompleteEvent("Complete the Tutorial", "Press the directional keys to move", PrintLog);
    }

    private void PrintLog()
    {
        Debug.Log("Quest completed");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Horizontal") > 0f)
        {
            horizontalCompleted = true; 
        }

        if(Input.GetAxis("Vertical") > 0f)
        {
            verticalCompleted = true; 
        }   
        
        if(!movementTutorialCompleted && horizontalCompleted && verticalCompleted) 
        {
            movementTutorialCompleted = true;
            soqetInspector.CompletePlayerQuest("Complete the Tutorial", "Press the directional keys to move");   
        }
    }
}
