using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingArrowAttackState : FSMState
{
    protected Vector3 destPos;
    protected Vector3 initial;
    protected float speed;

    private bool attackOver = false;
    private bool chargeOver = false;

    private float stunTimer = 0;
    private float stunInterval = 1.5f;

    private float attackTimer = 0;
    private float attackInterval = 2f;

    private Animator animator;

    public FlyingArrowAttackState(float inSpeed, Animator inAnimator)
    {
        stateType = FSMStateType.FlyingArrowAttacking;

        speed = inSpeed * 2;

        animator = inAnimator;
    }

    public override void EnterStateInit()
    {
        attackOver = false;
        chargeOver = false;

        initial = Vector3.zero;

        stunTimer = WorldData.Instance.worldTimer + stunInterval;
        attackTimer = WorldData.Instance.worldTimer + attackInterval;

        Debug.Log("Flying Arrow State Entered.");
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
        if (initial == Vector3.zero)
        {
            initial = player.position;
            npc.GetComponent<BeetleEnemyController>().initialPlayerPos = player.position;
        }

        // STEP 1: Charge player while spinning
        // 
        if(!chargeOver)
        {
            // Determine the direction the enemy should look towards
            Quaternion targetRotation = Quaternion.LookRotation(initial - npc.position);

            // Smoothly rotate and move towards the target point.
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, speed * Time.deltaTime);
            npc.position = Vector3.MoveTowards(npc.position, new Vector3(initial.x, initial.y, initial.z), speed * Time.deltaTime);

            if(IsInRange(npc, initial, 1))
            {
                chargeOver = true;
            }
        }
        
        // STEP 2: Damage player, or stay stunned in ground/walls
        // 
        if(chargeOver)
        {
            if (npc.GetComponent<BeetleEnemyController>().hitPlayer)
            {
                // Damage
                npc.GetComponent<UnitHealth>().DamageUnit(npc.GetComponent<EnemyController>().damage);
                attackOver = true;

                Debug.Log("Hit Player.");
            }
            else if(WorldData.Instance.worldTimer >= attackTimer)
            {
                attackOver = true;

                Debug.Log("Attack ended due to time.");
            }
            else
            {
                // Stun
                if (WorldData.Instance.worldTimer >= stunTimer)
                {
                    attackOver = true;
                }

                Debug.Log("Hit Object.");
            }
        }
    }
}
