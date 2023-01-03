using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationMachine : MonoBehaviour
{
    public Animator playerAnimator;

    public static PlayerAnimationMachine playerAnimationInstance;
    // Start is called before the first frame update
    void Start()
    {
        playerAnimationInstance = this;
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void UpdatePlayerAnim(PlayerAnimState state)
    {
        switch (state)
        {
            case PlayerAnimState.Idle:
                if (CheckAnim(PlayerAnimState.Idle)) return;
                playerAnimator.SetTrigger("isIdle");

                return;
            case PlayerAnimState.Walking:
                if (CheckAnim(PlayerAnimState.Walking)) return;
                playerAnimator.SetTrigger("isWalking");

                return;
            case PlayerAnimState.Running:
                if (CheckAnim(PlayerAnimState.Running)) return;
                playerAnimator.SetTrigger("isRunning");

                return;
        }
    }

    public bool CheckAnim(PlayerAnimState state)
    {
        switch (state)
        {
            case PlayerAnimState.Idle:
                
                return playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
               
               
            case PlayerAnimState.Walking:
                return playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk");


            case PlayerAnimState.Running:
                return playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Run");

            default:
                Debug.LogError("Animation problem");
                return false;
        }

    }

}
public enum PlayerAnimState
{
    Idle,
    Walking,
    Running
}