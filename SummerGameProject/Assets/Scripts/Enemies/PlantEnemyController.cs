using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEnemyController : MonoBehaviour
{
    
    public float attackRadius;
    public float movementSpeed;

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
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            transform.Translate(directionToPlayer * movementSpeed * Time.deltaTime);
        }
    }
}
