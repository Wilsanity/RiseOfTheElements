using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Player Health Phase Event", menuName = "Scriptable Objects/Player Health Phase Events")]
public class SO_PlayerHealthPhaseEvents : SO_BaseUnitPhaseEvent
{

    public override void HealthPhase1Event()
    {
        int i = 0;
        if (IsCompleted(i)) return;

        //Custom player logic
        Debug.Log("This is sent from the player");
    }

    public override void HealthPhase2Event()
    {
        int i = 1;
        if (IsCompleted(i)) return;

        //Custom player logic
        Debug.Log("This is sent from the player");
    }

    public override void DefeatedLogic()
    {
        base.DefeatedLogic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Destroy(unitAssigned);
    }
}
