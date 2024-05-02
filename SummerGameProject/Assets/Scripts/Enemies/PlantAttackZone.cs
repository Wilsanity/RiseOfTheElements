using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAttackZone : MonoBehaviour
{
    public float rotationSpeed = 5.0f;

    private bool isAttacking = false;
    private bool isRotating = false;
    private GameObject player;

    // Update is called once per frame
    void Update()
    {
        //if (isRotating)
        //{
        //    RotateTowardsPlayer();
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject;

            if (!isAttacking) StartCoroutine(AttackPlayer());

        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        //This rotates with a delay 
        yield return new WaitForSeconds(1.0f);

        
        isRotating = true;
        Debug.Log("Plant Attack");
       
        UnitHealth playerHealth = player.GetComponent<UnitHealth>();
        playerHealth.DamageUnit(1);
         

        yield return new WaitForSeconds(1.0f);

        isAttacking = false;
        isRotating = false;
        transform.rotation = Quaternion.identity;
    }

    //void RotateTowardsPlayer()
    //{
    //    if (player != null)
    //    {
    //        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
    //        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

    //        //Rotate towards player
    //        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    //    }
    //}
}
