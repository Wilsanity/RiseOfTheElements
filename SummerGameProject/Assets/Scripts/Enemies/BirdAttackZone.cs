using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAttackZone : MonoBehaviour
{
    public bool isCollidingPlayer = false;
    public bool isCollidingGround = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCollidingPlayer = true;
            isCollidingGround = false;
            Debug.Log("Collided with player...");
        }
        else if (other.CompareTag("Ground"))
        {
            isCollidingPlayer = false;
            isCollidingGround = true;
        }
        else
        {
            isCollidingPlayer = false;
            isCollidingGround = false;
        }
    }
}
