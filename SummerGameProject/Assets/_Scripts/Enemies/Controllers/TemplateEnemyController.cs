using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script provides a template structure for future enemy controllers scripts. Do not edit this script, used as a reference only.
/// To be deleted in the future.
/// </summary>
public class TemplateEnemyController : EnemyController
{
    //[Header("Title")]
    //

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void ConstructFSM()
    {
        // Create States
        //
        // Create State #1
        // INSERT_STATE_SCRIPT stateName = new INSERT_STATE_SCRIPT(parameters);

        // Create State #2

        // Create State #3

        // Create Dead State
        DeadState dead = new DeadState(animator);

        // Add Transitions
        //
        // Transitions out of #1 state
        //INSERT_STATE_#1.AddTransition(TransitionType.INSERT_TRANSITION, FSMStateType.INSERT_STATE_TYPE);

        // Transitions out of #2 state

        // Transitions out of #3 state

        // Add States to List
        //AddState(INSERT_STATE_#1)
        //AddState(INSERT_STATE_#2)
        //AddState(INSERT_STATE_#3)
        AddState(dead);
    }
}
