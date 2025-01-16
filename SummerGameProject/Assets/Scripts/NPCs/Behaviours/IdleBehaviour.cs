using Kibo.NPCs.Stations;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Kibo.NPCs.Behaviour
{
    public class IdleBehaviour : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [Header("Stations")]
        [SerializeField] private Station[] stations;
        [SerializeField][Range(0f, 1f)] private float workChance = .5f;
        [Header("Wandering")]
        [SerializeField] private Bounds wanderBounds = new(Vector3.zero, Vector3.one);
        [SerializeField][Min(0f)] private float positionTolerance = 1e-3f, idleTime = 1f;
        [Header("Events")]
        [SerializeField] private UnityEvent TargetFound;
        [SerializeField] private UnityEvent TargetReached, TargetCleared;
        [Header("Debug")]
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private Color targetColor = Color.green;
        [SerializeField] private bool showStations, showWanderBounds, showTarget, logEvents;

        private Station targetStation;
        private Vector3? targetWanderPosition;
        private float timeAtTarget;

        public NavMeshAgent Agent => agent;
        public Station[] Stations => stations;
        public Bounds WanderBounds => wanderBounds;
        public UnityEvent TargetFoundEvent => TargetFound;
        public UnityEvent TargetReachedEvent => TargetReached;
        public UnityEvent TargetClearedEvent => TargetCleared;
        public Station TargetStation => targetStation;
        public Vector3? TargetPosition => targetStation ? targetStation.transform.position : targetWanderPosition;
        public bool HasTarget => TargetPosition.HasValue;
        public bool IsAtTarget => HasTarget && Vector3.Distance(transform.position, TargetPosition.Value) <= (targetStation ? targetStation.Radius : positionTolerance);

        #region Unity Messages
        private void Awake()
        {
            Assert.IsNotNull(agent, $"{nameof(IdleBehaviour)}'s {nameof(agent)} is null");
            Assert.IsTrue(stations.All(station => station), $"{nameof(IdleBehaviour)}'s {nameof(stations)} array has null elements");

            if (logEvents)
            {
                TargetFound.AddListener(() => Debug.Log(nameof(TargetFound)));
                TargetReached.AddListener(() => Debug.Log(nameof(TargetReached)));
                TargetCleared.AddListener(() => Debug.Log(nameof(TargetCleared)));
            }
        }

        private void Start()
        {
            FindTarget();
        }

        private void OnEnable()
        {
            agent.enabled = true;
        }

        private void OnDisable()
        {
            agent.enabled = false;
        }

        private void Update()
        {
            if (HasTarget) agent.destination = TargetPosition.Value;

            if (IsAtTarget)
            {
                if (timeAtTarget == 0) TargetReached.Invoke();
                timeAtTarget += Time.deltaTime;

                agent.ResetPath();

                float pauseTime = targetStation ? targetStation.ActionTime : idleTime;
                if (timeAtTarget >= pauseTime) FindTarget();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;

            if (showStations)
            {
                foreach (Station station in stations)
                {
                    if (station == null) continue;

                    Gizmos.DrawWireSphere(station.transform.position, station.Radius);
                }
            }

            if (showWanderBounds)
            {
                Gizmos.DrawWireCube(wanderBounds.center, wanderBounds.size);
            }

            if (Application.isPlaying && showTarget && HasTarget)
            {
                Gizmos.color = targetColor;
                Gizmos.DrawWireCube(TargetPosition.Value, Vector3.one);
            }
        }
        #endregion

        #region Targeting
        [ContextMenu(nameof(FindTarget))]
        public void FindTarget()
        {
            bool shouldWork = workChance == 1f || UnityEngine.Random.value < workChance;
            if (shouldWork && stations.Length > 0)
            {
                targetStation = stations[UnityEngine.Random.Range(0, stations.Length)];
                targetWanderPosition = null;
            }
            else
            {
                Vector3? randomPosition = NavMeshUtility.GetRandomPointInBounds(wanderBounds);
                Assert.IsTrue(randomPosition.HasValue, $"{nameof(wanderBounds)} has no intersection with the {nameof(NavMesh)}");

                targetStation = null;
                targetWanderPosition = randomPosition;
            }

            timeAtTarget = 0f;

            TargetFound.Invoke();
        }

        [ContextMenu(nameof(ClearTarget))]
        public void ClearTarget()
        {
            targetStation = null;
            targetWanderPosition = null;

            TargetCleared.Invoke();
        } 
        #endregion
    }
}