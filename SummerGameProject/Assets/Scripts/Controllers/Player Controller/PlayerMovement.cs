using AmplifyShaderEditor;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [Header("Plug in Fields")]
    [SerializeField] private CinemachineFreeLook _cameraFreeLook;
    // serialized fields 
    [Header("Movement Variables")]
    [SerializeField, Tooltip("Target max speed")] 
    private float _moveSpeed = 1;
    [SerializeField, Tooltip("Acceleration applied to reach max speed")] 
    private float _acceleration = 5;
    [SerializeField, Tooltip("Multipier on speed when sprinting"), Range(1, 5)] 
    private float _sprintMultiplier = 2;

    [Header("Jumping")]
    [SerializeField, Tooltip("Target jump height")] 
    public float _jumpHeight = 1.5f;
    [SerializeField, Tooltip("Acceleration percentage applied when airborne (prevents wild air movements)")]
    private float _airControl = 0.1f;
    [SerializeField, Tooltip("Can player change direction while airborne")] 
    private bool _airTurning = true;
    [SerializeField, Tooltip("Will gravity be managed by this script, or default unity physics")]
    private bool _useCustomGravity = true;
    [SerializeField, Tooltip("Custom gravity acceleration")]
    private float _gravityValue = -15f;

    [Header("Dodge")]
    [SerializeField] private float _shortDodgeDistance = 1;
    [SerializeField] private float _shortDodgeTime = 0.5f;
    [SerializeField, Tooltip("Long dodge will cover x more distnace in x more time, to keep speed constant between dodges")] 
    private float _longDodgeMultiplier = 2;
    [SerializeField] private float _dodgeCooldown = 0.5f;
    [SerializeField] private float _airDodgeSpeedModifier = 5;
    [SerializeField] private float _airDodgeAccelerationModifier = 5;

    [Header("Wolf Run")]
    [SerializeField] private bool _isWolfRunning = false;
    [SerializeField, Tooltip("Wolf run speed multiplier off the BASE movement speed (overwrites sprint speed, does not multiply it)")] 
    private float _wolfRunSpeedMultiplier = 4;
    [SerializeField, Tooltip("Sprint time required before wolf run kicks in")]
    private float _wolfRunStartUpTime = 3;
    [SerializeField, Tooltip("Max turn speed in degrees per second while wolf running")]
    private float _wolfRunTurnSpeed = 10;
    [SerializeField, Tooltip("Multiplier to jump height while wolf running")]
    private float _wolfRunJumpHeightMultiplier = 2;
    [SerializeField, Tooltip("Minimum speed that must be mantained for wolf run to be valid")]
    private float _wolfRunMinSpeed = 5;
    [SerializeField, Tooltip("Camera FOV while on wolf run")]
    private float _wolfRunCameraFOV = 60;
    [SerializeField, Tooltip("Camera FOV Lerp back duration")]
    private float _FOVLerpBackDuration = 0.5f;

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
    private PlayerAnimationMachine _animationStateMachine;

    // private variables
    private float _moveSpeedMultiplier = 1;
    private bool _isDodging = false;
    public bool _canDodgedThisJump = false;
    private bool _doingActualDodge;
    private float _lastDodgeTime = 0;
    private float _dodgeSpeed;
    private Vector3 _dodgeLockedDirection;
    private bool _goIntoLongDodge;
    private Coroutine _wolfRunWarmUp;
    private float _cameraFOVDefaultValue = 0;

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
    public bool CanStartGroundDodge { get { return IsGrounded && !_isDodging && _lastDodgeTime + _dodgeCooldown <= Time.time && !_isWolfRunning; } }
    public bool CanAirDodge { get { return _canDodgedThisJump && !IsGrounded; } }
    public bool IsDodging {  get { return _isDodging;} }
    public bool IsWolfRunning { get { return _isWolfRunning;} }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animationStateMachine = GetComponentInChildren<PlayerAnimationMachine>();
        _cameraFOVDefaultValue = _cameraFreeLook.m_Lens.FieldOfView;

        // if we handle gravity from here, disable gravity interaction from default unity physics
        _rigidbody.useGravity = !_useCustomGravity;
    }

    // public methods
    public void SetMoveInput(Vector3 moveInput)
    {
        // if player cannot move, zero out the incoming movement input, thus preventing movement
        if (!CanMove)
        {
            MoveInput = Vector3.zero;
        }

        // just in case something happens with inputs, clamp the magnitue of the incoming vector
        moveInput = Vector3.ClampMagnitude(moveInput, 1f);

        // set input to 0 if small incoming value (in case of slight tilt of joystick with controller)
        HasMoveInput = moveInput.magnitude > 0.1f;
        moveInput = HasMoveInput ? moveInput : Vector3.zero;

        // change the move input to vector 3 (for easier calculations), set to XZ plane and normalize, just in case
        moveInput = moveInput.normalized;
        MoveInput = new Vector3(moveInput.x, 0, moveInput.z);

        // in case we need it, create a local direction
        LocalMoveInput = transform.InverseTransformDirection(MoveInput);

        //sprint and wolf run checks
        if (IsSprinting && !_isWolfRunning && _wolfRunWarmUp == null && HasMoveInput && _rigidbody.velocity.magnitude > _wolfRunMinSpeed/2)
        {
            _wolfRunWarmUp = StartCoroutine(WolfRunTimer());
        }

        if (_debugEnabled) Debug.DrawRay(transform.position, MoveInput, Color.green);
    }

    public void SetLookDirection(Vector3 lookDirection)
    {
        // if we can't move, or input magnitude is small, return
        if (!CanMove || lookDirection.magnitude < 0.1f)
        {
            HasTurnInput = false;
            return;
        }
        // else, register look direction as input direction
        HasTurnInput = true;
        LookDirection = new Vector3(lookDirection.x, 0f, lookDirection.z).normalized;
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
        _moveSpeedMultiplier = GetMoveSpeedMultiplier();
        _animationStateMachine.UpdatePlayerAnim(PlayerAnimState.IsSprinting, isSprinting);

        if (!isSprinting)
        {
            WolfRunValidationAndDisable();
        }
    }
    // attempts a jump, will fail if not grounded
    public void Jump()
    {
        if (!CanMove || !IsGrounded) return;
        // calculate jump velocity from jump height and gravity
        float multiplier = _isWolfRunning ? _wolfRunJumpHeightMultiplier : 1f;
        float jumpVelocity = Mathf.Sqrt(2f * -_gravityValue * _jumpHeight * multiplier);
        // override current y velocity but maintain x/z velocity
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, jumpVelocity, _rigidbody.velocity.z);
        _animationStateMachine.JumpAnimation();
    }

    // updates
    private void FixedUpdate()
    {
        IsGrounded = CheckGrounded();
        _animationStateMachine.UpdatePlayerAnim(PlayerAnimState.IsGrounded, IsGrounded);
        //Set the NavMeshAgent destination using nma.SetDestination.
        //if (_navMeshAgent.enabled) _navMeshAgent.SetDestination(transform.position + MoveInput);
        MovePlayer();
        bool isMoving = HasMoveInput && _rigidbody.velocity.magnitude > 0.1f;
        _animationStateMachine.UpdatePlayerAnim(PlayerAnimState.IsMoving, isMoving);
        //Debug.DrawRay(transform.position, GroundNormal, Color.magenta);
        if(_isWolfRunning || _wolfRunWarmUp != null)
        {
            WolfRunValidationAndDisable();
        }
    }

    private void Update()
    {
        // rotation is on normal update for a smoother rotation
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
        if(_isDodging)
        {
            targetVelocity = _dodgeLockedDirection * (_dodgeSpeed);
        }
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
        //rotates character towards movement direction
        if (_lookInMovementDirection && HasTurnInput && !_isDodging && (IsGrounded || _airTurning))
        {
            Quaternion targetRotation = Quaternion.LookRotation(LookDirection);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
            // rotate sprite character properly
            _rigidbody.MoveRotation(rotation);
        }
    }
    public void TryGroundDodge(float multiWindowTime)
    {
        if(CanStartGroundDodge)
        {
            _lastDodgeTime = Time.time;
            _dodgeSpeed = _shortDodgeDistance / _shortDodgeTime;
            _goIntoLongDodge = false;
            _animationStateMachine.UpdatePlayerAnim(PlayerAnimState.LongDodge, _goIntoLongDodge);

            if (HasMoveInput)
            {
                Vector3 input = MoveInput;
                Vector3 right = Vector3.Cross(transform.up, input);
                _dodgeLockedDirection = Vector3.Cross(right, GroundNormal);
            }
            else
            {
                _dodgeLockedDirection = LookDirection * -1;
            }
            StartCoroutine(PreGroundDodge(multiWindowTime));
        }
    }
    public void TryAirDodge()
    {
        if(CanAirDodge)
        {
            if(HasMoveInput)
            {
                Vector3 input = MoveInput;
                Vector3 right = Vector3.Cross(transform.up, input);
                _dodgeLockedDirection = Vector3.Cross(right, GroundNormal);
            }
            else
            {
                _dodgeLockedDirection = LookDirection;
            }
            _canDodgedThisJump = false;
            Vector3 targetVelocity = _dodgeLockedDirection * (_dodgeSpeed * _airDodgeSpeedModifier);
            Vector3 velocityDiff = targetVelocity - _rigidbody.velocity;
            velocityDiff.y = 0f;
            Vector3 acceleration = velocityDiff * (_acceleration * _airDodgeAccelerationModifier);
            _rigidbody.AddForce(acceleration * _rigidbody.mass);
            _animationStateMachine.UpdatePlayerAnim(PlayerAnimState.AirDodge);
        }
    }
    private IEnumerator PreGroundDodge(float multiWindowTime)
    {
        _isDodging = true;
        _animationStateMachine.UpdatePlayerAnim(PlayerAnimState.ShortDodge, true);

        float elapsed = 0;
        while (elapsed < multiWindowTime)
        {
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        float dodgeTime = _goIntoLongDodge ? _shortDodgeTime * _longDodgeMultiplier : _shortDodgeTime;
        //Debug.Log("pre dodging done");
        _animationStateMachine.UpdatePlayerAnim(PlayerAnimState.LongDodge, _goIntoLongDodge);

        //if (!_goIntoLongDodge)
        //{
        //    Debug.Log("Doing short dodge");
        //}
        //else
        //{
        //    Debug.Log("Doing long dodge");
        //}
        StartCoroutine(GroundDodge(dodgeTime));
    }
    public void QueueLongDodge()
    {
        if(!_doingActualDodge)
        {
            //Debug.Log("long dodge queded");
            _goIntoLongDodge = true;
        }
    }
    private IEnumerator GroundDodge(float dodgeTime)
    {
        _doingActualDodge = true;
        float elapsed = 0;
        while (elapsed < dodgeTime)
        {
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log("dodging done");
        _isDodging = false;
        _doingActualDodge = false;
    }

    private IEnumerator WolfRunTimer()
    {
        float elapsed = 0;
        float fovStart = _cameraFreeLook.m_Lens.FieldOfView;
        while (elapsed < _wolfRunStartUpTime)
        {
            //gradiually increase fov
            _cameraFreeLook.m_Lens.FieldOfView = Mathf.Lerp(fovStart, _wolfRunCameraFOV, elapsed / _wolfRunStartUpTime);
            // particles?
            Mathf.Clamp(elapsed += Time.deltaTime,0,_wolfRunStartUpTime);
            yield return new WaitForEndOfFrame();
        }
        SetWolfRun(true);
        _cameraFreeLook.m_Lens.FieldOfView = _wolfRunCameraFOV;
        _wolfRunWarmUp = null;
    }
    private void SetWolfRun(bool isWolfRunning)
    {
        //camera
        _isWolfRunning = isWolfRunning;
        _animationStateMachine.UpdatePlayerAnim(PlayerAnimState.IsWolfRunning, isWolfRunning);
        _moveSpeedMultiplier = GetMoveSpeedMultiplier();
        if (!isWolfRunning && _cameraFreeLook.m_Lens.FieldOfView != _cameraFOVDefaultValue)
        {
            StartCoroutine(LerpBackCameraFOV());
        }
    }
    private void WolfRunValidationAndDisable()
    {
        if(!IsSprinting || _rigidbody.velocity.magnitude < _wolfRunMinSpeed)
        {
            if(_wolfRunWarmUp != null)
            {
                StopCoroutine(_wolfRunWarmUp);
                _wolfRunWarmUp = null;
            }
            SetWolfRun(false);
        }
    }
    private IEnumerator LerpBackCameraFOV()
    {
        float elapsed = 0;
        float fovStart = _cameraFreeLook.m_Lens.FieldOfView;
        while (elapsed < _FOVLerpBackDuration)
        {
            _cameraFreeLook.m_Lens.FieldOfView = Mathf.Lerp(fovStart, _cameraFOVDefaultValue, elapsed / _FOVLerpBackDuration);
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _cameraFreeLook.m_Lens.FieldOfView = _cameraFOVDefaultValue;
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
            _canDodgedThisJump = true;
            return true;
        }
        return false;
    }
    public float GetMoveSpeedMultiplier()
    {
        if (_isWolfRunning)
        {
            return _wolfRunSpeedMultiplier;
        }
        if(IsSprinting)
        {
            return _sprintMultiplier;
        }
        return 1;
    }

}


