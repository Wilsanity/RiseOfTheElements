using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Kibo.Interactable.UI
{
    public class InteractorUI : MonoBehaviour
    {
        [SerializeField] private Interactor interactor;
        [SerializeField] private TMP_Text text;

        #region Unity Messgaes
        private void Awake()
        {
            Assert.IsNotNull(interactor, $"{nameof(InteractorUI)}'s {interactor} is null");
            Assert.IsNotNull(text, $"{nameof(InteractorUI)}'s {text} is null");
        }

        private void Start()
        {
            interactor.NewInteractableFoundEvent.AddListener(OnNewInteractableFoundEvent);
            interactor.InteractableLostEvent.AddListener(OnInteractableLostEvent);
        }

        private void OnEnable()
        {
            text.enabled = true;
        }

        private void OnDisable()
        {
            text.enabled = false;
        } 
        #endregion

        private void OnNewInteractableFoundEvent()
        {
            text.enabled = true;
            text.text = interactor.Interactable.InteractionName;
        }

        private void OnInteractableLostEvent()
        {
            text.enabled = false;
        }
    }
}