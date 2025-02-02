using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BeetleWanderState : FSMState
{
    protected Vector3 destPos;
    protected float wanderSpeed;
    protected float radius;

    private Animator animator;

    public BeetleWanderState(float inSpeed, float inRadius, Animator inAnimator)
    {
        stateType = FSMStateType.Wandering;

        radius = inRadius;
        wanderSpeed = inSpeed * 0.5f;

        animator = inAnimator;
    }

    public override void EnterStateInit()
    {
        Debug.Log("Beetle Wander State Entered.");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (npc.GetComponent<UnitHealth>().CurrentHealth == 0)
        {
            npc.GetComponent<NavMeshAgent>().isStopped = true;

            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
            return;
        }
        // Within Range
        if (IsInRange(npc, player.position, npc.GetComponent<EnemyController>().EXIT_RANGE))
        {
            npc.GetComponent<NavMeshAgent>().isStopped = true;

            // Check if enemy is in taunt mode
            //if(npc.GetComponent<BeetleEnemyController>().IN_TAUNT_MODE)
            if(!npc.GetComponent<BeetleEnemyController>().IN_AGGRO)
            {
                // Poison Mucus
                npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InTauntMode);
            }
            // Check if enemy is aggro (has been hit)
            else if (npc.GetComponent<BeetleEnemyController>().IN_AGGRO)
            {
                // Check if enemy is in melee range
                if(IsInRange(npc, player.position, npc.GetComponent<BeetleEnemyController>().MELEE_RANGE))
                {
                    // Horn Swipe OR Flying Arrow picked at random
                    int rand = Random.Range(1, 3);

                    switch (rand)
                    {
                        // Horn Swipe
                        case 1:
                            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InMeleeRange);
                            break;
                        // Flying Arrow
                        case 2:
                            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.OutOfMeleeRange);
                            break;
                    }
                }
                else
                {
                    // Flying Arrow
                    npc.GetComponent<EnemyController>().PerformTransition(TransitionType.OutOfMeleeRange);
                }
            }
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();

        if(agent.isStopped == true) { agent.isStopped = false; }

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
