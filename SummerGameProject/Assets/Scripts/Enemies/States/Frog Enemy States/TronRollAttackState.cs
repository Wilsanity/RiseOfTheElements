using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TronRollAttackState.cs
/// This script handles the tron roll attack for the frog enemy.
/// </summary>
public class TronRollAttackState : FSMState
{
    protected Vector3 destPos;
    private Vector3 initialPlayerPos;

    private Animator animator;
    private bool attackOver = false;

    private int speed;
    protected float radius;
    private float bounceHeight;
    private float wanderHeight;

    private bool moveToPlayerComplete = false;
    private bool spewPoisonComplete = false;
    private bool spreadPoisonComplete = false;
    private bool spinComplete = false;

    private int numOfPoints = 8;
    private GameObject[] pathPoints;

    private float timer = 0f;
    private float interval = 2f;

    // Constructor
    public TronRollAttackState(int s, float bH, float h, float r, Animator anim)
    {
        stateType = FSMStateType.TronRollAttacking;

        // Set parameters
        animator = anim;
        speed = s;
        radius = r;
        bounceHeight = bH;
        wanderHeight = h;
    }

    public override void EnterStateInit()
    {
        pathPoints = null;

        attackOver = false;
        moveToPlayerComplete = false;
        spewPoisonComplete = false;
        spreadPoisonComplete = false;
        spinComplete = false;

        timer = 0;

        Debug.Log("Tron Roll Attack Entered.");
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
        // STEP 1: Move towards the player
        // 
        if (!moveToPlayerComplete)
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
                moveToPlayerComplete = true;
                return;
            }

            // Determine the direction the enemy should look towards
            Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);

            // Smoothly rotate and move towards the target point.
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, speed * Time.deltaTime);
            npc.position = Vector3.MoveTowards(npc.position, destPos, speed * Time.deltaTime);
        }

        // STEP 2: Bounce upwards once, spew poison and fall into it
        //
        if (!spewPoisonComplete && moveToPlayerComplete)
        {
            destPos = initialPlayerPos;

            if (Vector3.Distance(npc.position, destPos) <= 0.1f)
            {
                initialPlayerPos = Vector3.zero;
                spewPoisonComplete = true;

                timer = WorldData.Instance.worldTimer + interval;
                return;
            }

            // Determine the direction the enemy should look towards
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.down);

            // Smoothly rotate and move towards the target point.
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, (speed * 3) * Time.deltaTime);

            if (Quaternion.Angle(npc.rotation, targetRotation) <= 0.01f)
            {
                npc.position = Vector3.MoveTowards(npc.position, destPos, (speed * 3) * Time.deltaTime);
            }
        }

        // STEP 3: Spin in poison (cool down and player animation)
        //
        if (!spinComplete && spewPoisonComplete)
        {
            if(WorldData.Instance.worldTimer >= timer)
            {
                timer = 0;
                spinComplete = true;
            }

            // SPIN
            Debug.Log("Spinning..");
        }

        // STEP 4: Move around randomly, spreading poison trail
        //
        if (!spreadPoisonComplete && spinComplete)
        {
            npc.GetComponent<TrailRenderer>().enabled = true;

            // If path has not yet been set, generate new points
            if (pathPoints == null)
            {
                pathPoints = GeneratePathPoints(numOfPoints, npc, pathPoints, npc.position, radius, wanderHeight, destPos);

                // Set first position to the first point in the path
                destPos = pathPoints[0].transform.position;
            }
            else
            {
                // Move enemy along path
                for (int i = 0; i < pathPoints.Length; i++)
                {
                    if (Vector3.Distance(npc.position, pathPoints[i].transform.position) <= 0.1f)
                    {
                        if (i + 1 > pathPoints.Length - 1)
                        {
                            //destPos = pathPoints[0].transform.position;
                            spreadPoisonComplete = true;
                            return;
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
                npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, speed * Time.deltaTime);
                npc.position = Vector3.MoveTowards(npc.position, destPos, speed * Time.deltaTime);
            }
        }

        if (moveToPlayerComplete && spewPoisonComplete && spreadPoisonComplete)
        {
            npc.GetComponent<TrailRenderer>().Clear();
            npc.GetComponent<TrailRenderer>().enabled = false;

            attackOver = true;
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

        if (waypointsGO.childCount > 0)
        {
            //if we're in the editor and we click "Generate Waypoints" button, we want to 
            //recycle the waypoints and spawn new ones.
            //If we're in the application and there's already children, we should use those instead of creating
            //new ones
            if (Application.isPlaying)
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
        for (int i = 0; i < pointsNum; i++)
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

        for (int i = 0; i < childrenGOs.Length; i++)
        {
            Transform childTransform = waypointsGO.GetChild(i).GetComponent<Transform>();
            childrenGOs[i] = childTransform.gameObject;
        }
        return childrenGOs;
    }

    public static Transform ChildIsNamed(string nameToCheck, Transform npc)
    {
        foreach (Transform child in npc.transform.GetComponentInChildren<Transform>())
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

        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(parentObject.GetChild(i).gameObject);
        }
    }
}
