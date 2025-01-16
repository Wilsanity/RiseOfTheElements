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

    }
}
