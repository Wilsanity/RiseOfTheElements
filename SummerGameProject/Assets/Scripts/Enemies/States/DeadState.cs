using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : FSMState
{
    protected Vector3 destPos;
    protected float speed;

    protected bool justDied = false;

    private EnemyController enemyController;

    private Animator animator;

    // Constructor
    public DeadState(Animator inAnimator, EnemyController inEnemyController)
    {
        enemyController = inEnemyController;
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
