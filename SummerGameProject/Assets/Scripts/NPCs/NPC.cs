using Kibo.NPCs.Behaviour;
using UnityEngine;
using UnityEngine.Assertions;

namespace Kibo.NPCs
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] private IdleBehaviour idleBehaviour;
        [Header("Mesh Bones")]
        [Tooltip("Optional")]
        [SerializeField] private Transform head;

        private void Awake()
        {
            Assert.IsNotNull(idleBehaviour, $"{nameof(NPC)}'s {nameof(idleBehaviour)} is null");
        }

        private void Update()
        {
            FaceTarget();
        }

        private void FaceTarget()
        {
            Vector3 bodyForward = Vector3.ProjectOnPlane(idleBehaviour.TargetPosition.Value - transform.position, transform.up);
            if (bodyForward == Vector3.zero) return;

            if (idleBehaviour.HasTarget) transform.forward = bodyForward;
            if (head) head.forward = idleBehaviour.TargetStation ? idleBehaviour.TargetPosition.Value - head.position : bodyForward;
        }
    }
}