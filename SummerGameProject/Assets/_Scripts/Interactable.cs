using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using UnityEngine.UI;
using TMPro;

public class Interactable : MonoBehaviour
{
    InputAction interact;
    bool isInRange;

    [SerializeField] float radius = 3f;
    [SerializeField] UnityEvent interactAction;
    [SerializeField] bool destroyOnInteract;


    private GameObject child;

    void OnDrawGizmosSelected()
    {
        //Draw Yellow Wire Sphere to show the range of the interactable
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius / gameObject.transform.lossyScale.x);

    }

    private void Awake()
    {
        //Set up a Sphere Trigger Collider for interaction
        SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        collider.radius = radius;

        //Small fix for scaled object, or our radius is massive
        if (gameObject.transform.lossyScale.x > 1.0f)
        {
            collider.radius = radius / gameObject.transform.lossyScale.x;
        }

        //On Awake Spawn our child (Might be heavy)

        //Create our child & Make it a child.
        child = new GameObject();
        child.transform.parent = this.transform;
        child.transform.SetLocalPositionAndRotation(new Vector3(0.0f, 1.0f, 0.0f), child.transform.transform.localRotation);
        TextMeshPro setup = child.AddComponent<TextMeshPro>();

        //
        setup.rectTransform.sizeDelta = new Vector2(5.0f, 1.0f);
        setup.fontSize = 2;
        setup.alignment = TextAlignmentOptions.Center;


        child.SetActive(false);



        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the player enters the trigger, ready the input for the interaction
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            EnableToolTip();
            //Get the input action for the interaction
            interact = other.GetComponent<PlayerInput>().actions["Interact"];
            interact.Enable();

            //This line will trigger the interaction when the input is pressed
            interact.performed += ctx => Interact();
        }
    }

    private void EnableToolTip()
    {
        Debug.Log("Create our text");

        //Here we create our howvering text.
        TextMeshPro tmp = child.GetComponent<TextMeshPro>();

        //Add our text.
        tmp.text = "Press e to interact!";

        child.SetActive(true);
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
