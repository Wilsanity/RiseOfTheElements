using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private SphereCollider _interactionCollider;
    //[SerializeField] private Vector3 _colliderCenter;
    //[SerializeField] private float _colliderRadius;
    private Collider[] _currentInteractableObjects;
    [SerializeField] private LayerMask _whatsInteractable;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnDrawGizmos()
    {
        //for use with Physics.OverlapPhere Only
        //Debug.Log(_colliderCenter);
        //Gizmos.DrawWireSphere(_colliderCenter, _colliderRadius); // Highlights the interaction collider for debugging
        Gizmos.DrawWireSphere(transform.position + _interactionCollider.center, _interactionCollider.radius);
    }
    // Update is called once per frame
    void Update()
    {
        //_currentInteractableObjects = Physics.OverlapSphere(_colliderCenter, _colliderRadius, LayerMask.NameToLayer("InteractableObject"), QueryTriggerInteraction.Collide);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        if (other.gameObject.layer == LayerMask.NameToLayer("InteractableObject"))
        {
            _currentInteractableObjects = Physics.OverlapSphere(transform.position + _interactionCollider.center, _interactionCollider.radius, _whatsInteractable);

            if (_currentInteractableObjects.Length > 0)
            {
                Debug.Log(_currentInteractableObjects[0].gameObject.name);
            }
        }   
    }
}
