using Kibo.Data;
using Kibo.Interactable;
using Kibo.NPCs.Behaviours;
using Kibo.NPCs.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Kibo.NPCs
{
    public class NPC : GlobalIdentityBehaviour<NPC, NPCData>, IInteractable
    {
        [Tooltip("Optional")]
        [SerializeField] private IdleBehaviour idleBehaviour;
        [Tooltip("Optional")]
        [SerializeField] private LookAtBehaviour lookAtBehaviour;
        [Tooltip("Optional")]
        [SerializeField] private EnvironmentAwarenessBehaviour environmentAwarenessBehaviour;
        [Header("Mesh Bones")]
        [Tooltip("Optional")]
        [SerializeField] private Transform head;
        [Header("Events")]
        [SerializeField] private UnityEvent wasInteractedWith;

        public UnityEvent WasInteractedWith => wasInteractedWith;
        public virtual string InteractionName => $"Talk to {name}";

        #region Unity Messages
        protected virtual void OnEnable()
        {
            if ((lookAtBehaviour == null || lookAtBehaviour.Target == null) && idleBehaviour) idleBehaviour.enabled = true;
            if (lookAtBehaviour) lookAtBehaviour.enabled = true;
            if (environmentAwarenessBehaviour) environmentAwarenessBehaviour.enabled = true;
        }

        protected virtual void OnDisable()
        {
            if (idleBehaviour) idleBehaviour.enabled = false;
            if (lookAtBehaviour) lookAtBehaviour.enabled = false;
            if (environmentAwarenessBehaviour) environmentAwarenessBehaviour.enabled = false;
        }

        protected override void Start()
        {
            base.Start();

            if (lookAtBehaviour)
            {
                lookAtBehaviour.TargetApproachedEvent.AddListener(OnTargetApproached);
                lookAtBehaviour.TargetDepartedEvent.AddListener(OnTargetDeparted);
            }

            if (environmentAwarenessBehaviour)
            {
                environmentAwarenessBehaviour.EmitterHeard.AddListener(OnEmitterHeard);
                environmentAwarenessBehaviour.EmitterReached.AddListener(OnEmitterApproached);
            }
        }

        protected virtual void Update()
        {
            FaceTarget();
        }
        #endregion

        private void FaceTarget()
        {
            Vector3 target;
            if (environmentAwarenessBehaviour && environmentAwarenessBehaviour.TargetEmitter) target = environmentAwarenessBehaviour.TargetEmitter.transform.position;
            else if (lookAtBehaviour && lookAtBehaviour.Target) target = lookAtBehaviour.Target.position;
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

        #region Look At Behaviour
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

        #region Environment Awareness Behaviour
        private void OnEmitterHeard()
        {
            if (idleBehaviour) idleBehaviour.enabled = false;
            if (lookAtBehaviour) lookAtBehaviour.enabled = false;
        }

        private void OnEmitterApproached()
        {
            if (idleBehaviour) idleBehaviour.enabled = true;
            if (lookAtBehaviour) lookAtBehaviour.enabled = true;
        }
        #endregion

        #region Interaction
        public virtual bool Interact()
        {
            wasInteractedWith.Invoke();

            Debug.Log(InteractionName); // TODO: Integrate with upcoming dialog system

            return true;
        }
        #endregion

        #region Save/Load Overrides
        public override void SaveTo(ref NPCData npcData)
        {
            npcData.GUID = GUID;
            npcData.Name = name;
        }

        public override void LoadFrom(NPCData npcData)
        {
            name = npcData.Name;
        } 
        #endregion
    }
}