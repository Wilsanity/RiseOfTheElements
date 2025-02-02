using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BounceBombAttackState.cs
/// This script handles the bounce bomb attack for the frog enemy.
/// </summary>
public class BounceBombAttackState : FSMState
{
    private Animator animator;
    private bool attackOver = false;
    private int speed;

    protected Vector3 destPos;
    protected Vector3 initialPlayerPos;
    private Vector3[] bouncePoints;

    private float bounceHeight = 0;

    private bool bouncedInPlaceComplete = false;
    private bool bounceToPlayerComplete = false;
    private bool slamPlayerComplete = false;
    private bool chargingComplete = false;

    private int count = 0;

    private float timer = 0f;
    private float interval = 2f;

    // Constructor
    public BounceBombAttackState(int s, float h, Animator anim)
    {
        stateType = FSMStateType.BounceBombAttacking;

        // Set parameters
        animator = anim;
        speed = s;
        bounceHeight = h;
    }

    public override void EnterStateInit()
    {
        initialPlayerPos = Vector3.zero;
        destPos = Vector3.zero;

        count = 0;
        timer = 0;

        bouncePoints = null;

        attackOver = false;

        bouncedInPlaceComplete = false;
        bounceToPlayerComplete = false;
        slamPlayerComplete = false;
        chargingComplete = false;

        Debug.Log("Bounce Bomb Attack Entered.");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Cooldown
        if (attackOver)
        {
            // Enter Rest State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.Cooldown);
            return;
        }

        // Dead
        if (npc.GetComponent<UnitHealth>().CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
            return;
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        if (initialPlayerPos == Vector3.zero)
        {
            initialPlayerPos = player.position;
        }

        // STEP 1: Bounce once in place
        // 
        if (!bouncedInPlaceComplete)
        {
            if (count < 1)
            {
                // If path has not yet been set, generate new points
                if (bouncePoints == null)
                {
                    bouncePoints = new Vector3[2];

                    bouncePoints[0] = new Vector3(npc.position.x, 0.5f, npc.position.z);//new Vector3(0, 0.5f, 31);
                    bouncePoints[1] = new Vector3(npc.position.x, bounceHeight, npc.position.z);

                    // Set first position to the first point in the path
                    destPos = bouncePoints[0];
                }
                else
                {
                    // Move enemy along path
                    for (int i = 0; i < bouncePoints.Length; i++)
                    {
                        if (Vector3.Distance(npc.position, bouncePoints[i]) <= 0.1f)
                        {
                            if (i + 1 > bouncePoints.Length - 1)
                            {
                                destPos = bouncePoints[0];
                                count++;
                            }
                            else
                            {
                                destPos = bouncePoints[i + 1];
                            }
                        }
                    }

                    // Determine the direction the enemy should look towards
                    Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);

                    // Smoothly rotate and move towards the target point.
                    npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, speed * Time.deltaTime);
                    npc.position = Vector3.MoveTowards(npc.position, destPos, speed * Time.deltaTime);
                }
            }
            else
            {
                if (Vector3.Distance(npc.position, destPos) <= 0.1f)
                {
                    count = 0;
                    bouncedInPlaceComplete = true;

                    timer = WorldData.Instance.worldTimer + interval;
                    return;
                }

                destPos = bouncePoints[0];
                // Determine the direction the enemy should look towards
                Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);

                // Smoothly rotate and move towards the target point.
                npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, speed * Time.deltaTime);
                npc.position = Vector3.MoveTowards(npc.position, destPos, speed * Time.deltaTime);
            }
        }

        // STEP 2: Bounce downwards and squish enemy model to "charge" poision
        // 
        if (!chargingComplete && bouncedInPlaceComplete)
        {
            if (npc.transform.localScale != new Vector3(1, 1, 0.5f))
            {
                npc.transform.localScale += new Vector3(0, 0, -0.1f);
            }

            // STEP 3: Cool down while charging attack
            if (WorldData.Instance.worldTimer >= timer)
            {
                chargingComplete = true;
                timer = 0;
                return;
            }
        }

        // STEP 4: Bounce above the player
        // 
        if (!bounceToPlayerComplete && bouncedInPlaceComplete && chargingComplete)
        {
            // Reset scale
            if (npc.transform.localScale != new Vector3(1, 1, 1))
            {
                npc.transform.localScale += new Vector3(0, 0, 0.1f);
            }
            else
            {
                destPos = player.position;
                destPos.y = bounceHeight;

                if (Vector3.Distance(npc.position, destPos) <= 0.1f)
                {
                    initialPlayerPos = player.position;
                    bounceToPlayerComplete = true;

                    timer = WorldData.Instance.worldTimer + (interval / 2);
                    return;
                }

                // Determine the direction the enemy should look towards
                Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);

                // Smoothly rotate and move towards the target point.
                npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, speed * Time.deltaTime);
                npc.position = Vector3.MoveTowards(npc.position, destPos, speed * Time.deltaTime);
            }
        }

        // STEP 5: Linger in air and spin
        // 
        if(bounceToPlayerComplete)
        {
            if(WorldData.Instance.worldTimer > timer)
            {
                // STEP 6: Slam down into the player
                // 
                if (!slamPlayerComplete && bounceToPlayerComplete && bouncedInPlaceComplete && chargingComplete)
                {
                    destPos = initialPlayerPos;

                    if (Vector3.Distance(npc.position, destPos) <= 0.1f)
                    {
                        initialPlayerPos = Vector3.zero;
                        slamPlayerComplete = true;
                        return;
                    }

                    // Determine the direction the enemy should look towards
                    Quaternion targetRotation = Quaternion.LookRotation(destPos);

                    // Smoothly rotate and move towards the target point.
                    npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, (speed * 3) * Time.deltaTime);
                    npc.position = Vector3.MoveTowards(npc.position, destPos, (speed * 3) * Time.deltaTime);
                }
            }
        }
       
        // STEP 7: Attack over
        //
        if (slamPlayerComplete && bounceToPlayerComplete && bouncedInPlaceComplete && chargingComplete)
        {
            attackOver = true;
        }
    }
}
