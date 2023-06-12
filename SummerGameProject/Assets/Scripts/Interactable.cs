using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    InputAction interact;
    bool isInRange;

    [SerializeField] float radius = 3f;
    [SerializeField] UnityEvent interactAction;
    [SerializeField] bool destroyOnInteract;

    void OnDrawGizmosSelected()
    {
        //Draw Yellow Wire Sphere to show the range of the interactable
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Awake()
    {
        //Set up a Sphere Trigger Collider for interaction
        SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        collider.radius = radius;
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the player enters the trigger, ready the input for the interaction
        if (other.CompareTag("Player"))
        {
            isInRange = true;

            //Get the input action for the interaction
            interact = other.GetComponent<PlayerInput>().actions["Interact"];
            interact.Enable();

            //This line will trigger the interaction when the input is pressed
            interact.performed += ctx => Interact();
        }
    }

    private void Interact()
    {
        //If the player isn't in range, quit opporation
        if (!isInRange) return;

        //Else do the interaction
        interactAction.Invoke();
        if (destroyOnInteract) Destroy(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        //If player leaves trigger area, disable the interaction
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            interact.Disable();
        }
    }
}
