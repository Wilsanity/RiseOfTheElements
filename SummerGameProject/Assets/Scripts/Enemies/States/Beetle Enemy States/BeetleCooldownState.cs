using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleCooldownState : FSMState
{
    private float timer = 0.0f;
    private float interval = 0.0f;

    private bool coolDownOver = false;

    private Animator animator;

    public BeetleCooldownState(float i, Animator anim)
    {
        stateType = FSMStateType.Cooldown;

        // Set parameters
        interval = i;

        animator = anim;
    }

    public override void EnterStateInit()
    {
        animator.SetBool("Idle", true);
        animator.SetBool("Attacking", false);
        animator.SetBool("Hit", false);

        coolDownOver = false;

        timer = WorldData.Instance.worldTimer + interval;
        Debug.Log("Cooldown State Entered.");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (npc.GetComponent<UnitHealth>().CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }
        if (coolDownOver)
        {
            // Within Range
            if (IsInRange(npc, player.position, npc.GetComponent<EnemyController>().EXIT_RANGE))
            {
                // Check if enemy is in taunt mode
                //if (npc.GetComponent<BeetleEnemyController>().IN_TAUNT_MODE)
                if (!npc.GetComponent<BeetleEnemyController>().IN_AGGRO)
                {
                    // Poison Mucus
                    npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InTauntMode);
                }
                // Check if enemy is aggro (has been hit)
                else if (npc.GetComponent<BeetleEnemyController>().IN_AGGRO)
                {
                    // Check if enemy is in melee range
                    if (IsInRange(npc, player.position, npc.GetComponent<BeetleEnemyController>().MELEE_RANGE))
                    {
                        // Horn Swipe OR Flying Arrow picked at random
                        int rand = Random.Range(1, 4);

                        switch (rand)
                        {
                            // Horn Swipe
                            case 1:
                                npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InMeleeRange);
                                break;
                            // Horn Swipe
                            case 2:
                                npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InMeleeRange);
                                break;
                            // Flying Arrow
                            case 3:
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
            else
            {
                // Wander State
                npc.GetComponent<EnemyController>().PerformTransition(TransitionType.OutOfRange);
            }
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
