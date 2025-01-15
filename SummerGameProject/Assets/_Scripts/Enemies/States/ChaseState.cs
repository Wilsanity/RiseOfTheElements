using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This state allows the enemy to chase the player.
/// </summary>
public class ChaseState : FSMState
{
    protected Vector3 destPos;
    protected float chaseSpeed;

    private Animator animator;

    public ChaseState(float inSpeed, Animator inAnimator)
    {
        stateType = FSMStateType.Chasing;

        chaseSpeed = inSpeed;

        animator = inAnimator;
    }

    public override void EnterStateInit()
    {
        Debug.Log("Chase State Entered.");
        //TO DO: Implement Animations
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (npc.GetComponent<UnitHealth>().CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
            return;
        }
        // Within attacking rage
        if (IsInRange(npc, player.position, npc.GetComponent<EnemyController>().ENTER_RANGE))
        {
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InAttackRange);
            Debug.Log("Enemy is in attack range.");
        }
        // Out of range
        if (!IsInRange(npc, player.position, npc.GetComponent<EnemyController>().EXIT_RANGE))
        {
            // Wander
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.OutOfRange);
            Debug.Log("Enemy is out of range.");
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        agent.destination = player.position;
    }
}
