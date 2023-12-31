using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Barrier : MonoBehaviour
{
    int currentHealth;
    public int maxHealth = 20;

    [SerializeField] private GameObject movingBarrier;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <=0) { Die(); }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void MoveBarrier()
    {
        GameObject.Destroy(movingBarrier);
    }
}
