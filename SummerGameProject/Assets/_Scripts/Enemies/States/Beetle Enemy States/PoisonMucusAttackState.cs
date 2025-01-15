using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonMucusAttackState : FSMState
{
    private GameObject projectileGO;
    private GameObject projectilePointGO;

    protected Vector3 destPos;
    private float speed;

    private float rotateTimer = 0;
    private float rotateInterval = 1.5f;

    private bool hasRotated = false;
    private bool hasShot = false;
    private bool attackOver = false;

    private Animator animator;

    public PoisonMucusAttackState(float inSpeed, Animator inAnimator)
    {
        stateType = FSMStateType.MucusAttacking;

        speed = inSpeed;

        animator = inAnimator;
    }

    public override void EnterStateInit()
    {
        attackOver = false;
        hasRotated = false;
        hasShot = false;

        rotateTimer = 0;

        Debug.Log("Poison Mucus State Entered.");
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
        if(attackOver)
        {
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.Cooldown);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        if (projectileGO == null)
        {
            projectileGO = npc.GetComponent<BeetleEnemyController>().projectile;
        }        
        
        if (projectilePointGO == null)
        {
            projectilePointGO = npc.GetComponent<BeetleEnemyController>().projectilePoint;
        }

        // STEP 1: Hover Near Player - TODO
        //

        // STEP 2: Turn back towards player
        //
        if(!hasRotated && !hasShot)
        {
            if(rotateTimer == 0)
            {
                rotateTimer = WorldData.Instance.worldTimer + rotateInterval;
            }

            destPos = player.position;

            // Determine the direction the enemy should look towards
            Quaternion targetRotation = Quaternion.LookRotation(npc.position - destPos);

            // Smoothly rotate and move towards the target point.
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, 5 * Time.deltaTime);

            // Check if enemy is within the right range (angle), or if time has run out
            // Note: adding the timer helps to avoid the enemy getting stuck trying to "match" the angle if the player is moving too much
            if(WorldData.Instance.worldTimer > rotateTimer || Quaternion.Angle(npc.rotation, targetRotation) <= 2f)
            {
                hasRotated = true;
            }
        }

        // STEP 3: Shoot projectiles at player
        //
        if(hasRotated && !hasShot)
        {
            GameObject GO = GameObject.Instantiate(projectileGO, projectilePointGO.transform.position, Quaternion.identity);
            Vector3 force = -npc.forward;
            force += new Vector3(0, 300, 0);

            GO.GetComponent<Rigidbody>().AddRelativeForce(force.x * 800, force.y, force.z * 1000);

            hasShot = true;
        }


        // STEP 4: If it doesn't hit the player, leave poison on ground
        //
        // Handled by separate script.

        // STEP 5: End attack
        //
        if(hasRotated && hasShot)
        {
            attackOver = true;
        }
    }
}
