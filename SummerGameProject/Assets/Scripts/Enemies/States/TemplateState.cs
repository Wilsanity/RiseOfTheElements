using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script provides a template structure for future enemy state scripts. Do not edit this script, used as a reference only.
/// To be deleted in the future.
/// </summary>
public class TemplateState : FSMState
{
    // Protected Variables
    // Insert variables needed for state

    // Constructor
    public TemplateState(/*parameters for constructor*/)
    {
        //stateType = FSMStateType.INSERT_CURRENT_STATE_TYPE; *ALWAYS set this*

        // Set parameters
        //ex. speed = inSpeed;
    }

    public override void EnterStateInit()
    {
        // Initialize when state is entered
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Criteria to transition out of state
        //
        // ex. enemy is within range of player, transition to attack state
        // 
        // if (IsInRange(npc, player.position, npc.GetComponent<EnemyController>().ENTER_RANGE))
        // {
        //     npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InAttackRange);
        //     Debug.Log("Enemy is in attack range.");
        // }
    }

    public override void Act(Transform player, Transform npc)
    {
        // Actions performed while in current state
        //
        //ex. enemy moves towards player's position
    }
}