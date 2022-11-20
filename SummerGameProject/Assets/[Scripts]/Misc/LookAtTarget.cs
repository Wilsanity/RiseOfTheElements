using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public GameObject Target;
    public Vector3 targetPos;

    // Use this for initialization
    void Start()
    {
        targetPos = Camera.main.WorldToScreenPoint(Target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 center = new Vector3(0, this.transform.position.y, 0);
        Vector3 targetPos = Target.transform.position;
        targetPos.y = transform.position.y;
        transform.LookAt(targetPos);

    }
}
