using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class NPC : MonoBehaviour
{
    //Member variables
    private int m_Id;



    //Fix
    public bool m_RotFix = false;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void interactBegin()
    {
    }

    public void orientToInterest(Transform transform_)
    {
        GameObject player = GameObject.FindWithTag("Player");
        transform_ = player.transform;

        Vector3 temp = new Vector3(transform_.position.x, this.transform.position.y, transform_.position.z);
        transform.LookAt(temp, Vector3.up);


        //Fix here... For origin issue.
        if (m_RotFix)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x - 90.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }

}