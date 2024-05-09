using AmplifyShaderEditor;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    // serialize fields 
    [Header("Movement Variables")]
    [SerializeField, Tooltip("Target max speed")] 
    private float _moveSpeed = 1;
    [SerializeField, Tooltip("Acceleration applied to reach max speed")] 
    private float _acceleration = 5;
    [SerializeField, Tooltip("Multipier on speed when sprinting"), Range(1, 5)] 
    private float _sprintMultiplier = 2;

    [Header("Jumping")]
    [SerializeField, Tooltip("Target jump height")] 
    private float _jumpHeight = 1.5f;
    [SerializeField, Tooltip("Acceleration percentage applied when airborne (prevents wild air movements)")]
    private float _airControl = 0.1f;
    [SerializeField, Tooltip("Can player change direction while airborne")] 
    private bool _airTurning = true;
    [SerializeField, Tooltip("Will gravity be managed by this script, or default unity physics")]
    private bool _useCustomGravity = true;
    [SerializeField, Tooltip("Custom gravity acceleration")]
    private float _gravityValue = -15f;

    [Header("Movement Settings")]
    [SerializeField, Tooltip("Force character to look in move direction")] 
    private bool _lookInMovementDirection = true;
    [SerializeField, Tooltip("Speed at which character faces new target direction")] 
    private float _turnSpeed = 10f;

    [Header("Grounded Raycast Settings")]
    [SerializeField, Tooltip("Height inside character where grounding ray starts")] 
    protected float _groundCheckOffset = 0.1f;
    [SerializeField, Tooltip("Raycast distance, anything inside this will be considered grounded")] 
    protected float _groundCheckDistance = 0.4f;
    [SerializeField, Tooltip("Maximum climbable slope, character will slip on anything higher")] 
    protected float _maxSlopeAngle = 40f;
    [SerializeField, Tooltip("Layer mask(s) considered ground")] 
    protected LayerMask _groundLayerMask;

    [Header("Misc.")]
    [SerializeField] private bool _debugEnabled = false;

    // components
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;

    // private variables
    private float _moveSpeedMultiplier = 1;

    // properties
    public bool CanMove { get; set; } = true;
    public Vector3 MoveInput { get; private set; }
    public Vector3 LocalMoveInput { get; private set; }
    public Vector3 LookDirection { get; private set; }
    public bool HasMoveInput { get; private set; }
    public bool HasTurnInput { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsGrounded { get; private set; }
    public Vector3 GroundNormal { get; private set; } = Vector3.up;
    public float LastGroundedTime { get; private set; }
    public Vector3 LastGroundedPosition { get; private set; }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        // if we handle gravity from here, disable gravity interaction from default unity physics
        _rigidbody.useGravity = !_useCustomGravity;
    }

    // public methods
    public void SetMoveInput(Vector2 moveInput)
    {
        // if player cannot move, zero out the incoming movement input, thus preventing movement
        if (!CanMove)
        {
            MoveInput = Vector2.zero;
        }

        // just in case something happens with inputs, clamp the magnitue of the incoming vector
        moveInput = Vector3.ClampMagnitude(moveInput, 1f);

        // set input to 0 if small incoming value (in case of slight tilt of joystick with controller)
        HasMoveInput = moveInput.magnitude > 0.1f;
        moveInput = HasMoveInput ? moveInput : Vector2.zero;

        // change the move input to vector 3 (for easier calculations), set to XZ plane and normalize, just in case
        moveInput = moveInput.normalized;
        MoveInput = new Vector3(moveInput.x, 0, moveInput.y);

        // in case we need it, create a local direction
        LocalMoveInput = transform.InverseTransformDirection(MoveInput);

        if(_debugEnabled) Debug.DrawRay(transform.position, LocalMoveInput, Color.green);
    }

    public void SetLookDirection(Vector2 lookDirection)
    {
        // if we can't move, or input magnitude is small, return
        if (!CanMove || lookDirection.magnitude < 0.1f)
        {
            HasTurnInput = false;
            return;
        }
        // else, register look direction as input direction
        HasTurnInput = true;
        LookDirection = new Vector3(lookDirection.x, 0f, lookDirection.y).normalized;
    }

    // stop player movement and zero out physics forces
    public void StopPlayer()
    {
        SetMoveInput(Vector3.zero);
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    public void SetIsSprinting(bool isSprinting)
    {
        // set IsSprinting, if yes, then movespeed multiplier equals sprint speed modifier
        IsSprinting = isSprinting;
        _moveSpeedMultiplier = isSprinting ? _sprintMultiplier : 1f;
    }

    // updates
    private void FixedUpdate()
    {
        IsGrounded = CheckGrounded();
        //Set the NavMeshAgent destination using nma.SetDestination.
        if (_navMeshAgent.enabled) _navMeshAgent.SetDestination(transform.position + MoveInput);
        MovePlayer();
        RotatePlayer();
    }
    // private methods
    private void MovePlayer()
    {
        // find flattened movement vector based on ground normal
        Vector3 input = MoveInput;
        Vector3 right = Vector3.Cross(transform.up, input);
        Vector3 forward = Vector3.Cross(right, GroundNormal);

        // calculates target velocity based on max speed and multiliers. If we cannot move, target velocity becomes zero
        Vector3 targetVelocity = forward * (_moveSpeed * _moveSpeedMultiplier);
        if (!CanMove) targetVelocity = Vector3.zero;

        // calculates acceleration required to reach desired velocity and applies air control if not grounded
        Vector3 velocityDiff = targetVelocity - _rigidbody.velocity;
        velocityDiff.y = 0f;
        float control = IsGrounded ? 1f : _airControl;
        Vector3 acceleration = velocityDiff * (_acceleration * control);

        // zeros acceleration if airborne and not trying to move (allows for nice jumping arcs)
        if (!IsGrounded && !HasMoveInput) acceleration = Vector3.zero;

        // add gravity, if needed
        if (_useCustomGravity)
        {
            acceleration += GroundNormal * _gravityValue;
        }

        // add that final force to the rigidbody
        _rigidbody.AddForce(acceleration * _rigidbody.mass);
    }

    private void RotatePlayer()
    {
        // rotates character towards movement direction
        if (_lookInMovementDirection && HasTurnInput && (IsGrounded || _airTurning))
        {
            Quaternion targetRotation = Quaternion.LookRotation(LookDirection);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
            // rotate sprite character properly
            _rigidbody.MoveRotation(rotation);
        }
    }

    private bool CheckGrounded()
    {
        // raycast downwards, looking for ground layer
        Vector3 groundedRaycastStart = transform.position + transform.up * _groundCheckOffset;
        bool hit = Physics.Raycast(groundedRaycastStart, -transform.up, out RaycastHit hitInfo, _groundCheckDistance, _groundLayerMask);

        // set ground surface normal, will get an updated one afetr the raycast in case of slope
        GroundNormal = Vector3.up;

        // if ground wasn't hit, character is not grounded
        if (!hit) return false;

        // test angle between character up and ground, angles above _maxSlopeAngle are invalid
        bool angleValid = Vector3.Angle(transform.up, hitInfo.normal) < _maxSlopeAngle;
        if (angleValid)
        {
            // record last time character was grounded and set correct floor normal direction
            LastGroundedTime = Time.timeSinceLevelLoad;
            GroundNormal = hitInfo.normal;
            LastGroundedPosition = transform.position;
            return true;
        }
        return false;
    }

}


