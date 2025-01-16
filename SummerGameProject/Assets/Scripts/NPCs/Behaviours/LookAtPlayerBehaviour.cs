using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Kibo.NPCs.Behaviour
{
    [RequireComponent(typeof(SphereCollider))]
    public class LookAtPlayerBehaviour : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [Header("Events")]
        [SerializeField] private UnityEvent PlayerApproached;
        [SerializeField] private UnityEvent PlayerDeparted;
        [Header("Debug")]
        [SerializeField] private bool logEvents;

        private HashSet<Transform> players = new();
        private SphereCollider lookRangeTrigger;

        public UnityEvent PlayerApproachedEvent => PlayerApproached;
        public UnityEvent PlayerDepartedEvent => PlayerDeparted;
        public Transform[] Players => players.ToArray();
        public Transform Player => players.FirstOrDefault(player => player != null);
        public float Range => lookRangeTrigger.radius;

        #region Unity Messages
        private void Awake()
        {
            lookRangeTrigger = GetComponent<SphereCollider>();
            lookRangeTrigger.isTrigger = true;

            if (logEvents)
            {
                PlayerApproached.AddListener(() => Debug.Log(nameof(PlayerApproached)));
                PlayerDeparted.AddListener(() => Debug.Log(nameof(PlayerDeparted)));
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
            AddPlayer(other.transform);
        }

        private void OnTriggerExit(Collider other)
        {
            RemovePlayer(other.transform);
        }
        #endregion

        #region Player Tracking
        private void AddPlayer(Transform playerTransform)
        {
            if (!playerTransform.CompareTag(playerTag) || players.Contains(playerTransform)) return;

            players.Add(playerTransform);
            PlayerApproached.Invoke();
        }

        private void RemovePlayer(Transform playerTransform)
        {
            if (!players.Contains(playerTransform)) return;

            players.Remove(playerTransform);
            PlayerDeparted.Invoke();
        } 
        #endregion
    }
}