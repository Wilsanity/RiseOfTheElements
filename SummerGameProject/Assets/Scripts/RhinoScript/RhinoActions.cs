using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhinoActions : MonoBehaviour
{
    [SerializeField]
    Collider col1;
    [SerializeField]
    Collider col2;

    private Transform player;

    Vector3 groundedNormal;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //Face towards the player immediately when triggered
        Vector3 direction = transform.position - player.position;
        direction.y = 0f; //Keep enemy's rotation level
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime);

        

        ChargeAttack();
    }

    private void ChargeAttack()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Face towards the player 
            Vector3 direction = transform.position - player.position;
            direction.y = 0f; //Keep enemy's rotation level
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = targetRot;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        
    }
}
