using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleProjectile : MonoBehaviour
{
    private int damageAmount = 1;

    private float lifetime = 5f;
    private bool puddleActivated = false;

    private void Awake()
    {
        puddleActivated = false;
    }

    private void Update()
    {
        if (puddleActivated)
        {
            StartCoroutine(TriggerPoisonPuddle());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (puddleActivated)
        {
            if (collision.collider.CompareTag("Player"))
            {
                Debug.Log("Player touched poison. (Poison not implemented)");
                collision.collider.GetComponent<UnitHealth>().DamageUnit(damageAmount);
            }
        }
        else
        {
            if (collision.collider.CompareTag("Player"))
            {
                Debug.Log("Hit player. (Poison not implemented.");

                collision.collider.GetComponent<UnitHealth>().DamageUnit(damageAmount);

                Destroy(gameObject);
            }
            else if (collision.collider.CompareTag("Ground"))
            {
                puddleActivated = true;
                Debug.Log("Hit ground.");
            }
            else if (collision.collider.CompareTag("Enemy"))
            {
                return;
            }
            else
            {
                Debug.Log(collision.collider.name);
            }
        }
    }

    private IEnumerator TriggerPoisonPuddle()
    {
        // Note: this will likely be replaced in the future and is here for prototyping purposes
        if (transform.localScale != new Vector3(0.8f, 0.8f, 0.2f))
        {
            transform.localScale += new Vector3(0.1f, 0.1f, -0.1f);
        }

        yield return new WaitForSeconds(lifetime);

        Destroy(gameObject);
    }
}
