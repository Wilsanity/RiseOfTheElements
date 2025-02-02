using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Kibo.Environment
{
    public abstract class EnvironmentEmitter : MonoBehaviour
    {
        [SerializeField] private float range = 10f;
        [SerializeField] private LayerMask listenerMask = 1;
        [SerializeField] private QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.UseGlobal;
        [Header("Events")]
        [SerializeField] private UnityEvent emitted;
        [Header("Debug")]
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private bool showRange;

        public UnityEvent Emitted => emitted;
        protected Color GizmoColor => gizmoColor;

        protected virtual void OnDrawGizmos()
        {
            if (showRange)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawWireSphere(transform.position, range);
            }
        }

        public void Emit()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, listenerMask, queryTrigger);
            var collidersInRange = colliders.Where(collider => Vector3.Distance(transform.position, collider.transform.position) <= range);
            IEnvironmentListener[] listeners = collidersInRange.Select(collider => collider.GetComponent<IEnvironmentListener>()).Where(listener => listener != null).ToArray();

            foreach (var listener in listeners) listener.Listen(this);

            Emitted.Invoke();
        }
    }
}