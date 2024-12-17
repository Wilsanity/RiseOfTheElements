using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClimbing : MonoBehaviour
{
    [Header("Climbing Settings")]
    [SerializeField] private LayerMask _climbableLayerMask;
    [SerializeField, Tooltip("How far ahead to check for climbable surface")]
    private float _climbCheckDistance = 0.7f;
    [SerializeField, Tooltip("Climb speed along the wall")]
    private float _climbSpeed = 3f;
    [SerializeField, Tooltip("Turn off gravity when climbing")]
    private bool _disableGravityWhileClimbing = true;

    private PlayerMovement _playerMovement;
    private PlayerAnimationMachine _animationStateMachine;
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;

    private InputAction _climbAction;

    private bool _isClimbing = false;
    private Vector3 _wallNormal = Vector3.zero;

    public bool IsClimbing { get { return _isClimbing; } }

    void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _animationStateMachine = GetComponentInChildren<PlayerAnimationMachine>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();

        if (_playerInput != null && _playerInput.actions != null)
        {
            _climbAction = _playerInput.actions["Climb"];
        }
    }

    private void OnEnable()
    {
        if (_climbAction != null)
        {
            _climbAction.performed += OnClimbPerformed;
            // We will not rely on canceled anymore, so no need to subscribe
            // _climbAction.canceled += OnClimbCanceled;
        }
    }

    private void OnDisable()
    {
        if (_climbAction != null)
        {
            _climbAction.performed -= OnClimbPerformed;
            // _climbAction.canceled -= OnClimbCanceled;
        }
    }

    private void FixedUpdate()
    {
        if (_isClimbing)
        {
            ClimbMovement();
        }
    }

    private void OnClimbPerformed(InputAction.CallbackContext ctx)
    {
        if (!_isClimbing && _playerMovement.CanMove)
        {
            TryStartClimbing();
        }
    }

    // Removed OnClimbCanceled since we no longer want to rely on holding the button.
    // private void OnClimbCanceled(InputAction.CallbackContext ctx)
    // {
    //     if (_isClimbing)
    //     {
    //         ExitClimb();
    //     }
    // }

    private void TryStartClimbing()
    {
        Vector3 startPos = transform.position + Vector3.up * 1f;
        if (Physics.Raycast(startPos, transform.forward, out RaycastHit hit, _climbCheckDistance, _climbableLayerMask))
        {
            _isClimbing = true;
            _wallNormal = hit.normal;

            if (_disableGravityWhileClimbing)
            {
                _rigidbody.useGravity = false;
            }

            _rigidbody.velocity = Vector3.zero;
            _playerMovement.CanMove = false;

           /* if (_animationStateMachine != null)
                _animationStateMachine.UpdatePlayerAnim(PlayerAnimState.IsClimbing, true);*/
        }
    }

    private void ExitClimb()
    {
        _isClimbing = false;

        if (_disableGravityWhileClimbing)
        {
            _rigidbody.useGravity = true;
        }

        _playerMovement.CanMove = true;

       /* if (_animationStateMachine != null)
            _animationStateMachine.UpdatePlayerAnim(PlayerAnimState.IsClimbing, false);*/
    }

    private void ClimbMovement()
    {
        if (!_isClimbing) return;

        // Project movement onto the wall surface
        Vector3 wallUp = Vector3.up;
        Vector3 wallRight = Vector3.Cross(_wallNormal, wallUp).normalized;
        Vector3 wallUpDir = Vector3.Cross(wallRight, _wallNormal).normalized;

        // The player's directions are reversed, so invert them:
        // If pressing forward (Z positive), we want to go UP. If pressing right (X positive), we want to go RIGHT.
        // Currently: climbDirection = (wallRight * input.x + wallUpDir * input.z)
        // To reverse it correctly, let's invert both axes:
        Vector3 input = _playerMovement.MoveInput;
        // Invert input so that pressing forward (positive Z) goes UP, and pressing right (positive X) goes RIGHT as intended.
        // If currently up is down, that means we need to flip the sign on the wallUpDir and wallRight.
        // Let's just multiply input by -1 to correct it:
        input = -input;
        // Now pressing forward (Z positive) will move up the wall, pressing right (X positive) will move right along the wall.

        Vector3 climbDirection = (wallRight * input.x + wallUpDir * input.z) * _climbSpeed;

        _rigidbody.velocity = climbDirection;

        RotateWhileClimbing();

        // Check if we've reached the top of the wall:
        // We'll raycast forward at a slightly higher position to see if there's still climbable surface.
        // If player tries to climb upward (input.z > 0), check if we still have climbable surface ahead.
        if (input.z > 0)
        {
            Vector3 topCheckPos = transform.position + Vector3.up * 1.5f; // check a bit higher
            if (!Physics.Raycast(topCheckPos, transform.forward, _climbCheckDistance, _climbableLayerMask))
            {
                // No more climbable surface ahead, exit climbing
                ExitClimb();
            }
        }
    }

    private void RotateWhileClimbing()
    {
        float turnSpeed = 10f;
        if (_playerMovement != null)
            turnSpeed = _playerMovement._turnSpeed;

        Quaternion targetRotation = Quaternion.LookRotation(-_wallNormal);
        _rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime));
    }
}
