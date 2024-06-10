using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the Bird Enemy's behaviour.
/// </summary>
public class BirdEnemyController : EnemyController
{
    public BirdAttackZone attackZoneGO;

    [Tooltip("This is the number of random path points you want to generate for this enemy.")]
    [SerializeField]
    private int pointsNum;

    [Tooltip("This represents the Y position of the enemy. Path points will generate at this height only and the enemy will return to this height when moving away.")]
    [SerializeField]
    private float height = 9.6f;

    [Tooltip("This represents the distance around the centre the enemy's path points will generate on. A larger radius will allow a larger area for points to generate.")]
    [SerializeField]
    private float radius;

    [Tooltip("This represents the centre of the enemy in which the radius for the wandering will take place. In most cases, you may set this to the starting position of the enemy.")]
    [SerializeField]
    private Vector3 center;

    [Tooltip("The speed the enemy will attack at.")]
    [SerializeField]
    private float attackSpeed = 7f;

    [Tooltip("The speed the enemy will move away at.")]
    [SerializeField]
    private float moveAwaySpeed = 5f;

    [Tooltip("The length of the enemy's cooldown state.")]
    [SerializeField]
    private float coolDownInterval = 5f;

    // Stored during attack state to use during cooldown calculations
    [HideInInspector]
    public Vector3 initialPlayerPos;

    protected override void Initialize()
    {
        base.Initialize();

        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    protected override void ConstructFSM()
    {
        // Create States
        //
        // Create Aerial Wander State
        AerialWanderState aerialWanderState = new AerialWanderState(center, height, radius, pointsNum, animator);

        // Create Swoop Attack State
        SwoopAttackState swoopAttackState = new SwoopAttackState(attackSpeed, attackZoneGO, initialPlayerPos, animator);

        // Create Flee State
        AerialMoveAwayState moveAwayState = new AerialMoveAwayState(height, moveAwaySpeed, animator);

        // Create Cooldown State
        CooldownState cooldDownState = new CooldownState(coolDownInterval, animator);

        // Create Damage State
        TakeDamageState takeDamageState = new TakeDamageState(animator);

        // Create Dead State
        DeadState dead = new DeadState(animator);

        // Add Transitions
        //
        // Transitions out of Aerial Wander State
        aerialWanderState.AddTransition(TransitionType.InAttackRange, FSMStateType.Attacking);
        aerialWanderState.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Transitions out of Swoop Attack State
        //swoopAttackState.AddTransition(TransitionType.OutOfRange, FSMStateType.Wandering);
        swoopAttackState.AddTransition(TransitionType.AttackOver, FSMStateType.MovingAway);
        swoopAttackState.AddTransition(TransitionType.DamageTaken, FSMStateType.TakingDamage);
        swoopAttackState.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Transitions out of Move Away State
        moveAwayState.AddTransition(TransitionType.Cooldown, FSMStateType.Cooldown);
        moveAwayState.AddTransition(TransitionType.DamageTaken, FSMStateType.TakingDamage);
        moveAwayState.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Transitions out of Cool Down State
        cooldDownState.AddTransition(TransitionType.OutOfRange, FSMStateType.Wandering);
        cooldDownState.AddTransition(TransitionType.InAttackRange, FSMStateType.Attacking);
        cooldDownState.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Transition out of Damage State
        takeDamageState.AddTransition(TransitionType.DamageTaken, FSMStateType.MovingAway);
        takeDamageState.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Add States to List
        AddState(aerialWanderState);
        AddState(swoopAttackState);
        AddState(moveAwayState);
        AddState(cooldDownState);
        AddState(takeDamageState);
        AddState(dead);
    }
}
