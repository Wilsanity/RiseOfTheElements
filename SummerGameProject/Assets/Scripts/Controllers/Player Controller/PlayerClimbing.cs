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

    [Header("Ledge Settings")]
    [SerializeField] private float _ledgeCheckHeight = 1.5f; // Height to check for ledge
    [SerializeField] private float _ledgeGrabOffset = 0.5f;  // Offset for grabbing ledge
    [SerializeField] private float _ledgeMoveSpeed = 2f;
    [SerializeField] private float _ledgeClimbUpSpeed = 3f;

    private PlayerMovement _playerMovement;
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;

    private InputAction _climbAction;
    private InputAction _jumpAction;

    private bool _isClimbing = false;
    private bool _isHanging = false;  // New state for ledge holding
    private Vector3 _wallNormal = Vector3.zero;
    private Vector3 _ledgePosition; // Position where the player grabs the ledge

    public bool IsClimbing { get { return _isClimbing; } }
    public bool IsHanging { get { return _isHanging; } }

    void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();

        if (_playerInput != null && _playerInput.actions != null)
        {
            _climbAction = _playerInput.actions["Climb"];
            _jumpAction = _playerInput.actions["Jump"];
        }
    }

    private void OnEnable()
    {
        if (_climbAction != null)
        {
            _climbAction.performed += OnClimbPerformed;
        }
        if (_jumpAction != null)
        {
            _jumpAction.performed += OnJumpPerformed;
        }
    }

    private void OnDisable()
    {
        if (_climbAction != null)
        {
            _climbAction.performed -= OnClimbPerformed;
        }
        if (_jumpAction != null)
        {
            _jumpAction.performed -= OnJumpPerformed;
        }
    }

    private void FixedUpdate()
    {
        if (_isClimbing)
        {
            ClimbMovement();
        }
        else if (_isHanging)
        {
            LedgeMovement();
        }
    }

    private void OnClimbPerformed(InputAction.CallbackContext ctx)
    {
        if (!_isClimbing && _playerMovement.CanMove)
        {
            TryStartClimbing();
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (_isHanging)
        {
            // Climb up from the ledge
            ClimbUpLedge();
        }
        else if (_isClimbing)
        {
            // Fall off the wall
            ExitClimb();
        }

    }

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
        }
    }

    private void ExitClimb()
    {
        _isClimbing = false;
        _isHanging = false;

        // Restore normal physics movement
        _rigidbody.useGravity = true;
        _rigidbody.constraints = RigidbodyConstraints.None;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation; // Allow normal movement

        _playerMovement.CanMove = true;
    }

    private void ClimbMovement()
    {
        if (!_isClimbing) return;

        Vector3 wallUp = Vector3.up;
        Vector3 wallRight = Vector3.Cross(_wallNormal, wallUp).normalized;
        Vector3 wallUpDir = Vector3.Cross(wallRight, _wallNormal).normalized;

        Vector3 input = -_playerMovement.MoveInput;
        Vector3 climbDirection = (wallRight * input.x + wallUpDir * input.z) * _climbSpeed;

        _rigidbody.velocity = climbDirection;
        RotateWhileClimbing();

        if (input.z > 0)
        {
            Vector3 topCheckPos = transform.position + Vector3.up * _ledgeCheckHeight;
            if (!Physics.Raycast(topCheckPos, transform.forward, _climbCheckDistance, _climbableLayerMask))
            {
                EnterLedgeGrab();
            }
        }
    }

    private void EnterLedgeGrab()
    {
        _isClimbing = false;
        _isHanging = true;

        // Completely stop physics movement
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.useGravity = false;

        // Freeze Rigidbody Y-movement to prevent slipping
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        // Snap player to ledge position to avoid sliding
        _ledgePosition = transform.position + Vector3.up * _ledgeGrabOffset;
        transform.position = _ledgePosition;
    }

    private void LedgeMovement()
    {
        if (!_isHanging) return;

        Vector3 wallRight = Vector3.Cross(Vector3.up, _wallNormal).normalized;
        Vector3 input = _playerMovement.MoveInput;

        Vector3 ledgeMoveDirection = wallRight * input.x * _ledgeMoveSpeed;

        // Move along the ledge
        Vector3 newPosition = transform.position + ledgeMoveDirection * Time.fixedDeltaTime;

        // Ensure we don't move in Y-axis while hanging
        newPosition.y = _ledgePosition.y;

        transform.position = newPosition;

        // Update the ledge position to prevent teleportation
        _ledgePosition = newPosition;
    }

    private void ClimbUpLedge()
    {
        if (!_isHanging) return;

        _isHanging = false;
        StartCoroutine(ClimbLedgeCoroutine());
    }

    private IEnumerator ClimbLedgeCoroutine()
    {
        Vector3 climbTarget = _ledgePosition + Vector3.up * 1f + transform.forward * 0.5f;
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(transform.position, climbTarget, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = climbTarget;
        ExitClimb();
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
