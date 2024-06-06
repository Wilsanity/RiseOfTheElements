using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownState : FSMState
{
    private float timer = 0.0f;
    private float interval = 0.0f;

    private bool coolDownOver = false;

    public CooldownState(float i)
    {
        stateType = FSMStateType.Cooldown;

        // Set parameters
        interval = i;
    }

    public override void EnterStateInit()
    {
        coolDownOver = false;

        timer = WorldData.Instance.worldTimer + interval;
        Debug.Log("Cool Down Entered...");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (npc.GetComponent<EnemyController>().GetHealth() == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }
        if (coolDownOver)
        {
            // Out of Range
            if (!IsInRange(npc, player.position, npc.GetComponent<EnemyController>().ENTER_RANGE))
            {
                // Aerial Wander
                npc.GetComponent<EnemyController>().PerformTransition(TransitionType.OutOfRange);
            }
            // Attack
            if (IsInRange(npc, player.position, npc.GetComponent<EnemyController>().ENTER_RANGE))
            {
                // Return to attack state
                npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InAttackRange);
            }
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        if(WorldData.Instance.worldTimer >= timer)
        {
            coolDownOver = true;
        }

        // Face the player
        // Determine the direction the enemy should look towards
        Quaternion targetRotation = Quaternion.LookRotation(player.position - npc.position);

        // Smoothly rotate and move towards the target point.
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, 2 * Time.deltaTime);
    }
}
