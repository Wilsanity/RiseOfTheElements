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
            Assert.IsNotNull(viewPoint, $"{nameof(viewPoint)} is not assigned on {name}'s {GetType().Name}");

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

        protected virtual void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || !isActiveAndEnabled) return;

            Gizmos.DrawLine(viewPoint.position, InteractPoint);
            if (interactable != null) Gizmos.DrawWireSphere(InteractPoint, .5f);
        }
        #endregion

        #region Interaction
        private void CheckForInteractable()
        {
            Ray ray = new(viewPoint.position, Forward);
            bool hitInteractable = Physics.SphereCast(ray, aimAssistRadius, out hitInfo, maxInteractDistance, interactMask);
            IInteractable newInteractable = hitInteractable ? hitInfo.collider.GetComponent<IInteractable>() : null;
            if (newInteractable != interactable)
            {
                if (newInteractable != null) NewInteractableFound.Invoke();
                else InteractableLost.Invoke();
            }

            interactable = newInteractable;
        }

        public void OnInteract(InputValue value)
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