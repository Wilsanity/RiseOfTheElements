using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventMessenger : MonoBehaviour
{
    public GameObject playerObj;




    private PlayerController playerController;

    private void Awake()
    {

        playerController = playerObj.GetComponent<PlayerController>();

        //Get our necessary components.
    }


    public void ComboCheck()
    {
        Debug.Log("Is this called by the animation?");
        //Forward it to our playerController for processing.

        playerController.ComboCheck();


    }

    public void EndAttack()
    {
        playerController.EndAttack();
    }


}
