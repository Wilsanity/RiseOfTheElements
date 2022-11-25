using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*using UnityEngine.InputSystem;*/

public class CameraController2 : MonoBehaviour
{
    /*#region Components
    
    GameControls gameControls;
    #region Input Actions

    InputAction gamepadLook;
    InputAction mouseLook;
    InputAction aim;

    #endregion

    GameManager gameManager;

    PlayerInput playerInput;

    #endregion

    #region Inspector

    [Header("Camera")]

    [SerializeField]
    Transform player;

    [SerializeField]
    Vector3 cameraDefaultOffset, cameraOverTheSholderOffset;

    [Header("Input Sensitivity")]

    [SerializeField]
    Vector2 mouseInputSensitivity, gamePadInputSensitivity;
    
    [SerializeField]
    float clampTop, clampBottom;

    #endregion

    #region Variables

    private float lookX, lookY;

    #endregion

    private void Awake()
    {
        //Find Gamemanager
        gameManager = FindObjectOfType<GameManager>();

        //Set up input
        gameControls = new GameControls();
        #region input actions

        mouseLook = gameControls.Camera.MouseLook;
        gamepadLook = gameControls.Camera.GamepadLook;
        aim = gameControls.Camera.Aim;

        #endregion

        //Find input method or device
        playerInput = gameManager.GetComponent<PlayerInput>();

        //Find player transform
        player = gameManager.Player.transform;
    }

    private void OnEnable()
    {
        //enable controls
        gameControls.Enable();

        //cursor set up
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        //disable controls
        gameControls.Disable();

        //cursor set up
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        //Rotate the camera to follow player looking input
        UpdateRotation();
    }

    private void FixedUpdate()
    {
        //Update the camera position to follow the player
        UpdatePosition();
    }

    private void UpdateRotation()
    {
        //find player input for looking
        Vector2 lookDelta = LookDelta();

        //Collect camera's current world rotation on the y axis
        Quaternion newRotation = Camera.main.transform.rotation * Quaternion.Euler(-lookDelta.y, 0, 0);

        *//*  Clamping rotation for the camera is wierd af...
        Becase Unity stores the rotation of a transfrom on the x axis starting at 0 at the horizon and then going up and around to 360,
        we need the clamped rotation angle on the x axis to a value between 0 and 90 or between 270 and 360 degrees. *//*

        //If the rotation is greater than 180 degrees, clamp the rotation between 270 and 360 degrees, else clamp between 0 and 90 degrees. + added a little bit of leeway
        lookX = newRotation.eulerAngles.x > 180 ?
            Mathf.Clamp(newRotation.eulerAngles.x - lookDelta.y, 270 + clampBottom, 360) :
            Mathf.Clamp(newRotation.eulerAngles.x - lookDelta.y, 0, 90 - clampTop);

        //Rotation on the y axis is easy, just add the look delta to the current rotation
        lookY = Camera.main.transform.eulerAngles.y + lookDelta.x;

        //Apply the rotation to the camera
        Camera.main.transform.localEulerAngles = new Vector3(lookX, lookY, 0);
    }

    private Vector2 LookDelta()
    {
        *//*  The values for look input from diffrent devices are well... diffrent.
        Here we determine if the player is using a Mouse of Gamepad for input and 
        return a modified Vecrot 2 to account for the discrepancy.
        
            Plus it'll be easy to add a case in for new input devices later here. *//*

        //Find the current control scheme
        switch (playerInput.currentControlScheme)
        {
            case "Mouse And Keyboard": // If using mouse and keyboard...
                Vector2 inputMouse = mouseLook.ReadValue<Vector2>();
                return new Vector2(inputMouse.x * mouseInputSensitivity.x, inputMouse.y * mouseInputSensitivity.y);
                
            case "Gamepad": // If using gamepad...
                Vector2 inputGamepad = gamepadLook.ReadValue<Vector2>();
                return new Vector2(inputGamepad.x * gamePadInputSensitivity.x, inputGamepad.y * gamePadInputSensitivity.y);
                
            default: // If no valid input device...
                Debug.LogError($"'{gameObject.name}' Control Scheme Not Recognized!");
                return Vector2.zero;
        }
    }
    
    private void UpdatePosition()
    {
        //Find the offset to use for the camera
        Vector3 setCameraOffset = aim.IsPressed() ? cameraOverTheSholderOffset : cameraDefaultOffset;
        Vector3 newCameraPos, offset;

        //If there is a wall detected behind the player that obstructs the camera, move the camera to the wall
        if (Physics.Raycast(player.position, -transform.forward, out RaycastHit hit, Mathf.Abs(setCameraOffset.z)))
        {
            float distance = Vector3.Distance(player.position, hit.point);
            newCameraPos = player.position - Camera.main.transform.forward * (distance - 0.01f);
            offset = hit.normal;
        }
        else // If no wall is detected, move the camera to the pre designated offset
        {
            newCameraPos = player.position - Camera.main.transform.forward * Mathf.Abs(setCameraOffset.z);
            offset = player.transform.rotation * new Vector3(setCameraOffset.x, setCameraOffset.y, 0);
        }

        //Update the camera position
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newCameraPos + offset, Time.deltaTime * 15);
    } 
    
    public Vector3 cameraHorizontalTransForward
    {
        get { return Quaternion.Euler(0, lookY, 0) * new Vector3(0, 0, 1); }
    }

    public Vector3 cameraHorizontalTransRight
    {
        get { return Quaternion.Euler(0, lookY, 0) * new Vector3(1, 0, 0); }
    }

    public Vector3 LookPoint
    {
        get // return the point the camera is looking at
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit)) return hit.point;
            else return Vector3.zero;
        }
    }*/
}
