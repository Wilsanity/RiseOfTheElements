using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayersMetric : MonoBehaviour
{
    [Header("PlayerReferences")]
    private PlayerMovement playerMovement;
    private PlayerController playerController;
    [SerializeField] Rigidbody playerRB;

    [Header("StatText")]
    [SerializeField] TextMeshProUGUI statsText;

    [Header("Variables")]
    public bool getVelocity;


    [Header("PlayerMovementDisplay")]
   
    public bool displayVelocity = true;
    public bool displayIsGrounded = true;
    public bool displayMaxJumpHeight = true;
    public bool displayIsFalling = true;


    [Header("PlayerInputDislay")]
    public bool displayMovementInput = true;
    public bool displayIsJumping = true;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
    }


    private void Update()
    {
        UpdateStatsDisplay();
    }

    private void UpdateStatsDisplay()
    {
     
        string stats = "";

        if (displayMovementInput)
        {
            stats += $"Movement Input: {GetMovementInputText()}\n";
        }
        if (displayVelocity)
        {
            stats += $"Velocity: {GetVelocity()}\n";
        }
        if (displayIsGrounded)
        {
            stats += $"isGrounded: {playerController.CheckForIsGrounded()}\n";
        }
        if (displayMaxJumpHeight)
        {
            stats += $"JumpHeight: {playerMovement._jumpHeight}\n";
        }
        if (displayIsFalling)
        {
            stats += $"IsFalling: {playerController.CheckForIsFalling()}\n";
        }
        if (displayIsJumping)
        {
            stats += $"isJumping: {playerController.CheckForJumping()}\n";
        }
    
        statsText.text = stats;
    }
   
    private  Vector3 GetVelocity()
    {
        return playerRB.velocity;
    }

    private string GetMovementInputText()
    {
        Vector3 movementInput = playerController.GetMovementInput();
        string inputText = "";

        if (movementInput.z > 0)
            inputText += "W";
        else if (movementInput.z < 0)
            inputText += "S";

        if (movementInput.x > 0)
            inputText += "D";
        else if (movementInput.x < 0)
            inputText += "A";

        if (inputText == "")
            inputText = "No Input";

        return inputText;
    }

   

}
