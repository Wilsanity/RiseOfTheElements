using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMonster_CooldownState : FSMState
{
    private float timer = 0.0f;
    private float interval = 0.0f;

    private bool coolDownOver = false;

    private Animator animator;

    private RootMonsterEnemyController _enemyController;

    public RootMonster_CooldownState(float duration, Animator anim, RootMonsterEnemyController enemyController)
    {
        stateType = FSMStateType.Cooldown;

        _enemyController = enemyController;

        // Set parameters
        interval = duration;

        animator = anim;
    }

    public override void EnterStateInit()
    {
        ///animator.SetBool("Idle", true);
        ///animator.SetBool("Attacking", false);
        ///animator.SetBool("Hit", false);

        coolDownOver = false;

        timer = WorldData.Instance.worldTimer + interval;
        //Debug.Log("Cool Down Entered...");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (_enemyController.UnitHealthScript.CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }
        if (coolDownOver)
        {
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.EnterIdle);
            
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        if (WorldData.Instance.worldTimer >= timer)
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
