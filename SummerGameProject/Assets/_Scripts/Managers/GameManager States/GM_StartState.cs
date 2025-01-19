using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM_StartState : FSMState
{
    private Slider slider;

    // Constructor
    public GM_StartState(Slider inSlider)
    {
        stateType = FSMStateType.Start;
        slider = inSlider;
    }

    public override void EnterStateInit()
    {
        Debug.Log("Start State Entered.");
        /*
        TODO
        Initialize everything that needs to be set Up
        Player
        Any Enemies
         */
    }

    public override void Reason(Transform player, Transform gm)
    {
        switch (slider.value)
        {
            case 1:
                break;
            case 2:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Pausing);
                break;
            case 3:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Ending);
                break;
            case 4:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Playing);
                break;
            default:
                break;
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        //TODO
        //Any player or NPC functionality that needs to be loaded at the start, IE before the Player has the chance to interact
    }
}


/*
NOTES
This State should realistically only initialize and setup any componenets that need to be set up at the START of a screen
For example,
Player Functionality
NPC Functionality,
As well as spawning things in that need to be spawned in at the beginning of the level

For Now, 
I will represent all of this with a couple cubes i think?
*/