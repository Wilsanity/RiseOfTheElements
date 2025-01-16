using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM_PauseState : FSMState
{
    private Slider slider;

    // Constructor
    public GM_PauseState(Slider inSlider)
    {
        stateType = FSMStateType.Pause;
        slider = inSlider;
    }

    public override void EnterStateInit()
    {
        Debug.Log("Pause State Entered.");
    }

    public override void Reason(Transform player, Transform gm)
    {
        switch (slider.value)
        {
            case 1:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Starting);
                break;
            case 2:
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

    }
}
