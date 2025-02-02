using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// BeetleEnemyController.cs
/// This script creates and maintains the states/transitions for the beetle enemy.
/// </summary>
public class BeetleEnemyController : EnemyController
{
    public GameObject projectile;
    public GameObject projectilePoint;

    [SerializeField]
    private float cooldownInterval;

    [Header("Health Variables")]
    private float healthLastFrame;
    private UnitHealth health;
    private float attackedTimer = 0f;
    private float attackedInterval = 5f;

    [Header("Variables")]
    public bool IN_AGGRO = false;
    public bool IN_TAUNT_MODE = false;
    public int RANGED_ATTACK_RANGE;
    public int MELEE_RANGE;

    [Header("Navigation")]
    [Tooltip("This represents the distance around the centre the enemy's path points will generate on. A larger radius will allow a larger area for points to generate.")]
    [SerializeField]
    private float radius;

    [HideInInspector]
    public Vector3 initialPlayerPos;
    
    [HideInInspector]
    public bool hitPlayer = false;

    protected override void Initialize()
    {
        base.Initialize();

        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;

        health = GetComponent<UnitHealth>();
        healthLastFrame = health.CurrentHealth;

        attackedTimer = WorldData.Instance.worldTimer + attackedInterval;
    }

    protected override void FSMUpdate()
    {
        base.FSMUpdate();

        // Check timer every frame
        if (IN_AGGRO)
        {
            if (WorldData.Instance.worldTimer >= attackedTimer)
            {
                IN_AGGRO = false;
                IN_TAUNT_MODE = true;

                healthLastFrame = health.CurrentHealth;
            }

            //Debug.Log("World Timer: " + WorldData.Instance.worldTimer);
            //Debug.Log("Attacked Timer: " + attackedTimer);
        }
        else
        {
            if (health.CurrentHealth < healthLastFrame)
            {
                // Enemy was hit, triggers aggro mode
                IN_AGGRO = true;
                IN_TAUNT_MODE = false;

                attackedTimer = WorldData.Instance.worldTimer + attackedInterval;
            }
        }
    }

    protected override void ConstructFSM()
    {
        // Create States
        //
        // Create Wander State
        BeetleWanderState wander = new BeetleWanderState(speed, radius, animator);

        // Create Poison Mucus State
        PoisonMucusAttackState mucusAttack = new PoisonMucusAttackState(speed, animator);

        // Create Flying Arrow State
        FlyingArrowAttackState flyingArrowAttack = new FlyingArrowAttackState(speed, animator);

        // Create Horn Swipe State
        HornSwipeAttackState hornSwipeAttack = new HornSwipeAttackState(speed, animator);

        // Create Cooldown State
        BeetleCooldownState cooldown = new BeetleCooldownState(cooldownInterval, animator);

        // Create Dead State
        DeadState dead = new DeadState(animator);

        // Add Transitions
        //
        // Transitions out of Wander state
        wander.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);
        wander.AddTransition(TransitionType.InTauntMode, FSMStateType.MucusAttacking);
        wander.AddTransition(TransitionType.InMeleeRange, FSMStateType.HornSwipeAttacking);
        wander.AddTransition(TransitionType.OutOfMeleeRange, FSMStateType.FlyingArrowAttacking);

        // Transitions out of Poison Mucus state
        mucusAttack.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);
        mucusAttack.AddTransition(TransitionType.Cooldown, FSMStateType.Cooldown);

        // Transitions out of Flying Arrow State
        flyingArrowAttack.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);
        flyingArrowAttack.AddTransition(TransitionType.Cooldown, FSMStateType.Cooldown);

        // Transitions out of Horn Swipe State
        hornSwipeAttack.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);
        hornSwipeAttack.AddTransition(TransitionType.Cooldown, FSMStateType.Cooldown);

        // Transitions out of Cooldown state
        cooldown.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);
        cooldown.AddTransition(TransitionType.InTauntMode, FSMStateType.MucusAttacking);
        cooldown.AddTransition(TransitionType.InMeleeRange, FSMStateType.HornSwipeAttacking);
        cooldown.AddTransition(TransitionType.OutOfMeleeRange, FSMStateType.FlyingArrowAttacking);
        cooldown.AddTransition(TransitionType.OutOfRange, FSMStateType.Wandering);

        // Add States to List
        AddState(wander);
        AddState(mucusAttack);
        AddState(flyingArrowAttack);
        AddState(hornSwipeAttack);
        AddState(cooldown);
        AddState(dead);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CurrentStateType == FSMStateType.FlyingArrowAttacking)
        {
            if (collision.collider.CompareTag("Player"))
            {
                hitPlayer = true;
            }
            else
            {
                hitPlayer = false;
            }
        }
    }
}
