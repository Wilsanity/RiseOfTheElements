using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This state handles an enemy's death.
/// </summary>
public class DeadState : FSMState
{
    protected Vector3 destPos;
    protected float speed;

    protected bool justDied = false;

    private Animator animator;

    // Constructor
    public DeadState(Animator inAnimator)
    {
        stateType = FSMStateType.Dead;
        animator = inAnimator;
    }

    public override void EnterStateInit()
    {
        justDied = true;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //TO DO: Implement Animations
    }

    public override void Act(Transform player, Transform npc)
    {
        if (justDied)
            justDied = false;

        if (this != null) GameObject.Destroy(npc.gameObject);

        Debug.Log("Enemy died.");
    }
}
