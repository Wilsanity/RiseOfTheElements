using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// BounceAttackState.cs
/// This script handles the bounce attack behaviour for the frog enemy.
/// </summary>
public class BounceAttackState : FSMState
{
    // TODO
    // - Implement bouncing to player
    // - Move enemy closer to player before attack

    protected Vector3 destPos;
    private Vector3[] bouncePoints;
    private Vector3 initialPlayerPos;

    private Animator animator;
    private bool attackOver = false;
    private int speed;
    private float bounceHeight = 0;

    private bool bouncedInPlaceComplete = false;
    private bool bounceToPlayerComplete = false;
    private bool slamPlayerComplete = false;

    private int numBounces = 0;
    private int count = 0;

    private float timer = 0f;
    private float interval = 1f;
    private int hits = 0;

    // Constructor
    public BounceAttackState(int s, float h, Animator anim)
    {
        stateType = FSMStateType.BounceAttacking;

        // Set parameters
        speed = s;
        animator = anim;

        bounceHeight = h;

        numBounces = Random.Range(1, 3);
    }

    public override void EnterStateInit()
    {
        // Start attack timer
        numBounces = Random.Range(1, 3);
        count = 0;
        bouncedInPlaceComplete = false;
        bounceToPlayerComplete = false;
        slamPlayerComplete = false;

        bouncePoints = null;

        attackOver = false;

        timer = 0;
        hits = 0;

        Debug.Log("Bounce Attack Entered.");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Cooldown
        if (attackOver)
        {
            // Low HP, move immediately into a bounce bomb attack
            if (npc.GetComponent<UnitHealth>().CurrentHealth <= npc.GetComponent<UnitHealth>().MaxHealth * 0.20f)
            {
                npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InBounceBombRange);
                return;
            }
            else
            {
                // Enter Rest State
                npc.GetComponent<EnemyController>().PerformTransition(TransitionType.Cooldown);
                return;
            }
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
        // STEP 1: Bounce once or twice in place
        //
        if (!bouncedInPlaceComplete)
        {
            if (count < numBounces)
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

                    Debug.Log($"Count: {count} NumBounces: {numBounces}");

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

        // STEP 2: Move above the player's initial position
        //
        if (!bounceToPlayerComplete && bouncedInPlaceComplete)
        {
            // Store player's initial position
            if (initialPlayerPos == Vector3.zero)
            {
                initialPlayerPos = player.position;

                destPos = initialPlayerPos;
                destPos.y = bounceHeight;
            }

            if (Vector3.Distance(npc.position, destPos) <= 0.1f)
            {
                //initialPlayerPos = Vector3.zero;
                bounceToPlayerComplete = true;
                return;
            }

            // Determine the direction the enemy should look towards
            Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);

            // Smoothly rotate and move towards the target point.
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, speed * Time.deltaTime);
            npc.position = Vector3.MoveTowards(npc.position, destPos, speed * Time.deltaTime);
        }

        // STEP 3: Slam down towards the player's initial position
        //
        if(!slamPlayerComplete && bounceToPlayerComplete)
        {
            // Store player's initial position
            //if (initialPlayerPos == Vector3.zero)
            //{
                //initialPlayerPos = player.position;

                destPos = initialPlayerPos;
            //}

            if (Vector3.Distance(npc.position, destPos) <= 0.1f)
            {
                initialPlayerPos = Vector3.zero;
                slamPlayerComplete = true;

                timer = WorldData.Instance.worldTimer + interval;
                hits++;
                return;
            }

            // Determine the direction the enemy should look towards
            Quaternion targetRotation = Quaternion.LookRotation(npc.forward);

            // Smoothly rotate and move towards the target point.
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, (speed * 3) * Time.deltaTime);
            npc.position = Vector3.MoveTowards(npc.position, destPos, (speed * 3) * Time.deltaTime);
        }

        // Cool down on ground before repeating attack movement or ending attack
        //
        if (bouncedInPlaceComplete && bounceToPlayerComplete && slamPlayerComplete)
        {
            if (hits > 3)
            {
                attackOver = true;
                return;
            }

            if (WorldData.Instance.worldTimer >= timer)
            {
                bounceToPlayerComplete = false;
                slamPlayerComplete = false;

                timer = 0;
                return;
            }
        }
    }
}
