using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    
    public float radius = 3f;

    public bool isInRange;
    public KeyCode interactKey;
    public UnityEvent interactAction;
    public bool destroyOnInteract;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, radius);
    }


    private void OnTriggerStay(Collider collision)
    {
        if (Input.GetKeyDown(interactKey))
        {
            interactAction.Invoke();

            if (destroyOnInteract)
                Destroy(gameObject);
        }
    }
}
