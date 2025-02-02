using Kibo.Environment;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Kibo.NPCs.Behaviours
{
    public class EnvironmentAwarenessBehaviour : MonoBehaviour, IEnvironmentListener
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float emitterApproachDistance = 3f;
        [Header("Events")]
        [SerializeField] private UnityEvent emitterHeard, emitterReached;
        [Header("Debug")]
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private bool showTarget;

        public UnityEvent EmitterHeard => emitterHeard;
        public UnityEvent EmitterReached => emitterReached;
        public EnvironmentEmitter TargetEmitter { get; private set; }

        #region Unity Messages
        private void Awake()
        {
            Assert.IsNotNull(agent, $"{nameof(EnvironmentAwarenessBehaviour)}'s {nameof(agent)} is null");
        }

        private void FixedUpdate()
        {
            if (TargetEmitter == null) return;

            if (Vector3.Distance(transform.position, TargetEmitter.transform.position) <= emitterApproachDistance)
            {
                TargetEmitter = null;

                emitterReached.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying && showTarget && TargetEmitter)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawWireSphere(TargetEmitter.transform.position, emitterApproachDistance);
            }
        } 
        #endregion

        public void Listen(EnvironmentEmitter emitter)
        {
            if (!isActiveAndEnabled) return;

            TargetEmitter = emitter;
            agent.SetDestination(emitter.transform.position);

            emitterHeard.Invoke();
        }
    }
}