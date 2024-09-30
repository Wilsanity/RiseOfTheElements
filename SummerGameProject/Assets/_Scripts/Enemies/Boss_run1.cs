using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_run1 : StateMachineBehaviour
{
    Transform player;
    Rigidbody rb;
    public float speed = 2.5f;
    public float attackRange = 3f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Move towards the player.
        Vector3 target = new Vector3(player.position.x, player.position.y, player.position.z);
        Vector3 newPosition = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        if (Vector3.Distance(player.position, rb.position) <= attackRange)
        {
            //Attack the player
            animator.SetTrigger("hammerSwing");
            animator.SetTrigger("doubleHammer");
            animator.SetTrigger("stomp");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("hammerSwing");
    }
}
