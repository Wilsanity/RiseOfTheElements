using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornSwipeAttackState : FSMState
{
    protected Vector3 destPos;
    protected float speed;

    private bool attackOver = false;
    private bool playerDamaged = false;

    private float attackTimer = 0;
    private float attackInterval = 2f;

    private Animator animator;

    public HornSwipeAttackState(float inSpeed, Animator inAnimator)
    {
        stateType = FSMStateType.HornSwipeAttacking;

        speed = inSpeed;

        animator = inAnimator;
    }

    public override void EnterStateInit()
    {
        attackOver = false;
        playerDamaged = false;

        attackTimer = WorldData.Instance.worldTimer + attackInterval;

        Debug.Log("Horn Swipe State Entered.");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (npc.GetComponent<UnitHealth>().CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
            return;
        }

        // Cooldown
        if (attackOver)
        {
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.Cooldown);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        // STEP 1: Slightly tilt downwards - TODO
        //

        // STEP 2: Swipe horn at player
        //
        // Note: Animation will take place here and proper checks will be in place,
        // for now the player will just take damage if in range for testing purposes
        if(!playerDamaged)
        {
            if (IsInRange(npc, player.position, npc.GetComponent<BeetleEnemyController>().MELEE_RANGE))
            {
                player.GetComponent<UnitHealth>().DamageUnit(npc.GetComponent<EnemyController>().damage);
                playerDamaged = true;
            }
        }

        if(WorldData.Instance.worldTimer >= attackTimer)
        {
            attackOver = true;
        }
    }
}
