using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This state allows the enemy to patrol a defined aerial radius, using random points on the Navmesh.
/// </summary>
public class AerialWanderState : FSMState
{
    protected Vector3 destPos;
    protected float wanderSpeed;
    protected float radius;
    protected float height;
    protected Vector3 center;

    private int numOfPoints;

    private GameObject[] pathPoints;

    private Animator animator;

    // Constructor
    public AerialWanderState(Vector3 c, float h, float r, int p, Animator anim)
    {
        stateType = FSMStateType.Wandering;

        // Set parameters
        numOfPoints = p;

        height = h;
        radius = r;
        center = c;

        animator = anim;
    }

    public override void EnterStateInit()
    {
        animator.SetBool("Idle", true);
        animator.SetBool("Attacking", false);
        animator.SetBool("Hit", false);

        // Initialize when state is entered
        Debug.Log("Aerial Wander State Entered...");
    }

    public override void Reason(Transform player, Transform npc)
    {
        // Dead
        if (npc.GetComponent<UnitHealth>().CurrentHealth == 0)
        {
            // Dead State
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.NoHealth);
        }
        // Within Range
        if (IsInRange(npc, player.position, npc.GetComponent<EnemyController>().ENTER_RANGE))
        {
            // Swoop Attack
            npc.GetComponent<EnemyController>().PerformTransition(TransitionType.InAttackRange);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        // If path has not yet been set, generate new points
        if(pathPoints == null)
        {
            GeneratePathPoints(numOfPoints);
        }
        else
        {
            // Move enemy along path
            for (int i = 0; i < pathPoints.Length; i++)
            {
                if (Vector3.Distance(npc.position, pathPoints[i].transform.position) <= 0.5f)
                {
                    if (i + 1 > pathPoints.Length - 1)
                    {
                        destPos = pathPoints[0].transform.position;
                    }
                    else
                    {
                        destPos = pathPoints[i + 1].transform.position;
                    }
                }
            }

            // Determine the direction the enemy should look towards
            Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);

            // Smoothly rotate and move towards the target point.
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, 2 * Time.deltaTime);
            npc.position = Vector3.MoveTowards(npc.position, destPos, 2 * Time.deltaTime);
        }
    }

    /// <summary>
    /// Generates points on a path for enemy to follow during wander state.
    /// </summary>
    /// <param name="pointsNum"> number of points on the path</param>
    public void GeneratePathPoints(int pointsNum)
    {
        // Stores new path positions
        Vector3[] points = new Vector3[pointsNum];

        // Initialize array to number of desired points
        pathPoints = new GameObject[pointsNum];

        // Create a path point for each desired number of points
        for(int i = 0; i < pointsNum; i++)
        {
            // Generate random point within a radius around the center
            points[i] = center + (Random.insideUnitSphere * radius);

            // Set high to preset value to stay consistent and avoid obstacles
            points[i].y = height;

            // Create object for path point
            GameObject GO = new GameObject($"point {i}");
            GO.transform.position = points[i];

            // Set path point
            pathPoints[i] = GO;
        }

        // Set first position to the first point in the path
        destPos = pathPoints[0].transform.position;
    }
}
