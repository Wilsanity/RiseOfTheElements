using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM_PlayState : FSMState
{
    private Slider slider;

    // Constructor
    public GM_PlayState(Slider inSlider)
    {
        stateType = FSMStateType.Play;
        slider = inSlider;
    }

    public override void EnterStateInit()
    {
        Debug.Log("Play State Entered.");
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
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Pausing);
                break;
            case 4:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Ending);
                break;
            default:
                break;
        }
    }

    public override void Act(Transform player, Transform gm)
    {
        Time.timeScale = 1;
    }
}
