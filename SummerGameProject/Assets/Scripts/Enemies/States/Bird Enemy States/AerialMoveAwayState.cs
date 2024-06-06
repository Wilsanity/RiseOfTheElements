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

    private bool moveAwayComplete = false;

    // Constructor
    public AerialMoveAwayState(float h, float r)
    {
        stateType = FSMStateType.MovingAway;

        height = h;
        moveAwaySpeed = r;
    }

    public override void EnterStateInit()
    {
        moveAwayComplete = false;

        Debug.Log("Aerial Move Away State Entered...");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (npc.GetComponent<EnemyController>().GetHealth() == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }

        if(moveAwayComplete)
        {
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.Cooldown);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        if (initial == Vector3.zero)
        {
            initial = npc.position;
        }

        if(final != Vector3.zero)
        {
            // Determine the direction the enemy should look towards
            Quaternion targetRotation = Quaternion.LookRotation(final - npc.position);

            // Smoothly rotate and move towards the target point.
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, moveAwaySpeed * Time.deltaTime);
            npc.position = Vector3.MoveTowards(npc.position, final, moveAwaySpeed * Time.deltaTime);

            if(Vector3.Distance(npc.position, final) <= 0.5)
            {
                moveAwayComplete = true;
            }
        }
        else
        {
            final.x = player.position.x + (player.position.x - initial.x) + 3;
            final.y = height;
            final.z = player.position.z + (player.position.z - initial.z) + 3;
        }
        ///Debug.Log("(" + final.x + "," + final.y + "," + final.z + ")");
    }
}
