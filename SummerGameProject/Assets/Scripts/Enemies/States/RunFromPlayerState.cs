using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunFromPlayerState : FSMState
{
    protected Vector3 destPos;
    protected float chaseSpeed;

    private Animator animator;
    private RootMonsterEnemyController _enemyController;
    private float _originalTime;

    public RunFromPlayerState(float inSpeed, Animator inAnimator, RootMonsterEnemyController enemyController)
    {
        stateType = FSMStateType.MovingAway;

        chaseSpeed = inSpeed;

        animator = inAnimator;

        _enemyController = enemyController;

    }

    public override void EnterStateInit()
    {
        Debug.Log("Run From Player State Entered.");
        //TO DO: Implement Animations
        _originalTime = Time.time;
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (_enemyController.UnitHealthScript.CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
            return;
        }


        

        if (_originalTime < Time.time + _enemyController.RunAwayMaxTime) return;

        // Out of range
        if (!IsInRange(npc, player.position, (int)_enemyController.SpikeShieldRadius))
        {

            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
            agent.destination = npc.position;

            // Wander
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.OutOfRange);

        }


        // Within attacking rage
        if (IsInRange(npc, player.position, (int)_enemyController.SpikeShieldRadius))
        {

            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
            agent.destination = npc.position;

            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.Shield);
            
        }
        
    }

    public override void Act(Transform player, Transform npc)
    {
        Debug.Log("ACTING");
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();

        Vector3 dirToPlayer = player.position - npc.position;
        dirToPlayer.Normalize();

        //Now get the opposite direction and add randomness
        Vector3 oppositeDir = -dirToPlayer * 5;
        agent.speed = chaseSpeed;
        agent.destination = player.position + new Vector3(oppositeDir.x, player.position.y, oppositeDir.z);
    }
}
