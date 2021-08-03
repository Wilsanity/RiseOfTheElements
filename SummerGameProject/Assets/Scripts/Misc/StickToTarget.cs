using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToTarget : MonoBehaviour
{

    public Transform target;
    public Vector3 offset = new Vector3(0, 0, 0);


    Vector3 truePosition;


     void Update()
     {
        truePosition = target.position + offset;


        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            transform.localPosition = truePosition;
        }
    }
}
