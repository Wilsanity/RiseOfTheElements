using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM_EndState : FSMState
{
    private Slider slider;
    private SceneObjects _sceneObjects;

    // Constructor
    public GM_EndState(Slider inSlider, SceneObjects sceneObjects)
    {
        stateType = FSMStateType.End;
        slider = inSlider;
        _sceneObjects = sceneObjects;
    }

    public override void EnterStateInit()
    {
        Debug.Log("End State Entered.");

        foreach (var go in _sceneObjects)
        {
            var Destroying = go.GetComponent<IStateInterface>();
            if (Destroying != null)
            {
                Debug.Log("Ebd for game object being called");
                Destroying.End();
            }
        }
    }

    public override void Reason(Transform player, Transform gm)
    {
        switch (slider.value)
        {
            case 1:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Starting);
                break;
            case 2:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Playing);
                break;
            case 3:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Pausing);
                break;
            case 4:
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