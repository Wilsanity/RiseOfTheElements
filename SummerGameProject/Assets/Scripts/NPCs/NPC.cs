using Kibo.Interactable;
using Kibo.NPCs.Behaviour;
using UnityEngine;

namespace Kibo.NPCs
{
    public class NPC : MonoBehaviour, IInteractable
    {
        [Tooltip("Optional")]
        [SerializeField] private IdleBehaviour idleBehaviour;
        [Tooltip("Optional")]
        [SerializeField] private LookAtBehaviour lookAtBehaviour;
        [Header("Mesh Bones")]
        [Tooltip("Optional")]
        [SerializeField] private Transform head;

        public string InteractionName => $"Talk to {name}";

        #region Unity Messages
        private void Start()
        {
            if (lookAtBehaviour)
            {
                lookAtBehaviour.TargetApproachedEvent.AddListener(OnTargetApproached);
                lookAtBehaviour.TargetDepartedEvent.AddListener(OnTargetDeparted);
            }
        }

        private void OnEnable()
        {
            if ((lookAtBehaviour == null || lookAtBehaviour.Target == null) && idleBehaviour) idleBehaviour.enabled = true;
            if (lookAtBehaviour) lookAtBehaviour.enabled = true;
        }

        private void OnDisable()
        {
            if (idleBehaviour) idleBehaviour.enabled = false;
            if (lookAtBehaviour) lookAtBehaviour.enabled = false;
        }

        private void Update()
        {
            FaceTarget();
        }
        #endregion

        #region Look At Event Listeners
        private void OnTargetApproached()
        {
            if (idleBehaviour) idleBehaviour.enabled = false;
        }

        private void OnTargetDeparted()
        {
            if (lookAtBehaviour.Target) return;

            if (idleBehaviour) idleBehaviour.enabled = true;
        } 
        #endregion

        private void FaceTarget()
        {
            Vector3 target;
            if (lookAtBehaviour && lookAtBehaviour.Target) target = lookAtBehaviour.Target.position;
            else if (idleBehaviour && idleBehaviour.HasTarget) target = idleBehaviour.TargetPosition.Value;
            else return;

            Vector3 bodyForward = Vector3.ProjectOnPlane(target - transform.position, transform.up);
            if (bodyForward == Vector3.zero) return;

            transform.forward = bodyForward;
            if (head)
            {
                bool lookDirectly = idleBehaviour && idleBehaviour.enabled && idleBehaviour.TargetStation;
                head.forward = lookDirectly ? target - head.position : bodyForward;
            }
        }

        #region Interaction
        public bool Interact()
        {
            Debug.Log(InteractionName); // TODO: Integrate with upcoming dialog system

            return true;
        }
        #endregion
    }
}