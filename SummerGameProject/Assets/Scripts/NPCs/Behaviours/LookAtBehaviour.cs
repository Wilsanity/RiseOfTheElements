using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Kibo.NPCs.Behaviours
{
    [RequireComponent(typeof(SphereCollider))]
    public class LookAtBehaviour : MonoBehaviour
    {
        [SerializeField] private string[] targetTags = { "Player" };
        [Header("Events")]
        [SerializeField] private UnityEvent TargetApproached;
        [SerializeField] private UnityEvent TargetDeparted;
        [Header("Debug")]
        [SerializeField] private bool logEvents;

        private HashSet<Transform> targets = new();
        private SphereCollider lookRangeTrigger;

        public string[] TargetTags => targetTags;
        public UnityEvent TargetApproachedEvent => TargetApproached;
        public UnityEvent TargetDepartedEvent => TargetDeparted;
        public Transform[] Targets => targets.ToArray();
        public Transform Target => targets.FirstOrDefault(player => player != null);
        public float Range => lookRangeTrigger.radius;

        #region Unity Messages
        private void Awake()
        {
            lookRangeTrigger = GetComponent<SphereCollider>();
            lookRangeTrigger.isTrigger = true;

            if (logEvents)
            {
                TargetApproached.AddListener(() => Debug.Log(nameof(TargetApproached)));
                TargetDeparted.AddListener(() => Debug.Log(nameof(TargetDeparted)));
            }
        }

        private void OnEnable()
        {
            lookRangeTrigger.enabled = true;
        }

        private void OnDisable()
        {
            lookRangeTrigger.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            AddTarget(other.transform);
        }

        private void OnTriggerExit(Collider other)
        {
            RemoveTarget(other.transform);
        }
        #endregion

        #region Target Tracking
        private void AddTarget(Transform targetTransform)
        {
            if (!targetTags.Contains(targetTransform.tag) || !targets.Add(targetTransform)) return;

            TargetApproached.Invoke();
        }

        private void RemoveTarget(Transform playerTransform)
        {
            if (!targets.Remove(playerTransform)) return;

            TargetDeparted.Invoke();
        }
        #endregion
    }
}