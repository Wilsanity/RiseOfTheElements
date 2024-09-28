using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FrogCooldownState.cs
/// This script handles the cool down behaviour for the frog enemy.
/// </summary>
public class FrogCooldownState : FSMState
{
    private float timer = 0.0f;
    private float interval = 0.0f;

    private bool coolDownOver = false;

    private Animator animator;

    public FrogCooldownState(float i, Animator anim)
    {
        stateType = FSMStateType.Cooldown;

        // Set parameters
        interval = i;

        animator = anim;
    }

    public override void EnterStateInit()
    {
        coolDownOver = false;

        timer = WorldData.Instance.worldTimer + interval;

        Debug.Log("Cooldown Entered.");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (npc.GetComponent<UnitHealth>().CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }
        // Cool down ended
        if (coolDownOver)
        {
            // If in agro range, attack
            if (IsInRange(npc, player.position, npc.GetComponent<EnemyController>().ENTER_RANGE))
            {
                // If in bounce bomb range, transition to bounce bomb attack
                if (IsInRange(npc, player.position, npc.GetComponent<FrogEnemyController>().BOUNCE_BOMB_RANGE))
                {
                    npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InBounceBombRange);
                    return;
                }
                // Otherwise, pick a random attack
                else
                {
                    int rand = Random.Range(1, 3);

                    switch (rand)
                    {
                        // Bounce Attack
                        case 1:
                            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InBounceRange);
                            break;
                        // Tron Roll Attack
                        case 2:
                            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InTronRollRange);
                            break;
                        case 3:
                            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InBounceBombRange);
                            break;
                    }
                    return;
                }
            }
            else
            {
                npc.GetComponent<EnemyController>().PerformTransition(TransitionType.OutOfRange);
                return;
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
