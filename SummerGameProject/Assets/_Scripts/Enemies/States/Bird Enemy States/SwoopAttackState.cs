using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This state allows the enemy to perform a swoop attack towards the player.
/// </summary>
public class SwoopAttackState : FSMState
{
    private float speed = 0;

    private int health = 0;

    private Animator animator;

    private BirdAttackZone zone;

    private bool attackOver = false;

    private Vector3 initial = Vector3.zero;

    private GameObject hitSpot;

    // Constructor
    public SwoopAttackState(float s, GameObject attackZone, Vector3 pos, Animator anim)
    {
        stateType = FSMStateType.Attacking;

        speed = s;
        zone = attackZone.GetComponent<BirdAttackZone>();
        hitSpot = attackZone;
        animator = anim;

        initial = pos;
    }

    public override void EnterStateInit()
    {
        animator.SetBool("Attacking", true);
        animator.SetBool("Idle", false);
        animator.SetBool("Hit", false);

        attackOver = false;

        zone.isCollidingPlayer = false;
        zone.isCollidingGround = false;

        initial = Vector3.zero;

        health = 0;

        //Debug.Log("Swoop Attack State Entered...");
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
        // Check if Attack is Over
        else if (attackOver)
        {
            // Aerial Move Away state for cooldown
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.AttackOver);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        if (health == 0) { health = npc.GetComponent<UnitHealth>().CurrentHealth; }

        if (initial == Vector3.zero)
        {
            initial = player.position;
            npc.GetComponent<BirdEnemyController>().initialPlayerPos = player.position;
        }

        // Determine the direction the enemy should look towards
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(initial.x, initial.y - 3.5f, initial.z) - npc.position);

        // Smoothly rotate and move towards the target point.
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, speed * Time.deltaTime);
        npc.position = Vector3.MoveTowards(npc.position, new Vector3(initial.x, initial.y - 4.5f, initial.z), speed * Time.deltaTime);

        // Check if enemy has collided with player and deal damage
        if (zone.isCollidingPlayer)
        {
            player.GetComponent<UnitHealth>().DamageUnit(npc.GetComponent<EnemyController>().damage);
            attackOver = true;
        }
        // Attack time is over, end attack
        else if (Vector3.Distance(npc.position, initial) <= 2f)
        {
            attackOver = true;
        }
        // Enemy hit ground, end attack
        else if (Physics.Raycast(hitSpot.transform.position, hitSpot.transform.forward, out RaycastHit hit, 2f))
        {
            if(hit.collider.CompareTag("Ground"))
            {
                attackOver = true;
                //Debug.Log("Hit Ground.");
            }
        }
    }
}
