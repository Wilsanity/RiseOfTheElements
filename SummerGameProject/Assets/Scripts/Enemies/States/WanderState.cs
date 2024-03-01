using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This state allows the enemy to patrol a defined radius, using random points on the Navmesh.
/// </summary>
public class WanderState : FSMState
{
    protected Vector3 destPos;
    protected float wanderSpeed;
    protected float radius;

    private Animator animator;

    public WanderState(float inSpeed, float inRadius, Animator inAnimator)
    {
        stateType = FSMStateType.Wandering;

        radius = inRadius;
        wanderSpeed = inSpeed * 0.5f;

        animator = inAnimator;
    }

    public override void EnterStateInit()
    {
        Debug.Log("Wander State Entered.");
        //TO DO: Implement Animations
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
        // Within Range
        if (IsInRange(npc, player.position, npc.GetComponent<EnemyController>().EXIT_RANGE))
        {
            // Chase
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InChaseRange);
            Debug.Log("Enemy in range.");
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();

        // Check if npc has reached it's destination before assigning new point
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (RandomPointOnNavMesh(npc.position, radius, out destPos))
            {
                agent.SetDestination(destPos);
                Debug.DrawRay(destPos, Vector3.up, Color.blue, 1.0f); // used to debug position
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="center"> centre of wander area </param>
    /// <param name="range"> the search radius </param>
    /// <param name="result"> the random position found </param>
    /// <returns> returns true if point is found </returns>
    private bool RandomPointOnNavMesh(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}
