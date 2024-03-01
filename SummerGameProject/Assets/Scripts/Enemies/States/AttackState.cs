using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This state is a script to simulate an attack.
/// </summary>
public class AttackState : FSMState
{
    protected Vector3 destPos;

    private Animator animator;

    public AttackState(float inSpeed, Animator inAnimator)
    {
        stateType = FSMStateType.Attacking;

        animator = inAnimator;
    }

    public override void EnterStateInit()
    {
        Debug.Log("Attack State Entered.");
        //TODO: Implement Animations
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (npc.GetComponent<EnemyController>().GetHealth() == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
            return;
        }
        // Out of attacking range, in chase range
        if (!IsInRange(npc, player.position, npc.GetComponent<EnemyController>().ENTER_RANGE))
        {
            // Chase
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InChaseRange);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        agent.destination = player.position;

        Debug.Log("Attacking player.");

        // TODO: implement cool down
    }
}
