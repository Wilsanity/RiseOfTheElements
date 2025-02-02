using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// FrogEnemyController.cs
/// Handles the behaviour logic for the frog enemy.
/// </summary>
public class FrogEnemyController : EnemyController
{
    //[Header("Title")]
    //
    [SerializeField]
    private float cooldownInterval;

    [SerializeField]
    public int BOUNCE_BOMB_RANGE;

    [Tooltip("This is the number of random path points you want to generate for this enemy.")]
    [SerializeField]
    private int pointsNum;

    [Tooltip("This represents the Y position of the enemy. Path points will generate at this height only and the enemy will return to this height when moving away.")]
    [SerializeField]
    private float height = 9.6f;

    [Tooltip("This represents the Y position of the enemy's bounce.")]
    [SerializeField]
    private float bounceHeight = 9.6f;

    [Tooltip("This represents the distance around the centre the enemy's path points will generate on. A larger radius will allow a larger area for points to generate.")]
    [SerializeField]
    private float radius;

    [Tooltip("This represents the centre of the enemy in which the radius for the wandering will take place. In most cases, you may set this to the starting position of the enemy.")]
    [SerializeField]
    private Vector3 center;

    [SerializeField]
    private GameObject[] waypoints;

    protected override void Initialize()
    {
        base.Initialize();

        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;

        UnpackPrefab();
    }

    protected override void ConstructFSM()
    {
        // Create States
        //
        // Create Bounce State
        BounceAttackState bounce = new BounceAttackState(speed, bounceHeight, animator);

        // Create Bounce Bomb State
        BounceBombAttackState bounceBomb = new BounceBombAttackState(speed, bounceHeight, animator);

        // Create Tron Roll State
        TronRollAttackState tronRoll = new TronRollAttackState(speed, bounceHeight, center.y, radius, animator);

        // Create Cooldown State
        FrogCooldownState cooldown = new FrogCooldownState(cooldownInterval, animator);

        // Create Wander State
        FrogWanderState wander = new FrogWanderState(center, height, radius, pointsNum, animator);

        // Create Dead State
        DeadState dead = new DeadState(animator);

        // Add Transitions
        //
        // Transitions out of Bounce State
        bounce.AddTransition(TransitionType.Cooldown, FSMStateType.Cooldown);
        bounce.AddTransition(TransitionType.InBounceBombRange, FSMStateType.BounceBombAttacking);
        bounce.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Transitions out of Bounce Bomb State
        bounceBomb.AddTransition(TransitionType.Cooldown, FSMStateType.Cooldown);
        bounceBomb.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Transitions out of Tron Roll State
        tronRoll.AddTransition(TransitionType.Cooldown, FSMStateType.Cooldown);
        tronRoll.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Transition out of Wander State
        wander.AddTransition(TransitionType.InBounceRange, FSMStateType.BounceAttacking);
        wander.AddTransition(TransitionType.InBounceBombRange, FSMStateType.BounceBombAttacking);
        wander.AddTransition(TransitionType.InTronRollRange, FSMStateType.TronRollAttacking);
        wander.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Transitions out of Cooldown State
        cooldown.AddTransition(TransitionType.InBounceRange, FSMStateType.BounceAttacking);
        cooldown.AddTransition(TransitionType.InBounceBombRange, FSMStateType.BounceBombAttacking);
        cooldown.AddTransition(TransitionType.InTronRollRange, FSMStateType.TronRollAttacking);
        cooldown.AddTransition(TransitionType.OutOfRange, FSMStateType.Wandering);
        cooldown.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Add States to List
        AddState(wander);
        AddState(bounce);
        AddState(bounceBomb);
        AddState(tronRoll);
        AddState(cooldown);
        AddState(dead);
    }

    private void UnpackPrefab()
    {
        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(gameObject);

        if (isPrefab)
        {
            PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }
    }

    public void GenerateWaypoints()
    {
        //Check to see if there's a child Object named "Waypoints"
        waypoints = AerialWanderState.GeneratePathPoints(pointsNum, transform, waypoints, center, radius, height, destPos);
        //if so, clear all children GameObjects and then spawn in new waypoints

        //if not, then make one and spawn objects in new waypoints
    }

    private void OnDrawGizmos()
    {
        Vector3 centerHeight = new Vector3(center.x, height, center.z);
        Handles.DrawWireDisc(centerHeight, Vector3.up, radius);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, centerHeight);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(centerHeight, 0.3f);
    }
}
