using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyController : EnemyController
{
    [Header("Movement Variables")]
    public float wanderRadius;

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void ConstructFSM()
    {
        // Create States
        //
        // Create Wander State
        WanderState wander = new WanderState(speed, wanderRadius, animator);

        // Create Chase State
        ChaseState chase = new ChaseState(speed, animator);

        // Create Attack State
        AttackState attack = new AttackState(speed, animator);

        // Create Dead State
        DeadState dead = new DeadState(animator, this);
        
        // Add Transitions
        //
        // Transitions out of wander state
        wander.AddTransition(TransitionType.InChaseRange, FSMStateType.Chasing);
        wander.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Transitions out of chase state
        chase.AddTransition(TransitionType.InAttackRange, FSMStateType.Attacking);
        chase.AddTransition(TransitionType.OutOfRange, FSMStateType.Wandering);
        chase.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Transitions out of attack state
        attack.AddTransition(TransitionType.InChaseRange, FSMStateType.Chasing);
        attack.AddTransition(TransitionType.NoHealth, FSMStateType.Dead);

        // Add States to List
        AddState(wander);
        AddState(chase);
        AddState(attack);
        AddState(dead);
    }
}
