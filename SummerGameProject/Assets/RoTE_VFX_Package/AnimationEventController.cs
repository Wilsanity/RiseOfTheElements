using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationEventController : MonoBehaviour
{
    public GameObject objAnimation;
    public ParticleSystem particle1;
    public ParticleSystem particle2;
    public ParticleSystem particle3;
    public ParticleSystem particle4;

    public void punchSwoosh()
    {
        particle1.GetComponent<ParticleSystem>().Play(true);
        particle1.GetComponent<Animation>().Play();
    }
    
   
    public void impact()
    {
        particle2.GetComponent<ParticleSystem>().Play(true);
        particle2.GetComponent<Animation>().Play();
    }


    public void smallPunchR()
    {
        particle3.GetComponent<ParticleSystem>().Play(true);
        //particle3.GetComponent<Animation>().Play();
    }

    public void smallPunchL()
    {
        particle4.GetComponent<ParticleSystem>().Play(true);
        //particle4.GetComponent<Animation>().Play();
    }



    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

            GetComponent<Animator>().SetTrigger("uppercutCombo");
            objAnimation.SetActive(true);
        }
        else
        {
            GetComponent<Animator>().ResetTrigger("uppercutCombo");

        }


    }

}
