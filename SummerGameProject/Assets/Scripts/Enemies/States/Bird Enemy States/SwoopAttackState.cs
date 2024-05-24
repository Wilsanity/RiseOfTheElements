using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This state allows the enemy to perform a swoop attack towards the player.
/// </summary>
public class SwoopAttackState : FSMState
{
    private float attackTimer = 0.0f;
    private float attackInterval = 2.0f;

    private float speed = 0;

    // Constructor
    public SwoopAttackState(float s)
    {
        stateType = FSMStateType.Attacking;

        speed = s;
    }

    public override void EnterStateInit()
    {
        // Initialize when state is entered
        attackTimer = WorldData.Instance.worldTimer + attackInterval;
        Debug.Log("Swoop Attack State Entered...");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (npc.GetComponent<EnemyController>().GetHealth() == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }
        // Check if Attack is Over
        if(IsInRange(npc, player.position, 2) || WorldData.Instance.worldTimer >= attackTimer)
        {
            // Player is already out of range
            if (!IsInRange(npc, player.position, npc.GetComponent<EnemyController>().ENTER_RANGE))
            {
                // Aerial Wander
                npc.GetComponent<EnemyController>().PerformTransition(TransitionType.OutOfRange);
            }
            else
            {
                // Aerial Move Away state for cooldown
                npc.GetComponent<EnemyController>().PerformTransition(TransitionType.AttackOver);
            }
        }
        /*// Out of Range
        if (!IsInRange(npc, player.position, npc.GetComponent<EnemyController>().ENTER_RANGE))
        {
            // Aerial Wander
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.OutOfRange);
            Debug.Log("Enemy out of swoop range.");
        }*/
    }

    public override void Act(Transform player, Transform npc)
    {
        //attackTimer += Time.deltaTime;

        // Determine the direction the enemy should look towards
        Quaternion targetRotation = Quaternion.LookRotation(player.position - npc.position);

        // Smoothly rotate and move towards the target point.
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, speed * Time.deltaTime);
        npc.position = Vector3.MoveTowards(npc.position, player.position, speed * Time.deltaTime);


        //npc.position = Vector3.MoveTowards(npc.position, final, 10 * Time.deltaTime);

        ///Debug.Log("Attack Timer is: " + attackTimer.ToString());
    }
}
