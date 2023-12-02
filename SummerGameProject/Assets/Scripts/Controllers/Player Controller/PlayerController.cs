using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region components

    PlayerInput playerInput;
    #region input actions

    InputAction moveAction;
    InputAction sprintAction;
    InputAction jumpAction;
    #endregion

    Animator anim;
    CapsuleCollider capsule;
    Rigidbody body;

    Transform cameraFollowTargetTransform;

    public float health = 10;

    private NavMeshAgent nma;

    #endregion

    #region inspector

    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float sprintPower;

    #endregion

    #region variables

    bool isGrounded;
    bool jumpOnCoolDown;
    
    Vector3 GroundedNormal;

    private GameObject plantEnemy;

    #endregion

    void Start()
    {
        plantEnemy = GameObject.FindGameObjectWithTag("PlantEnemy");
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        #region input actions

        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];

        #endregion

        anim = GetComponentInChildren<Animator>();
        nma = GetComponent<NavMeshAgent>();
        capsule = GetComponent<CapsuleCollider>();
        body = GetComponent<Rigidbody>();

        cameraFollowTargetTransform = transform.GetChild(0).transform;

        //if a portal was used to telleport
        if (PlayerPrefs.GetInt("isPortalUsed", 0) == 1)
        {
            //Find the name of the portal that was used
            string currentPortal = PlayerPrefs.GetString("currentPortal");
            if (currentPortal != null)
            {
                //move the player to the portal's spawn position
                transform.position = GameObject.Find(currentPortal).transform.GetChild(0).position;
                PlayerPrefs.SetInt("isPortalUsed", 0);
            }
        }
    }

    private void OnEnable()
    {
        //single press button input notation. 
        #region input actions

        jumpAction.performed += ctx => StartCoroutine(Jump());

        #endregion

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

        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    public void TakeDamage()
    {
        health -= 1;
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("DamageZone"))
        {
            PlantAIController plantAI = plantEnemy.GetComponent<PlantAIController>();
            if (plantAI != null)
            {
                plantAI.TakeDamage();
                Debug.Log("Plant Damaged");
            }
        }
    }

    private void Move()
    {

        //Reads player input as a vector2
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        if (sprintAction.ReadValue<float>() != 0) moveInput *= sprintPower;

        /*
        Because we want the player to move at a consistant speed regardless of the angle of the ground they're walking on.
        We find the ground normal and find the cross product for the forward and right vectors.
        We then use the cross product to find the direction the player should move in, and we apply our input to that direction.
        */

        //----------------------old non-navmesh code-------------------------------------------------------------
        //Find the new forward and right vectors
        //Vector3 forward = Vector3.Cross(GroundedNormal, cameraFollowTargetTransform.right);
        //Vector3 right = Vector3.Cross(GroundedNormal, cameraFollowTargetTransform.forward);

        ////Apply the input to the new forward and right vectors and use those values as the Rigidbodies velocity
        //Vector3 moveDirection = forward * -moveInput.y + right * moveInput.x;
        //----------------------old non-navmesh code--------------------------------------------------------------


        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        moveDirection = cameraFollowTargetTransform.TransformDirection(moveDirection);
        moveDirection.y = 0f;

        //Ensures that the NavMeshAgent is enabled before setting its destination.
        //Set the NavMeshAgent destination using nma.SetDestination.
        if (nma.enabled) nma.SetDestination(transform.position + moveDirection);

        //Rotate the player to face forward
        Quaternion targetRotation = moveDirection != Vector3.zero ? Quaternion.LookRotation(moveDirection, Vector3.up) : transform.rotation;
        
        if (moveInput.magnitude >= 0.3)
        {
            PlayerAnimationMachine.UpdatePlayerAnim(PlayerAnimState.IsMoving, true, anim);
            PlayerAnimationMachine.UpdatePlayerAnim(PlayerAnimState.IsSprinting, sprintAction.ReadValue<float>() != 0, anim);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            PlayerAnimationMachine.UpdatePlayerAnim(PlayerAnimState.IsMoving, false, anim);
        }

        if (isGrounded) body.velocity = Vector3.Lerp(body.velocity, moveDirection * moveSpeed, Time.deltaTime * 6f);
        else body.velocity = Vector3.Lerp(body.velocity, moveDirection * moveSpeed, Time.deltaTime * 1f);
    }

    private IEnumerator Jump()
    {
        //if player is either not grounded or the jump is still on a cool down, stop the coroutine.
        if (!isGrounded || jumpOnCoolDown) yield break;

        jumpOnCoolDown = true;

        isGrounded = false;

        //Stop navmeshagent to ensure a controlled jump.
        nma.velocity = Vector3.zero;

        //Calculate the jump direction based on current ground normal.
        Vector3 jumpDirection = GroundedNormal + Vector3.up;

        //Set nma's velocity for a jump;
        nma.velocity = jumpDirection * jumpPower;

        //----------------------old non-navmesh code--------------------------------------------------------------
        //Vector3 vertical = new Vector3(0.0f, body.velocity.y, 0.0f);
        //Vector3 horizontal = new Vector3(body.velocity.x, 0.0f, body.velocity.z);
        //body.velocity = (horizontal + (vertical * 0.1f));
        //body.AddForce(horizontal * 10, ForceMode.Force); //Jumping while moving gives a slight boost in your current direction.
        //body.AddForce(GroundedNormal * jumpPower * 75, ForceMode.Force); //Pushes off the ground, using the normal of the collision surface.
        //body.AddForce(Vector3.up * jumpPower * 25, ForceMode.Force);
        //----------------------old non-navmesh code--------------------------------------------------------------

        //Wait for the jump to cool down
        yield return new WaitForSeconds(0.1f);

        jumpOnCoolDown = false;
    }
}
