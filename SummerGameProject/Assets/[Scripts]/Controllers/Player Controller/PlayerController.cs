using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region components

    PlayerInput playerInput;
    #region input actions

    InputAction moveAction;

    #endregion

    Rigidbody body;
    CapsuleCollider capsule;

    Transform cameraFollowTargetTransform;

    #endregion

    #region inspector

    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;

    #endregion

    #region variables
    #endregion

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        #region input actions
        
        moveAction = playerInput.actions["Move"];

        #endregion

        body = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        cameraFollowTargetTransform = transform.GetChild(0).transform;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        //Reads player input as a vector2
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        /*
        Because we want the player to move at a consistant speed regardless of the angle of the ground they're walking on.
        We fire a raycast to find the ground normal and find the cross product for the forward and right vectors.
        We then use the cross product to find the direction the player should move in, and we apply our input to that direction.
        */
        
        //If raycast detects a surface...
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5f))
        {
            //Rotate the player to face forward
            Quaternion targetRotation = Quaternion.Euler(0, cameraFollowTargetTransform.eulerAngles.y, 0);
            if (moveInput.magnitude != 0) transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            //Find the new forward and right vectors
            Vector3 forward = Vector3.Cross(hit.normal, transform.right);
            Vector3 right = Vector3.Cross(hit.normal, transform.forward);

            //Apply the input to the new forward and right vectors and use those values as the Rigidbodies velocity
            Vector3 moveDirection = forward * -moveInput.y + right * moveInput.x;
            body.velocity = Vector3.Lerp(body.velocity, moveDirection * moveSpeed, Time.deltaTime * 10f);
        }
        else
        {
            //else add a force in the movement direction. NOT COMPLETE
            body.AddForce(moveInput, ForceMode.Force);
        }
    }
}
