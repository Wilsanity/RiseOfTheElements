using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnIcon : MonoBehaviour
{
    [SerializeField] Transform target;
    void OnEnable()
    {
        if (target == null)
        {
            target = Camera.main.transform;
        }
        StartCoroutine(LookAtTarget());
    }

    private IEnumerator LookAtTarget()
    {
        while (this.gameObject.activeInHierarchy)
        {
            Vector3 dir = target.position - transform.position;
            transform.rotation = Quaternion.LookRotation(dir);
            yield return null;
        }
    }
}
