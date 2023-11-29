using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlantAIController : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 4;
    public float timeToRotate = 2;
    public float speedWalk = 6;
    public float speedRun = 9;

    public float viewRadius = 15;
    public float viewAngle = 90;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float meshResolution = 1f;
    public int edgeIteraions = 4;
    public float edgeDistance = 0.5f;

    public Transform[] patrolPoints;
    int m_currentPatrolPointIndex;

    Vector3 m_playerPreviousPos = Vector3.zero;
    Vector3 m_playerPos;

    float m_waitTime;
    float m_TimeToRotate;
    bool m_PlayerInRange;
    bool m_PlayerNear;
    bool m_IsPatrol;
    bool m_CaughtPlayer;


    // Start is called before the first frame update
    void Start()
    {
        m_playerPos = Vector3.zero;
        m_IsPatrol = true;
        m_CaughtPlayer = false;
        m_PlayerInRange = false;
        m_waitTime = startWaitTime;
        m_currentPatrolPointIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(patrolPoints[m_currentPatrolPointIndex].position);
    }

    // Update is called once per frame
    void Update()
    {
        EnvironmentView();

        if (!m_IsPatrol)
        {
            ChasePlayer();
        }
        else
        {
            Patroling();
        }
    }

    void CaughtPlayer()//The player is found.
    {
        m_CaughtPlayer = true;
    }

    void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);
        if (Vector3.Distance(transform.position, player) <= 0.3)
        {
            if (m_waitTime <= 0)
            {
                m_PlayerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(patrolPoints[m_currentPatrolPointIndex].position);
                m_waitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
            }

            else
            {
                Stop();
                m_waitTime -= Time.deltaTime;
            }
        }
    }

    void Move(float speed)//Moves the enemy
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void Stop()//Stops the enemy movement
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    public void NextPatrolPoint()//Gets the enemy to move to each subsequent patrol spot
    {
        m_currentPatrolPointIndex = (m_currentPatrolPointIndex + 1) % patrolPoints.Length;
        navMeshAgent.SetDestination(patrolPoints[m_currentPatrolPointIndex].position);
    }

    void EnvironmentView()//Ensures the enemy can detect the player
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    m_PlayerInRange = true;
                    m_IsPatrol = false;
                }
                else
                {
                    m_PlayerInRange = false;
                }
            }

            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                m_PlayerInRange = false;
            }

            if (m_PlayerInRange)
            {
                m_playerPos = player.transform.position;
            }
        }
    }

    private void Patroling()//Move the enemy between patrol points
    {
        if(m_PlayerNear)
        {
            if (m_TimeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(m_playerPreviousPos);
            }
            else 
            {
                Stop();
                m_TimeToRotate -= Time.deltaTime;
            }
        }

        else
        {
            m_PlayerNear = false;
            m_playerPreviousPos = Vector3.zero;
            navMeshAgent.SetDestination(patrolPoints[m_currentPatrolPointIndex].position);
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (m_waitTime <= 0)
                {
                    NextPatrolPoint();
                    Move(speedWalk);
                    m_waitTime = startWaitTime;
                }
                else 
                {
                    Stop();
                    m_waitTime = Time.deltaTime;
                }
            }
        }
    }

    private void ChasePlayer()
    {
        //Set the destination to the player's location.
        m_PlayerNear = false;
        m_playerPreviousPos = Vector3.zero;

        //Check if the enemy reached the stopping distance. If the enemy isn't
        //near the player, the enemy waits a bit then returns to patrolling.
        if (!m_CaughtPlayer)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(m_playerPos);
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (m_waitTime <= 0 && !m_CaughtPlayer && 
                Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                m_IsPatrol = true;
                m_PlayerNear = false;
                Move(speedWalk);
                m_TimeToRotate = timeToRotate;
                m_waitTime = startWaitTime;
                navMeshAgent.SetDestination(patrolPoints[m_currentPatrolPointIndex].position);
            }

            else 
            {
                if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 2.5f)
                {
                    Stop();
                    m_waitTime -= Time.deltaTime;
                }
            }
        }
    }
}
