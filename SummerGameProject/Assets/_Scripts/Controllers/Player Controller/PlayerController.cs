
using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour, IDamageable
{
    #region components

    PlayerInput playerInput;
    PlayerAnimationMachine _playerAnimationMachine;
    PlayerAttack _attack;
    PlayerMovement _movement;

    #region input actions

    InputAction moveAction;
    InputAction sprintAction;
    InputAction jumpAction;
    InputAction attackAction;
    #endregion

    Animator anim;
    CapsuleCollider capsule;
    Rigidbody body;

    Transform cameraFollowTargetTransform;

    public float health = 10;

    private NavMeshAgent nma;

    #endregion

    #region inspector

    
    [SerializeField] Image healthBar;


    [SerializeField] float attackDistance = 10f;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] public int attackDamage = 5;
    [SerializeField] LayerMask attackLayer;
    [SerializeField] GameObject hitSpot;

    [SerializeField] private float _dodgeDoubleTapWindow = 0.4f;
    [SerializeField] private float _dodgeCheckThreshold = 0.01f;


    #endregion

    #region variables

    private bool isGrounded;
    private bool jumpOnCoolDown;
    private Vector3 GroundedNormal;
    private IEnumerator jumpCoroutine;

    private bool attacking = false;
    private bool readyToAttack = true;
    private IEnumerator attackCoroutine;
    private Vector2 _moveInputRaw;

    private Coroutine _dodgeDoubleInputWaitCoroutine;
    private bool isJumping; // Check if the player is pressing jump
    private bool isFalling; // Check if the player is falling
    private Vector3 lastGroundedPosition; // Store the position when grounded
    #endregion


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        _playerAnimationMachine = GetComponentInChildren<PlayerAnimationMachine>();
        _movement = GetComponent<PlayerMovement>();
        _attack = GetComponent<PlayerAttack>();
        #region input actions

        //moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];
        attackAction = playerInput.actions["Attack"];

        //We bind our interaction here to allow for different combo attacks depending on press type
        attackAction.started += ctx => Attack();
        jumpAction.performed += ctx => TryJump();
        sprintAction.performed += ctx => SetSprint(true);
        sprintAction.canceled += ctx => SetSprint(false);
        //moveAction.performed += ctx => SetMoveInput(ctx);

        #endregion

        anim = GetComponentInChildren<Animator>();
        nma = GetComponent<NavMeshAgent>();
        capsule = GetComponent<CapsuleCollider>();
        body = GetComponent<Rigidbody>();

        
        cameraFollowTargetTransform = transform.GetChild(0).transform;

        HandlePortalTeleport();
    }

    

    private void OnEnable()
    {
        //single press button input notation. 

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
        //Move();
        ProcessMovementInput();

        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        CheckFalling();
        CheckJumping();



        //Used for combo
        if (inputTimer)
        {
            triggerDelay += Time.deltaTime;
        }
    }

    private void HandlePortalTeleport()
    {
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

    //The original movement Calcs that were in Update
    private void ProcessMovementInput()
    {
        // get input from the playerInput, process it to match camera forward
        Vector3 up = Vector3.up;
        Vector3 right = Camera.main.transform.right;
        Vector3 forward = Vector3.Cross(right, up);
        Vector3 moveInput = forward * _moveInputRaw.y + right * _moveInputRaw.x;

        Debug.DrawRay(transform.position, moveInput, Color.yellow);

        // send player input to character movement for further processing
        _movement.SetMoveInput(moveInput);
        _movement.SetLookDirection(moveInput);
    }

    private void OnMove(InputValue value)
    {
        _moveInputRaw = value.Get<Vector2>();
    }
    private void SetSprint(bool sprintValue)
    {
        _movement.SetIsSprinting(sprintValue);
    }
    private void TryJump()
    {
        _movement.Jump();
    }
    private void OnDodge()
    {
        if (_movement.IsGrounded && (_movement.MoveInput.x > _dodgeCheckThreshold || _movement.MoveInput.x < -_dodgeCheckThreshold))
        {
            //try normal dodge
            // if the player has not begin the pre-dodge coroutine, start it
            if (_movement.CanStartGroundDodge)
            {
                
                _movement.TryGroundDodge(_dodgeDoubleTapWindow);
            }
            // else, send the toggle to turn the dodge into a long dodge
            else if (_movement.IsDodging)
            {
                _movement.QueueLongDodge();
            }
        }
        else
        {
            _movement.TryAirDodge();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "PortalFX_V2")//TEMPORARY CODE: If the player collides with the portal, the cave scene starts.
        {
            SceneTransitionController.Instance.LoadSpecificSceneString("Cave Scene");
            //SceneManager.LoadScene("Cave Scene");
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
        anim.SetTrigger("Hit");
        health -= 1;
        healthBar.fillAmount = health / 10f;
        if (health <= 0)
        {
            OnPlayerDeath();
        }
    }

    public void DealDamage()
    {

    }

    public void ComboCheck()
    {
        if (doDoubleAttack)
        {
            Debug.Log("Double attack");
            anim.SetTrigger("AttackDouble");

            

            doDoubleAttack = false;
            inputTimer = false;
        }
    }

    public void EndAttack()
    {
        Debug.Log("End attack triggered via anim event");


        inputTimer = false;
        triggerDelay = 0.0f;
    }

    private float triggerDelay = 0.0f;
    private bool inputTimer = false;
    private bool doDoubleAttack = false;
    //}
    public void Attack()
    {
        //if not ready to attack or is attacking, return
        //if (!readyToAttack || attacking) return;

        // else set ready to attack false nad attack 
        readyToAttack = false;
        attacking = true;


        //Initiate attack.
        if (!inputTimer)
        {
            Debug.Log("Attack normal triggered");


            anim.SetTrigger("Attack");
            inputTimer = true;
        }
        else
        {

            Debug.Log("Input timer is enabled");

            if (triggerDelay <= 0.5f)
            {
                Debug.Log("Trigger delay is less than 0.5 seconds do double attk");
                doDoubleAttack = true;
            }
        }
        triggerDelay = 0.0f;


        /*

        //Start our timer on first input press
        if (inputTimer == false)
        {
            StartCoroutine(CheckCombo());
        }
        else
        {
            //Our timer is enabled (Within window of combo opportunity.
            //
            StopCoroutine(this.CheckCombo());
            anim.SetTrigger("AttackDouble");

            Invoke(nameof(ResetAttack), attackSpeed);
            AttackRayCast();
        }
        */
    }


    IEnumerator Attacking()
    {
        //if not ready to attack or is attacking, return
        if (!readyToAttack || attacking) yield break;

        // else set ready to attack false nad attack 
        readyToAttack = false;
        attacking = true;

        AttackRayCast();

        //finish attacking and reset the attack with delay attackSpeed
        yield return new WaitForSeconds(attackSpeed);
        
        ResetAttack();
        yield break;
    }

    //Reset 
    void ResetAttack()
    {
        attacking = false; 
        readyToAttack = true;
    }

    //Create a raycast and give damage to the first target hit
    void AttackRayCast()
    {
        //So we don't get an error if we accidentally forget to assign the hitspot
        Transform startOfTransform = hitSpot != null ? hitSpot.transform : transform;
        
        //Using the a gameobject and create a raycast from there
        if(Physics.Raycast(startOfTransform.position , startOfTransform.forward, out RaycastHit hit, attackDistance))
        {
            Debug.Log(hit.transform.gameObject.name);
            
            UnitHealth unitHealth = hit.transform.GetComponent<UnitHealth>();
           
            if (unitHealth == null)
            {
                //try getting their parent if the first one fails
                unitHealth = hit.transform.parent.transform.GetComponent<UnitHealth>();
                if (unitHealth == null) return;
            }

            unitHealth.DamageUnit(1);
        }
        //else if(Physics.Raycast(startOfTransform.position, (startOfTransform.forward + startOfTransform.up), out RaycastHit hitAngled, attackDistance))
        //{
        //    UnitHealth unitHealth = hitAngled.transform.GetComponent<UnitHealth>();

        //    if (unitHealth == null)
        //    {
        //        //try getting their parent if the first one fails
        //        unitHealth = hitAngled.transform.parent.transform.GetComponent<UnitHealth>();
        //        if (unitHealth == null) return;
        //    }

        //    unitHealth.DamageUnit(1);
        //}

        /// Uncomment this if you want to damage bird enemy by only clicking (TESTING ONLY)
        GameObject enemy = GameObject.Find("BeetleEnemy");
        Debug.Log(enemy);
        if (enemy != null) enemy.GetComponent<UnitHealth>().DamageUnit(1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (hitSpot != null)
        {
            Gizmos.DrawLine(hitSpot.transform.position, hitSpot.transform.position + hitSpot.transform.forward * attackDistance);
            //Gizmos.DrawLine(hitSpot.transform.position, hitSpot.transform.position + (hitSpot.transform.forward + hitSpot.transform.up) * attackDistance);
        }

    }

    private void OnPlayerDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Destroy(gameObject);
    }

    // Debug and Stat Check
    public Vector3 GetMovementInput()
    {
        return new Vector3(_moveInputRaw.x, 0, _moveInputRaw.y);
    }

    public bool CheckForIsGrounded()
    {
        return isGrounded;
    }

    public bool CheckForIsFalling()
    {
        return isFalling;
    }

    public bool CheckForJumping()
    {
        return isJumping;
    }

    private void CheckFalling()
    {
        if (!isGrounded)
        {
            // Check if the player has fallen below a certain threshold from last grounded position
            if (transform.position.y < lastGroundedPosition.y - 0.1f || transform.position.y <= _movement._jumpHeight) // Adjust the threshold as needed
            {
                isFalling = true;
            }
            else
            {
                isFalling = false;
            }
        }
        else
        {
            // Update last grounded position when grounded
            lastGroundedPosition = transform.position;
            isFalling = false;
        }
    }

    private void CheckJumping()
    {
        // Check if the jump action is triggered (pressed down)
        if (jumpAction.triggered)
        {
            isJumping = true;
        }

        // Check if the jump action is no longer performed (released)
        if (jumpAction.ReadValue<float>() <= 0)
        {
            isJumping = false;
        }
    }



    // legacy code
    //This is when the player attacks the cave plant enemies. This is a temporary solution since using an array caused them collectively to die
    //when only 1 was killed by the player.
    //private void OnTriggerEnter(Collider other)
    //{

    //   if(other.gameObject.CompareTag("DamageZone") && attackAction.ReadValue<float>() != 0)
    //    {
    //        PlantAIController plantAI = plantEnemy.GetComponent<PlantAIController>();
    //        if (plantAI != null)
    //        {
    //            plantAI.TakeDamage();
    //            Debug.Log("Plant1 Health: " + plantAI.health);
    //        }
    //    }
    //}

    //private void Move()
    //{

    //    //Reads player input as a vector2
    //    Vector2 moveInput = moveAction.ReadValue<Vector2>();
    //    if (sprintAction.ReadValue<float>() != 0) moveInput *= sprintPower;

    //    /*
    //    Because we want the player to move at a consistant speed regardless of the angle of the ground they're walking on.
    //    We find the ground normal and find the cross product for the forward and right vectors.
    //    We then use the cross product to find the direction the player should move in, and we apply our input to that direction.
    //    */

    //    //----------------------old non-navmesh code-------------------------------------------------------------
    //    //Find the new forward and right vectors
    //    //Vector3 forward = Vector3.Cross(GroundedNormal, cameraFollowTargetTransform.right);
    //    //Vector3 right = Vector3.Cross(GroundedNormal, cameraFollowTargetTransform.forward);

    //    ////Apply the input to the new forward and right vectors and use those values as the Rigidbodies velocity
    //    //Vector3 moveDirection = forward * -moveInput.y + right * moveInput.x;
    //    //----------------------old non-navmesh code--------------------------------------------------------------


    //    Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
    //    moveDirection = cameraFollowTargetTransform.TransformDirection(moveDirection);
    //    moveDirection.y = 0f;

    //    //Ensures that the NavMeshAgent is enabled before setting its destination.
    //    //Set the NavMeshAgent destination using nma.SetDestination.
    //    if (nma.enabled) nma.SetDestination(transform.position + moveDirection);

    //    //Rotate the player to face forward
    //    Quaternion targetRotation = moveDirection != Vector3.zero ? Quaternion.LookRotation(moveDirection, Vector3.up) : transform.rotation;

    //    if (moveInput.magnitude >= 0.3)
    //    {
    //        PlayerAnimationMachine.UpdatePlayerAnim(PlayerAnimState.IsMoving, true, anim);
    //        PlayerAnimationMachine.UpdatePlayerAnim(PlayerAnimState.IsSprinting, sprintAction.ReadValue<float>() != 0, anim);
    //        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    //    }
    //    else
    //    {
    //        PlayerAnimationMachine.UpdatePlayerAnim(PlayerAnimState.IsMoving, false, anim);
    //    }

    //    if (isGrounded) body.velocity = Vector3.Lerp(body.velocity, moveDirection * moveSpeed, Time.deltaTime * 6f);
    //    else body.velocity = Vector3.Lerp(body.velocity, moveDirection * moveSpeed, Time.deltaTime * 1f);
    //}

    //private IEnumerator Jump()
    //{
    //    //if player is either not grounded or the jump is still on a cool down, stop the coroutine.
    //    if (!isGrounded || jumpOnCoolDown) yield break;

    //    jumpOnCoolDown = true;

    //    isGrounded = false;

    //    //Stop navmeshagent to ensure a controlled jump.
    //    nma.velocity = Vector3.zero;

    //    //Calculate the jump direction based on current ground normal.
    //    Vector3 jumpDirection = GroundedNormal + Vector3.up;

    //    //Set nma's velocity for a jump;
    //    nma.velocity = jumpDirection * jumpPower;

    //    //----------------------old non-navmesh code--------------------------------------------------------------
    //    //Vector3 vertical = new Vector3(0.0f, body.velocity.y, 0.0f);
    //    //Vector3 horizontal = new Vector3(body.velocity.x, 0.0f, body.velocity.z);
    //    //body.velocity = (horizontal + (vertical * 0.1f));
    //    //body.AddForce(horizontal * 10, ForceMode.Force); //Jumping while moving gives a slight boost in your current direction.
    //    //body.AddForce(GroundedNormal * jumpPower * 75, ForceMode.Force); //Pushes off the ground, using the normal of the collision surface.
    //    //body.AddForce(Vector3.up * jumpPower * 25, ForceMode.Force);
    //    //----------------------old non-navmesh code--------------------------------------------------------------

    //    //Wait for the jump to cool down
    //    yield return new WaitForSeconds(0.1f);

    //    jumpOnCoolDown = false;


}
