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
    InputAction sprintAction;
    InputAction jumpAction;
    #endregion

    Rigidbody body;
    CapsuleCollider capsule;

    Transform cameraFollowTargetTransform;

    #endregion

    #region inspector

    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float sprintPower;

    #endregion

    #region variables

    bool isGrounded;
    Vector3 GroundedNormal;


    const int initialTimer = 10;
    int timer = 10;
    bool countdown;
    #endregion

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        #region input actions

        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];

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
        //  isGrounded = false;
    }
    private void FixedUpdate()
    {
        if (countdown)
        {
            timer--;
        }
        else
        {
            timer = initialTimer;
        }
        if (timer <= 0)
        {
            countdown = false;
        }

    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GroundedNormal = collision.GetContact(0).normal;
            isGrounded = true;
        }

        // Debug-draw all contact points and normals
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
                    }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }
    private void Move()
    {

        //Reads player input as a vector2
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        bool sprintInput = sprintAction.ReadValue<float>() != 0;
        if (sprintInput) moveInput *= sprintPower;

        bool jumpInput = jumpAction.ReadValue<float>() != 0;
        /*
        Because we want the player to move at a consistant speed regardless of the angle of the ground they're walking on.
        We fire a raycast to find the ground normal and find the cross product for the forward and right vectors.
        We then use the cross product to find the direction the player should move in, and we apply our input to that direction.
        */
        //Rotate the player to face forward
        Quaternion targetRotation = Quaternion.Euler(0, cameraFollowTargetTransform.eulerAngles.y, 0);
        if (moveInput.magnitude >= 0.3) transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        //If raycast detects a surface...



        //Find the new forward and right vectors
        Vector3 forward = Vector3.Cross(GroundedNormal, transform.right);
        Vector3 right = Vector3.Cross(GroundedNormal, transform.forward);

        //Apply the input to the new forward and right vectors and use those values as the Rigidbodies velocity
        Vector3 moveDirection = forward * -moveInput.y + right * moveInput.x;

        if (isGrounded) body.velocity = Vector3.Lerp(body.velocity, moveDirection * moveSpeed, Time.deltaTime * 6f);
        else body.velocity = Vector3.Lerp(body.velocity, moveDirection * moveSpeed, Time.deltaTime * 1f);


        if (jumpInput && isGrounded && !countdown) Jump();

    }

    private void Jump()
    {
        countdown = true;
        isGrounded = false;
        Vector3 vertical = new Vector3(0.0f, body.velocity.y, 0.0f);
        Vector3 horizontal = new Vector3(body.velocity.x, 0.0f, body.velocity.z);
        body.velocity = (horizontal + (vertical * 0.1f));
        body.AddForce(horizontal * 10, ForceMode.Force);//Jumping while moving gives a slight boost in your current direction.
        body.AddForce(GroundedNormal * jumpPower * 75, ForceMode.Force);//Pushes off the ground, using the normal of the collision surface.
        body.AddForce(Vector3.up * jumpPower * 25, ForceMode.Force);
    }
}
