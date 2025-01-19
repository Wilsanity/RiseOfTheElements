using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM_EndState : FSMState
{
    private Slider slider;

    // Constructor
    public GM_EndState(Slider inSlider)
    {
        stateType = FSMStateType.End;
        slider = inSlider;
    }

    public override void EnterStateInit()
    {
        Debug.Log("End State Entered.");
    }

    public override void Reason(Transform player, Transform gm)
    {
        switch (slider.value)
        {
            case 1:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Starting);
                break;
            case 2:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Pausing);
                break;
            case 3:
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

    }
}

/*
NOTES
This State should Destroy any objects in the world that have not already been destroyed, and stop all timers or anything that may have been running in the background
For example,
Player
NPC

For Now, 
I will represent all of this with a couple cubes i think?
*/