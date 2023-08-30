using SOQET.Inspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Player Movement / FPS Input")]
public class PlayerMovement : MonoBehaviour {

    //variables to control movement speed
    private float speed = 6;
    private float gravity = -9.8f;

    //to complete tutorial
    private bool horizontalCompleted = false;
    private bool verticalCompleted = false;
    private bool movementTutorialCompleted = false;

    //reference variable
    [SerializeField] private CharacterController _charController;
    [SerializeField] private MouseLook mouseLookX;
    [SerializeField] private MouseLook mouseLookY;
    [SerializeField] private SOQET.Inspector.SoqetInspector soqetInspector;

    private void OnEnable()
    {
        //subscribe to the Press the directional keys to move "On Start Event"
        soqetInspector.SubscribeToQuestOnStartEvent("Complete the Tutorial", "Press the directional keys to move", OnStartMoveTutorial);

        //subscribe to the Press the directional keys to move "On Complete Event"
        soqetInspector.SubscribeToQuestOnCompleteEvent("Complete the Tutorial", "Press the directional keys to move", OnCompleteMoveTutorial);
    }

    private void OnDisable()
    {
        //unsubscribe from the Press the directional keys to move quest to prevent memory leaks
        soqetInspector.UnsubscribeFromQuestOnCompleteEvent("Complete the Tutorial", "Press the directional keys to move", OnCompleteMoveTutorial);

        soqetInspector.UnsubscribeFromQuestOnStartEvent("Complete the Tutorial", "Press the directional keys to move", OnStartMoveTutorial);
    }

    private void OnCompleteMoveTutorial()
    {
        SOQET.Debugging.Debug.Log("On Complete Move Tutorial");
        EnableMouseLook();
    }

    private void OnStartMoveTutorial()
    {
        SOQET.Debugging.Debug.Log("On Start Move Tutorial");
        DisableMouseLook();
    }

    private void DisableMouseLook()
    {
        mouseLookX.Deactivate();
        mouseLookY.Deactivate();
    }

    private void EnableMouseLook()
    {
        mouseLookX.Activate();
        mouseLookY.Activate();
    }

	void FixedUpdate()
    {
        // to handle player movement along the x and z axis
        float moveHorizontal = Input.GetAxis("Horizontal") * speed;
        float moveVertical = Input.GetAxis("Vertical") * speed ;
        Vector3 movement = new Vector3(moveHorizontal  ,0f  ,moveVertical);
        
        // to limit diagonal movement speed to the same speed as movement along an axis 
        movement = Vector3.ClampMagnitude(movement, speed);
        movement.y = gravity;

        // To become Framerate independent
        movement *= Time.deltaTime;

        //to transform the vector from local to global coordinates
        movement = transform.TransformDirection(movement);

        //to move the character
        _charController.Move(movement);

        //complete tutorial
        horizontalCompleted = moveHorizontal > 0f || moveHorizontal < 0f;
        verticalCompleted = moveVertical > 0f || moveVertical < 0f;
        

        if (!movementTutorialCompleted && horizontalCompleted && verticalCompleted)
        {
            movementTutorialCompleted = true;
            soqetInspector.CompletePlayerQuest("Complete the Tutorial", "Press the directional keys to move");
        }
    }

}
