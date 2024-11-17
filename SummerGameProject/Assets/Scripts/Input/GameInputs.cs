using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputs : MonoBehaviour
{
    public static GameInputs instance;

    public Vector2 MoveInput { get; private set; }

    public Vector2 LookInput { get; private set; }

    public bool jumpPressed { get; private set; }

    public bool atkPressed { get; private set; }

    public bool lockPressed { get; private set; }

    public bool pausedPressed { get; private set; }

    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction atkAction;
    private InputAction lockAction;
    private InputAction pauseAction;

    //control schemes
    public static string GamepadControlScheme = "Gamepad";
    public static string KeyboardControlScheme = "KeyBoard And Mouse";

    public static string CurrentControlScheme { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerInput = GetComponent<PlayerInput>();

        SetupInputAction();
    }

    private void Update()
    {
        UpdateInputs();
    }

    private void SetupInputAction()
    {
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        atkAction = playerInput.actions["Attack"];
        lockAction = playerInput.actions["Lock In"];
        pauseAction = playerInput.actions["Pause"];
    }

    private void UpdateInputs()
    {
        MoveInput = moveAction.ReadValue<Vector2>();
        LookInput = lookAction.ReadValue<Vector2>();
        jumpPressed = jumpAction.WasPressedThisFrame();
        atkPressed = atkAction.WasPerformedThisFrame();
        lockPressed = lockAction.WasPressedThisFrame();
        pausedPressed = pauseAction.WasPressedThisFrame();
    }

    public void SwitchControls(PlayerInput input)
    {
        CurrentControlScheme = input.currentControlScheme;
        print(CurrentControlScheme);
    }
}
