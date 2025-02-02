using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WolfRunBreakable : MonoBehaviour
{
    public UnityEvent OnWolfRunCollision;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out PlayerMovement movement) && movement.IsWolfRunning)
        {
            OnWolfRunCollision.Invoke();
        }
    }
}
