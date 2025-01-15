using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageState : FSMState
{
    private Animator animator;

    private float hitAnimTimer = 0f;

    // Constructor
    public TakeDamageState(Animator anim)
    {
        stateType = FSMStateType.TakingDamage;

        animator = anim;
    }

    public override void EnterStateInit()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Attacking", false);
        animator.SetBool("Hit", true);

        hitAnimTimer = WorldData.Instance.worldTimer + 1.5f;

        //Debug.Log("Taking damage...");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (npc.GetComponent<UnitHealth>().CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }
        // Damage Applied
        if(WorldData.Instance.worldTimer >= hitAnimTimer)
        {
            // Move Away State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.DamageTaken);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        // Determine the direction the enemy should look towards
        Quaternion targetRotation = Quaternion.LookRotation(npc.forward, npc.up);

        // Smoothly rotate and move towards the target point.
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, 4 * Time.deltaTime);
    }
}
