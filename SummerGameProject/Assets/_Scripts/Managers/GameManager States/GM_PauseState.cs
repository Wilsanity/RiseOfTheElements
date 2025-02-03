using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM_PauseState : FSMState
{
    private Slider slider;
    private GameObject pauseScreen;

    // Constructor
    public GM_PauseState(Slider inSlider, GameObject screen)
    {
        stateType = FSMStateType.Pause;
        slider = inSlider;
        pauseScreen = screen;
    }

    public override void EnterStateInit()
    {
        pauseScreen.SetActive(true);
        Debug.Log("Pause State Entered.");
    }

    public override void Reason(Transform player, Transform gm)
    {
        switch (slider.value)
        {
            case 1:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Starting);
                pauseScreen.SetActive(false);
                break;
            case 2:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Playing);
                pauseScreen.SetActive(false);
                break;
            case 3:
                break;
            case 4:
                gm.GetComponent<GameManager>().PerformTransition(TransitionType.Ending);
                pauseScreen.SetActive(false);
                break;
            default:
                break;
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        Time.timeScale = 0;
    }
}
