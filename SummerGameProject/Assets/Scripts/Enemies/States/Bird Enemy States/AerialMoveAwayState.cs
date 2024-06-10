using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This state allows the enemy to move away from the player to an aerial position on the Navmesh.
/// </summary>
public class AerialMoveAwayState : FSMState
{
    private Vector3 final = Vector3.zero;
    private Vector3 initial = Vector3.zero;

    private float height = 0;
    private float moveAwaySpeed = 0;
    private int health = 0;

    private bool moveAwayComplete = false;

    private Animator animator;

    // Constructor
    public AerialMoveAwayState(float h, float r, Animator anim)
    {
        stateType = FSMStateType.MovingAway;

        height = h;
        moveAwaySpeed = r;

        animator = anim;
    }

    public override void EnterStateInit()
    {
        animator.SetBool("Idle", true);
        animator.SetBool("Attacking", false);
        animator.SetBool("Hit", false);

        moveAwayComplete = false;

        initial = Vector3.zero;
        final = Vector3.zero;

        health = 0;

        Debug.Log("Aerial Move Away State Entered...");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (npc.GetComponent<UnitHealth>().CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }
        else if (npc.GetComponent<UnitHealth>().CurrentHealth < health && health != 0)
        {
            // Take Damage State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.DamageTaken);
        }
        else if (moveAwayComplete)
        {
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.Cooldown);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        if (health == 0) { health = npc.GetComponent<UnitHealth>().CurrentHealth; }

        if (initial == Vector3.zero)
        {
            initial = npc.position;
        }

        if(final != Vector3.zero)
        {
            // Determine the direction the enemy should look towards
            Quaternion targetRotation = Quaternion.LookRotation(final - npc.position);

            // Smoothly rotate and move towards the target point.
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, 1f * Time.deltaTime);
            npc.position = Vector3.MoveTowards(npc.position, final, moveAwaySpeed * Time.deltaTime);

            if(Vector3.Distance(npc.position, final) <= 0.5)
            {
                moveAwayComplete = true;
            }
        }
        else
        {
            //final.x = player.position.x + (player.position.x - initial.x) /** 5f*/;
            //final.y = height;
            //final.z = player.position.z + (player.position.z - initial.z)/* * 5f*/;

            Vector3 initialPlayerPos = npc.GetComponent<BirdEnemyController>().initialPlayerPos;

            final.x = initialPlayerPos.x + (initialPlayerPos.x - initial.x) * 2;
            final.y = height;
            final.z = initialPlayerPos.z + (initialPlayerPos.z - initial.z) * 2f;

            // Testing Purposes Only
            //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //go.transform.position = final;
        }
        ///Debug.Log("(" + final.x + "," + final.y + "," + final.z + ")");
    }
}
