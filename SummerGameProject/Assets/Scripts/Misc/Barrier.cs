using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField]
    int health;

    int fullHealth; 

    private void Awake()
    {
        health = fullHealth;

    }



    private void Die()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
