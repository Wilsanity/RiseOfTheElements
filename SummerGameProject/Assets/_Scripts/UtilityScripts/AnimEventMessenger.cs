using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A necessary messenger class to allow the use of anim events to send information to the player controller script.
//Needed since the player animator is not attached to the same gameobject as the player controller script...
public class AnimEventMessenger : MonoBehaviour
{
    public GameObject playerObj;
    private PlayerController playerController;

    private void Awake()
    {

        playerController = playerObj.GetComponent<PlayerController>();

        //Get our necessary components.
    }

    public void PunchEnable(int fistId)
    {
        playerController.PunchEnable(fistId);
    }

    public void PunchDisable(int fistId)
    {
        playerController.PunchDisable(fistId);
    }


}
