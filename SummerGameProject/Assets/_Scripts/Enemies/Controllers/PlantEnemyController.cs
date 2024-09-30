using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEnemyController : MonoBehaviour
{
    
    public float attackRadius;
    public float movementSpeed;
    public float rotateSpeed;
    public float rotateDelay;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRadius)
        {
            StartCoroutine(RotateToPlayer());
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            transform.Translate(directionToPlayer * movementSpeed * Time.deltaTime);
        }
    }

    IEnumerator RotateToPlayer()
    {
        yield return new WaitForSeconds(rotateDelay);

        if (player != null)
        {
            Vector3 rotationDirection = (player.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);

            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)//Rotate towards player
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
