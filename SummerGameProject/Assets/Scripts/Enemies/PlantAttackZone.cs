using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAttackZone : MonoBehaviour
{
    public float rotationSpeed = 5.0f;

    private GameObject player;
    private bool isRotating = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //if (isRotating)
        //{
        //    RotateTowardsPlayer();
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(AttackPlayer());
        }
    }

    IEnumerator AttackPlayer()
    {
        //This rotates with a delay 
        yield return new WaitForSeconds(1.0f);

        isRotating = true;

        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.TakeDamage();
            }
        }

        yield return new WaitForSeconds(1.0f);

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
