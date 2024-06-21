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
            pathPoints = GeneratePathPoints(numOfPoints, npc,pathPoints,center,radius,height,destPos);
            
            // Set first position to the first point in the path
            destPos = pathPoints[0].transform.position;
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
    public static GameObject[] GeneratePathPoints(int pointsNum, Transform npc, GameObject[] pathPoints, Vector3 center, float radius, float height, Vector3 destPos)
    {
        
        //Creating a child gameObject named "Waypoints" if it's not already created
        Transform waypointsGO = ChildIsNamed("Waypoints", npc);

        if(waypointsGO.childCount > 0)
        {
            //if we're in the editor and we click "Generate Waypoints" button, we want to 
                //recycle the waypoints and spawn new ones.
            //If we're in the application and there's already children, we should use those instead of creating
                //new ones
            if(Application.isPlaying)
            {
                pathPoints = GetChildrenGameObjects(waypointsGO);
                waypointsGO.transform.parent = null;
                return pathPoints;
               
            }
            else
            {
                DestroyChildren(waypointsGO);
            }
            
        }

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
            GO.AddComponent<DrawGizmosSphere>();
            // Set path point
            pathPoints[i] = GO;

            //Setting the transform parent of the waypoints
            pathPoints[i].transform.parent = waypointsGO;

            if (Application.isPlaying)
            {
                waypointsGO.transform.parent = null;
            }
        }
        return pathPoints;

    }

    public static GameObject[] GetChildrenGameObjects(Transform waypointsGO)
    {
        int childCount = waypointsGO.childCount;
        GameObject[] childrenGOs = new GameObject[childCount];

        for(int i = 0; i < childrenGOs.Length; i++)
        {
            Transform childTransform = waypointsGO.GetChild(i).GetComponent<Transform>();
            childrenGOs[i] = childTransform.gameObject;
        }

        return childrenGOs; 
    }

    public static Transform ChildIsNamed(string nameToCheck, Transform npc)
    {
        foreach(Transform child in npc.transform.GetComponentInChildren<Transform>())
        {
            if (child.name.Equals(nameToCheck)) return child;
        }

        GameObject go = new GameObject(nameToCheck);
        go.transform.parent = npc.transform;
        return go.transform;
    }

    public static void DestroyChildren(Transform parentObject)
    {
        int childCount = parentObject.childCount;

        for(int i = childCount - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(parentObject.GetChild(i).gameObject);
        }

    }
}
