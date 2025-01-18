using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Kibo.Interactable
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private Transform viewPoint;
        [Tooltip("Optional")]
        [SerializeField] private Transform forwardOverride;
        [Header("Physics Checks")]
        [SerializeField][Min(0f)] private float aimAssistRadius = .25f;
        [SerializeField][Min(0f)] private float maxInteractDistance = 1f;
        [SerializeField] private LayerMask interactMask = 1;
        [Header("Events")]
        [SerializeField] private UnityEvent NewInteractableFound;
        [SerializeField] private UnityEvent InteractableLost;
        [Header("Debug")]
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private bool showPhysicsChecks;
        [SerializeField] private bool logEvents;

        private IInteractable interactable;
        private RaycastHit hitInfo;

        public UnityEvent NewInteractableFoundEvent => NewInteractableFound;
        public UnityEvent InteractableLostEvent => InteractableLost;
        public IInteractable Interactable => interactable;
        public Vector3 InteractPoint => interactable != null ? hitInfo.point : viewPoint.position + maxInteractDistance * Forward;
        public Vector3 Forward => forwardOverride ? forwardOverride.forward : viewPoint.forward;

        #region Unity Messages
        protected virtual void Awake()
        {
            Assert.IsNotNull(viewPoint, $"{GetType().Name}'s {nameof(viewPoint)} is null");

            if (logEvents)
            {
                NewInteractableFound.AddListener(() => Debug.Log(nameof(NewInteractableFound)));
                InteractableLost.AddListener(() => Debug.Log(nameof(InteractableLost)));
            }
        }

        protected virtual void FixedUpdate()
        {
            CheckForInteractable();
        }

        protected virtual void OnDrawGizmos()
        {
            if (!Application.isPlaying || !isActiveAndEnabled || !showPhysicsChecks) return;

            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(viewPoint.position, InteractPoint);
            if (interactable != null) Gizmos.DrawWireSphere(InteractPoint, .5f);
        }
        #endregion

        #region Interaction
        private void CheckForInteractable()
        {
            IInteractable oldInteractable = interactable;

            Ray ray = new(viewPoint.position, Forward);
            bool hitInteractable = Physics.SphereCast(ray, aimAssistRadius, out hitInfo, maxInteractDistance, interactMask);
            interactable = hitInteractable ? hitInfo.collider.GetComponent<IInteractable>() : null;
            if (interactable != oldInteractable)
            {
                if (interactable != null) NewInteractableFound.Invoke();
                else InteractableLost.Invoke();
            }
        }

        private void OnInteract(InputValue value)
        {
            if (!value.isPressed) return;

            _ = Interact();
        }

        public virtual bool Interact()
        {
            if (!isActiveAndEnabled || interactable == null) return false;

            return interactable.Interact();
        } 
        #endregion
    }
}